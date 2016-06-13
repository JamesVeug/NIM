using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShardUI : MonoBehaviour
{
	private static ShardUI instance = null;

    public Text pickupCounterText;
    public MovementWaypoint closestShardToMaster;
    public GameObject navigatorPrefab;
    public GameObject player;

	private int totalPickups = 0;
	private GameObject masterShard;

    // Navigation Path
    private MovementWaypoint lastPlayerPoint;
    public float maxNavNodes = 10;
    public float navTimeDelay = 2;
    private float nextTime = 0f;

    // Current time for the scaling of the bar
    private float currentTime = 1f;

    // Scroll bar to represent how much we have collected
    public Image shardImage;
    private bool alertedCompletion = false;
    private float currentSpeed = 0;
    private int pickups = 0;

    // We picked up a shard
    public void pickupShard()
    {
        pickups++;
        if (currentTime >= 1)
        {
            currentTime = 0f;
        }
    }

	void Awake(){
		masterShard = FindObjectOfType<MasterCrystal> ().gameObject;
		totalPickups = FindObjectsOfType<PickupTemp> ().Length;

		instance = this;
	}

    // Use this for initialization
    void Start () {
        shardImage.fillAmount = 0;
    }

    void Update()
    {
        if( shardImage == null)
        {
            Debug.LogError("No scroll bar has been added to GameObject '" + gameObject.name + "'");
            return;
        }

        // Change the scroll bar
        float expectedPickup = ((float)pickups / (float)totalPickups);
        float currentPickup = shardImage.fillAmount;

        float newPickup = currentPickup += (expectedPickup - currentPickup) * currentTime;
        shardImage.fillAmount = newPickup;
        currentTime = Mathf.Min(1, currentTime += Time.deltaTime);


        // Update pickup Counter
        if (pickupCounterText != null)
        {
            pickupCounterText.text = pickups.ToString() + "/" + totalPickups.ToString();
        }
        else
        {
            Debug.LogWarning("Missing PickupCounter from ShardUI in " + gameObject.name);
        }
        

        // Check if we have completed the bar
        if (shardImage.fillAmount >= 1)
        {

            if (player == null)
            {
                Debug.Log("Missing Player reference in ShardUI script! " + gameObject.name);
            }
            else if (masterShard == null)
            {
                Debug.Log("Missing MasterShard reference in ShardUI script! " + gameObject.name);
            }
            else if (!alertedCompletion)
            {
                completedBar();
            }
            else
            {
                continuePath();
            }
        }
    }

    // We have collected all the shards
    public void completedBar()
    {
        Debug.Log("Picked up all Shards! =D");

        // Don't call this method again
        alertedCompletion = true;
        startPath();

		masterShard.SetActive (true);
    }

    public void continuePath()
    {

        MovementWaypoint current = player.GetComponent<Movement>().currentMovementWaypoint;
        lastPlayerPoint = current;
        startPath();

        // Conditions:
        // It's time to create new one after a delay
        // We have not already exceeded the amount of nodes
        // The path has more than 2 nodes in it
        // The last navigator is further than x distance from the player
        if( Time.time > nextTime && 
            FollowPath.FollowPathCount() < maxNavNodes && 
            FollowPath.getPath().Count > 2 &&
            (FollowPath.getLastNavigator().transform.position-player.transform.position).magnitude > 4)
        {
            // Create an AI to follow the path
            if (navigatorPrefab != null)
            {
                GameObject o = (GameObject)Instantiate(navigatorPrefab);
            }
            else
            {
                Debug.LogWarning("Missing Navigator Prefab. Can not draw path!");
            }
            nextTime = Time.time + navTimeDelay;
        }
    }

    //
    // NAVIGATION
    //
    public void startPath()
    {

        List<GameObject> path = getPath();
        for( int i = 0; i < path.Count-1; i++)
        {
            Vector3 current = path[i].transform.position;
            Vector3 next = path[i+1].transform.position;
            Debug.DrawLine(current, next, Color.blue, 5);
        }

        FollowPath.setPath(path);
        if (FollowPath.FollowPathCount() <= 0 )
        {
            // Create an AI to follow the path
            GameObject o = (GameObject)Instantiate(navigatorPrefab);
            nextTime = Time.time + navTimeDelay;
        }
    }

    public List<GameObject> getPath()
    {
        MovementWaypoint start = closestShardToMaster;
        MovementWaypoint end = player.GetComponent<Movement>().currentMovementWaypoint;
        lastPlayerPoint = end;

        HashSet<MovementWaypoint> visited = new HashSet<MovementWaypoint>();
        List<AStarNode> fringe = new List<AStarNode>();
        addNode(fringe, null, start,false);
        //Debug.Log("Added " + fringe.Count);

        AStarNode star = null;
        while (fringe.Count > 0)
        {
            //Debug.Log("Fringe " + fringe.Count);
            star = fringe[0]; fringe.RemoveAt(0);
            MovementWaypoint node = star.current;

            // Found target
            if ( node == end)
            {
                break;
            }

            // Don't visit if we already have
            if(visited.Contains(node))
            {
                continue;
            }
            visited.Add(node);

            // Add all the children to the fringe
            if (node.previous != null) addNode(fringe, star, node.previous, false);
            if (node.next != null) addNode(fringe, star, node.next, false);
            if (node.previousPhasePoint != null) addNode(fringe, star, node.previousPhasePoint, true);
            if (node.nextPhasePoint != null) addNode(fringe, star, node.nextPhasePoint, true);
        }


        // Couldn't find the path
        if (star.current != end) {
            //Debug.LogError("Couldn't find path");
            return null;
        }

        List<GameObject> nodes = new List<GameObject>();

        AStarNode it = star;
        while(it != null)
        {
            nodes.Add(it.current.gameObject);
            it = it.last;
        }

        // First position is nim
        nodes[0] = player.gameObject;

        // Last position should be the position of the master shard
        nodes[nodes.Count - 1] = masterShard.gameObject;
        return nodes;
    }

    private void addNode(List<AStarNode> list, AStarNode last, MovementWaypoint toAdd, bool isPhasePoint)
    {
        // Distance from this node to the master shard
        float heuristic = toAdd.distance(masterShard.transform.position);

        float distanceToNextPoint = last == null ? 0 : isPhasePoint ? 0.5f : last.current.distance(toAdd);
        float distance = last != null ? last.distance + distanceToNextPoint : 0;
        AStarNode path = new AStarNode(distance, heuristic, last, toAdd);
        
        for(int i = 0; i < list.Count;  i++)
        {

            
            if(list[i].compareTo(path) > 0)
            {
                list.Insert(i, path);
                return;
            }
        }

        // Didn't add into the list. So add to the end
        list.Add(path);
    }

	public static ShardUI getInstance(){
		return instance;
	}
}

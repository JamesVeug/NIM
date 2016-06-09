using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShardUI : MonoBehaviour
{
    public int totalPickups = 0;
    public Text pickupCounterText;
    public MovementWaypoint closestShardToMaster;
    public GameObject navigatorPrefab;
    public GameObject masterShard;
    public GameObject player;

    private MovementWaypoint lastPlayerPoint;

    //private static float newScale = 0f; // Change size to this
    public static float maxScaleSpeed = 0.25f;
    public static float scaleSpeed = 0.1f;
    public static float speedDecay = 0.002f;

    private Scrollbar bar;
    private bool alertedCompletion = false;
    private static float currentSpeed = 0;
    private static int pickups = 0;

    // We picked up a shard
    public static void pickupShard()
    {
        pickups++;
        currentSpeed = Mathf.Min(maxScaleSpeed, currentSpeed + scaleSpeed);
    }

	// Use this for initialization
	void Start () {
        bar = GetComponent<Scrollbar>();
        bar.size = 0;
        currentSpeed = scaleSpeed;
    }

    void Update()
    {
        if( bar == null)
        {
            Debug.LogError("No scroll bar has been added to GameObject '" + gameObject.name + "'");
            return;
        }

        // Change the scroll bar
        float pickupRatio = ((float)pickups / (float)totalPickups);
        if (pickupRatio > bar.size)
        {
            bar.size = Mathf.Min(bar.size + currentSpeed * Time.deltaTime, pickupRatio);
        }
        else
        {
            bar.size = Mathf.Max(bar.size - currentSpeed * Time.deltaTime, pickupRatio);
        }

        // Update pickup Counter
        pickupCounterText.text = pickups.ToString() + "/" + totalPickups.ToString();

        // Decay the speed so it doesn't stay so fast
        if ( bar.size == pickupRatio)
        {
            //Debug.Log("Fast");
            // Decay faster if we have reached our target
            currentSpeed = Mathf.Max(currentSpeed - speedDecay*2, scaleSpeed);
        }
        else
        {
            // Slow down the update rate
            currentSpeed = Mathf.Max(currentSpeed - speedDecay, scaleSpeed);
        }
        //Debug.Log("Speed " + currentSpeed);

        // Check if we have completed the bar
        if (bar.size >= 1)
        {
            if (!alertedCompletion)
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
    }

    public void continuePath()
    {
        MovementWaypoint current = player.GetComponent<Movement>().currentMovementWaypoint;
        if( current != lastPlayerPoint)
        {
            lastPlayerPoint = current;
            startPath();
        }
    }

    //
    // NAVIGATION
    //
    public void startPath()
    {
        if (player == null || masterShard == null)
        {
            Debug.LogError("Missing Player or MasterShard reference in ShardUI script!");
            return;
        }

        List<Vector3> path = getPath();
        for( int i = 0; i < path.Count-1; i++)
        {
            Vector3 current = path[i];
            Vector3 next = path[i+1];
            Debug.DrawLine(current, next, Color.blue, 10);
        }

        GameObject o = (GameObject)Instantiate(navigatorPrefab);
        o.transform.position = player.transform.position;
        FollowPath followPath = o.AddComponent<FollowPath>();
        FollowPath.setPath(path);

        /*GameObject o2 = (GameObject)Instantiate(navigatorPrefab);
        o2.transform.position = player.transform.position;
        FollowPath followPath2 = o2.AddComponent<FollowPath>();*/
    }

    public List<Vector3> getPath()
    {
        MovementWaypoint start = closestShardToMaster;
        MovementWaypoint end = player.GetComponent<Movement>().currentMovementWaypoint;
        lastPlayerPoint = end;

        HashSet<MovementWaypoint> visited = new HashSet<MovementWaypoint>();
        List<AStarNode> fringe = new List<AStarNode>();
        addNode(fringe, null, start);
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
            if (node.previous != null) addNode(fringe, star, node.previous);
            if (node.next != null) addNode(fringe, star, node.next);
            if (node.previousPhasePoint != null) addNode(fringe, star, node.previousPhasePoint);
            if (node.nextPhasePoint != null) addNode(fringe, star, node.nextPhasePoint);
        }


        // Couldn't find the path
        if (star.current != end) {
            //Debug.LogError("Couldn't find path");
            return null;
        }

        List<Vector3> nodes = new List<Vector3>();

        AStarNode it = star;
        while(it != null)
        {
            nodes.Add(it.current.transform.position);
            it = it.last;
        }

        //Debug.Log("FOUND path");

        // First position is nim
        nodes[0] = player.transform.position;

        // Last position should be the position of the master shard
        nodes[nodes.Count - 1] = masterShard.transform.position;
        return nodes;
    }

    private void addNode(List<AStarNode> list, AStarNode last, MovementWaypoint toAdd)
    {
        // Distance from this node to the master shard
        float heuristic = toAdd.distance(masterShard.transform.position);
        float distance = last != null ? last.distance + last.current.distance(toAdd) : 0;
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
}

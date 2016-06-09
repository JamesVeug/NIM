using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowPath : MonoBehaviour {
    private static List<FollowPath> navigators = new List<FollowPath>();
    private static List<GameObject> path;
    private List<GameObject> currentPath;

    private int index = 0;
    public float speed = 10f;

    public void Start()
    {
        navigators.Add(this);
        currentPath = path;
        transform.position = currentPath[0].transform.position + new Vector3(0, 1, 0);
    }

	// Update is called once per frame
	void Update () {

        if ( index == currentPath.Count-1)
        {
            AIBuzzer buzz = GetComponent<AIBuzzer>();
            if (buzz != null && (currentPath.Count <= 2 || buzz.enabled == true) )
            {
                buzz.origin = currentPath[currentPath.Count - 1];
                buzz.enabled = path.Count <= 2;
                if (buzz.enabled)
                {
                    // Buzz and do nothing else
                    return;
                }
            }

            // Reached the end. Kill ourselves
            navigators.Remove(this);
            Destroy(gameObject);
            return;
        }

        Vector3 next = currentPath[index + 1].transform.position;
        if ((next - transform.position).magnitude < 1)
        {
            index++;
        }


        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, next, step);
    }

    public static void setPath(List<GameObject> p)
    {
        path = p;
    }

    public static int FollowPathCount()
    {
        return navigators.Count;
    }

    public static List<GameObject> getPath()
    {
        return path;
    }
    public static GameObject getLastNavigator()
    {
        return navigators[navigators.Count - 1].gameObject;
    }
}

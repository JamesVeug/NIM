using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FollowPath : MonoBehaviour {
    private static List<FollowPath> navigators = new List<FollowPath>();
    private static List<GameObject> path;
    private List<GameObject> currentPath;

    private int pathIndex = 0;
    public float speed = 10f;

    public void Start()
    {
        navigators.Add(this);
        currentPath = path;
        transform.position = currentPath[0].transform.position + new Vector3(0, 1, 0);
    }

	// Update is called once per frame
	void Update () {

        if ( pathIndex == currentPath.Count-1)
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
            removeThisNavigator();
            return;
        }
        else if( navigators.Capacity > 1)
        {
            // See if we are too close to another navigator
            int navigatorIndex = getIndexInFollowers();
            if(navigatorIndex > 0 )
            {
                // We are not the last one in the list
                float distance = (navigators[navigatorIndex - 1].transform.position-transform.position).magnitude;
                //Debug.Log("Distance " + distance);
                if ( distance < 3)
                {
                    //Debug.Log("Die");
                    // We are too close to one that is ahead of us. Destroy ourselves
                    removeThisNavigator();
                    return;
                }
            }
        }

        // Move to the next point if we need to
        Vector3 next = currentPath[pathIndex + 1].transform.position;
        if ((next - transform.position).magnitude < 1)
        {
            pathIndex++;
        }

        // Move towards the point
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, next, step);
    }

    private int getIndexInFollowers()
    {
        for(int i = 0; i < navigators.Count; i++)
        {
            FollowPath p = navigators[i];
            if( p == this)
            {
                return i;
            }
        }

        return -1;
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

    public void removeThisNavigator()
    {
        navigators.Remove(this);
        Destroy(gameObject);
    }
}

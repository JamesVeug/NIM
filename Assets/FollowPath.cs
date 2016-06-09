using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowPath : MonoBehaviour {
    private static List<FollowPath> navigators = new List<FollowPath>();
    private static List<Vector3> path;

    private int index = 0;
    public float speed = 10f;
	
    public void Start()
    {
        navigators.Add(this);
    }

	// Update is called once per frame
	void Update () {

        if( index >= path.Count)
        {
            return;
        }

        Vector3 next = path[index + 1];
        if ((next - transform.position).magnitude < 1)
        {
            index++;
        }

        if ( index == path.Count-1)
        {
            index = 0;
            transform.position = path[index];
        }


        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, next, step);
    }

    public static void setPath(List<Vector3> p)
    {
        Debug.Log("p " + p + " a " + path);
        int difference = p.Count - (path != null ? path.Count : p.Count);
        foreach (FollowPath f in navigators)
        {
            f.index += difference;
            Debug.Log("New Index " + f.index);
        }
        path = p;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SplineInterpolator))]
public class WaypointGroup : MonoBehaviour
{
    private Transform[] transforms = null;

    private SplineInterpolator interp;

    void Start()
    {
        interp = GetComponent<SplineInterpolator>();
        SetupSplineInterpolator(interp, GetTransforms());
        interp.StartInterpolation(null, false, eWrapMode.ONCE);
    }

    public Vector3 getPoint(int index, float speed)
    {
        Transform[] trans = GetTransforms();
        if (trans.Length < 2)
        {
            return Vector3.zero;
        }
        

        float currTime = (((float)index)/10) * speed / 100;
        Vector3 currPos = interp.GetHermiteAtTime(currTime);
        return currPos;
    }

    public Vector3 getPoint(MovementWaypoint p, float speed)
    {
        SplineInterpolator interp = GetComponent<SplineInterpolator>();
        return SplineInterpolator.GetHermiteInternal(p, speed);
    }

    private Transform[] GetTransforms()
    {
        if (this.transforms == null)
        {
            List<Transform> transforms = new List<Transform>();
            foreach (Transform child in transform)
                transforms.Add(child);

            transforms.Remove(transform);
            transforms.Sort(delegate (Transform a, Transform b)
            {
                return a.name.CompareTo(b.name);
            });

            this.transforms = transforms.ToArray();
        }

        return this.transforms;
    }

    void SetupSplineInterpolator(SplineInterpolator interp, Transform[] trans)
    {
        interp.Reset();

        float step = 10 / (trans.Length - 1);

        int c;
        for (c = 0; c < trans.Length; c++)
        {
            interp.AddPoint(trans[c].position, trans[c].rotation, step * c, new Vector2(0, 1));
        }
    }
}

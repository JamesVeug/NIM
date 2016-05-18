using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SplineInterpolator))]
public class DrawSplines : MonoBehaviour {

    public eWrapMode WrapMode = eWrapMode.ONCE;
    public float Duration = 10;

    void SetupSplineInterpolator(SplineInterpolator interp, Transform[] trans)
    {
        interp.Reset();

        float step = Duration / (trans.Length - 1);

        int c;
        for (c = 0; c < trans.Length; c++)
        {
            interp.AddPoint(trans[c].position, trans[c].rotation, step * c, new Vector2(0, 1));
        }
    }

    /// <summary>
    /// Returns children transforms, sorted by name.
    /// </summary>
    public Transform[] GetTransforms()
    {
        //Debug.Log("GetTransforms " + gameObject.name);
        List<Component> components = new List<Component>(GetComponentsInChildren(typeof(Transform)));
        //Debug.Log("Components " + components.Count);

        List<Transform> transforms = components.ConvertAll(c => (Transform)c);
        //Debug.Log("transforms " + transforms.Count);

        transforms.Remove(transform);
        transforms.Sort(delegate (Transform a, Transform b)
        {
            return a.name.CompareTo(b.name);
        });

        return transforms.ToArray();
    }

    void OnDrawGizmos()
    {
        Transform[] trans = GetTransforms();
        if (trans.Length < 2)
            return;

        SplineInterpolator interp = GetComponent(typeof(SplineInterpolator)) as SplineInterpolator;
        SetupSplineInterpolator(interp, trans);
        interp.StartInterpolation(null, false, WrapMode);


        Vector3 prevPos = trans[0].position;
        for (int c = 1; c <= 100; c++)
        {
            float currTime = c * Duration / 100;
            Vector3 currPos = interp.GetHermiteAtTime(currTime);
            float mag = (currPos - prevPos).magnitude * 2;
            Gizmos.color = new Color(mag, 0, 0, 1);
            Gizmos.DrawLine(prevPos, currPos);
            prevPos = currPos;
        }
    }
}

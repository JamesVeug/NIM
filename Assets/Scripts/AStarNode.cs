using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarNode {

    public float distance;
    public float heuristic;
    public AStarNode last;
    public MovementWaypoint current;

    public AStarNode(float d, float h, AStarNode l, MovementWaypoint c) {
        distance = d;
        heuristic = h;
        last = l;
        current = c;
    }

    public int compareTo(AStarNode other)
    {
        float a = distance + heuristic;
        float b = other.distance + other.heuristic;
        float c = a - b;
        return c > 0 ? 1 : c == 0 ? 0 : -1 ;
    }
}

using UnityEngine;
using System.Collections;

public class AIPatrol : MonoBehaviour {

    public Transform goal;

    void Start() {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position;
    }
    
}

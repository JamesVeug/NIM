using UnityEngine;
using System.Collections;

public class AIPatrol : MonoBehaviour {

    public Transform goal;
    public NavMeshAgent agent;
    public GameObject target;
    public bool isWaypointTarget;
    public Animator anim;

    private bool angered;
   

    void Start() {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            anim.SetBool("Walking", false);
            angered = false;
    }

    void FixedUpdate() {

        var targetDist = Vector3.Distance(target.transform.position, this.transform.position);
        if (angered == true)
        {
            MovementWaypoint waypointScript = target.GetComponent<MovementWaypoint>();
            anim.SetBool("Walking", true);
            agent.SetDestination(target.transform.position);
            
        }
        else {
            anim.SetBool("Walking", false);
        }
    }

    void OnCollisionEnter(Collision col) {

        if (col.gameObject.tag == "Player")
        {
            if (isWaypointTarget)
            {
                //transform.position = Vector3.MoveTowards(transform.position, pointB.transform.position, 10 * Time.deltaTime);
            }
            else {//over the edge target

            }
        }
     
    }

    public void setAggression(bool agress) {
        angered = agress;
    }
}

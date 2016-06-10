using UnityEngine;
using System.Collections;

public class AIScenicPatrol : MonoBehaviour {

    public NavMeshAgent agent;
    public Transform target1;
    public Transform target2;
    public Animator anim;
    public int counter;

    private Transform currentTarget;


    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        anim.SetBool("Walking", false);
        currentTarget = target1;
        counter = 0;
    }

    void FixedUpdate()
    {
      //  Debug.Log("COUNTER: " + counter);
        
        if (counter >= 300)
        {
            anim.SetBool("Walking", true);

            //Debug.Log("REMAINS: " + agent.remainingDistance);
            agent.SetDestination(currentTarget.position);
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
               // agent.SetDestination(new Vector3(1000,1000,1000));//disable destination?
                switchTarget();
                counter = 0;
                anim.SetBool("Walking", false);

            }
            
        }
        else {
            counter++;
        }

    }




    private void switchTarget() {
        if (currentTarget.Equals(target1))
        {
            currentTarget = target2;
            Debug.Log("TEST1");
        }
        else {
            currentTarget = target1;
            Debug.Log("TEST2");
        }
    }
}

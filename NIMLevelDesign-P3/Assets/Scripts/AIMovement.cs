using UnityEngine;
using System.Collections;

public class AIMovement : MonoBehaviour
{
    const int STATES_PATROL = 0;
    const int STATES_CHASE = 1;
    const int STATES_IDLE = 2;

    NavMeshAgent agent;
    CharacterController controller;

    public float gravity = 5f;
    public float speed = 5f;

    public float maxDistance = 5;
    public float maxSeachRange = 5;
    public float loseSearchDistance = 10;
    public GameObject[] waypoints;

    private int wayPointIndex = 0;
    private int currentState = 0;

    // Who we are chasing
    GameObject target = null;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();

        agent.SetDestination(waypoints[0].transform.position);

        agent.updatePosition = true;
        agent.updateRotation = true;
        
        agent.Move(agent.desiredVelocity);
        controller.Move(new Vector3(0, -gravity, 0));

        currentState = STATES_PATROL;
    }

    // Update is called once per frame
    void Update()
    {
        var targetPoint = agent.nextPosition;
        Vector3 lookat = targetPoint - transform.position;
        if (lookat != Vector3.zero)
        {
            var targetRotation = Quaternion.LookRotation(lookat);
            

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 50.0f * Time.deltaTime);
        }

        // Check state machine
        if (currentState == STATES_CHASE)
        {
            chase();
        }
        else if( foundPlayer() && target != null )
        {
            currentState = STATES_CHASE;
        }
        else if ( currentState == STATES_PATROL)
        {
            patrol();
        }
        else if (currentState == STATES_IDLE)
        {
            idle();
        }
        
        
    }

    

    bool foundPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if( player == null)
        {
            //Debug.LogWarning("CAN NOT FIND PLAYER");
            return false;
        }

        float distance = (player.transform.position - transform.position).magnitude;
        if (distance > maxSeachRange)
        {
            //Debug.LogWarning("Too FAR " + distance);

            // Too far away
            return false;
        }

        // Found the target!
        target = player;
        return true;
    }

    void patrol()
    {
        GameObject nextWayPoint = waypoints[wayPointIndex];
        float distanceToPoint = (nextWayPoint.transform.position - transform.position).magnitude;
        if (distanceToPoint < maxDistance)
        {
            wayPointIndex++;
            if (wayPointIndex >= waypoints.Length)
            {
                wayPointIndex = 0;
            }
        }
        else
        {
            agent.SetDestination(waypoints[wayPointIndex].transform.position);
            agent.Move(agent.desiredVelocity);
            controller.Move(new Vector3(0, gravity, 0));
        }
    }

    void chase()
    {
        if( target == null)
        {
            currentState = STATES_PATROL;
            return;
        }

        float distanceToPoint = (target.transform.position - transform.position).magnitude;
        if( distanceToPoint > loseSearchDistance)
        {
            currentState = STATES_CHASE;
            target = null;
        }
        else
        {
            // We have someone to chase
            agent.SetDestination(target.transform.position);
            agent.Move(agent.desiredVelocity);


            Vector3 direction2 = new Vector3(0, -gravity, 0).normalized;
            direction2 *= speed;

            controller.Move(direction2 * Time.deltaTime);
        }
    }

    void idle()
    {

    }
}
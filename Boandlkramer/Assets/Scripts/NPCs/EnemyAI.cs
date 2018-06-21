using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


abstract public class State<AIClass>
{
    // this method is called when the new state is entered
    abstract public void Enter(AIClass ai);

    // called anytime we update
    abstract public void Execute(AIClass ai);

    // called when the state is changed
    abstract public void Exit(AIClass ai);

}


class Patrol : State<EnemyAI>
{
    public override void Enter(EnemyAI ai)
    {
        Debug.Log("Enter patrol");
    }

    public override void Execute(EnemyAI ai)
    {
        // get all colliders within the awareness radius
        Collider[] collidingObjects = Physics.OverlapSphere(ai.transform.position, ai.awarenessRadius);
        foreach (Collider coll in collidingObjects)
        {
            // we found a player within the awareness radius - this object will become the new target
            if (coll.tag == "Player")
            {
                ai.SetNewTarget(coll.gameObject);
                ai.ChangeState(new Hunt());
                // stop further searching
                return;
            }
        }
        
        // keep walking home if not aggro and not there yet...
        NavMeshAgent agent = ai.GetAgent();
        if (agent != null)
        {
            agent.stoppingDistance = 0;
            agent.destination = ai.initialPosition;
        }
    }

    public override void Exit(EnemyAI ai)
    {
        Debug.Log("Exit patrol");
    }
}

class Hunt : State<EnemyAI>
{
    public override void Enter(EnemyAI ai)
    {
        Debug.Log("Enter hunt");
    }

    public override void Execute(EnemyAI ai)
    {
        GameObject target = ai.GetTarget();

        if (target != null)
        {
			ai.GetAgent ().SetDestination (target.transform.position);
			if (Vector3.Distance (ai.transform.position, target.transform.position) <= 1f) {

				ai.ChangeState (new Attack ());
				return;
			}

            if (Vector3.Distance(ai.transform.position, target.transform.position) <= ai.scentRadius)
            {
                // we are close enough, so chase the target further on
                // destination of nav-agent is set to the current target in order to chase the target.
                NavMeshAgent agent = ai.GetAgent();

                if (agent != null)
                {
                    agent.stoppingDistance = target.GetComponent<NavMeshAgent>().baseOffset;
                    agent.destination = target.transform.position;
                }

            }
            else
            {
                // we have lost the scent of the player, go back home
                Debug.Log("Lost scent of the player! Going back home...");
                ai.ChangeState(new ReturnHome());
            }

            // we are too far from home, go back again
            if (Vector3.Distance(ai.transform.position, ai.initialPosition) > ai.maximalRadius)
            {
                Debug.Log("Oh dear, my home is far far away, i want to go back home....");
                ai.ChangeState(new ReturnHome());
            }
        }
    }


    public override void Exit(EnemyAI ai)
    {
        Debug.Log("Exit hunt");
    }
}

class ReturnHome : State<EnemyAI>
{
    public override void Enter(EnemyAI ai)
    {
        Debug.Log("Enter return home");
        ai.ResetTarget();
    }

    public override void Execute(EnemyAI ai)
    {
        NavMeshAgent agent = ai.GetAgent();
        if (agent != null)
        {
            agent.stoppingDistance = 0;
            agent.destination = ai.initialPosition;

            if (Vector3.Distance(agent.transform.position, ai.initialPosition) < ai.homeZoneRadius)
            {
                // we have are close enough to the starting point, start patroling again
                Debug.Log("Smells like home again....");
                ai.ChangeState(new Patrol());
            }
        }

    }

    public override void Exit(EnemyAI ai)
    {
        Debug.Log("Exit return home");
    }
}

class Attack : State<EnemyAI> {

	public override void Enter (EnemyAI ai) {

		Debug.Log ("Enter Attack");
		ai.GetAgent ().SetDestination (ai.transform.position);
	}

	public override void Execute (EnemyAI ai) {

		if (ai.GetTarget () == null || ai.GetTarget ().GetComponent<Character> () == null) {

			ai.ChangeState (new Patrol ());
			return;
		}

		if (Vector3.Distance (ai.transform.position, ai.GetTarget ().transform.position) > 1.2f) {

			ai.ChangeState (new Hunt ());
			return;
		}

		ai.GetComponent<Enemy> ().Attack (ai.GetTarget ().GetComponent<Character> ());
	}

	public override void Exit (EnemyAI ai) {

		Debug.Log ("Exit Attack");
	}
}

[RequireComponent (typeof (NavMeshAgent))]
public class EnemyAI : MonoBehaviour {

    [Header("AI Data")]
    // Within this radius the enemy recognizes the player - then he will target the player and he will start chasing him
    public float awarenessRadius = 2.0f;

    // outside this radius the enemy wants to go gack to the initial position. The enemy will lose his target
    public float maximalRadius = 5.0f;

    // if the distance from the enemy to the target is larger than this float, the enemy will also go back to the initial position and he will lose his target
    public float scentRadius = 3.0f;

    /* if the enemy is on his way back home and the distance left is less than this float, the enemy considers himself being at home again.
     * In particular, the enemy will change to hunting mode again, if the player is close */
    public float homeZoneRadius = 1.0f;

    // the target the enemy is chasing (usually the player)
    protected GameObject currentTarget = null;

    // Initial position of the Enemy. This is where the Enemy will go back if he is not chasing the player anymore
    public Vector3 initialPosition;

    // stores current state the AI is in (patrolling, hunting, ...)
    private State<EnemyAI> currentState;

    // reference to the nav agent component
   protected NavMeshAgent agent;

    public void Start()
    {
        initialPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();

        ChangeState(new Patrol());
    }

 
    void Update ()
    {
        if (currentState != null)
            currentState.Execute(this);
	}

    // A new Target for the enemy to chase is set and the State now is hunting
    public void SetNewTarget(GameObject prey)
    {
        currentTarget = prey;
    }



    public void ChangeState (State<EnemyAI> newState)
    {
        if (newState != null)
        {
            // exit old state
            if (currentState != null)
                currentState.Exit(this);

            // set new current state and enter it
            currentState = newState;
            currentState.Enter(this);
            
        }
    }

    public NavMeshAgent GetAgent()
    {
        return agent;
    }

    public GameObject GetTarget()
    {
        return currentTarget;
    }

    public void ResetTarget()
    {
        if (currentTarget != null)
            currentTarget = null;
    }
}

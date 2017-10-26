using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class BoneWolfAI : MonoBehaviour
{
    NavMeshAgent agent;
    GameObject player;
    Animator animator;
    public float walkRadius = 5;
    State state = State.Howl;
    public float wanderSpeed = 2;
    public float chargeSpeed = 10;
    public float chaseSpeed = 5;
    Vector3 chargeDirection;

    enum State
    {
        Wander,
        Howl,
        ChargeAttack,
        Attack
    };


    float stateTimer;


    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Wander:
                UpdateWander();
                break;
            case State.Howl:
                StartHowl();
                break;
            case State.ChargeAttack:
                UpdateWander();
                break;
            case State.Attack:
                UpdateAttack();
                break;

        }




    }

    void UpdateWander()
    {
        agent.speed = wanderSpeed;
        animator.SetFloat("Speed", agent.velocity.magnitude / 2);
        if (agent.remainingDistance < .1)
        {
            // find a random point nearby and move to it
            Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
            Vector3 finalPosition = hit.position;

            agent.SetDestination(finalPosition);
        }
        // do a line of sight check to player, and if we see him Howl

    }

    void UpdateCharge()
    {
        // move fast along charge vector
        // if we hit player, do charge damage
        // if we hit a wall, do damage/stun to us

        //when finihsed, startattack
    }

    void UpdateAttack()
    {
        //play animationm
        // do damage if player in range
        // countdown
        agent.speed = wanderSpeed;
        animator.SetFloat("Speed", agent.velocity.magnitude / 2);
        agent.SetDestination(player.transform.position);
        if (Vector3.Distance(transform.position, player.transform.position) < 2)
        {
            //attack
            animator.SetBool("Attacking", true);
            agent.isStopped = true;
        }
        else
        {
            animator.SetBool("Attacking", false);
            agent.isStopped = false;
        }


        //when finihsed, startattack
    }

    void StartHowl()
    {
        animator.SetBool("Howling", true);
        StartCoroutine(Howling());
    }


    IEnumerator Howling()
    {
        yield return new WaitForSeconds(1f);
        while (!animator.IsInTransition(0))
        {
            yield return new WaitForSeconds(.01f);
        }
        animator.SetBool("Howling", false);
        ChooseAttack();
    }


    void ChooseAttack()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < 10.f)
            StartAttack();
        else
            StartCharge();
    }

    void StartCharge()
    {
        chargeDirection = (player.transform.position - transform.position).normalized;
        state = State.ChargeAttack;
    }

    void StartAttack()
    {
        state = State.Attack;
        // if the player is far away, set state to charge,
        // other wise set state to attack
    }

}

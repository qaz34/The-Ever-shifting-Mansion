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
    State state = State.Wander;
    public float wanderSpeed = 2;
    public float chargeSpeed = 10;
    public float chaseSpeed = 5;



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
                UpdateHowl();
                break;
            case State.ChargeAttack:
                UpdateWander();
                break;
            case State.Attack:
                UpdateWander();
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


    void UpdateHowl()
    {
        // count down timer, and then StartAttack
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

        //when finihsed, startattack
    }

    void StartHowl()
    {
        state = State.Howl;
        // animmation

        // noise
    }

    void StartCharge()
    {
        state = State.ChargeAttack;
    }

    void StartAttack()
    {
        state = State.Attack;
        // if the player is far away, set state to charge,
        // other wise set state to attack
    }

}

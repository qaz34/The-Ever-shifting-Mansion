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
    State state = State.Attack;
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


    public float stateTimer;


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
               // UpdateHowl();
                break;
            case State.ChargeAttack:
                UpdateCharge();
                break;
            case State.Attack:
                UpdateAttack();
                break;

        }

        if (state == State.ChargeAttack)
            agent.speed = chargeSpeed;
        else
            agent.speed = chaseSpeed;

        if (stateTimer >= 0)
        {
            stateTimer -= Time.deltaTime;
        }
        else
        {
            ChooseAttack();
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


           
            agent.SetDestination(transform.position + chargeDirection * 10);

    


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
        if (Vector3.Distance(transform.position, player.transform.position) < 2f)
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
        if (Vector3.Distance(player.transform.position, transform.position) < 2f)
            StartAttack();
        else
            StartCharge();
    }

    void StartCharge()
    {
        // for the next 3 seconds, move in thjis direction
        chargeDirection = (player.transform.position - transform.position).normalized;

        stateTimer = 2;
        state = State.ChargeAttack;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state == State.ChargeAttack)
        {
            if (other.tag == "Player")
                Destroy(other.gameObject);
            state = State.Howl;
        }
        else
        {
            //do nothing
        }
    }

    void StartAttack()
    {
        stateTimer = 2;
        state = State.Attack;
        // if the player is far away, set state to charge,
        // other wise set state to attack
    }

}

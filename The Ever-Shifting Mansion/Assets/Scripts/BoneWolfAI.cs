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
    public float walkRadius;
    public State state = State.Wander;
    public float wanderSpeed;
    public float chargeSpeed;
    public float stunTime;
    Vector3 chargeDirection;

    public enum State
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
                StartHowl();
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
            agent.speed = wanderSpeed;

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


        agent.enabled = false;
        if (Vector3.Dot(agent.transform.forward, chargeDirection) > 0.8f)
        {
            agent.transform.position += chargeSpeed * chargeDirection * Time.deltaTime;
        }
        //agent.SetDestination(transform.position + chargeDirection * 10);

        agent.transform.forward = Vector3.Lerp(agent.transform.forward, chargeDirection, .1f);


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

    IEnumerator Stunned()
    {
        yield return new WaitForSeconds(stunTime);
        animator.SetBool("Stunned", false);
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
        chargeDirection = (player.transform.position - transform.position);
        chargeDirection.y = 0;
        chargeDirection.Normalize();

        stateTimer = 3;
        state = State.ChargeAttack;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state == State.ChargeAttack)
        {
            if (other.tag == "Player")
                player.GetComponent<Health>().CurrentHealth -= 40;
            if (other.tag == "Stun")
            {
                animator.SetBool("Stunned", true);
                
                Stunned();
            }
            
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

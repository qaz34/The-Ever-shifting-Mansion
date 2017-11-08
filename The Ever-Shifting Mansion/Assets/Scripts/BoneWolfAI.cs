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
    public float chargeTime;
    public bool hasSeen = false;

    public enum State
    {
        Wander,
        Howl,
        ChargeAttack,
        Attack,
        Search
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
        if (stateTimer > 0)
        {
            stateTimer -= Time.deltaTime;
        }
        switch (state)
        {
            case State.Wander:
                UpdateWander();
                break;
            case State.Howl:
                UpdateHowl();
                break;
            case State.ChargeAttack:
                UpdateCharge();
                break;
            case State.Attack:
                UpdateAttack();
                break;
            case State.Search:
                UpdateSearch();
                break;

        }
    }
    void StartSearch()
    {
        //set search anim
        //state timer for wait before moving
    }
    void UpdateSearch()
    {
        //check to see player infront
        //wait for state timer
        //move to find player
        //one found player choose attack
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

            stateTimer = 0;
        }

        // do a line of sight check to player, and if we see him Howl
        {
            RaycastHit hit;
            Vector3 toPlayer = player.transform.position - transform.position;
            Physics.Raycast(transform.position, toPlayer.normalized, out hit, toPlayer.magnitude);
            float angle = Vector3.Angle(transform.forward, player.transform.position);
            if ((hit.transform && (angle < 45 && hit.transform.tag == "Player")))
            {
                hasSeen = true;
            }
        }
        if (hasSeen)
        {
            StartHowl();
        }
    }
    void StartCharge()
    {
        // for the next 3 seconds, move in this direction
        agent.enabled = false;
        chargeDirection = (player.transform.position - transform.position);
        chargeDirection.y = 0;
        chargeDirection.Normalize();
        agent.speed = chargeSpeed;
        stateTimer = chargeTime;
        state = State.ChargeAttack;
    }
    void UpdateCharge()
    {
        if (stateTimer <= 0)
        {

        }
        if (Vector3.Dot(agent.transform.forward, chargeDirection) > 0.8f)
        {
            agent.transform.position += chargeSpeed * chargeDirection * Time.deltaTime;
        }
        agent.transform.forward = Vector3.Lerp(agent.transform.forward, chargeDirection, .1f);
    }

    void StartAttack()
    {
        stateTimer = 2;
        state = State.Attack;

    }
    void UpdateAttack()
    {
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
        state = State.Howl;
        animator.SetBool("Howling", true);
        StartCoroutine(Howling());
    }
    void UpdateHowl()
    {

    }


    IEnumerator Howling()
    {
        yield return new WaitForSeconds(1f);
        while (!animator.IsInTransition(0))
        {
            yield return new WaitForSeconds(.01f);
        }
        Debug.Log("HowlDone");
        animator.SetBool("Howling", false);
        ChooseAttack();
    }
    void StartStunned()
    {
        StartCoroutine(Stunned());
    }
    IEnumerator Stunned()
    {
        animator.SetBool("Stunned", true);
        stateTimer = stunTime;
        yield return new WaitForSeconds(stunTime);
        animator.SetBool("Stunned", false);
        ChooseAttack();
    }

    void ChooseAttack()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < 2f)
            StartAttack();
        if (Vector3.Distance(player.transform.position, transform.position) > 2.5f && Vector3.Distance(player.transform.position, transform.position) < 10f)
            StartCharge();
        if (Vector3.Distance(player.transform.position, transform.position) >= 10f && !hasSeen)
            state = State.Wander;
    }

 

    private void OnTriggerEnter(Collider other)
    {
        if (state == State.ChargeAttack)
        {
            if (other.tag == "Player")
                player.GetComponent<Health>().CurrentHealth -= 40;
            if (other.tag == "Stun")
            {
                StartStunned();
            }
            state = State.Howl;
        }
    }


}

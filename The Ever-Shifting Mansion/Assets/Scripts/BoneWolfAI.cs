using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class BoneWolfAI : MonoBehaviour
{
    NavMeshAgent agent;
    GameObject player;
    Animator animator;
    AudioSource audioSource;
    public float walkRadius;
    public State state = State.Wander;
    public float wanderSpeed;
    public float chargeSpeed;
    public float stunTime;
    Vector3 chargeDirection;
    public float chargeTime;
    public bool hasSeen = false;
    bool isAttacking = false;

    public enum State
    {
        Wander,
        Howl,
        ChargeAttack,
        Attack,
        Search,
        Stunned
    };


    public float stateTimer;


    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
            case State.Stunned:
                UpdateStunned();
                break;

        }
    }
    void StartSearch()
    {
        //set search anim
        animator.SetBool("Search", true);
        //state timer for wait before moving
        agent.enabled = true;
        state = State.Search;
        stateTimer = 2;

    }
    void UpdateSearch()
    {
        if (stateTimer <= 0)
        {
            {
                agent.speed = wanderSpeed * 2;
                walkRadius = 5;
                animator.SetFloat("Speed", agent.velocity.magnitude / 2);
                if (agent.remainingDistance < .1)
                {
                    // find a random point nearby and move to it
                    Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
                    randomDirection += transform.position;
                    NavMeshHit aiHit;
                    NavMesh.SamplePosition(randomDirection, out aiHit, walkRadius, 1);
                    Vector3 finalPosition = aiHit.position;

                    agent.SetDestination(finalPosition);

                    stateTimer = 0;
                }


                RaycastHit hit;
                Vector3 toPlayer = player.transform.position - transform.position;
                Physics.Raycast(transform.position, toPlayer.normalized, out hit, toPlayer.magnitude);
                float angle = Vector3.Angle(player.transform.position - transform.position, transform.forward);
                if (hit.transform && (angle < 90 && hit.transform.tag == "Player" && toPlayer.magnitude < 10f))
                {
                    hasSeen = true;
                }
            }
            if (hasSeen)
            {
                StartHowl();
            }

            //wait for state timer
            stateTimer = 0;
            animator.SetBool("Search", false);
        }
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
            float angle = Vector3.Angle(player.transform.position - transform.position, transform.forward);
            if (hit.transform && angle < 45 && hit.transform.tag == "Player" && toPlayer.magnitude < 10f)
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
            StartSearch();
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
        bool withinRange = (Vector3.Distance(transform.position, player.transform.position) < 2f);
        if (!isAttacking && withinRange)
        {
            stateTimer = 1; //animTime     
            isAttacking = true;
            animator.SetBool("Attacking", true);
            agent.isStopped = true;
        }

        if (isAttacking && !withinRange)
        { 
           animator.SetBool("Attacking", false);
           agent.isStopped = false;
        }

        if (Vector3.Distance(transform.position, player.transform.position) > 8f)
        {
            StartHowl();
            agent.isStopped = true;
        }

        if (stateTimer <= 0)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < 3f)
            {
                player.GetComponent<Health>().CurrentHealth -= 20;
                Debug.Log(player.GetComponent<Health>().CurrentHealth);

            }

            isAttacking = false;
        }
        if (isAttacking)
        {
            agent.transform.forward = Vector3.Lerp(agent.transform.forward, chargeDirection, .1f);

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
        Debug.Log("start howl");
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
        state = State.Stunned;
        StartCoroutine(Stunned());
    }
    void UpdateStunned()
    {

    }
    IEnumerator Stunned()
    {
        animator.SetBool("Stunned", true);
        yield return new WaitForSeconds(stunTime);
        animator.SetBool("Stunned", false);
        StartAttack();
    }

    void ChooseAttack()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < 4f)
            StartAttack();
        if (Vector3.Distance(player.transform.position, transform.position) >= 4f)
            StartCharge();

    }



    private void OnTriggerEnter(Collider other)
    {
        if (state == State.ChargeAttack)
        {
            agent.enabled = true;
            if (other.tag == "Player")
            {
                player.GetComponent<Health>().CurrentHealth -= 40;
                Debug.Log(player.GetComponent<Health>().CurrentHealth);
                ChooseAttack();
            }
            if (other.tag == "Stun")
            {
                StartStunned();
            }
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class HuskAI : MonoBehaviour
{
    NavMeshAgent agent;
    GameObject player;
    public Weapon weapon;
    float lastAttacked;
    public bool hasSeen = false;

    public AnimationCurve speedDistanceTrigger;
    Vector3 prevPos = new Vector3();
    Vector3 prevDir = new Vector3();
    Animator animator;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<CombatController>().fired += FiredWeapon;
        prevPos = transform.position;
        prevDir = transform.forward;
    }
    void FiredWeapon()
    {
        hasSeen = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (!hasSeen)
        {
            int mask = 1 << LayerMask.NameToLayer("Ignore Raycast");
            mask = ~mask;
            RaycastHit hit;
            Vector3 toPlayer = player.transform.position - transform.position;
            Physics.Raycast(transform.position, toPlayer.normalized, out hit, toPlayer.magnitude);
            CharacterCont cc = player.GetComponent<CharacterCont>();
            float speed = cc.currentSpeed;
            float distance = Vector3.Distance(transform.position, player.transform.position);
            float inverseTime = (speed / distance);
            float angle = Vector3.Angle(transform.forward, player.transform.position - transform.position);
            if (inverseTime > 1 || (hit.transform && (angle < 45 && hit.transform.tag == "Player") && hit.transform.GetComponent<CharacterCont>().currentSpeed > .1f))
            {
                hasSeen = true;
            }
        }
        if (hasSeen)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < (weapon ? ((MeleeWep)weapon).arcRadius : 1))
            {
                transform.forward = player.transform.position - transform.position;

                if (weapon && Time.time - lastAttacked > weapon.fireRate)
                {
                    lastAttacked = Time.time;
                    player.GetComponent<Health>().CurrentHealth -= weapon.damage;
                    animator.SetTrigger("AttackWeak");
                }
                agent.SetDestination(transform.position);
            }
            else
            {
                agent.SetDestination(player.transform.position);

            }
        }

        UpdateAnimator();
        prevPos = transform.position;
        prevDir = transform.forward;
    }

    void UpdateAnimator()
    {
        float speed = (transform.position - prevPos).magnitude / Time.deltaTime;
        float turn = Vector3.SignedAngle(prevDir, transform.forward, Vector3.up);
        animator.SetFloat("Move", (speed / agent.speed));
        animator.SetFloat("Turn", turn);
    }
}

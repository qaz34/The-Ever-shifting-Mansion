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
    List<Vector3> positions = new List<Vector3>();
    bool hasSeen = false;
    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.enabled == false)
        {
            if (Physics.Raycast(transform.position, -transform.up, 1))
            {
                agent.enabled = true;
                GetComponent<Rigidbody>().isKinematic = true;
            }
            return;
        }
        int mask = 1 << LayerMask.NameToLayer("Target") | 1 << LayerMask.NameToLayer("Ignore Raycast");
        mask = ~mask;
        RaycastHit hit;
        Vector3 toPlayer = player.transform.position - transform.position;
        if (Physics.Raycast(transform.position, toPlayer.normalized, out hit, toPlayer.magnitude, mask))
        {
            StartCoroutine(SetPath());
            positions = new List<Vector3>();
            if (hit.transform.tag == "Player")
            {
                agent.SetDestination(player.transform.position);
            }
        }
        else if (hasSeen)
        {
            if (positions.Count > 0)
            {
                if (agent.remainingDistance < 1)
                    positions.Remove(positions[0]);
                agent.SetDestination(positions[0]);
            }
        }
        if (Vector3.Distance(transform.position, player.transform.position) < 2)
        {
            agent.isStopped = true;
            if (Time.time - lastAttacked > weapon.fireRate)
            {
                lastAttacked = Time.time;
                Debug.Log("Done doing an attack");
                player.GetComponent<Health>().CurrentHealth -= weapon.damage;
            }
        }
        else
            agent.isStopped = false;
    }
    public void KnockBack(Vector3 force)
    {
        agent.enabled = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);
    }
    IEnumerator SetPath()
    {
        for (;;)
        {
            positions.Add(player.transform.position);
            yield return new WaitForSeconds(.5f);
        }
    }
}

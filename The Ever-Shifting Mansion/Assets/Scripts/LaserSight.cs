using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class LaserSight : MonoBehaviour
{
    LineRenderer lr;
    // Use this for initialization
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        lr.SetPositions(new Vector3[] { transform.position, (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity)) ? hit.point : transform.forward * 1000 });
    }
}

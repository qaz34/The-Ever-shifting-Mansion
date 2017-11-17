using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Melee", menuName = "Weapons/Melee", order = 1)]
public class MeleeWep : Weapon
{
    [Tooltip("Swings Per Second")]
    public float arcRadius = 1;
    public float arcAngle = 10;

    public float knockBackForce = 1;
    public float knockBackAngle = 10;
    public override bool Fire(Transform position)
    {
        var hits = Physics.OverlapSphere(position.position, arcRadius, 1 << LayerMask.NameToLayer("Target"));
        foreach (var hit in hits)
        {
            var to = (hit.transform.position - position.position);
            to.y = 0;
            if (Vector3.Angle(position.forward, to) < arcAngle)
            {
                hit.transform.GetComponent<Health>().CurrentHealth -= damage;
            }
        }
        return true;
    }
}

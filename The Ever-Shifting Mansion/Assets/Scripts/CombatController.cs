using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using InControl;
using System.Linq;
public class CombatController : MonoBehaviour
{

    public Weapon equipWeapon;
    public float rotateSpeed;
    public float snapAngle = 25;
    public float snapSpeed = .1f;
    public GameObject particleEffect;

    CharacterCont charCont;
    float timeLastShot;
    bool switched;
    bool startAim;
    List<Target> validTargetsSorted = new List<Target>();
    int currentTargetIndex;
    struct Target
    {
        public float xPos;
        public GameObject obj;
        public Target(float xPosition, GameObject theObj)
        {
            xPos = xPosition;
            obj = theObj;
        }
    }

    List<GameObject> GetAllTargets()
    {
        var all = FindObjectsOfType<GameObject>();
        List<GameObject> targetList = new List<GameObject>();
        foreach (var target in all)
            if (target.layer == LayerMask.NameToLayer("Target"))
                targetList.Add(target);
        return targetList;
    }
    // Use this for initialization
    void Start()
    {
        charCont = GetComponent<CharacterCont>();
    }
    void GetValidTargets()
    {
        var hits = GetAllTargets();
        var validTargets = new List<Target>();
        int mask = 1 << LayerMask.NameToLayer("Target") | 1 << LayerMask.NameToLayer("Ignore Raycast");
        mask = ~mask;
        foreach (var target in hits)
        {
            if (!Physics.Raycast(transform.position, (target.transform.position - transform.position).normalized, (target.transform.position - transform.position).magnitude, mask))
            {
                validTargets.Add(new Target(Vector3.Distance(transform.position, target.transform.position), target));
            }
        }
        if (validTargets.Count > 0)
        {
            validTargets.Sort(delegate (Target a, Target b) { return a.xPos.CompareTo(b.xPos); });
            int closestTarget = 0;
            float distance = Mathf.Infinity;
            for (int i = 0; i < validTargets.Count; i++)
            {
                float dis = Vector3.Distance(transform.position, validTargets[i].obj.transform.position);
                if (dis < distance)
                {
                    distance = dis;
                    closestTarget = i;
                }
            }
            validTargetsSorted = validTargets;
        }
    }
    bool Within(float num, float a)
    {
        return (num > -a && num < a);
    }
    // Update is called once per frame
    void Update()
    {

        InputDevice device = InputManager.ActiveDevice;
        if (device.LeftTrigger.IsPressed)
        {
            charCont.aiming = true;
            float rotateAmount = device.LeftStickX * rotateSpeed;
            transform.Rotate(transform.up, rotateAmount);
            if (Within(device.LeftStickX, .1f))
            {
                GetValidTargets();
                Target closestInAngle = new Target(0, null);
                foreach (var target in validTargetsSorted)
                {
                    Vector3 to = target.obj.transform.position - transform.position;
                    if (Vector3.Angle(transform.forward, to.normalized) < snapAngle)
                    {
                        closestInAngle = target;
                        break;
                    }
                }
                if (closestInAngle.obj != null)
                {
                    transform.forward = Vector3.Lerp(transform.forward, (closestInAngle.obj.transform.position - transform.position).normalized, snapSpeed);
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                }
            }          
            if (device.RightTrigger.WasPressed)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit))
                {
                    Health targetHealth = hit.transform.GetComponent<Health>();
                    if (targetHealth)
                    {
                        targetHealth.CurrentHealth -= equipWeapon.damage;
                        GameObject particle = Instantiate(particleEffect);
                        particle.transform.position = hit.point;
                        particle.transform.up = (hit.transform.position - transform.position).normalized;
                        particle.transform.parent = hit.transform.parent;
                    }
                }
            }
        }
        else
        {
            charCont.aiming = false;
            startAim = false;
            currentTargetIndex = -1;
        }
    }
}


//if (currentTargetIndex == -1)
//{
//    switched = true;
//    GetValidTargets();
//    currentTargetIndex = 0;
//}
//if (currentTargetIndex != -1)
//{
//    transform.forward = Vector3.Lerp(transform.forward, validTargetsSorted[currentTargetIndex].obj.transform.position - transform.position, .2f);


//    if (device.LeftStickX < -.5f && !switched)
//    {
//        currentTargetIndex = (currentTargetIndex - 1 < 0) ? validTargetsSorted.Count - 1 : currentTargetIndex - 1;
//        switched = true;
//    }
//    else if (device.LeftStickX > .5f && !switched)
//    {
//        currentTargetIndex = (currentTargetIndex + 1) % validTargetsSorted.Count;
//        switched = true;
//    }
//    else if (Within(device.LeftStickX, -0.4f, 0.4f))
//    {
//        switched = false;
//    }
//}
//charCont.aiming = true;
//if (device.RightTrigger.IsPressed)
//{

//    if (equipWeapon.holdToFire)
//    {
//        if (Time.time - timeLastShot > equipWeapon.fireRate)
//        {
//            timeLastShot = Time.time;

//            RaycastHit hit;
//            var dir = validTargetsSorted[currentTargetIndex].obj.transform.position - transform.position;
//            if (Physics.Raycast(transform.position, dir.normalized, out hit, dir.magnitude))
//            {
//                hit.transform.gameObject.SetActive(false);
//                currentTargetIndex = 0;
//            }
//            Debug.Log("Bang!!");

//            GetValidTargets();
//        }
//    }
//    else
//    {
//        if (device.RightTrigger.WasPressed)
//        {
//            timeLastShot = Time.time;
//            RaycastHit hit;
//            var dir = validTargetsSorted[currentTargetIndex].obj.transform.position - transform.position;
//            if (Physics.Raycast(transform.position, dir.normalized, out hit, dir.magnitude))
//            {
//                hit.transform.gameObject.SetActive(false);
//                currentTargetIndex = 0;
//            }
//            Debug.Log("Bang");
//            GetValidTargets();
//        }
//    }
//}
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
    public delegate void Fire();
    public Fire fired;
    CharacterCont charCont;
    float timeLastShot;
    bool switched;
    public Transform raycastPosition;
    AudioSource audioSource;
    List<Target> validTargetsSorted = new List<Target>();

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
        var hits = FindObjectsOfType<HuskAI>();
        var validTargets = new List<Target>();
        validTargetsSorted = new List<Target>();
        int mask = 1 << LayerMask.NameToLayer("Target") | 1 << LayerMask.NameToLayer("Ignore Raycast");
        mask = ~mask;
        foreach (var target in hits)
        {
            if (!Physics.Raycast(raycastPosition.position, (target.transform.position - transform.position).normalized, (target.transform.position - transform.position).magnitude, mask))
            {
                validTargets.Add(new Target(Vector3.Distance(transform.position, target.transform.position), target.gameObject));
            }
        }
        if (validTargets.Count > 0)
        {
            validTargets.Sort(delegate (Target a, Target b) { return a.xPos.CompareTo(b.xPos); });

            float distance = Mathf.Infinity;
            for (int i = 0; i < validTargets.Count; i++)
            {
                float dis = Vector3.Distance(transform.position, validTargets[i].obj.transform.position);
                if (dis < distance)
                    distance = dis;

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
        if (equipWeapon)
        {
            GetComponent<Animator>().SetBool("HoldingGun", true);
            InputDevice device = InputManager.ActiveDevice;
            if (device.LeftTrigger.IsPressed)
            {
                GetComponent<Animator>().SetBool("GunUp", true);
                GetComponent<Animator>().SetInteger("WepType", (int)equipWeapon.type);
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
                    if (equipWeapon.Fire(raycastPosition))
                    {
                        GetComponent<Animator>().SetTrigger("Fire");

                        // play the current weapons sounds TODO
                        if (equipWeapon.soundEffect)
                        {
                            audioSource.clip = equipWeapon.soundEffect;
                            audioSource.Play();
                        }
                    }
                    if (equipWeapon.type != WepType.MELEE)
                    {
                        fired?.Invoke();
                    }
                }
                if (device.Action3.WasPressed)
                {
                    if (equipWeapon.type != WepType.MELEE)
                        foreach (var ammo in GetComponent<Inventory>().ammo.Where(i => i.ammoType == ((RangedWep)equipWeapon).ammoType))
                        {
                            if (((RangedWep)equipWeapon).Reload(ammo) && ((RangedWep)equipWeapon).left != 0)
                            {
                                GetComponent<Animator>().SetTrigger("Reload");
                            }
                        }
                }
            }
            else
            {
                GetComponent<Animator>().SetBool("GunUp", false);
                charCont.aiming = false;
            }
        }
    }
}
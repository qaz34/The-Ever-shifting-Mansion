using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ItemContainer : MonoBehaviour
{
    void Start()
    {
        if (!GameObject.FindGameObjectWithTag("MapGen") && transform.parent == null)
        {
            RollRandom(new List<GameObject>());
        }
    }
    public void RollRandom(List<GameObject> noDestroy)
    {
        var avaliable = GetComponentsInChildren<Transform>().ToList();
        List<GameObject> objs = new List<GameObject>();

        foreach (var item in avaliable.Where(i => i.parent == transform))
            objs.Add(item.gameObject);


        var keep = objs[Random.Range(0, objs.Count)];

        foreach (var item in avaliable)
        {
            if (noDestroy.Contains(item.gameObject))
            {
                foreach (var parentObj in objs)
                {
                    if (parentObj == item.gameObject || parentObj.transform.Find(item.gameObject.name))
                    {
                        keep = parentObj;
                    }
                }
            }
        }
        foreach (var item in objs.Where(i => i.gameObject != keep))
            Destroy(item.gameObject);
        var conts = keep.GetComponentsInChildren<ItemContainer>();
        foreach (var cont in conts)
            cont.RollRandom(noDestroy);
    }
}

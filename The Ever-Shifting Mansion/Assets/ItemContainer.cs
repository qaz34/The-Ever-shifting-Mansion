using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ItemContainer : MonoBehaviour
{
    public void RollRandom()
    {
        var avaliable = GetComponentsInChildren<Transform>().ToList();
        List<GameObject> objs = new List<GameObject>();
        foreach (var item in avaliable.Where(i => i.parent == transform))
            objs.Add(item.gameObject);
        var keep = objs[Random.Range(0, objs.Count)];
        foreach (var item in objs.Where(i => i.transform != keep))
            Destroy(item.gameObject);
        var conts = keep.GetComponentsInChildren<ItemContainer>();
        foreach (var cont in conts)
            cont.RollRandom();
    }
}

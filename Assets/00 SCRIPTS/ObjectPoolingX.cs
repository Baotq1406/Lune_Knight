using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingX : Singleton<ObjectPoolingX>
{
    Dictionary<GameObject, List<GameObject>> _poolObjects = new Dictionary<GameObject, List<GameObject>>();

    public GameObject GetObject(GameObject key)
    {
        List<GameObject> _itemPool = new List<GameObject>();
        if (!_poolObjects.ContainsKey(key))
        {
            _poolObjects.Add(key, _itemPool);
        }
        else
        {
            _itemPool = _poolObjects[key];
        }


        foreach (GameObject g in _itemPool)
        {
            if (g.gameObject.activeSelf)
                continue;
            return g;
        }

        GameObject g2 = Instantiate(key, this.transform.position, Quaternion.identity);
        _poolObjects[key].Add(g2);
        return g2;
    }
}

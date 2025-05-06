using System;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    public static GameObjectManager instance;
    public List<ObjectWithTag> objs;

    private void Awake()
    {
        instance = this;
    }

    public GameObject FindGameObjectWithTag(string tag)
    {
        foreach (var obj in objs)
            if (obj.tag.Equals(tag))
                return obj.gameObject;

        return null;
    }

    [Serializable]
    public class ObjectWithTag
    {
        public string tag;
        public GameObject gameObject;
    }
}
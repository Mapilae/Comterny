using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundHolder", menuName = "ScriptableObjects/SoundHolder", order = 2)]
public class SoundHolder : ScriptableObject
{
    public List<DAudioClip> bgmList;
    public List<DAudioClip> seList;

    public AudioClip FindBGM(string name)
    {
        var item = bgmList.Find(x => Clean(x.name) == Clean(name));

        if (item.clip != null)
            return item.clip;
        Debug.LogWarning($"BGM '{name}' not found in bgmList.");
        return null;
    }

    public AudioClip FindSE(string name)
    {
        var item = seList.Find(x => Clean(x.name) == Clean(name));

        if (item.clip != null)
            return item.clip;
        Debug.LogWarning($"Sound effect '{name}' not found in seList.");
        return null;
    }

    private string Clean(string s)
    {
        return s.Trim().Replace("\r", "").Replace("\n", "");
    }


    [Serializable]
    public struct DAudioClip
    {
        public string name;
        public AudioClip clip;
    }
}
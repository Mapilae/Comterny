using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SoundHolder", menuName = "ScriptableObjects/SoundHolder", order = 2)]
public class SoundHolder : ScriptableObject
{
    [System.Serializable]
    public struct DAudioClip
    {
        public string name;
        public AudioClip clip;
    }
    public List<DAudioClip> bgmList;
    public List<DAudioClip> seList;

    public AudioClip FindBGM(string name)
    {
        return bgmList.Find(x => x.name == name).clip;
    }

    public AudioClip FindSE(string name)
    {
        return seList.Find(x => x.name == name).clip;
    }

}

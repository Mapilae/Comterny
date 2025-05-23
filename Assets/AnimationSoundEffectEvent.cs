using System.Collections.Generic;
using UnityEngine;

public class AnimationSoundEffectEvent : MonoBehaviour
{
    public List<string> clips;

    public void PlaySound(int index)
    {
        //반복 없는 단발재생 효과음
        SoundManager.instance.PlaySound(clips[index]);
    }

    public void PlayAmbience(int index)
    {
        Debug.Log("ambience try:" + gameObject.name);
        //loop 하는 효과음
        SoundManager.instance.PlayAmbience(clips[index]);
    }
}
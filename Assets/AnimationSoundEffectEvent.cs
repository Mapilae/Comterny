using System.Collections.Generic;
using UnityEngine;

public class AnimationSoundEffectEvent : MonoBehaviour
{
    public List<AudioClip> clips;

    public void PlaySound(int index)
    {
        SoundManager.instance.PlaySound(clips[index]);
    }
}
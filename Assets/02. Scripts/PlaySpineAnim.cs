using Spine;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;

public class PlaySpineAnim : MonoBehaviour
{
    public SkeletonAnimation skelAnim;
    public AnimationState animState;
    public Skeleton skeleton;
    
    
    [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)]
    public string eventName;

    [Space]
    public AudioSource audioSource;
    public AudioClip audioClip;
    public float basePitch = 1f;
    public float randomPitchOffset = 0.1f;

    Spine.EventData eventData;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        animState = skelAnim.AnimationState;
        skeleton = skelAnim.Skeleton;
        skelAnim.Initialize(false);
        eventData = skelAnim.Skeleton.Data.FindEvent(eventName);
        skelAnim.AnimationState.Event += HandleAnimationStateEvent;
    }

    public void SetAnim(string animName, bool isLoop)
    {
        if (skelAnim.AnimationName.Equals(animName)) return;
        //Debug.Log("Change Animation To : " + animName);
        animState.SetAnimation(0, animName, isLoop);
    }
    
    private void HandleAnimationStateEvent (TrackEntry trackEntry, Spine.Event e) {
        bool eventMatch = (eventData == e.Data); // Performance recommendation: Match cached reference instead of string.
        if (eventMatch) {
            Play();
        }
    }

    public void Play () {
        audioSource.pitch = basePitch + Random.Range(-randomPitchOffset, randomPitchOffset);
        audioSource.clip = audioClip;
        audioSource.Play();
    }
    
}
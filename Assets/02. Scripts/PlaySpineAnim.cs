using Spine;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;

public class PlaySpineAnim : MonoBehaviour
{
    public SkeletonAnimation skelAnim;
    public AnimationState animState;
    public Skeleton skeleton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        animState = skelAnim.AnimationState;
        skeleton = skelAnim.Skeleton;
    }

    public void SetAnim(string animName, bool isLoop)
    {
        if (skelAnim.AnimationName.Equals(animName)) return;
        //Debug.Log("Change Animation To : " + animName);
        animState.SetAnimation(0, animName, isLoop);
    }
}
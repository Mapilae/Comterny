using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CutsceneStep
{
    [TextArea] public string memo; //사용하지 않음.
    public StepEdge stepStart;
    public StepMiddle stepMiddle;
    public StepEdge stepEnd;
}

[Serializable]
public class MovingTransform
{
    public Transform target;
    public Transform startTrans;
    public Transform destinationTrans;
    public Vector3 destination;
    public float preWait;
    public float moveDuration = 1f;
    public float postArrivalWait;
    [HideInInspector] public Vector3 startPos;
    public GameObject objToHide;
    public GameObject objToEnable;
}


[Serializable]
public class StepEdge
{
    public string dirCmd;
    public List<AnimatorTrigger> animTriggers;
}

[Serializable]
public class StepMiddle
{
    public List<MovingTransform> movingObjects; // 카메라도 여기에 포함됨
}


[Serializable]
public class AnimatorTrigger
{
    public Animator animator;
    public string triggerName;
}


public class CutsceneManager : MonoBehaviour
{
    public int currentStep;
    public int currentMidStep;
    public List<CutsceneStep> steps;
    private DirectingManager directingManager;

    private void OnEnable()
    {
        directingManager = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DirectingManager>();
        if (steps == null)
        {
            Debug.Log("steop is null");
            return;
        }
        if (steps.Count == 0) {
            Debug.Log("steps is empty");
            return; //비어있는 경우 시작 안함
        }
        PlayerMovement.isCutscene = true;
        StopAllCoroutines();
        StartCoroutine(StepStart());
    }

    private IEnumerator StepStart()
    {
        yield return null;
        if (MapOveray.instance.gameObject.activeInHierarchy&&!MapOveray.isOFF){
            Debug.Log("MapOveray is ON, waiting for it to turn OFF");
            yield return new WaitUntil(() => MapOveray.isOFF);}
        else
            yield return null;
        currentStep = 0;
        currentMidStep = 0;
        MobileUI.instance.HideThis(true);
        CameraFollow.instance.ExcludeLayerInCamera(true);
        directingManager.LetterboxOn();
        StartCoroutine(RunStep(currentStep));
    }

    private void EndCutscene()
    {
        CameraFollow.doFollowPlayer = true;
        PlayerMovement.isCutscene = false;
        CameraFollow.instance.ExcludeLayerInCamera(false);
        MobileUI.instance.HideThis(false);
        directingManager.LetterboxOff();
    }

    private IEnumerator RunStep(int stepIndex)
    {
        if (stepIndex >= steps.Count)
        {
            //end ALL
            EndCutscene();
            yield return new WaitForSeconds(1.5f);
            this.gameObject.SetActive(false);
            yield break;
        }

        var step = steps[stepIndex];


        // Start Command
        if (!string.IsNullOrEmpty(step.stepStart.dirCmd))
        {
            Debug.Log($"StartCmd: {step.stepStart.dirCmd}");
            directingManager.RunCMD(step.stepStart.dirCmd);
        }

        // Trigger animations at start
        foreach (var trigger in step.stepStart.animTriggers)
            trigger.animator.SetTrigger(trigger.triggerName);

        // Move all objects (including camera)
        var postWaits = new List<float>();
        currentMidStep = 0;

        foreach (var moveObj in step.stepMiddle.movingObjects)
        {
            StartCoroutine(MoveTransformWithWaits(moveObj));
            postWaits.Add(moveObj.preWait + moveObj.moveDuration + moveObj.postArrivalWait);
            var time = moveObj.preWait + moveObj.moveDuration + moveObj.postArrivalWait;
            if (time > 0f) yield return new WaitForSeconds(time);
            else yield return null;
        }

        // Wait for the maximum total time
        var maxTotalTime = postWaits.Count > 0 ? Mathf.Max(postWaits.ToArray()) : 0f;
        yield return new WaitForSeconds(maxTotalTime);

        // Trigger animations at end
        foreach (var trigger in step.stepEnd.animTriggers)
            trigger.animator.SetTrigger(trigger.triggerName);

        // End Command
        if (!string.IsNullOrEmpty(step.stepEnd.dirCmd))
        {
            Debug.Log($"EndCmd: {step.stepEnd.dirCmd}");
            directingManager.RunCMD(step.stepEnd.dirCmd);
        }

        yield return new WaitUntil(() => !DialogueManager.isPlayingDialogue);
        currentStep++;
        StartCoroutine(RunStep(currentStep));
    }

    private IEnumerator MoveTransformWithWaits(MovingTransform moveObj)
    {
        if (moveObj.objToHide != null) moveObj.objToHide.SetActive(false);
        if (moveObj.objToEnable != null) moveObj.objToEnable.SetActive(true);
        var isLocal = false;
        if (moveObj.destinationTrans != null)
            moveObj.destination = moveObj.destinationTrans.position;
        else isLocal = true;

        if (moveObj.target != null)
        {
            if (moveObj.startTrans != null)
                moveObj.startPos = moveObj.startTrans.position;
            else moveObj.startPos = moveObj.target.position;

            if (moveObj.target.GetComponent<CameraFollow>())
                CameraFollow.doFollowPlayer = false;

            yield return new WaitForSeconds(moveObj.preWait);


            var start = moveObj.startPos;
            var newPos = moveObj.destination;
            var end = new Vector3(newPos.x, newPos.y,
                isLocal ? moveObj.target.localPosition.z : moveObj.target.position.z);
            var elapsed = 0f;

            if (moveObj.target.GetComponent<PlayerMovement>() && moveObj.moveDuration > 0f)
            {
                var dist = end.x - start.x;
                PlayerMovement.instance.PlayerSpineAnimWalk();
                if (dist != 0f)
                    PlayerMovement.instance.SetPlayerLookingDirection(dist < 0);
            }

            while (elapsed < moveObj.moveDuration)
            {
                elapsed += Time.deltaTime;
                if (isLocal)
                    moveObj.target.localPosition = Vector3.Lerp(start, end, elapsed / moveObj.moveDuration);
                else
                    moveObj.target.position = Vector3.Lerp(start, end, elapsed / moveObj.moveDuration);
                yield return null;
            }

            if (isLocal)
                moveObj.target.localPosition = end;
            else
                moveObj.target.position = end;
            if (moveObj.target.GetComponent<PlayerMovement>())
            {
                PlayerMovement.instance.StopMoving();
                PlayerMovement.instance.SetPlayerLookingDirection(false);
            }
        }

        currentMidStep++;
        yield return new WaitForSeconds(moveObj.postArrivalWait);
    }

    
}
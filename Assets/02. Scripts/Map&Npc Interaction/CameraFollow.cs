using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraFollow : MonoBehaviour
{
    public Transform player; //플레이어 트랜스폼

    //SpriteRenderer sprRenderer;
    public float smoothSpeed = 50f;

    public float cDuration = 1.0f; // 이동 시간
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 부드러운 이동 곡선


    public bool isXFixed;
    public bool isYFixed;

    public bool isXFreezed;
    public bool isYFreezed;
    
    Transform cam;

    private float _timeX;
    private float _timeY;

    private bool isCoroutineRunning;

    private Vector3 targetPosition;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        targetPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (!isXFixed && !isXFreezed) targetPosition.x = player.position.x;
        else targetPosition.x = transform.position.x;
        if (!isYFixed && !isYFreezed) targetPosition.y = player.position.y;
        else targetPosition.y= transform.position.y;
        
        if(!isCoroutineRunning) transform.position = targetPosition;
    }

    public void ResetPosition(bool isFreezeMode)
    {
        if (!isFreezeMode)
        {
            isXFixed = false;
            isYFixed = false;
        }

        StartFollowPlayer(!(isXFreezed || isXFixed),!(isYFreezed || isYFixed), cDuration);
    }

    public void MoveToTarget(Vector3 targetVec, bool isX, bool isY)
    {
        isXFixed = isX;
        isYFixed = isY;
        StartSmoothMove(targetVec, isX, isY, cDuration);
    }

    private Vector3 currentTarget;
    private Coroutine moveCoroutine;
    private bool targetIsPlayer;
    
        /// <summary>
    /// 부드러운 이동 시작 함수
    /// </summary>
    /// <param name="targetPos">이동할 목표 위치</param>
    /// <param name="isXMove">X축 이동 허용 여부</param>
    /// <param name="isYMove">Y축 이동 허용 여부</param>
    /// <param name="duration">이동에 걸리는 시간</param>
    public void StartSmoothMove(Vector3 targetPos, bool isXMove = true, bool isYMove = true, float duration = 1f, bool isPlayer=false)
    {
        
        targetIsPlayer = isPlayer;
        if(!targetIsPlayer)currentTarget = targetPos;
        
        if (isCoroutineRunning)
        {StopCoroutine(moveCoroutine);
            
        }
        if(this.gameObject.activeInHierarchy)
        moveCoroutine = StartCoroutine(SmoothMoveRoutine(isXMove, isYMove, duration));
        
    }

    /// <summary>
    /// 플레이어를 따라가는 부드러운 이동 시작
    /// (player.position을 목표로 설정)
    /// </summary>
    public void StartFollowPlayer(bool isXMove = true, bool isYMove = true, float duration = 1f)
    {
        if (player == null)
        {
            Debug.LogWarning("플레이어가 할당되지 않았습니다.");
            return;
        }

        StartSmoothMove(player.position, isXMove, isYMove, duration, true);
    }

    private IEnumerator SmoothMoveRoutine(bool isXMove, bool isYMove, float duration)
    {
        isCoroutineRunning = true;

        Vector3 startPos = transform.position;
        Vector3 targetNow=targetIsPlayer?player.position:currentTarget;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (this == null) yield break;
            // 매 프레임 최신 target 반영
             targetNow = targetIsPlayer?player.position:currentTarget;

            elapsedTime += Time.deltaTime;
            float t = easeCurve.Evaluate(elapsedTime / duration);

           
            float newX = isXMove ? Mathf.Lerp(startPos.x, targetNow.x, t) : startPos.x;
            float newY = isYMove ? Mathf.Lerp(startPos.y, targetNow.y, t) : startPos.y;
            float newZ = startPos.z; // Z는 고정

            transform.position = new Vector3(newX, newY, newZ);

            bool isXReached = isXMove ? Mathf.Abs(transform.position.x - targetNow.x) < 0.05f : true;
            bool isYReached = isYMove ? Mathf.Abs(transform.position.y - targetNow.y) < 0.05f : true;

            if (isXReached && isYReached)
            {
                
                break;
            }


            yield return null;
        }

        // 최종 위치 보정
        transform.position = new Vector3(
            isXMove ? targetNow.x : transform.position.x,
            isYMove ? targetNow.y : transform.position.y,
            transform.position.z
        );

        isCoroutineRunning = false;
        moveCoroutine = null;
    }

    public void FreezeMove(bool isX, bool isY)
    {
        if (isX) isXFreezed = true;
        if (isY) isYFreezed = true;
        Debug.Log("Freeze move: " + isXFreezed + ", " + isYFreezed);

    }


    public void UnfreezeMove(bool isX, bool isY)
    {
        if (isX) isXFreezed = false;
        if (isY) isYFreezed = false;
        Debug.Log("unfreeze move: " + isXFreezed + ", " + isYFreezed);

        ResetPosition(true);
    }
}
using System.Collections;
using UnityEngine;

/*
public class CameraFollow : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public Vector2 minPosition; // 카메라가 이동할 최소 위치
    public Vector2 maxPosition; // 카메라가 이동할 최대 위치
    public float smoothSpeed = 0.125f; // 카메라 이동 부드러움 정도
    public string limitTag = "CameraLimit"; // 카메라 제한 태그
    public LayerMask limitLayer; // 카메라 제한 레이어

    private Vector3 targetPosition;
    private bool limitX = false;
    private bool limitY = false;

    void FixedUpdate()
    {
        if (player == null) return;

        // 목표 위치 계산
        targetPosition = new Vector3(
            limitX ? transform.position.x : player.position.x,
            limitY ? transform.position.y : player.position.y,
            transform.position.z
        );

        // 충돌 체크 및 제한 적용
        ApplyCameraLimits();

        // 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }

    void ApplyCameraLimits()
    {
        // 특정 레이어 또는 태그가 있는 오브젝트에 닿으면 제한 적용
        Collider2D limitCollider = Physics2D.OverlapPoint(player.position, limitLayer);
        if (limitCollider != null || HasTaggedObjectNearby())
        {
            Vector3 limitPos = limitCollider != null
                ? limitCollider.transform.position
                : GetClosestTaggedObject().position;

            // 오른쪽 제한 감지
            if (player.position.x > limitPos.x) limitX = true;

            // 아래쪽 제한 감지
            if (player.position.y < limitPos.y) limitY = true;
        }
        else
        {
            limitX = false;
            limitY = false;
        }
    }

    bool HasTaggedObjectNearby()
    {
        return GetClosestTaggedObject() != null;
    }

    Transform GetClosestTaggedObject()
    {
        GameObject[] limitObjects = GameObject.FindGameObjectsWithTag(limitTag);
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject obj in limitObjects)
        {
            float dist = Vector2.Distance(obj.transform.position, player.position);
            if (dist < 2.0f && dist < minDist)
            {
                minDist = dist;
                closest = obj.transform;
            }
        }

        return closest;
    }

    public LayerMask targetLayer;
    public Camera mainCamera;

    private void Start()
    {
        mainCamera=Camera.main;
    }

    void Update()
    {
        Image[] images = FindObjectsOfType<Image>(); // UI Image 찾기

        foreach (Image image in images)
        {
            if (((1 << image.gameObject.layer) & targetLayer) != 0) // Layer 체크
            {
                if (IsVisible(image.rectTransform))
                {
                    Debug.Log($"{image.gameObject.name}가 보이고 있어요!");
                }
            }
        }
    }
    public CameraTrigger trigger;
    bool IsVisible(RectTransform rectTransform)
    {
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners); // UI 요소의 월드 좌표 가져오기

        foreach (Vector3 corner in worldCorners)
        {
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(corner);
            if (screenPoint.z > 0 &&
                screenPoint.x > 0 && screenPoint.x < Screen.width &&
                screenPoint.y > 0 && screenPoint.y < Screen.height)
            {
                return true; // 하나라도 카메라 화면 안에 있으면 보이는 것
            }
        }
        return false;
    }
}
*/


public class CameraFollow : MonoBehaviour
{
    public Transform player; //플레이어 트랜스폼

    //SpriteRenderer sprRenderer;
    public float smoothSpeed = 50f;

    public float cDuration = 1.0f; // 이동 시간
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 부드러운 이동 곡선


    public bool isFixed;
    public bool _isXFixed;
    public bool _isYFixed;

    public bool isFreezed;
    public bool isXFreezed;
    public bool isYFreezed;

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
        targetPosition = transform.position;
        if (!_isXFixed && !isXFreezed) targetPosition.x = player.position.x;
        if (!_isYFixed && !isYFreezed) targetPosition.y = player.position.y;

        transform.position = targetPosition;
    }

    public void ResetPosition(bool isFreezeMode)
    {
        if (!isFreezeMode)
        {
            isFixed = false;
            _isXFixed = false;
            _isYFixed = false;
        }
        // if (isCoroutineRunning) StopAllCoroutines();

        StartCoroutine(SmoothMoveTrans(isFreezeMode, isXFreezed || _isXFixed,
            isYFreezed || _isYFixed,
            cDuration));
    }

    public void MoveToTarget(Vector3 targetVec, bool isX, bool isY)
    {
        // if (isCoroutineRunning) StopAllCoroutines();
        isFixed = isX || isY;
        _isXFixed = isX;
        _isYFixed = isY;
        StartCoroutine(SmoothMove(targetVec, isX, isY, cDuration));
    }

    private IEnumerator SmoothMove(Vector3 targetPos, bool isX, bool isY, float duration)
    {
        isCoroutineRunning = true;
        Vector3 startPosition = transform.position;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (this == null) yield break;
            elapsedTime += Time.deltaTime;
            float t = easeCurve.Evaluate(elapsedTime / duration); // 부드러운 이동 적용

            float newX = isX ? Mathf.Lerp(startPosition.x, targetPos.x, t) : startPosition.x;
            float newY = isY ? Mathf.Lerp(startPosition.y, targetPos.y, t) : startPosition.y;
            float newZ = startPosition.z; // Z축은 그대로 유지

            transform.position = new Vector3(newX, newY, newZ);
            yield return null;
        }

        // 최종 위치 보정
        transform.position = new Vector3(
            isX ? targetPos.x : startPosition.x,
            isY ? targetPos.y : startPosition.y,
            startPosition.z
        );
        isCoroutineRunning = false;
    }

    private IEnumerator SmoothMoveTrans(bool isFreezeMode, bool isXNoMove, bool isYNoMove, float duration)
    {
        isCoroutineRunning = true;
        Vector3 startOffset = transform.position; // - player.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            //if (this == null) yield break;
            elapsedTime += Time.deltaTime;
            // 현재 위치의 상대적 오프셋
            float t = easeCurve.Evaluate(elapsedTime / duration); // 부드러운 이동 적용
            startOffset = transform.position;

            // 매 프레임마다 startOffset을 갱신하며 보간 수행
            float newX = isXNoMove ? startOffset.x : Mathf.Lerp(startOffset.x, player.transform.position.x, t);
            float newY = isYNoMove ? startOffset.y : Mathf.Lerp(startOffset.y, player.transform.position.y, t);
            float newZ = startOffset.z; // Z축은 그대로 유지

            transform.position = new Vector3(newX, newY, newZ);
//            Debug.Log(transform.position.y + "+ repeat: " + t);
            yield return null;
        }

        //      Debug.Log("repeat: Finish");
        // 최종 위치 보정
        transform.position = new Vector3(
            isXNoMove ? startOffset.x : player.transform.position.x,
            isYNoMove ? startOffset.y : player.transform.position.y,
            startOffset.z
        );

        isCoroutineRunning = false;
    }


    public void FreezeMove(bool isX, bool isY)
    {
        if (isX) isXFreezed = true;
        if (isY) isYFreezed = true;
        Debug.Log("Freeze move: " + isXFreezed + ", " + isYFreezed);

        isFreezed = true;
    }


    public void UnfreezeMove(bool isX, bool isY)
    {
        if (isX) isXFreezed = false;
        if (isY) isYFreezed = false;
        Debug.Log("unfreeze move: " + isXFreezed + ", " + isYFreezed);

        isFreezed = isXFreezed || isYFreezed;
        //ResetPosition(true);
    }
}
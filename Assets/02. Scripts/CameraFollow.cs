using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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
    public float smoothSpeed = 50f;
    private Vector3 defaultOffset;

    private static bool isXFixed = false;
    private static bool isYFixed = false;
    
    private Vector3 targetPosition;

    private static float timeX = 0f;
    private static float timeY = 0f;
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        defaultOffset = transform.position - player.position;
        targetPosition = transform.position; 
    }

    private void LateUpdate()
    {
        // isXFixed, isYFixed
        Vector3 desiredPosition = player.position + defaultOffset;

        if (isXFixed)
        {
            desiredPosition.x = transform.position.x; // X축 고정
        }

        if (isYFixed)
        {
            desiredPosition.y = transform.position.y; // Y축 고정
        }

        if (timeX < 1.5f)
        {
            // 부드럽게 이동하도록 Lerp 사용
            targetPosition.x = Vector3.Lerp(targetPosition, desiredPosition, smoothSpeed * Time.deltaTime).x;
        }
        if (timeY < 1.5f)
        {
            // 부드럽게 이동하도록 Lerp 사용
            targetPosition.y = Vector3.Lerp(targetPosition, desiredPosition, smoothSpeed * Time.deltaTime).y;
        }
        else targetPosition = desiredPosition;
        
        timeX += Time.deltaTime;
        timeY += Time.deltaTime;
        
        transform.position = targetPosition;
        
       // Debug.Log(timeX+"sec, X Fix? "+isXFixed+", Y Fix? "+isYFixed);
    }


    public static void SetIsXFixed(bool isFixed)
    {
        if(CameraFollow.isXFixed!=isFixed) timeX = 0f;
        CameraFollow.isXFixed = isFixed;
        
    }
    public static void SetIsYFixed(bool isFixed)
    {
        if(CameraFollow.isXFixed!=isFixed) timeY = 0f;
        CameraFollow.isYFixed = isFixed;
    }

    public void SetFixedPosition(Vector3 newPosition)
    {
        isXFixed = isYFixed=true;
        //fixedPosition = newPosition;
    }

    public void ResetToFollow()
    {
        isXFixed = isYFixed= false;
    }
}

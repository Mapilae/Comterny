using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public float smoothSpeed = 1f; // 카메라 이동 속도
    private Vector3 defaultOffset; // 기본 카메라 오프셋

    private bool isFixed = false; // 카메라 고정 여부
    private Vector3 fixedPosition; // 고정될 위치

    private void Start()
    {
        // 기본적으로 플레이어를 중심에 두는 오프셋 설정
        defaultOffset = transform.position - player.position;
    }

    private void LateUpdate()
    {
        if (isFixed)
        {
            // 카메라가 특정 위치에 고정됨
            transform.localPosition = Vector3.Lerp(transform.position, fixedPosition, smoothSpeed);
        }
        else
        {
            // 평소에는 플레이어를 따라다님
            Vector3 targetPosition = player.position + defaultOffset;
            transform.localPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        }
    }

    // 카메라를 특정 위치에 고정하는 함수 (발판에서 호출)
    public void SetFixedPosition(Vector3 newPosition)
    {
        isFixed = true;
        fixedPosition = newPosition;
    }

    // 카메라를 다시 플레이어 따라가도록 설정하는 함수 (발판에서 호출)
    public void ResetToFollow()
    {
        isFixed = false;
    }
}

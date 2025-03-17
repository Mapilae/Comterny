using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; //플레이어 트랜스폼
    public float smoothSpeed = 1f;
    private Vector3 defaultOffset;

    private bool isFixed = false;
    private Vector3 fixedPosition;

    private void Start()
    {
        
        defaultOffset = transform.position - player.position;
    }

    private void LateUpdate()
    {
        if (isFixed)
        {
           
            transform.localPosition = Vector3.Lerp(transform.position, fixedPosition, smoothSpeed);
        }
        else
        {
        
            Vector3 targetPosition = player.position + defaultOffset;
            transform.localPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        }
    }

    public void SetFixedPosition(Vector3 newPosition)
    {
        isFixed = true;
        fixedPosition = newPosition;
    }

    public void ResetToFollow()
    {
        isFixed = false;
    }
}

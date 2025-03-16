using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public Vector3 MaxCameraPosition; // 카메라가 최대로 갈 수 있는 위치
    private CameraFollow cameraFollow; // 카메라 스크립트 참조
    public bool useX = true;
    public bool useY = true;

    private void Start()
    {
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어가 발판을 밟았을 때
        {
            Vector3 currentCameraPos = Camera.main.transform.position;
            Vector3 newCameraPos = currentCameraPos;
            bool shouldUpdate = false;

            // X축 제한 적용 (현재 위치가 MaxCameraPosition.x보다 크거나 작을 경우)
            if (useX && (currentCameraPos.x < MaxCameraPosition.x))
            {
                newCameraPos.x = MaxCameraPosition.x;
                shouldUpdate = true;
            }

            // Y축 제한 적용 (현재 위치가 MaxCameraPosition.y보다 크거나 작을 경우)
            if (useY && (currentCameraPos.y > MaxCameraPosition.y))
            {
                newCameraPos.y = MaxCameraPosition.y;
                shouldUpdate = true;
            }

            // 필요할 때만 카메라 위치 업데이트
            if (shouldUpdate)
            {
                cameraFollow.SetFixedPosition(newCameraPos);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어가 발판을 떠나면
        {
            cameraFollow.ResetToFollow();
        }
    }

/*    public Camera mainCamera;
    public LayerMask targetLayer;

    void Update()
    {
        Renderer[] renderers = FindObjectsOfType<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            if (((1 << renderer.gameObject.layer) & targetLayer) != 0) // 특정 레이어 체크
            {
                if (renderer.isVisible)
                {
                    Debug.Log($"{renderer.gameObject.name}이(가) 카메라에서 렌더링 중!");
                }
            }
        }
    }*/
}

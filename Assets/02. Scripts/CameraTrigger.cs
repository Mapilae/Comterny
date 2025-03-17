using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    /// <summary>
    /// 만들다 말았음. 생각대로 작동 안함.
    /// </summary>
    public Vector3 MaxCameraPosition;
    private CameraFollow cameraFollow;
    public bool useX = true;
    public bool useY = true;

    private void Start()
    {
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            Vector3 currentCameraPos = Camera.main.transform.position;
            Vector3 newCameraPos = currentCameraPos;
            bool shouldUpdate = false;

            if (useX && (currentCameraPos.x < MaxCameraPosition.x))
            {
                newCameraPos.x = MaxCameraPosition.x;
                shouldUpdate = true;
            }

            if (useY && (currentCameraPos.y > MaxCameraPosition.y))
            {
                newCameraPos.y = MaxCameraPosition.y;
                shouldUpdate = true;
            }

            if (shouldUpdate)
            {
                cameraFollow.SetFixedPosition(newCameraPos);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // �÷��̾ ������ ������
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
            if (((1 << renderer.gameObject.layer) & targetLayer) != 0) // Ư�� ���̾� üũ
            {
                if (renderer.isVisible)
                {
                    Debug.Log($"{renderer.gameObject.name}��(��) ī�޶󿡼� ������ ��!");
                }
            }
        }
    }*/
}

using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    /// <summary>
    /// 만들다 말았음. 생각대로 작동 안함.
    /// </summary>

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            CameraFollow.SetIsXFixed(false);
            CameraFollow.SetIsYFixed(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Bounds bounds = GetComponent<BoxCollider2D>().bounds;
            Vector3 playerPos = other.transform.position;

            bool fixX = playerPos.x < bounds.min.x || playerPos.x > bounds.max.x;
            bool fixY = playerPos.y < bounds.min.y || playerPos.y > bounds.max.y;

            CameraFollow.SetIsXFixed(fixX);
            CameraFollow.SetIsYFixed(fixY);
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

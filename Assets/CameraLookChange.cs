using UnityEngine;

public class CameraLookChange : MonoBehaviour
{
    public Vector3 target;
    public bool isUsingX;
    public bool isUsingY;
    private CameraFollow cam;
    private bool isUsed;

    private void Start()
    {
        cam = Camera.main.GetComponent<CameraFollow>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player")) OnEnter();
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player")) OnExit();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) OnEnter();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) OnExit();
    }

    private void OnEnter()
    {
        if (isUsed) return;
        isUsed = true;
        cam.MoveToTarget(target, isUsingX, isUsingY);
    }

    private void OnExit()
    {
        isUsed = false;
        cam.ResetPosition(false);
    }
}
using UnityEngine;

public class CameraFreeze : MonoBehaviour
{
    public bool isFreezeX;
    public bool isFreezeY;
    private CameraFollow cam;
    private bool isUsed;

    private void Start()
    {
        cam = Camera.main.GetComponent<CameraFollow>();
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

        cam.FreezeMove(isFreezeX, isFreezeY);
    }

    private void OnExit()
    {
        isUsed = false;
        cam.UnfreezeMove(isFreezeX, isFreezeY);
    }
}
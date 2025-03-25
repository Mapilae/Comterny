using UnityEngine;

public class CameraFreeze : MonoBehaviour
{
    private static bool _isFreezingX;
    private static bool _isFreezingY;
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
        if (isFreezeX) _isFreezingX = true;
        if (isFreezeY) _isFreezingY = true;
        cam.FreezeMove(_isFreezingX, _isFreezingY);
    }

    private void OnExit()
    {
        isUsed = false;
        cam.UnfreezeMove(_isFreezingX, _isFreezingY);
        if (isFreezeX) _isFreezingX = false;
        if (isFreezeY) _isFreezingY = false;
    }
}
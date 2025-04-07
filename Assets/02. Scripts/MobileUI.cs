using UnityEngine;

public class MobileUI : MonoBehaviour
{
    private PlayerMovement playerMovement;

    private void Start()
    {
#if UNITY_EDITOR
        gameObject.SetActive(false);
#endif
#if UNITY_ANDROID
        gameObject.SetActive(true);
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
#endif
    }

    public void SetIsSpace(bool isSpace)
    {
        playerMovement.SetIsSpace(isSpace);
    }
}
using UnityEngine;

public class MobileUI : MonoBehaviour
{
    public static MobileUI instance;
    private Canvas canvas;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        canvas = gameObject.GetComponent<Canvas>();
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

    public void HideThis(bool hide)
    {
        canvas.enabled = !hide;
    }
}
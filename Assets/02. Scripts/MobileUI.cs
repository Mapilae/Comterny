using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileUI : MonoBehaviour
{
    public static MobileUI instance;
    private static readonly int IsActive = Animator.StringToHash("isActive");

    public static bool isJoyOn = false;

    public Image itemButtonImg;
    public Sprite[] buttonIcon;

    public Sprite nullSprite;

    //private PlayerMovement playerMovement;
    public GameObject mobileOnlyObj;
    public GameObject pcObj; //space bar guide
    private Animator buttonAnim;
    private Canvas canvas;

    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        canvas = gameObject.GetComponent<Canvas>();
        buttonAnim = itemButtonImg.transform.parent.GetComponent<Animator>();

        buttonAnim.GetComponent<Button>().onClick.AddListener(() =>
        {
            // 버튼 클릭 후 선택 해제
            EventSystem.current.SetSelectedGameObject(null);
        });

#if UNITY_EDITOR
        //canvas.enabled = false;
        mobileOnlyObj.SetActive(false);
        pcObj.SetActive(true);
#endif
#if UNITY_ANDROID || UNITY_IOS
        mobileOnlyObj.SetActive(true);
        pcObj.SetActive(false);
        //canvas.enabled = true;
     //   playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
#endif
#if UNITY_WEBGL
        mobileOnlyObj.SetActive(isJoyOn);
        pcObj.SetActive(true);
#endif
    }

    public void SetIsSpace(bool isSpace)
    {
        PlayerMovement.instance.SetIsSpace(isSpace);
    }

    public void HideThis(bool hide)
    {
        canvas.enabled = !hide;
        // mobileOnlyObj.SetActive(!hide);
    }

    public void SetIconImage(InteractableType type)
    {
        buttonAnim.SetBool(IsActive, true);
        switch (type)
        {
            case InteractableType.NPC:
                itemButtonImg.sprite = buttonIcon[1]; //말 걸기
                break;
            default:
                itemButtonImg.sprite = buttonIcon[0];
                break;
        }
    }

    public void NothingClose()
    {
        itemButtonImg.sprite = nullSprite;
        if (buttonAnim != null && buttonAnim.gameObject.activeInHierarchy) buttonAnim.SetBool(IsActive, false);
    }
}
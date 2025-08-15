using System;
using TMPro;
using UnityEngine;

public class StackQueueSwitch : MonoBehaviour
{
    [Serializable]
    public enum DataStructureType
    {
        STACKPOP,
        STACKPUSH,
        QUEUEPOP,
        QUEUEPUSH
    }

    public DataStructureType thisType;

    [Header("When Player Come Close")] public bool isClose; //플레이어가 해당 아이템의 트리거 영역에 있는지 여부
    public GameObject objToEnable; //트리거 영역에 있는 경우, 활성화하는 오브젝트(ex.반짝이는 효과)

    //[Header("After Find Item, to do?")] public Sprite[] sprToChange;
    public bool canUse = true; //중복 줍기 방지
    public TMP_Text notiText;
    public StackMinigame stackMinigame;
    public QueueMinigame queueMinigame;
    private Animator anim;

    private bool isButtonDown;

    private SpriteRenderer sprRender;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        sprRender = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!canUse) return;
        if (isClose && PlayerMovement.isSpace)
        {
            canUse = false;
            PlayerMovement.instance.IsSpaceFalse(0f);
            // Debug.Log("Item picked up: " + item.itemName);

            if (objToEnable != null) objToEnable.SetActive(false);

            SoundManager.instance.PlaySound("item get");
            //if (!sprToChange.Equals(null)) sprRender.sprite = sprToChange;
            //   else gameObject.SetActive(false);
            PlayStackOrQueue();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canUse) return;
        if (collision.CompareTag("Player"))
        {
            isClose = true;
            if (objToEnable != null) objToEnable.SetActive(true);
            MobileUI.instance.SetIconImage(InteractableType.ITEM);
            anim.SetTrigger("down");
            isButtonDown = true;
            PlayStackOrQueue();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isClose = false;
            if (objToEnable != null) objToEnable.SetActive(false);
            MobileUI.instance.NothingClose();
            anim.SetTrigger("up");
            isButtonDown = false;
        }
    }

    private void PlayStackOrQueue()
    {
        if (!isButtonDown) return;
        var isSuccesslyPushed = 0;
        notiText.gameObject.SetActive(false);
        switch (thisType)
        {
            case DataStructureType.QUEUEPOP:
                isSuccesslyPushed = queueMinigame.PopOne();
                notiText.text = "큐에서 꺼낼 데이터 박스가 더 이상 없습니다!";
                break;
            case DataStructureType.QUEUEPUSH:
                isSuccesslyPushed = queueMinigame.PushOne();
                notiText.text = "큐에 넣을 데이터 박스를 가지고 있지 않습니다!";
                break;
            case DataStructureType.STACKPOP:
                isSuccesslyPushed = stackMinigame.PopButtonClick();
                notiText.text = "스택에서 꺼낼 데이터 책이 더 이상 없습니다!";
                break;
            case DataStructureType.STACKPUSH:
                isSuccesslyPushed = stackMinigame.PushButtonClick();
                notiText.text = "스택에 넣을 데이터 책을 가지고 있지 않습니다!";
                break;
        }

        canUse = true;
        if (isSuccesslyPushed == -1) notiText.gameObject.SetActive(true);
    }
}
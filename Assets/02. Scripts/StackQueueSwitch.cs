using System;
using UnityEngine;

public class StackQueueSwitch : MonoBehaviour
{
    public DataStructureType thisType;

    [Header("When Player Come Close")] public bool isClose; //플레이어가 해당 아이템의 트리거 영역에 있는지 여부
    public GameObject objToEnable; //트리거 영역에 있는 경우, 활성화하는 오브젝트(ex.반짝이는 효과)

    //[Header("After Find Item, to do?")] public Sprite[] sprToChange;
    public bool canUse = true; //중복 줍기 방지
    
    SpriteRenderer sprRender;
    
    public StackMinigame stackMinigame;
    public QueueMinigame queueMinigame;
    private Animator anim;

    private bool isButtonDown = false;
    
    [Serializable]
    public enum DataStructureType
    {
        STACKPOP,
        STACKPUSH,
        QUEUEPOP,
        QUEUEPUSH
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sprRender = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
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

    void PlayStackOrQueue()
    {
        if(!isButtonDown)return;
        switch (thisType)
        {
            case DataStructureType.QUEUEPOP:
                queueMinigame.PopOne();
                break;
            case DataStructureType.QUEUEPUSH:
                queueMinigame.PushOne();
                break;
            case DataStructureType.STACKPOP:
                stackMinigame.PopButtonClick();
                break;
            case DataStructureType.STACKPUSH:
                stackMinigame.PushButtonClick();
                break;
            
        }

        canUse = true;
    }
    

}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrashCan : MonoBehaviour
{
    public TMP_Text itemNameTxt;
    public Image itemImg;
    public Button throwButton;

    [Space] public GameObject trashFullRealMap;
    public GameObject trashEmptyRealMap;
    public Image trashFull;
    public Image trashEmpty;

    public Button closeButton;

    // private Inventory inventory;
    private bool isStartFromEmpty;

    private int itemID;

    private bool nothingToThrow;
    private Animator trashAnim;

    private void Start()
    {
        // inventory = Inventory.instance;
        //trashAnim = transform.GetChild(0).GetChild(0).GetComponent<Animator>(); //TrashBack GameObject
        trashAnim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        Inventory.instance.anything = gameObject;
        if (trashEmptyRealMap.gameObject.activeInHierarchy)
        {
            trashEmpty.enabled = true;
            trashFull.enabled = false;
        }
        else if (trashFullRealMap.gameObject.activeInHierarchy)
        {
            trashEmpty.enabled = false;
            trashFull.enabled = true;
        }

        //순서 바뀌면 안됨
        CheckIfSomethingThrowable();
    }

    private void OnDisable()
    {
        Inventory.instance.anything = null;
    }

    public void ThrowAwayItem()
    {
        if (Inventory.instance.holdingItem == null) return;
        if (itemID == -1) return;
        closeButton.interactable = false;
        throwButton.interactable = false;
        isStartFromEmpty = trashEmptyRealMap.gameObject.activeInHierarchy;

        trashAnim.SetTrigger(isStartFromEmpty ? "throwEmpty" : "throwFull");
    }


    public void OnTrashAnimEventMiddle()
    {
        if (isStartFromEmpty)
        {
            trashEmpty.enabled = false;
            trashFull.enabled = true;
            trashEmptyRealMap.gameObject.SetActive(false);
            trashFullRealMap.gameObject.SetActive(true);
            isStartFromEmpty = false;
        }

        PlayerData.instance.RemoveItem(itemID);
        Inventory.instance.UnholdItem();
        Debug.Log("아이템 제거 완료!");
        CheckIfSomethingThrowable();
    }

    public void OnTrashAnimEventEnd()
    {
        closeButton.interactable = true;
    }


    private void SetHoldingItem()
    {
        if (nothingToThrow)
        {
            itemImg.enabled = false;
            itemNameTxt.text = "버릴만한 아이템이 없습니다.";
            itemID = -1;
            throwButton.interactable = false;
            return;
        }

        if (Inventory.instance.holdingItem == null || !Inventory.instance.holdingItem.canThrowAway)
        {
            itemImg.enabled = false;
            itemNameTxt.text = "";
            itemID = -1;
            throwButton.interactable = false;
            return;
        }

        itemImg.enabled = true;
        throwButton.interactable = true;
        itemNameTxt.text = Inventory.instance.holdingItem.itemName;
        itemImg.sprite = Inventory.instance.holdingItem.itemSpr;
        itemID = Inventory.instance.holdingItem.itemID;
    }

    public void CheckIfSomethingThrowable()
    {
        var somethingThrowable = false;
        if (Inventory.instance == null) return;

        for (var i = 0; i < Inventory.instance.itemHoldList.Count; i++)
            if (Inventory.instance.itemHoldList[i].canThrowAway)
            {
                somethingThrowable = true;
                break;
            }

        nothingToThrow = !somethingThrowable;
        throwButton.gameObject.SetActive(!nothingToThrow);
        SetHoldingItem();
    }
}
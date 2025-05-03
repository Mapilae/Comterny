using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public VariableHolder varHolder;
    //public List<VariableIntStr> varList = new();

    /**
     * itemHoldParent는 유니티에서 SideBarInv를 지칭한다.
     */
    public Transform itemHoldParent;

    //public List<Item> currentItems;
    public int currentHoldingItemIndex = -1;

    [Header("Item Info")] public RectTransform itemInfoRect;

    public TMP_Text itemInfoNameText;
    public TMP_Text itemInfoDescText;

    /**
     * itemHoldList itemHoldParent의 자식 오브젝트를 관리하는 리스트
     */
    public List<ItemHolder> itemHoldList;

    public GameObject anything;
    private PlayerData playerData;

    // 플레이어 데이터 오브젝트를 찾고 그걷에 대한 정보를 가져온다
    // 
    private void Awake()
    {
        instance = this;
        playerData = GameObject.FindGameObjectWithTag("PlayerData").GetComponent<PlayerData>();
    }

    private void Start()
    {
        UpdateInventory();
    }

    public void ClickToHoldItem(int index)
    {
        if (playerData.items.Count == 0) return;

        //이전에 클릭했던 인덱스 아웃라인 끄기
        if (currentHoldingItemIndex >= 0)
            itemHoldList[currentHoldingItemIndex].image.transform.parent.GetComponent<Outline>().enabled = false;
        if (currentHoldingItemIndex.Equals(index))
        {
            currentHoldingItemIndex = -1;
            PlayerMovement.instance.UnholdItem();
            return;
        }

        currentHoldingItemIndex = index;
        //set outline enabled in its parent img
        itemHoldList[currentHoldingItemIndex].image.transform.parent.GetComponent<Outline>().enabled = true;

        var item = playerData.GetItemFromID(itemHoldList[currentHoldingItemIndex].itemID);

        PlayerMovement.instance.HoldItem(item.itemID,
            item.itemSpr);
        UseAnythingObj();
    }

    public void EnableItemInfo(int index)
    {
        itemInfoRect.anchoredPosition = new Vector2(itemInfoRect.anchoredPosition.x, -120 - 75 * index);
        var item = playerData.GetItemFromID(itemHoldList[index].itemID);

        if (item == null) return;
        itemInfoNameText.text = item.itemName;
        itemInfoDescText.text = item.itemDesc;
        itemInfoRect.gameObject.SetActive(true);
    }

    public void DisableItemInfo()
    {
        itemInfoRect.gameObject.SetActive(false);
    }

    /*
     플레이어 데이터 안에있는 데이터를 이중 포문을 사용하여 가져온다
     가져온 데이터를 itemHoldList에 넣는다
     ItemGet 함수가 addItem을 호출하고
     addItem이 실행되면 인벤토리 전체를 업데이트한다.
    */
    public void UpdateInventory()
    {
        if (itemHoldList == null) MakeList();
        else if (itemHoldList.Count == 0)
            MakeList();
        for (var i = 0; i < itemHoldList.Count; i++)
            if (i < playerData.items.Count)
            {
                var item = playerData.GetItemFromID(playerData.items[i]);
                itemHoldList[i].image.sprite = item.itemSpr;
                itemHoldList[i].itemID = item.itemID;
                itemHoldList[i].image.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                itemHoldList[i].image.sprite = null;
                itemHoldList[i].itemID = -1;
                itemHoldList[i].image.transform.parent.gameObject.SetActive(false);
            }
    }

    /**
     * MakeList() 함수는 itemHoldParent의 자식(아이템 전체를 관리하는 오브젝트)
     * 개수만큼 반복문을 돌려서 아이템 홀드 리스트의 추가한다.
     */
    private void MakeList()
    {
        itemHoldList = new List<ItemHolder>();
        for (var i = 0; i < itemHoldParent.childCount; i++)
            /*
             * itemHoldParent의 자식 오브젝트 = InvSlotButton
             * InvSlotButton의 자식 오브젝트 = IconSpr
             */
            itemHoldList.Add(new ItemHolder(itemHoldParent.GetChild(i).GetChild(0).GetComponent<Image>()));
    }

    public bool CheckItemIsInInventory(int itemID)
    {
        for (var i = 0; i < itemHoldList.Count; i++)
            if (itemHoldList[i].itemID.Equals(itemID))
                return true;

        return false;
    }

    public ItemHolder GetItemFromID(int itemID)
    {
        for (var i = 0; i < playerData.items.Count; i++)
            if (itemHoldList[i].itemID.Equals(itemID))
                return itemHoldList[i];

        return null;
    }

    private void UseAnythingObj()
    {
        if (anything == null) return;
        if (anything.GetComponent<LogicGateMinigame>())
            anything.GetComponent<LogicGateMinigame>().InventoryToReal(itemHoldList[currentHoldingItemIndex].itemID);
    }

    [Serializable]
    public class ItemHolder
    {
        public Image image;
        public int itemID;

        public ItemHolder(Image image, int itemID)
        {
            this.image = image;
            this.itemID = itemID;
        }

        public ItemHolder(Image image)
        {
            this.image = image;
            itemID = -1;
        }
    }
}
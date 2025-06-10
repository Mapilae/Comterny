using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    // public VariableHolder varHolder;
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

    public Item holdingItem;
    private int holdingItemCount;
    private PlayerData playerData;
    
    [Header("Hint System")]
    public HintPanel hintPanel;
    // 플레이어 데이터 오브젝트를 찾고 그걷에 대한 정보를 가져온다
    // 
    private void Awake()
    {
        instance = this;
        playerData = GameObject.FindGameObjectWithTag("PlayerData").GetComponent<PlayerData>();
        UpdateInventory();
    }

    private void Start()
    {
        //  UpdateInventory();
    }

    public void HoldLastIndexItem(bool isPlayerClickSelf=true)
    {
        //가장 마지막으로 얻은 아이템 들기. 아이템 획득시 뜨는 다이얼로그 하기 위한 목적.
        ClickToHoldItem(holdingItemCount - 1, isPlayerClickSelf);
//        Debug.Log("holded last item");
    }

    public void UnholdItem()
    {
        ClickToHoldItem(-1);
    }

    public void ClickToHoldItem(int index)
    {
        ClickToHoldItem(index, true);
    }
    
    public void ClickToHoldItem(int index, bool isPlayerClickSelf)
    {
        //이전에 클릭했던 인덱스 아웃라인 끄기
        if (currentHoldingItemIndex >= 0)
            itemHoldList[currentHoldingItemIndex].image.transform.parent.GetComponent<Outline>().enabled = false;

        if (currentHoldingItemIndex.Equals(index) || index.Equals(-1))
        {
            currentHoldingItemIndex = -1;
            PlayerMovement.instance.UnholdItem();
            holdingItem = null;
            return;
        }

        if (playerData.items.Count == 0) return; //위치 변경 금지

        currentHoldingItemIndex = index;
        //set outline enabled in its parent img
        itemHoldList[currentHoldingItemIndex].image.transform.parent.GetComponent<Outline>().enabled = true;

        var item = playerData.GetItemFromID(itemHoldList[currentHoldingItemIndex].itemID);

        if(isPlayerClickSelf){
            if (item.isToOpenHint != -1)
            {
                // 아이템을 획득했을 때, 해당 아이템의 힌트 스프라이트를 보여준다.
                //maybe we should make ReplaceItem() function instead of this
                playerData.RemoveItem(item.itemID);
                playerData.AddItem(item.isToOpenHint);
                item = playerData.GetItemFromID(item.isToOpenHint);
            }
            if (item.hintSprite != null)
            {
                //hint인 경우
                ShowHint(item);
            }
        
        }

        PlayerMovement.instance.HoldItem(item.itemID, item.itemSpr);
        holdingItem = item;
        UseAnythingObj();
    }

    public void EnableItemInfo(int index)
    {
        itemInfoRect.anchoredPosition = new Vector2(itemInfoRect.anchoredPosition.x, -120 - 75 * index);
        var item = playerData.GetItemFromID(itemHoldList[index].itemID);

        if (item == null) return;
        itemInfoNameText.text = item.itemName;
        itemInfoDescText.text = item.itemDesc;
        if (item.canThrowAway) itemInfoDescText.text += "<color=red>*버릴 수 있다.</color>";
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

        holdingItemCount = 0;
        for (var i = 0; i < itemHoldList.Count; i++)
            if (i < playerData.items.Count)
            {
                var item = playerData.GetItemFromID(playerData.items[i]);
                if (item == null)
                {
                    Debug.LogWarning(playerData.items[i]+"번 아이템을 찾지 못했습니다!");
                    
                }
                else{
                    itemHoldList[i].image.sprite = item.itemSpr;
                    itemHoldList[i].itemID = item.itemID;
                    itemHoldList[i].canThrowAway = item.canThrowAway;
                    itemHoldList[i].image.transform.parent.gameObject.SetActive(true);
                }
                holdingItemCount++;
            }
            else
            {
                itemHoldList[i].image.sprite = null;
                itemHoldList[i].itemID = -1;
                itemHoldList[i].canThrowAway = false;
                itemHoldList[i].image.transform.parent.gameObject.SetActive(false);
            }
    }

    void ShowHint(Item item)
    {
        if(item.hintSprite==null)return;
        hintPanel.hintImage.sprite = item.hintSprite;
        hintPanel.gameObject.SetActive(true);
        hintPanel.SetHintImage(item.hintSprite);
    }

    public void HideHint()
    {
        hintPanel.HideHint();
        UnholdItem();
        
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
        if (anything.GetComponent<TrashCan>())
            anything.GetComponent<TrashCan>().CheckIfSomethingThrowable();
    }

    [Serializable]
    public class ItemHolder
    {
        public Image image;
        public int itemID;
        public bool canThrowAway;

        public ItemHolder(Image image, int itemID, bool canThrowAway)
        {
            this.image = image;
            this.itemID = itemID;
            this.canThrowAway = canThrowAway;
        }

        public ItemHolder(Image image)
        {
            this.image = image;
            itemID = -1;
            canThrowAway = false;
        }
    }
}
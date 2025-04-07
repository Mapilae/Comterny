using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    //public VariableHolder varHolder;
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
    private List<Image> itemHoldList;
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
        // 
        MakeList();
        UpdateInventory();
    }

    public void ClickToHoldItem(int index)
    {
        if (playerData.items.Count == 0) return;

        if (currentHoldingItemIndex >= 0)
            itemHoldList[currentHoldingItemIndex].transform.parent.GetComponent<Outline>().enabled = false;
        if (currentHoldingItemIndex.Equals(index))
        {
            currentHoldingItemIndex = -1;
            PlayerMovement.instance.UnholdItem();
            return;
        }

        currentHoldingItemIndex = index;
        //set outline enabled in its parent img
        itemHoldList[currentHoldingItemIndex].transform.parent.GetComponent<Outline>().enabled = true;

        Item item = playerData.GetItemFromIndex(currentHoldingItemIndex);

        PlayerMovement.instance.HoldItem(item.itemID,
            item.itemSpr);
    }

    public void EnableItemInfo(int index)
    {
        itemInfoRect.anchoredPosition = new Vector2(itemInfoRect.anchoredPosition.x, -120 - 75 * index);
        Item item = playerData.GetItemFromIndex(currentHoldingItemIndex);

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
        for (int i = 0; i < itemHoldList.Count; i++)
            if (i < playerData.items.Count)
            {
                Item item = playerData.GetItemFromIndex(i);
                itemHoldList[i].sprite = item.itemSpr;
                itemHoldList[i].transform.parent.gameObject.SetActive(true);
            }
            else
            {
                itemHoldList[i].sprite = null;
                itemHoldList[i].transform.parent.gameObject.SetActive(false);
            }
    }

    /**
     * MakeList() 함수는 itemHoldParent의 자식(아이템 전체를 관리하는 오브젝트)
     * 개수만큼 반복문을 돌려서 아이템 홀드 리스트의 추가한다.
     */
    private void MakeList()
    {
        itemHoldList = new List<Image>();
        for (int i = 0; i < itemHoldParent.childCount; i++)
            /*
             * itemHoldParent의 자식 오브젝트 = InvSlotButton
             * InvSlotButton의 자식 오브젝트 = IconSpr
             */
            itemHoldList.Add(itemHoldParent.GetChild(i).GetChild(0).GetComponent<Image>());
    }
}
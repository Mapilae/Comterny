using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    //public VariableHolder varHolder;
    //public List<VariableIntStr> varList = new();
    public Transform itemHoldParent;

    //public List<Item> currentItems;
    public int currentHoldingItemIndex = -1;

    [Header("Item Info")] public RectTransform itemInfoRect;

    public TMP_Text itemInfoNameText;
    public TMP_Text itemInfoDescText;
    private List<Image> itemHoldList;
    private PlayerData playerData;

    private void Awake()
    {
        instance = this;
        playerData = GameObject.FindGameObjectWithTag("PlayerData").GetComponent<PlayerData>();
    }

    private void Start()
    {
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


    private void MakeList()
    {
        itemHoldList = new List<Image>();
        for (int i = 0; i < itemHoldParent.childCount; i++)
            itemHoldList.Add(itemHoldParent.GetChild(i).GetChild(0).GetComponent<Image>());
    }
}
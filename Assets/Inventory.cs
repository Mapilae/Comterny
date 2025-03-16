using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public Transform itemHoldParent;
    //상단 창 리스트
    public List<Image> itemHoldList;

    public List<Item> currentItems;
    public int currentHoldingItemIndex = -1;


    private void Awake()
    {
        instance = this;
    }

    public void ClickToHoldItem(int index)
    {
        if (currentItems.Count == 0)
        {
            return;
        }

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


        PlayerMovement.instance.HoldItem(currentItems[currentHoldingItemIndex].itemID, currentItems[currentHoldingItemIndex].itemSpr);
    }


    public void UpdateInventory()
    {
        for (int i = 0; i < itemHoldList.Count; i++)
        {
            if (i < currentItems.Count)
            {
                itemHoldList[i].sprite = currentItems[i].itemSpr;

            }
            else
            {
                itemHoldList[i].sprite = null;
            }
        }
    }


    void MakeList()
    {
        itemHoldList = new List<Image>();
        for (int i = 0; i < itemHoldParent.childCount; i++)
        {
            itemHoldList.Add(itemHoldParent.GetChild(i).GetChild(0).GetComponent<Image>());
        }
    }

    public void AddItemInInventory(Item item)
    {
        currentItems.Add(item);
        UpdateInventory();

    }

    public bool CheckIfItemIsInInventory(int itemID)
    {
        foreach (Item item in currentItems)
        {
            if (item.itemID == itemID)
            {
                return true;
            }
        }
        return false;
    }

    private void Start()
    {
        MakeList();
        UpdateInventory();
    }
}



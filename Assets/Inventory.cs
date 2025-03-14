using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Transform itemHoldParent;
    //상단 창 리스트
    public List<Image> itemHoldList;

    public List<Sprite> currentItems;

    public void UpdateInventory()
    {
        for (int i = 0; i < itemHoldList.Count; i++)
        {
            if (i < currentItems.Count)
            {
                itemHoldList[i].sprite = currentItems[i];

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
            itemHoldList.Add(itemHoldParent.GetChild(i).GetComponent<Image>());
        }
    }




    private void Start()
    {
        MakeList();
        UpdateInventory();
    }
}



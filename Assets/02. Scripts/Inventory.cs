using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditorInternal.VersionControl;


public class Inventory : MonoBehaviour
{
    public VariableHolder varHolder;
    public List<VariableIntStr> varList=new List<VariableIntStr>(); 
    
    public static Inventory instance;
    public Transform itemHoldParent;
    public List<Image> itemHoldList;

    public List<Item> currentItems;
    public int currentHoldingItemIndex = -1;
    
    [Header("Item Info")] 
    public RectTransform itemInfoRect;
    public TMP_Text itemInfoNameText;
    public TMP_Text itemInfoDescText;
    
    private void Awake()
    {
        instance = this;
    }

    public bool IsVariableExist(string varName)
    {
        return  varList.Exists(x => x.varName == varName);
    }

    public bool IsVariableExist(int varCode)
    {
        return varList.Exists(x => x.varCode == varCode);
    }

    public void AddVariable(string varNameOrCode)
    {
        VariableIntStr newVar;
        if (int.TryParse(varNameOrCode, out int result))
        {
            newVar=varHolder.FindVariable(result); 
        }
        else newVar=varHolder.FindVariable(result);

        if (!varList.Count.Equals(0))
        {
            if (IsVariableExist(newVar.varCode))
                Debug.Log("varName " + newVar.varName + " and varCode " + newVar.varCode +
                          " already exist, so did not add var");
            return;
        }
        
        varList.Add(newVar);
        Debug.Log("변수 등록: "+varList[varList.Count-1].varName);
        
        
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

    public void EnableItemInfo(int index)
    {
        itemInfoRect.anchoredPosition = new Vector2(itemInfoRect.anchoredPosition.x, -120-(75*index));
        itemInfoNameText.text = currentItems[index].itemName;
        itemInfoDescText.text = currentItems[index].itemDesc;
        itemInfoRect.gameObject.SetActive(true);
    }

    public void DisableItemInfo()
    {
        itemInfoRect.gameObject.SetActive(false);
    }

    public void UpdateInventory()
    {
        for (int i = 0; i < itemHoldList.Count; i++)
        {
            if (i < currentItems.Count)
            {
                itemHoldList[i].sprite = currentItems[i].itemSpr;
                itemHoldList[i].transform.parent.gameObject.SetActive(true);
            }
            else
            {
                itemHoldList[i].sprite = null;
                itemHoldList[i].transform.parent.gameObject.SetActive(false);
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

        varList = new();
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



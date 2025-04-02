using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public VariableHolder varHolder;
    public List<VariableIntStr> varList = new();
    public Transform itemHoldParent;
    public List<Image> itemHoldList;

    public List<Item> currentItems;
    public int currentHoldingItemIndex = -1;

    [Header("Item Info")] public RectTransform itemInfoRect;

    public TMP_Text itemInfoNameText;
    public TMP_Text itemInfoDescText;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        MakeList();
        UpdateInventory();
    }

    public bool IsVariableExist(string varName)
    {
        return varList.Exists(x => x.varName == varName);
    }

    public bool IsVariableExist(int varCode)
    {
        bool isTrue = false; // = varList.Exists(x => x.varCode == varCode);


        for (int i = 0; i < varList.Count; i++)
            if (varCode == varList[i].varCode)
            {
                isTrue = true;
                Debug.Log("true::" + varCode);
                break;
            }

        Debug.Log("is Variable Exist : " + varCode + " :: " + isTrue);
        return isTrue;
    }

    public void AddVariable(string varNameOrCode)
    {
        string[] vstr = varHolder.FindVariableStr(varNameOrCode);

        VariableIntStr newVar = new(vstr[0], int.Parse(vstr[1]));

        if (!varList.Count.Equals(0))
        {
            if (IsVariableExist(int.Parse(vstr[1])))
                Debug.Log("varName " + vstr[0] + " and varCode " + vstr[1] +
                          " already exist, so did not add var");
            else Debug.Log("변수 등록: " + vstr[1]);
            return;
        }

        Debug.Log("varList.Count = " + varList.Count);

        varList.Add(newVar);
    }

    public void ClickToHoldItem(int index)
    {
        if (currentItems.Count == 0) return;

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


        PlayerMovement.instance.HoldItem(currentItems[currentHoldingItemIndex].itemID,
            currentItems[currentHoldingItemIndex].itemSpr);
    }

    public void EnableItemInfo(int index)
    {
        itemInfoRect.anchoredPosition = new Vector2(itemInfoRect.anchoredPosition.x, -120 - 75 * index);
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


    private void MakeList()
    {
        itemHoldList = new List<Image>();
        for (int i = 0; i < itemHoldParent.childCount; i++)
            itemHoldList.Add(itemHoldParent.GetChild(i).GetChild(0).GetComponent<Image>());

        varList = new List<VariableIntStr>();
    }

    public void AddItemInInventory(Item item)
    {
        currentItems.Add(item);
        UpdateInventory();
    }

    public bool CheckIfItemIsInInventory(int itemID)
    {
        foreach (Item item in currentItems)
            if (item.itemID == itemID)
                return true;

        return false;
    }
}
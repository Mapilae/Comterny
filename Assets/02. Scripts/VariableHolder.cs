using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VariableIntStr
{
    public string varName;
    public int varCode = 100;

    public VariableIntStr(string varName, int varCode)
    {
        this.varName = varName;
        this.varCode = varCode;
    }

    public VariableIntStr()
    {
        varName = "";
        varCode = 100;
    }
}

[Serializable]
public class Item
{
    [Header("about items")] public int itemID;

    public string itemName;
    [TextArea] public string itemDesc;
    public Sprite itemSpr;
}

[CreateAssetMenu(fileName = "VariableHolder", menuName = "ScriptableObjects/VariableHolder", order = 3)]
public class VariableHolder : ScriptableObject
{
    [Header("All variables used In game is Listed here.\nVarCode must be >= 100.")]
    public List<VariableIntStr> allVariableList;

    [Header("-------------------------------------------------------------------")]
    [Header("All Items used In game is Listed here.\nitemID must be <= 99.")]
    public List<Item> allItemList;


    public VariableIntStr FindVariable(string varName)
    {
        return allVariableList.Find(x => x.varName == varName);
    }

    public VariableIntStr FindVariable(int varCode)
    {
        return allVariableList.Find(x => x.varCode == varCode);
    }

    public string[] FindVariableStr(string varName)
    {
        VariableIntStr vstr = allVariableList.Find(x => x.varName == varName);
        string[] newstr = { vstr.varName, vstr.varCode.ToString() };
        return newstr;
    }

    public Item FindItem(string itemName)
    {
        return allItemList.Find(x => x.itemName == itemName);
    }

    public Item FindItem(int itemID)
    {
        return allItemList.Find(x => x.itemID == itemID);
    }
}
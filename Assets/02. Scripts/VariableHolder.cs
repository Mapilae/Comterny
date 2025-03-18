using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class VariableIntStr
{
    public string varName;
    public int varCode=100;

    public VariableIntStr(string varName, int varCode)
    {
        this.varName = varName;
        this.varCode = varCode;
    }

    public VariableIntStr()
    {
        this.varName = "";
        this.varCode = 100;
    }
}

[System.Serializable]
public class Item
{
    [Header("about items")]
    public int itemID;
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

    public Item FindItem(string itemName)
    {
        return allItemList.Find(x => x.itemName == itemName);
    }

    public Item FindItem(int itemID)
    {
        return allItemList.Find(x => x.itemID == itemID);
    }
}

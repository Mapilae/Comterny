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

[CreateAssetMenu(fileName = "VariableHolder", menuName = "ScriptableObjects/VariableHolder", order = 3)]
public class VariableHolder : ScriptableObject
{
    [Header("All variables used In game is Saved here.")]
    [Header("VarCode Must be higher than 100")]
    public List<VariableIntStr> allVariableList;

    public VariableIntStr FindVariable(string varName)
    {
    
        return allVariableList.Find(x => x.varName == varName);
    }

    public VariableIntStr FindVariable(int varCode)
    {
        return allVariableList.Find(x => x.varCode == varCode);
    }
}

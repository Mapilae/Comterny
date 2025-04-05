using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    // 모든 데이터를 한곳에 두고 관리하는 클래스
    // 다만 다이얼로그(시나리오) 출력과 관련된 부분은 제외함 (이유: 다이얼로그 코드 내에서 호출 수가 너무 잦아서)
    //
    // 데이터의 종류
    // - 잠시 보관하는 데이터(게임을 끄면 사라지는 정보)
    // - 영구 보관하는 데이터(로컬 세이브/게임을 껐다 켜도 유지되어야 하는 정보)
    // - 코드에 내장된 데이터(비교용)
    // 관리의 종류
    // - 데이터 읽기 (return 값)
    // - 데이터 추가
    // - 특정 데이터가 존재하는지 비교

    [Header("Current Info")] public int crtStage;

    public List<int> items;
    public List<int> variables;

    [Space] public string version;

    public string playerName;

    [Header("ETC")] public VariableHolder varHolder;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void ChangeCrtStage(int stageNum)
    {
        crtStage = stageNum;
        Debug.Log("스테이지 변경: crtStage");
    }

    public bool IsVariableExist(int varCode)
    {
        bool isTrue = false; // = varList.Exists(x => x.varCode == varCode);


        for (int i = 0; i < variables.Count; i++)
            if (varCode == variables[i])
            {
                isTrue = true;
                Debug.Log("true::" + varCode);
                break;
            }

        Debug.Log("is Variable Exist : " + varCode + " :: " + isTrue);
        return isTrue;
    }

    public bool IsItemExist(int itemID)
    {
        bool isTrue = false; // = varList.Exists(x => x.varCode == varCode);


        for (int i = 0; i < items.Count; i++)
            if (itemID == variables[i])
            {
                isTrue = true;
                Debug.Log("true::" + itemID);
                break;
            }

        Debug.Log("is Item Exist : " + itemID + " :: " + isTrue);
        return isTrue;
    }

    public void AddVariable(string varNameOrCode)
    {
        string[] vstr = varHolder.FindVariableStr(varNameOrCode);

        int newVar = int.Parse(vstr[1]);

        if (!variables.Count.Equals(0))
        {
            if (IsVariableExist(int.Parse(vstr[1])))
                Debug.Log("varName " + vstr[0] + " and varCode " + vstr[1] +
                          " already exist, so did not add var");
            else Debug.Log("변수 등록: " + vstr[1]);
            return;
        }

        Debug.Log("varList.Count = " + variables.Count);

        variables.Add(newVar);
    }

    public Item GetItemFromIndex(int index)
    {
        return varHolder.FindItem(items[index]);
    }

    public Item GetItemFromID(int itemID)
    {
        return varHolder.FindItem(itemID);
    }

    public void AddItem(int itemID)
    {
        if (!variables.Count.Equals(0))
        {
            if (IsItemExist(itemID))
                Debug.Log("itemID " + itemID +
                          " already exist, so did not add in items");
            else Debug.Log("변수 등록: " + itemID);
            return;
        }

        items.Add(itemID);
        Inventory.instance.UpdateInventory();
    }
}
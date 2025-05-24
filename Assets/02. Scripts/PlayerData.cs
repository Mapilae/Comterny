using System.Collections.Generic;
using System.Linq;
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

    public static PlayerData instance;

    public static string playerName = "유저 디버그";

    [Header("Current Info")] public int crtStage;

    public List<int> items;
    public List<int> variables;

    [Space] public string version;
    public int difficulty; //난이도. 0: Easy, 1: 컴공모드

    [Header("ETC")] public VariableHolder varHolder;


    private void Awake()
    {
        if (instance != this && instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void SetData(LoadedDatas datas)
    {
        crtStage = datas.crtStage;
        difficulty = datas.difficulty;
        variables = datas.variables.ToList();
        items = datas.items.ToList();
        playerName = datas.playerName;
    }

    public void SetDifficulty(int diff)
    {
        difficulty = diff;
        //SAVE
        //Debug.Log("Player Difficulty Set: " + diff);
    }

    public void SetName(string newName)
    {
        playerName = newName;
        //SAVE
    }

    public void ChangeCrtStage(int stageNum)
    {
        crtStage = stageNum;
        Debug.Log("스테이지 변경: crtStage");
    }

    public bool IsVariableExist(int varCode)
    {
        var isTrue = false; // = varList.Exists(x => x.varCode == varCode);


        for (var i = 0; i < variables.Count; i++)
            if (varCode == variables[i])
            {
                isTrue = true;

                break;
            }

        // Debug.Log("is Variable Exist : " + varCode + " :: " + isTrue);
        return isTrue;
    }

    public bool IsItemExist(int itemID)
    {
        var isTrue = false; // = varList.Exists(x => x.varCode == varCode);


        for (var i = 0; i < items.Count; i++)
            if (itemID == items[i])
            {
                isTrue = true;

                break;
            }

        //  Debug.Log("is Item Exist : " + itemID + " :: " + isTrue);
        return isTrue;
    }

    //리스트에 변수 추가해줌
    public void AddVariable(int varCodeToAdd)
    {
        if (varCodeToAdd < 100)
        {
            AddItem(varCodeToAdd);
        }
        else
        {
            if (!variables.Count.Equals(0))
                if (IsVariableExist(varCodeToAdd))
                    //                Debug.Log("varName " + varCodeToAdd + " and varCode " + varCodeToAdd +" already exist, so did not add var");
                    return;

            // Debug.Log("변수 등록: " + varCodeToAdd);
            variables.Add(varCodeToAdd);
        }
    }

    public void RemoveVariable(int varCodeToRemove)
    {
        if (varCodeToRemove < 100)
        {
            RemoveItem(varCodeToRemove);
        }
        else
        {
            if (!variables.Count.Equals(0))
                if (IsVariableExist(varCodeToRemove))
                    return;


            variables.Remove(varCodeToRemove);
        }
    }

    // public Item GetItemFromIndex(int index)
    // {
    //     
    //     return varHolder.FindItem(items[index]);
    // }

    public Item GetItemFromID(int itemID)
    {
        return varHolder.FindItem(itemID);
    }

    public void AddItem(int itemID)
    {
        if (!items.Count.Equals(0))
            if (IsItemExist(itemID))
            {
                Debug.Log("itemID " + itemID +
                          " already exist, so did not add in items");
                return;
            }

        //Debug.Log("변수 등록: " + itemID);
        items.Add(itemID);
        Inventory.instance.UpdateInventory();
    }

    public void RemoveItem(int itemID)
    {
        if (!items.Count.Equals(0))
            if (IsItemExist(itemID))
            {
                items.Remove(itemID);
                Inventory.instance.UpdateInventory();
                Inventory.instance.DisableItemInfo();

                return;
            }

        Debug.Log("itemID " + itemID +
                  " is not exist, so did not remove from items");
        //Debug.Log("변수 등록: " + itemID);
    }
}
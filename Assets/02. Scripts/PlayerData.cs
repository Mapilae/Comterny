using System.Collections.Generic;
using UnityEngine;

// 다른 스크립트의 Awake보다 먼저 실행되어야 씬마다 놓인 사본이 FindGameObjectWithTag("PlayerData")에 잡히지 않는다
[DefaultExecutionOrder(-1000)]
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

    public static string playerName = SaveSystem.DefaultPlayerName;

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
            // 씬마다 놓인 PlayerData 프리팹 사본. DontDestroyOnLoad로 살아있는 원본만 쓴다.
            // Destroy는 프레임 끝에 일어나므로, 그 전에 다른 스크립트가 사본을 집지 않도록 바로 끄고 태그도 뗀다.
            gameObject.tag = "Untagged";
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private static void RefreshInventory()
    {
        // 보스 씬/메인메뉴처럼 인벤토리 UI가 없는 씬에서도 데이터는 그대로 유지된다
        if (Inventory.instance != null) Inventory.instance.UpdateInventory();
    }
    
    
    private void Start()
    {
        //SetResolution(); // 초기에 게임 해상도 고정
    }


    public void SetResolution()
    {
        int setWidth = 1920; // 사용자 설정 너비
        int setHeight = 1080; // 사용자 설정 높이

        int deviceWidth = Screen.width; // 기기 너비 저장
        int deviceHeight = Screen.height; // 기기 높이 저장

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth)* setWidth), true); // SetResolution 함수 제대로 사용하기

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        }
    }



/// <summary>이어하기: 세이브 내용을 그대로 적용</summary>
public void ApplySave(SaveFile data)
{
    playerName = string.IsNullOrWhiteSpace(data.playerName) ? SaveSystem.DefaultPlayerName : data.playerName;
    crtStage = data.crtStage;
    difficulty = data.difficulty;
    items = new List<int>(data.items ?? new List<int>());
    variables = new List<int>(data.variables ?? new List<int>());
    RefreshInventory();
}

/// <summary>이어하기(스테이지 중간): 저장 당시의 아이템·변수로 바꾼다</summary>
public void ApplyProgress(List<int> savedItems, List<int> savedVariables)
{
    items = new List<int>(savedItems ?? new List<int>());
    variables = new List<int>(savedVariables ?? new List<int>());
    RefreshInventory();
}

/// <summary>새로 시작: 이전 플레이(타이틀로 돌아오기 전 상태 등)를 모두 비운다</summary>
public void ResetForNewGame()
{
    playerName = SaveSystem.DefaultPlayerName;
    crtStage = 0;
    difficulty = 0;
    items = new List<int>();
    variables = new List<int>();
    RefreshInventory();
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
    // 저장은 첫 스테이지에 들어갈 때 SaveSystem이 자동으로 한다
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
        if (!IsVariableExist(varCodeToRemove))
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

public void AddItem(int itemID, bool isPlayerClickSelf=true)
{
    if (!items.Count.Equals(0)){
        //원래는 중복 허용 안했으나 중복 허용하도록 변경.
         if (IsItemExist(itemID))
        {
            if(!varHolder.FindItem(itemID).canGetMultiple){
                Debug.Log("itemID " + itemID +
                          " already exist. this itemID is not allowed to get multiple items, so did not add item");
                return;

            }
        }
    }
    //Debug.Log("변수 등록: " + itemID);
    items.Add(itemID);
    if (Inventory.instance == null) return;
    Inventory.instance.UpdateInventory();
    Inventory.instance.HoldLastIndexItem(isPlayerClickSelf);
}

public bool RemoveItem(int itemID)
{
    if (!items.Count.Equals(0))
        if (IsItemExist(itemID))
        {
            items.Remove(itemID);

            if (Inventory.instance != null)
            {
                Inventory.instance.UnholdItem();
                Inventory.instance.UpdateInventory();
                Inventory.instance.DisableItemInfo();
            }

            return true;//success
        }

    Debug.Log("itemID " + itemID +
              " is not exist, so did not remove from items");
    return false;//failed
    //Debug.Log("변수 등록: " + itemID);
}

public int RemoveItemAt(int index)
{
    if(index>= items.Count || index < 0)
    {
        Debug.LogError("Index out of range: " + index);
        return -1; // or throw an exception
    }
    int itemID= items[index];
    return RemoveItem(itemID)?itemID:-1;

}

public void ResetAll()
{
    items.Clear();
    variables.Clear();
    RefreshInventory();
}
}
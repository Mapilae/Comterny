using System;
using UnityEngine;

public class LoadedDatas
{
    public int crtStage;
    public int difficulty;
    public int isDataExists;
    public int[] items;
    public string playerName;
    public int[] variables;

    public LoadedDatas(int isDataExists, string playerName, int crtStage, int[] variables, int[] items, int difficulty)
    {
        this.isDataExists = isDataExists;
        this.playerName = playerName;
        this.crtStage = crtStage;
        this.variables = variables;
        this.items = items;
        this.difficulty = difficulty;
    }
}

public class SaveData : MonoBehaviour
{
    private const string DataPlayerName = "PlayerName";
    private const string DataCrtStage = "CrtStage";
    private const string DataDifficulty = "Difficulty";
    private const string DataItems = "Items";

    private const string DataVariables = "Variables";
    private const string DataIsDataExists = "IsDataExists";

    public static SaveData instance;

    ////저장할 데이터의 목록
    //유저 이름 playerName
    //현재 스테이지 crtStage
    //가지고 있는 변수 variables
    //가지고 있는 아이템 items
    //난이도 0,1 difficulty
    private PlayerData playerData;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerData = GetComponent<PlayerData>();
    }

    public LoadedDatas Load()
    {
        var isDataExists = PlayerPrefs.GetInt(DataIsDataExists, 0);
        var playerName = PlayerPrefs.GetString(DataPlayerName, "유저 디버그");
        var crtStage = PlayerPrefs.GetInt(DataCrtStage, 0);
        var difficulty = PlayerPrefs.GetInt(DataDifficulty, 0);
        var variables = DataSplit(PlayerPrefs.GetString(DataItems, ""));
        var items = DataSplit(PlayerPrefs.GetString(DataVariables, ""));

        return new LoadedDatas(isDataExists, playerName, crtStage, variables, items, difficulty);
    }

    public void Save()
    {
        //PlayerData에 있는 것을 가져와서 세이브한다.
        PlayerPrefs.SetInt(DataIsDataExists, 1);
        PlayerPrefs.SetString(DataPlayerName, PlayerData.playerName);
        PlayerPrefs.SetInt(DataCrtStage, playerData.crtStage);
        PlayerPrefs.SetInt(DataDifficulty, playerData.difficulty);
        PlayerPrefs.SetString(DataItems, DataCombine(playerData.items.ToArray()));
        PlayerPrefs.SetString(DataVariables, DataCombine(playerData.variables.ToArray()));
        Debug.Log(
            $"저장되었습니다! playerName={PlayerData.playerName}, crtStage={playerData.crtStage}, difficulty={playerData.difficulty}, " +
            $"variables={DataCombine(playerData.variables.ToArray())}, items={DataCombine(playerData.items.ToArray())}");
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteKey(DataIsDataExists);
        PlayerPrefs.DeleteKey(DataPlayerName);
        PlayerPrefs.DeleteKey(DataCrtStage);
        PlayerPrefs.DeleteKey(DataDifficulty);
        PlayerPrefs.DeleteKey(DataItems);
        PlayerPrefs.DeleteKey(DataVariables);
        Debug.Log("로컬에 저장된 데이터가 모두 삭제되었습니다.");
    }

    private string DataCombine(int[] data)
    {
        var result = "";
        for (var i = 0; i < data.Length; i++)
        {
            result += data[i];

            if (i != data.Length - 1)
                result += ",";
        }

        return result;
    }

    private int[] DataSplit(string data)
    {
        if (data == "") return Array.Empty<int>();
        var splitData = data.Split(',');
        var iData = new int[splitData.Length];
        for (var i = 1; i < splitData.Length; i++) iData[i] = int.Parse(splitData[i]);
        return iData;
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button loadButton;
    public TMP_Text loadText;
    private LoadedDatas loadedDatas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        loadedDatas = SaveData.instance.Load();
        if (loadedDatas.isDataExists.Equals(0))
        {
            loadButton.interactable = false;
            loadText.text = "";
        }
        else
        {
            loadButton.interactable = true;
            loadText.text =
                $"{loadedDatas.playerName}, {GetDifficulty(loadedDatas.difficulty)}, {GetCrtStage(loadedDatas.crtStage)}";
        }
    }

    public void LoadDataAndStartGame()
    {
        //on button click
        PlayerData.instance.SetData(loadedDatas);
        LodingScene.instance.LoadSceneDependOnCrtStage(loadedDatas.crtStage);
    }


    private static string GetDifficulty(int difficulty)
    {
        switch (difficulty)
        {
            case 0:
                return "이지 모드";
            case 1:
                return "컴퓨터공학도 모드";
        }

        return "";
    }

    private static string GetCrtStage(int crtStage)
    {
        switch (crtStage)
        {
            case 0:
                return "컴터니 입성";
            case 1:
                return "주기억부서";
            case 2:
                return "시스템버스정류장";
            case 3:
                return "중앙처리부서";
            case 4:
                return "시스템소프트웨어부서";
            case 5:
                return "운영체제부서 가는 길";
            case 6:
                return "운영체제부서";
        }

        return "";
    }
}
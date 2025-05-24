using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class SheetLoader : MonoBehaviour
{
    public static SheetLoader instance;

    private static readonly string url =
        "https://docs.google.com/spreadsheets/d/e/2PACX-1vQL4QXgDuaYrnk5mL4aT8DTinFtM80x6Y0j2zPQkI2Y_sUhcCnJCxANag4Khc2pWA5ZK4mXq0kJKo_f/pub?output=tsv";

    public static int chapNumber = 1;

    public static bool isLoading;
    public SheetData sheetData;

    [Header("처음 시작할 때 자동실행할 다이얼로그가 있는 경우")]
    public string startDialogueStr;

    [Header("Error MSG")] public GameObject errorObj;

    private string currentId = "";
    private string strSheetData;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartLoadSheet();
    }

    public void OnErrorObjButtonClick()
    {
        errorObj.SetActive(false);
        StartLoadSheet();
    }

    public void StartLoadSheet()
    {
        SheetData.instance.block = new List<Block>(0);
        StartCoroutine(LoadSheetOnStart());
    }

    private IEnumerator LoadSheetOnStart()
    {
        var sheetURL = url + "&range=A2:F&nocache=" + DateTime.UtcNow.Ticks + Random.Range(0, 100000);
        using (var www = UnityWebRequest.Get(sheetURL))
        {
            yield return www.SendWebRequest();

            /*if (www.isDone) strSheetData = www.downloadHandler.text;
            else
            {
                Debug.Log("Error: " + www.error);
                errorObj.SetActive(true);
            }*/

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error: " + www.error);
                errorObj.SetActive(true);
                yield break; // 코루틴 종료
            }

            strSheetData = www.downloadHandler.text;
        }

        isLoading = true;
        DisplayText();
        LoadInSheetData();
        //DialogueManager.instance.crtChapter = chapNumber;
    }


    private void DisplayText()
    {
//        Debug.Log(strSheetData);
    }

    public void LoadInSheetData()
    {
        var rows = strSheetData.Split('\n');
        for (var i = 0; i < rows.Length; i++)
        {
            // 0=ID   1=start_cmd  2=StandingNum   3=Name   4=Dialogue 5=after_cmd

            var columns = rows[i].Replace("\r", "").Split('\t');
            var id = columns[0];

            var start_cmd = columns[1];
            var standingNum = columns[2];
            var char_name = columns[3];
            var dialogue = columns[4];
            var after_cmd = columns[5];

            // �迭 ���� Ȯ��
            if (columns.Length < 5)
                // Debug.LogWarning("�߸��� ������ ����: " + rows[i]);
                continue; // �����Ͱ� ������ ���� �ǳʶ�

            if (id.Equals(standingNum) && dialogue.Equals(""))
                // Debug.Log("nothing is in cell!:" + columns[0] + ", " + columns[1] + ", " + columns[2] + ", " + columns[3]);
                continue;

            if (id.Equals("//")) continue;
            if (!id.Equals(""))
            {
                //new branch
                // 0=ID   1=start_cmd  2=StandingNum   3=Name   4=Dialogue 5=after_cmd

                Block block = new(id);

                sheetData.AddBlock(block);
                var cell = MakeCell(start_cmd, standingNum, char_name, dialogue, after_cmd);
                //block.AddCell(cell);
                sheetData.AddCell(id, cell);
                currentId = id;
            }
            else
            {
                //branch already exist
                id = currentId;
                var cell = MakeCell(start_cmd, standingNum, char_name, dialogue, after_cmd);
                sheetData.AddCell(id, cell);
            }
        }

        isLoading = false;
        currentId = "";
    }

    public void StartDialogue()
    {
        var dialogueManagerObj = GameObject.FindGameObjectWithTag("DialogueManager");
        if (dialogueManagerObj != null)
            dialogueManagerObj.GetComponent<DialogueManager>().StartDialogueManager();
        else
            Debug.Log("DialogueManager is null!");
        if (!startDialogueStr.Equals(""))
        {
//            Debug.Log("Auto StartDialogue " + startDialogueStr);
            DialogueManager.instance.StartDialogueInManager(startDialogueStr);
            PlayerMovement.canMove = false;
        }
    }


    private Cell MakeCell(string start_cmd, string standingNum, string char_name, string dialogue,
        string after_cmd)
    {
        // 0=ID   1=start_cmd  2=StandingNum   3=Name   4=Dialogue  5=after_cmd

        Cell block;
        if (standingNum.Equals("")) //standingNum이 비어있는 경우, 0을 기본값으로 채움.
            block = new Cell(char_name, "0", dialogue, start_cmd, after_cmd);
        else
            block = new Cell(char_name, standingNum, dialogue, start_cmd, after_cmd);

        return block;
    }
}
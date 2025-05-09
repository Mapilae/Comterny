using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SheetLoader : MonoBehaviour
{
    public static SheetLoader instance;

    // ���̾�α״� ���� �������� ��Ʈ�� �����
    // ���̾� �α� ������ ���� ĳ���� ���� ������ �ѹ� �ؾ� ����
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
        var sheetURL = url + "&range=A2:E&nocache=" + DateTime.UtcNow.Ticks;
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
            // 0=ID   1=StandingNum   2=Name   3=Dialogue

            var columns = rows[i].Replace("\r", "").Split('\t');
            var id = columns[0];

            // �迭 ���� Ȯ��
            if (columns.Length < 4)
                // Debug.LogWarning("�߸��� ������ ����: " + rows[i]);
                continue; // �����Ͱ� ������ ���� �ǳʶ�

            if (columns[0].Equals(columns[2]) && columns[3].Equals(""))
                // Debug.Log("�� ���Դϴ�!:" + columns[0] + ", " + columns[1] + ", " + columns[2] + ", " + columns[3]);
                continue;

            if (id.Equals("//")) continue;
            if (!id.Equals(""))
            {
                // 0=ID   1=StandingNum   2=Name   3=Dialogue

                Block block = new(id);

                sheetData.AddBlock(block);
                var cell = MakeCell(columns);
                //block.AddCell(cell);
                sheetData.AddCell(id, cell);
                currentId = id;
            }
            else
            {
                //{ //���� branch��
                id = currentId;
                var cell = MakeCell(columns);
                sheetData.AddCell(id, cell);
                //}
                //else continue; //�ּ�. ó��X, ��ŵ
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
            Debug.Log("Auto StartDialogue " + startDialogueStr);
            DialogueManager.instance.StartDialogueInManager(startDialogueStr);
            PlayerMovement.canMove = false;
        }
    }


    private Cell MakeCell(string[] columns)
    {
        // 0=ID   1=StandingNum   2=Name   3=Dialogue

        Cell block;
        if (columns[1].Equals(""))
            block = new Cell(columns[2], "0", columns[3], columns[4]);
        else
            block = new Cell(columns[2], columns[1], columns[3], columns[4]);

        return block;
    }
}
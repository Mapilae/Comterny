using System.Collections.Generic;
using UnityEngine;

public class SheetLoader : MonoBehaviour
{
    public static SheetLoader instance;
    public static int chapNumber = 1;
    public static bool isLoading;
    public SheetData sheetData;

    [Header("자동 실행 다이얼로그")] public string startDialogueStr;

    [Header("에러 오브젝트")] public GameObject errorObj;

    private string currentId = "";
    private string strSheetData;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // LoadTSVFromResources("dialogue");
    }

    public void LoadTSVFromResources(string fileNameWithoutExtension)
    {
        var tsvFile = Resources.Load<TextAsset>(fileNameWithoutExtension);

        if (tsvFile == null)
        {
            Debug.LogError("TSV 파일을 Resources에서 찾을 수 없습니다: " + fileNameWithoutExtension);
            ErrorOccured();
            return;
        }

        strSheetData = tsvFile.text;
        ParseTSV();
    }

    public void ParseTSV()
    {
        SheetData.instance.sheetDataObj.block = new List<Block>();
        isLoading = true;

        var rows = strSheetData.Split('\n');
        for (var i = 0; i < rows.Length; i++)
        {
            var columns = rows[i].Replace("\r", "").Split('\t');
            if (columns.Length < 5) continue;

            var id = columns[0];
            var start_cmd = columns[1];
            var standingNum = columns[2];
            var char_name = columns[3];
            var dialogue = columns[4];
            var after_cmd = columns[5];

            if (id.Equals(standingNum) && string.IsNullOrEmpty(dialogue)) continue;
            if (id.Equals("//")) continue;

            if (!string.IsNullOrEmpty(id))
            {
                var block = new Block(id);
                sheetData.AddBlock(block);
                var cell = MakeCell(start_cmd, standingNum, char_name, dialogue, after_cmd);
                sheetData.AddCell(id, cell);
                currentId = id;
            }
            else
            {
                id = currentId;
                var cell = MakeCell(start_cmd, standingNum, char_name, dialogue, after_cmd);
                sheetData.AddCell(id, cell);
            }
        }

        isLoading = false;
        currentId = "";
        StartDialogue();
    }

    private Cell MakeCell(string start_cmd, string standingNum, string char_name, string dialogue, string after_cmd)
    {
        return new Cell(char_name, string.IsNullOrEmpty(standingNum) ? "0" : standingNum, dialogue, start_cmd,
            after_cmd);
    }

    public void StartDialogue()
    {
        var dialogueManagerObj = GameObject.FindGameObjectWithTag("DialogueManager");
        if (dialogueManagerObj != null)
            dialogueManagerObj.GetComponent<DialogueManager>().StartDialogueManager();
        else
            Debug.Log("DialogueManager is null!");

        if (!string.IsNullOrEmpty(startDialogueStr))
        {
            DialogueManager.instance.StartDialogueInManager(startDialogueStr);
            PlayerMovement.canMove = false;
        }
    }

    public void ErrorOccured()
    {
        errorObj.SetActive(true);
    }

    public void ErrorDone()
    {
        errorObj.SetActive(false);
    }
}
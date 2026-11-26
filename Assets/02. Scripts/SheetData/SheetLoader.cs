using UnityEngine;

public class SheetLoader : MonoBehaviour
{
    public static SheetLoader instance;
    public static int chapNumber = 1;
    public static bool isLoading;
    public SheetData sheetData;

    [Header("자동 실행 다이얼로그")] public string startDialogueStr;

    [Header("에러 오브젝트")] public GameObject errorObj;

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
        isLoading = true;
        SheetData.instance.sheetDataObj.block = SheetTsvParser.Parse(strSheetData);
        isLoading = false;
        StartDialogue();
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
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    /// <summary>
    ///     다이얼로그를 클릭, 실행하는 역할 수행
    ///     다른 개인 비주얼노벨 게임에 있던 코드를 복사해와서 안쓰는 코드가 좀 섞여있습니다.
    /// </summary>
    public static DialogueManager instance;

    public static float FIXED_HEIGHT = Screen.height; //* 0.45f;

    public static bool isPlayingDialogue;

    [Header("Dialogue")] public Canvas uiCanvas;

    public List<Cell> dialogueList;
    public int crtIndex;

    public GameObject dialogueWholeObj;
    public Animator dialogWhole;
    public TMP_Text nameTxt;
    public TMP_Text contentTxt;
    public RectTransform contentTrans;
    public Volume currentVolumeObj;

    public SoundManager soundManager;
    public StandingSpriteManager sprManager;
    public StandingAnim standingImg;


    [Header("Others")] public TypeWriter typeWriter;
    public bool isPause;
    public bool isViewPause;
    public bool canClickToNext = true;
    public Button skipButton;
    [Header("CHOICE")] public GameObject choiceWholeObj;
    public GameObject[] choiceObjs;
    public string[] choiceCMD;
    private DirectingManager dirManager;

    private Inventory inventory;

    private Vector3[] standingDftPos = new Vector3[3];
    private currentState state;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //        StartDialogueManager();
        //set standingDftPos
        dirManager = GetComponent<DirectingManager>();
    }


    private void Update()
    {
        if (isPause || isViewPause) return;

        if (dialogWhole.gameObject.activeInHierarchy)
            //Spacebar or EnterKey, or Click Between bottom 0~700
            if (canClickToNext && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) ||
                                   (Input.mousePosition.y <= FIXED_HEIGHT && Input.GetMouseButtonDown(0)) ||
                                   Input.mouseScrollDelta.y != 0.0))
                ChangeDialogue();

#if UNITY_IOS
        if (Input.touchCount > 0)
        {
           // if (!isNoNext && (Input.GetTouch(0).phase == TouchPhase.Ended && Input.GetTouch(0).position.y <= FIXED_HEIGHT))
          //  {
          //      tmptext.text = Input.GetTouch(0).position.y.ToString();
           //     ChangeDialogue();
           // }
        }
#endif
#if UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            // if (!isNoNext && (Input.GetTouch(0).phase == TouchPhase.Ended && Input.GetTouch(0).position.y <= FIXED_HEIGHT))
            // {
            // tmptext.text = Input.GetTouch(0).position.y.ToString();
            //     ChangeDialogue();
            // }
        }
#endif
    }

    public void ItemGetDialogue(string msg)
    {
        List<Cell> dialogue = new()
        {
            new Cell("", "0", msg, "")
        };
        Inventory.instance.HoldLastIndexItem();
        state = currentState.ITEM;
        StartDialogue(dialogue);
    }

    public void StartDialogueManager()
    {
        inventory = uiCanvas.GetComponent<Inventory>();
        contentTrans = contentTxt.GetComponent<RectTransform>();
    }

    public void StartDialogue(List<Cell> dialogue)
    {
        MobileUI.instance.HideThis(true);
        ClearDialogue(dialogue);
        standingImg.StandingAlpha(true);
        dialogWhole.gameObject.SetActive(true);
        isPlayingDialogue = true;
        PlayerMovement.instance.ChangeCanMove(false);
        ChangeDialogue();
    }

    private void ClearDialogue(List<Cell> dialogue)
    {
        //다이얼로그를 초기상태로 초기화
        dialogueList = new List<Cell>();
        dialogueList = dialogue;

        crtIndex = 0;
    }


    public void ChangeDialogue()
    {
        if (!canClickToNext) return;


        /*if (currentBlock.bgm != null)
        {
            if (SoundManager.instance != null) SoundManager.instance.PlayBGM(currentBlock.bgm, currentBlock.bgmSubtitle, !currentBlock.isBGMNoLoop);
            else soundManager.PlayBGM(currentBlock.bgm, currentBlock.bgmSubtitle, !currentBlock.isBGMNoLoop);
            //if (currentBlock.se != null) SoundManager.instance.PlaySE(currentBlock.se);
        }*/

        dialogWhole.gameObject.SetActive(true);


        if (typeWriter.isTyping)
        {
            typeWriter.StopTyping();
            return;
        }

        if (dialogueList == null || dialogueList.Count <= crtIndex)
        {
            EndDialogue();
            return;
        }


        // if (sheetData.storyBlock[crtBranch].block[crtPage].se != null)
        //  {
        // SoundManager.instance.PlaySound(sheetData.storyBlock[crtBranch].block[crtPage].se, sheetData.storyBlock[crtBranch].block[crtPage].seSubtitle);
        //  }
        //if(�������)string content =content_ENG

        var content = ChangeIfPlayerNameIncluded(dialogueList[crtIndex].content);
        var strName = ChangeIfPlayerName(dialogueList[crtIndex].name);


        nameTxt.text = strName;
        nameTxt.transform.parent.gameObject.SetActive(!strName.Equals(""));


        standingImg.image.sprite =
            sprManager.FindSprite(dialogueList[crtIndex].name, dialogueList[crtIndex].standingNum);
        standingImg.image.enabled = standingImg.image.sprite != null;


        if (content.Contains("{"))
        {
            Regex regex = new(@"\{(.*?)\}");
            content = regex.Replace(content, match =>
            {
                var key = match.Groups[1].Value.Trim(); // ������ ���� �� ���� ����
                if (key.Equals("Player")) return PlayerData.playerName;
                var value = PlayerPrefs.GetString(key, "null");
                return value;
            });
        }

        content = content.Replace("<red>", "<color=red>");
        content = content.Replace("</red>", "</color>");

        content = content.Replace("<small>", "<size=70%>");
        content = content.Replace("</small>", "</size>");

        content = content.Replace("<big>", "<size=130%>");
        content = content.Replace("</big>", "</size>");

        content = content.Replace("<b>", "<b><color=orange>");
        content = content.Replace("</b>", "</color></b>");

        typeWriter.StartTyping(content, dialogueList[crtIndex].name); //(int)currentBlock.block[index].name_ch

        dirManager.RunCMD(dialogueList[crtIndex].cmd);

        //ChangeSprite();

        // if (index >= currentBlock.block.Count) isNoNext = true;
        //else isNoNext = false;
        crtIndex++;
    }

    private string ChangeIfPlayerName(string current)
    {
        if (current.Equals("유저 디버그")) //name=User Debug
        {
            if (PlayerData.playerName.Equals("유저 디버그")) return current; //name=User Debug
            return PlayerData.playerName;
        }

        return current;
    }

    private string ChangeIfPlayerNameIncluded(string current)
    {
        if (PlayerData.playerName.Equals("유저 디버그")) return current;

        if (current.Contains("유저") || current.Contains("디버그")) //name=User Debug
        {
            current = current.Replace("유저 디버그", PlayerData.playerName);
            current = current.Replace("유저", PlayerData.playerName);
            current = current.Replace("디버그", PlayerData.playerName);
            return current;
        }

        return current;
    }

    private void WhenLastPage()
    {
        /*
        switch (sheetData.storyBlock[crtBranch].ifEnd.ToString())
        {
            case "CHOICE": //choice
                {
                    isNoNext = true;
                    SetChoice();
                }
                break;
            case "NEW": //new
                {
                    ChangeCurrentBlock(currentBlock.newBlock);


                }
                break;
            case "MINIGAME"://minigame
                {
                    isNoNext = true;

                    dialogWhole.SetTrigger("OFF");
                    DisableSprite();
                    StartCoroutine(DisableObj(dialogWhole.gameObject, 0.25f));
                    currentBlock.minigameObj.SetActive(true);
                }
                break;
            case "GIFT"://gift
                {
                    //gift ���� ��ũ��Ʈ-�κ��丮or���� ��ũ��Ʈ�� �����ϱ�
                    inventory.StartGiftEvent(currentBlock.itemBlock);

                    //currentBlock =
                    //ChangeDialogue();
                }
                break;


        if (thisMode.Equals(state.ITEM))
        {
            currentBlock = null;
            isNoNext = true;
        }*/
    }

    private void DisableSprite()
    {
        standingImg.StandingAlpha(false);
    }

    public void EndDialogue()
    {
        if (typeWriter.isTyping) typeWriter.StopTyping(); //타이핑중인 경우 강제종료(삭제되었음에도 이전꺼가 끝까지 타이핑되는 버그 제거 위함)
        ClearDialogue(null);
        skipButton.interactable = false;
        skipButton.interactable = true;

        StartCoroutine(IEEndDialogue());
    }


    private IEnumerator Delay(float seconds)
    {
        canClickToNext = false;
        dialogWhole.gameObject.SetActive(false);
        yield return new WaitForSeconds(seconds);
        //Debug.Log("Play again");
        canClickToNext = true;
        dialogWhole.gameObject.SetActive(true);

        ChangeDialogue();

        yield return null;
    }

    public void StartDialogueInManager(string idName)
    {
        PlayerMovement.canMove = false;
        StartCoroutine(IEStartDialogue(idName));
    }

    private IEnumerator IEEndDialogue()
    {
        standingImg.StandingAlpha(false);
        dialogWhole.SetTrigger("end");
        yield return new WaitForSeconds(0.3f);

        isPlayingDialogue = false;


        MobileUI.instance.HideThis(false);


        if (state.Equals(currentState.ITEM))
        {
            Inventory.instance.UnholdItem();
            state = currentState.DIALOGUE;
        }

        PlayerMovement.instance.ChangeCanMove(true);
        dialogWhole.gameObject.SetActive(false);
    }

    private IEnumerator IEStartDialogue(string idName)
    {
        //if (!isLoaded) LoadDialogueFromSheetData();
        //Debug.Log("Start Dialogue waiting.");
        //WaitUntil waitUntil = new(() => isLoaded);
        //yield return waitUntil;
        //Debug.Log("Start Dialogue waiting end.");

        var cells = LoadCellsFromSheetData(idName);
        instance.StartDialogue(cells);

        PlayerMovement.instance.ChangeCanMove(false);
        yield return null;
    }

    public List<Cell> LoadCellsFromSheetData(string idName)
    {
        var index = SheetData.instance.block.FindIndex(x => x.id == idName);
        return SheetData.instance.block[index].cells;
    }

    private enum currentState
    {
        DIALOGUE,
        ITEM
    }


    #region CHOICE

    public void StartChoice()
    {
        choiceWholeObj.SetActive(true);
        //choice개수에 따라 choiceObjs 활성화하기
    }

    public void OnChoiceButtonClicked(int index)
    {
        Debug.Log("choice : " + choiceCMD[index]);
        choiceWholeObj.SetActive(false);
    }

    #endregion
}
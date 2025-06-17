using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

[Serializable]
public class Dialogue
{
    public Image wholeObj; //Panel
    public TMP_Text nameTxt;
    public TMP_Text contentTxt;
    public Sprite[] sprDialogueFrame;
    public Material[] matDialogueTxt;

    [Space] public StandingAnim standingImg;

    public Button skipButton;
    public Animator dialogAnim;


    public StandingSpriteManager sprManager;
    private RectTransform contentTrans;
    private Image dialogueFrame;

    private bool isFadeOn;
    private bool isRansomwareVillian;

    public void Init()
    {
        contentTrans = contentTxt.GetComponent<RectTransform>();
        dialogueFrame = dialogAnim.GetComponent<Image>();
        contentTrans = contentTxt.GetComponent<RectTransform>();
    }

    public bool IsActive()
    {
        return wholeObj != null && wholeObj.gameObject.activeInHierarchy;
    }

    public void SetActive(bool active)
    {
        wholeObj.gameObject.SetActive(active);
    }

    public void SetStandingImgAlpha(bool isFadeOn)
    {
        standingImg.StandingAlpha(isFadeOn);
        this.isFadeOn = isFadeOn;
    }

    public void SetNameTxt(string newName)
    {
        nameTxt.text = newName.Equals("") ? "" : $"< {newName} >";
    }

    public void SetStandingImg(string name, string standingNum)
    {
        var split = sprManager.StandingNumParse(name, standingNum);
        name = split[0];
        standingNum = split[1];
        var newSprite = sprManager.FindSprite(name, standingNum);
        standingImg.image.sprite = newSprite;
        standingImg.image.enabled = standingImg.image.sprite != sprManager.nullSprite;

        if (!isFadeOn && standingImg.image.enabled)
            SetStandingImgAlpha(true);

        SetFrameColor(name);
    }

    private void SetFrameColor(string name)
    {
        /* 랜섬웨어 '창'과 '이름'을 빨강으로 하기(폰트말고)
        테두리
        디버그: 5937F8
        컴파일러: FFFFFF
        프로세서: F0D63D
        램: 31DB6A
        랜섬웨어(갱생): FF0032
        랜섬웨어(빌런): 폰트 테두리에 Thickness 0.18f, Dilate 0.1f넣기. (평소는 둘다 0, 0)
         */

        string hex;
        var beforeIsRansomwareVillian = isRansomwareVillian;
        isRansomwareVillian = false;
        switch (name)
        {
            case "유저 디버그":
                hex = "#5937F8";
                break;
            case "램":
                hex = "#31DB6A";
                break;
            case "프로세서": //processor
                hex = "#F0D63D";
                break;
            case "컴파일러":
                hex = "#FFFFFF";
                break;
            case "랜섬웨어빌런":
                hex = "#FFFFFF";
                isRansomwareVillian = true;
//                Debug.Log("랜섬웨어빌런");
                break;
            case "랜섬웨어":
                hex = "#FF0032";
                break;
            default:
                hex = "#CCCCCC";
                break;
        }

        ColorUtility.TryParseHtmlString(hex, out var fontColor);
        fontColor.a = 0.85f;
        dialogueFrame.color = fontColor;
        nameTxt.color = fontColor;

        if (beforeIsRansomwareVillian ^ isRansomwareVillian)
        {
            dialogueFrame.sprite = sprDialogueFrame[isRansomwareVillian ? 1 : 0];
            contentTxt.fontSharedMaterial = matDialogueTxt[isRansomwareVillian ? 1 : 0];
        }
    }

    public void ResetSkipButton()
    {
        skipButton.interactable = false;
        skipButton.interactable = true;
    }

    public void SetTrigger(string animName)
    {
        dialogAnim.SetTrigger(animName);
    }
}

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

    public Dialogue dialogue;

    [Space] public Volume currentVolumeObj;

    public SoundManager soundManager;

    [Header("Others")] public TypeWriter typeWriter;
    public bool isPause;
    public bool isViewPause;
    public bool canClickToNext = true;

    private DirectingManager dirManager;

    private Inventory inventory;

    private Vector3[] standingDftPos = new Vector3[3];
    private currentState state;

    private void Awake()
    {
        instance = this;
        dirManager = GetComponent<DirectingManager>();
    }

    private void Start()
    {
        //        StartDialogueManager();
        //set standingDftPos

        dialogue.Init();
    }


    private void Update()
    {
        if (isPause || isViewPause) return;

        if (dialogue.IsActive())
        {
            var inputTriggered = false;

            // PC: Space, Enter, Mouse Click
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) ||
                Input.GetMouseButtonDown(0) || Input.mouseScrollDelta.y != 0.0f)
                inputTriggered = true;
            // 모바일: 터치가 '끝나는 순간'만 체크
            if (Input.touchCount > 0)
                foreach (var touch in Input.touches)
                    if (touch.phase == TouchPhase.Ended)
                    {
                        inputTriggered = true;
                        break;
                    }

            if (canClickToNext && inputTriggered)
            {
                ChangeDialogue();

                // 1) UI 버튼 Selected 상태 해제
                EventSystem.current.SetSelectedGameObject(null);

                // 2) 입력 축 상태(스페이스바 등) 모두 리셋
                Input.ResetInputAxes();
            }
        }
    }

    public void ItemGetDialogue(string msg)
    {
        List<Cell> dialogueCell = new()
        {
            new Cell("", "0", msg, "", "")
        };
        // Inventory.instance.HoldLastIndexItem();
        dialogue.wholeObj.enabled = false;
        state = currentState.ITEM;
        StartDialogue(dialogueCell);
    }

    public void StartDialogueManager()
    {
        inventory = uiCanvas.GetComponent<Inventory>();
    }

    public void StartDialogue(List<Cell> dialogueCell)
    {
        MobileUI.instance.HideThis(true);
        ClearDialogue(dialogueCell);
        CameraFollow.instance.ExcludeLayerInCamera(true);
        dialogue.SetActive(true);
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

        dialogue.SetActive(true);


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

        dialogue.SetNameTxt(strName);
        dialogue.SetStandingImg(dialogueList[crtIndex].name, dialogueList[crtIndex].standingNum);


        if (content.Contains("{"))
        {
            Regex regex = new(@"\{(.*?)\}");
            content = regex.Replace(content, match =>
            {
                var key = match.Groups[1].Value.Trim();

                // switch문에 따라 "player"도 처리하도록 통합
                var value = PlayerPrefs.GetString("PlayerName", PlayerData.playerName);

                // name 기준 분기
                switch (dialogueList[crtIndex].name)
                {
                    case "램":
                    case "프로세서":
                    case "랜섬웨어":
                    case "랜섬웨어빌런":
                        return key.Equals("player") && PlayerData.playerName.Equals("유저 디버그") ? "유저" : value;

                    case "컴파일러":
                    case "폰 노이만":
                    case "회장님":
                        return key.Equals("player") && PlayerData.playerName.Equals("유저 디버그") ? "디버그" : value;

                    default:
                        return key.Equals("player") ? PlayerData.playerName : value;
                }
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

        dirManager.RunCMD(dialogueList[crtIndex].start_cmd);

        //ChangeSprite();

        // if (index >= currentBlock.block.Count) isNoNext = true;
        //else isNoNext = false;
        //TypeWriter가 타이핑 이후에 WhenDialogueTypingEnd()를 자동으로 호출하고, 거기서 crtIndex를 1 증가시킴
    }

    public void WhenDialogueTypingEnd()
    {
        if (dirManager != null && dialogueList.Count > 0 && dialogueList.Count > crtIndex)
            if (dialogueList[crtIndex].after_cmd != "")
            {
                dirManager.RunCMD(dialogueList[crtIndex].after_cmd);
                Debug.Log("after_cmd: " + dialogueList[crtIndex].after_cmd);
            }

//        Debug.Log("WhenDialogueTypingEnd called");
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
        dialogue.SetStandingImgAlpha(false);
    }

    public void SkipDialogue()
    {
        dirManager.CheckCmdImportant(dialogueList, crtIndex);
        EndDialogue();
    }

    public void EndDialogue(bool checkDialogueTypingEnd = true)
    {
        if (typeWriter.isTyping) typeWriter.StopTyping(); //타이핑중인 경우 강제종료(삭제되었음에도 이전꺼가 끝까지 타이핑되는 버그 제거 위함)
        else if (checkDialogueTypingEnd) WhenDialogueTypingEnd();

        dialogue.ResetSkipButton();

        StartCoroutine(IEEndDialogue());

        PlayerMovement.instance.ChangeCanMove(true);
    }


    private IEnumerator Delay(float seconds)
    {
        canClickToNext = false;
        dialogue.SetActive(false);
        yield return new WaitForSeconds(seconds);
        //Debug.Log("Play again");
        canClickToNext = true;
        dialogue.SetActive(true);

        ChangeDialogue();

        yield return null;
    }

    public void StartDialogueInManager(string idName)
    {
        PlayerMovement.canMove = false;
        dialogue.wholeObj.enabled = true;
        StopCoroutine(nameof(IEStartDialogue));
        StartCoroutine(nameof(IEStartDialogue), idName);
    }

    private IEnumerator IEEndDialogue()
    {
        dialogue.SetStandingImgAlpha(false);
        dialogue.SetTrigger("end");
        yield return new WaitForSeconds(0.3f);

        isPlayingDialogue = false;

        ClearDialogue(null);

        if (!PlayerMovement.isCutscene)
        {
            MobileUI.instance.HideThis(false);
            CameraFollow.instance.ExcludeLayerInCamera(false);
        }

        if (state.Equals(currentState.ITEM))
        {
            Inventory.instance.UnholdItem();
            state = currentState.DIALOGUE;
        }

        PlayerMovement.instance.ChangeCanMove(true);
        dialogue.SetActive(false);
    }

    private IEnumerator IEStartDialogue(string idName)
    {
        //if (!isLoaded) LoadDialogueFromSheetData();
        //Debug.Log("Start Dialogue waiting.");
        //WaitUntil waitUntil = new(() => isLoaded);
        //yield return waitUntil;
        //Debug.Log("Start Dialogue waiting end.");

        List<Cell> cells = null;
        while (cells == null)
        {
            cells = LoadCellsFromSheetData(idName);
            if (cells == null)
                yield return new WaitForSeconds(1f);
        }

        SheetLoader.instance.ErrorDone(); //만약 에러 창 뜬 경우 없애기
        instance.StartDialogue(cells);

        PlayerMovement.instance.ChangeCanMove(false);
        yield return null;
    }

    public List<Cell> LoadCellsFromSheetData(string idName)
    {
        try
        {
            if (SheetData.instance.block == null || SheetData.instance.block.Count == 0)
                Debug.LogWarning("시트 로드 아직 안됨!");

            var index = SheetData.instance.block.FindIndex(x => x.id == idName);
            if (SheetData.instance.block.Count <= index)
                Debug.LogWarning($"index보다 시트 카운트수가 더 적습니다! {SheetData.instance.block.Count}<={index}");
            return SheetData.instance.block[index].cells;
        }
        catch
        {
            SheetLoader.instance.ErrorOccured();
            return null;
        }
    }

    private enum currentState
    {
        DIALOGUE,
        ITEM
    }
}
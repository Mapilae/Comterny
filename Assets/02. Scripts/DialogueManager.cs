using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

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
    private DirectingManager dirManager;

    private Inventory inventory;

    private Vector3[] standingDftPos = new Vector3[3];


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
            new Cell("", 0, msg, "")
        };
        StartDialogue(dialogue);
    }

    public void StartDialogueManager()
    {
        inventory = uiCanvas.GetComponent<Inventory>();
        contentTrans = contentTxt.GetComponent<RectTransform>();
    }

    public void StartDialogue(List<Cell> dialogue)
    {
        dialogueList = new List<Cell>();
        dialogueList = dialogue;
        standingImg.StandingAlpha(true);
        crtIndex = 0;
        dialogWhole.gameObject.SetActive(true);
        isPlayingDialogue = true;
        ChangeDialogue();
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
        }
        else
        {
            if (dialogueList.Count <= crtIndex)
            {
                dialogWhole.gameObject.SetActive(false);
                standingImg.StandingAlpha(false);
                PlayerMovement.instance.ChangeCanMove(true);
                isPlayingDialogue = false;
                Debug.Log("종료");
                return;
            }


            // if (sheetData.storyBlock[crtBranch].block[crtPage].se != null)
            //  {
            // SoundManager.instance.PlaySound(sheetData.storyBlock[crtBranch].block[crtPage].se, sheetData.storyBlock[crtBranch].block[crtPage].seSubtitle);
            //  }
            //if(�������)string content =content_ENG

            string content = dialogueList[crtIndex].content;
            // nameTxt.transform.parent.gameObject.SetActive(!dialogueList[crtIndex].name.Equals(""));
            nameTxt.text = ChangeIfPlayerName(dialogueList[crtIndex].name);
            standingImg.image.sprite =
                sprManager.FindSprite(dialogueList[crtIndex].name, dialogueList[crtIndex].standingNum);
            standingImg.image.enabled = standingImg.image.sprite != null;


            if (content.Contains("{"))
            {
                // Debug.Log("ġȯ ��: " + content);

                Regex regex = new(@"\{(.*?)\}");
                content = regex.Replace(content, match =>
                {
                    string key = match.Groups[1].Value.Trim(); // ������ ���� �� ���� ����
                    // Debug.Log($"ġȯ �õ�: {key}");

                    if (key.Equals("Player")) return PlayerMovement.playerName;
                    // PlayerPrefs���� �� ��������
                    string value = PlayerPrefs.GetString(key, "null");
                    // Debug.Log($"PlayerPrefs���� ������ ��: {value}");


                    return value;
                });

                // Debug.Log("ġȯ ��: " + content);
            }

            content = content.Replace("<red>", "<color=red>");
            content = content.Replace("</red>", "</color>");

            content = content.Replace("<small>", "<size=70%>");
            content = content.Replace("</small>", "</size>");

            content = content.Replace("<big>", "<size=130%>");
            content = content.Replace("</big>", "</size>");

            typeWriter.StartTyping(content, nameTxt.text); //(int)currentBlock.block[index].name_ch

            dirManager.RunCMD(dialogueList[crtIndex].cmd);

            //ChangeSprite();

            // if (index >= currentBlock.block.Count) isNoNext = true;
            //else isNoNext = false;
            crtIndex++;
        }
    }

    private string ChangeIfPlayerName(string current)
    {
        if (current.Equals("유저 디버그")) //name=User Debug
        {
            if (PlayerMovement.playerName.Equals("유저 디버그")) return current; //name=User Debug
            return PlayerMovement.playerName;
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
        StartCoroutine(IEStartDialogue(idName));
    }

    private IEnumerator IEStartDialogue(string idName)
    {
        //if (!isLoaded) LoadDialogueFromSheetData();
        //Debug.Log("Start Dialogue waiting.");
        //WaitUntil waitUntil = new(() => isLoaded);
        //yield return waitUntil;
        //Debug.Log("Start Dialogue waiting end.");

        int index = SheetData.instance.block.FindIndex(x => x.id == idName);
        instance.StartDialogue(SheetData.instance.block[index].cells);
        yield return null;
    }
}
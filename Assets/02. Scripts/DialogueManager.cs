using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("Dialogue")]
    public Canvas uiCanvas;
    Inventory inventory;

    public List<Dialogue> dialogueList;
    public int crtIndex = 0;

    public GameObject dialogueWholeObj;
    public Animator dialogWhole;
    public TMP_Text nameTxt;
    public TMP_Text contentTxt;
    public RectTransform contentTrans;
    public Volume currentVolumeObj;

    public SoundManager soundManager;
    public StandingSpriteManager sprManager;
    public Image standingImg;


    [Header("Others")]
    public TypeWriter typeWriter;
    public bool isPause = false;
    public bool isViewPause = false;
    public bool canClickToNext = true;
    bool isMiniOn = false;

    bool hasUnderletter = false;

    public static float FIXED_HEIGHT = Screen.height; //* 0.45f;
    float startDelaySecond = 0.0f;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //        StartDialogueManager();
        //set standingDftPos

    }

    public void StartDialogueManager()
    {

        inventory = uiCanvas.GetComponent<Inventory>();
        contentTrans = contentTxt.GetComponent<RectTransform>();



    }

    public void StartDialogue(List<Dialogue> dialogue)
    {
        dialogueList = new List<Dialogue>();
        dialogueList = dialogue;
        standingImg.enabled = true;
        crtIndex = 0;
        dialogWhole.gameObject.SetActive(true);
        Debug.Log("다이얼로그 변경됨: " + dialogueList[0].content);
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
        // 다이얼 로그가 꺼져 있다면 다이얼 로그를 키고 다이얼 로그가 켜져있다면 아무 변화도 없음
        dialogWhole.gameObject.SetActive(true);


        if (typeWriter.isTyping)
        {
            typeWriter.StopTyping();
            return;

        }
        else
        {

            if (dialogueList.Count <= crtIndex)
            {
                dialogWhole.gameObject.SetActive(false);
                standingImg.enabled = false;
                PlayerMovement.instance.ChangeCanMove(true);
                Debug.Log("실행중.");
                return;

            }


            // if (sheetData.storyBlock[crtBranch].block[crtPage].se != null)
            //  {
            // SoundManager.instance.PlaySound(sheetData.storyBlock[crtBranch].block[crtPage].se, sheetData.storyBlock[crtBranch].block[crtPage].seSubtitle);
            //  }
            //if(영어버전)string content =content_ENG

            string content = dialogueList[crtIndex].content;
            nameTxt.text = ChangeIfPlayerName(dialogueList[crtIndex].charName);
            standingImg.sprite = dialogueList[crtIndex].sprite;
            standingImg.enabled = standingImg.sprite != null;


            if (content.Contains("{"))
            {
                // Debug.Log("치환 전: " + content);

                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\{(.*?)\}");
                content = regex.Replace(content, match =>
                {
                    string key = match.Groups[1].Value.Trim(); // 변수명 추출 및 공백 제거
                                                               // Debug.Log($"치환 시도: {key}");

                    if (key.Equals("Player"))
                    {
                        return PlayerMovement.playerName;
                    }
                    // PlayerPrefs에서 값 가져오기
                    else
                    {
                        string value = PlayerPrefs.GetString(key, "null");
                        // Debug.Log($"PlayerPrefs에서 가져온 값: {value}");



                        return value;
                    }
                });

                // Debug.Log("치환 후: " + content);
            }


            typeWriter.StartTyping(content, nameTxt.text);//(int)currentBlock.block[index].name_ch


            //ChangeSprite();

            // if (index >= currentBlock.block.Count) isNoNext = true;
            //else isNoNext = false;
            crtIndex++;
        }
    }

    string ChangeIfPlayerName(string current)
    {

        if (current.Equals("유저 디버그"))
        {
            if (PlayerMovement.playerName.Equals("유저 디버그")) return current;
            else return PlayerMovement.playerName;
        }
        else return current;
    }


    void WhenLastPage()
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
                    //gift 여는 스크립트-인벤토리or선물 스크립트와 연동하기
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

    void DisableSprite()
    {
        standingImg.enabled = false;


    }

    Vector3[] standingDftPos = new Vector3[3];

    void ChangeSprite()
    {
        /*string[] standing = sheetData.storyBlock[crtBranch].block[crtPage].standing;
        //  bool isNoName = currentBlock.block[index].isNoName;
        Color gray = new Color32(163, 164, 168, 255);
        // Debug.Log("focusOn: " + focusOn);
        //StandingSpriteManager에서 스프라이트를 찾아옴. (캐 이름, 얼굴 종류 파라미터로 필요.)


        for (int i = 0; i < standingImg.Length; i++)
        {
            //string[] spr1 = standing[0].Split('_');
            //if (spr1.Length <= 1)
            //    spr1 = new string[] { "", "" };
            //Sprite spr = sprManager.FindSprite(spr1[0], spr1[1]);

            string[] spl = standing[i].Split('_');
            if (spl.Length <= 1)
                spl = new string[] { "", "" };

            Sprite spr = sprManager.FindSprite(spl[0], spl[1]);

            standingImg[i].sprite = spr;
            standingImg[i].color = (focusOn == i) ? Color.white : gray;
            standingImg[i].enabled = (spr != null && !isMiniOn);

            if (minigameState.Equals(1) && i.Equals(1))
            {
                currentMinigame.GetComponent<Minigame_Phone>().ChangeStandingImg(spr, focusOn == 1);
                standingImg[i].enabled = false;
            }

        }

        //2명만 있는 경우 중간으로 이동시키기
        if (standingImg[1].enabled && standingImg[0].enabled && !standingImg[2].enabled)
        {
            standingImg[1].transform.parent.localPosition = new Vector3(3.2f, standingImg[1].transform.parent.localPosition.y, standingImg[1].transform.parent.localPosition.z);
            standingImg[0].transform.parent.localPosition = new Vector3(-3.2f, standingImg[0].transform.parent.localPosition.y, standingImg[0].transform.parent.localPosition.z);

        }
        else if (standingImg[1].enabled && standingImg[2].enabled && !standingImg[0].enabled)
        {
            standingImg[2].transform.parent.localPosition = new Vector3(3.2f, standingImg[2].transform.parent.localPosition.y, standingImg[2].transform.parent.localPosition.z);
            standingImg[1].transform.parent.localPosition = new Vector3(-3.2f, standingImg[1].transform.parent.localPosition.y, standingImg[1].transform.parent.localPosition.z);

        }
        else
        {
            //compare with default position
            for (int i = 0; i < standingImg.Length; i++)
            {
                if (standingImg[i].enabled)
                {
                    if (standingImg[i].transform.parent.localPosition != standingDftPos[i])
                    {
                        standingImg[i].transform.parent.localPosition = standingDftPos[i];
                    }
                }
            }
        }

        */

    }


    IEnumerator Delay(float seconds)
    {
        canClickToNext = false;
        dialogWhole.gameObject.SetActive(false);
        yield return new WaitForSeconds(seconds);
        //Debug.Log("Play again");
        canClickToNext = true;
        dialogWhole.gameObject.SetActive(true);
        startDelaySecond = 0.0f;
        ChangeDialogue();

        yield return null;
    }



    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            isViewPause = !isViewPause;

            uiCanvas.enabled = !isViewPause;
        }

        if (isPause || isViewPause) return;

        if (dialogWhole.gameObject.activeInHierarchy)
        {
            //Spacebar or EnterKey, or Click Between bottom 0~700
            if (canClickToNext && ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || (Input.mousePosition.y <= FIXED_HEIGHT && Input.GetMouseButtonDown(0))) ||
                (Input.mouseScrollDelta.y != 0.0)))
            {

                ChangeDialogue();
            }
        }

#if UNITY_IOS
        if (Input.touchCount > 0)
        {
            if (!isNoNext && (Input.GetTouch(0).phase == TouchPhase.Ended && Input.GetTouch(0).position.y <= FIXED_HEIGHT))
            {
                tmptext.text = Input.GetTouch(0).position.y.ToString();
                ChangeDialogue();
            }
        }
#endif
#if UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            if (!isNoNext && (Input.GetTouch(0).phase == TouchPhase.Ended && Input.GetTouch(0).position.y <= FIXED_HEIGHT))
            {
                tmptext.text = Input.GetTouch(0).position.y.ToString();
                ChangeDialogue();
            }
        }
#endif

    }
}

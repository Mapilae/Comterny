using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TypeWriter))]
public class ChatBallon : NonPlayerCharacter
{
    [Header("말풍선 세팅")] public RectTransform ballonMid;
    public TMP_Text text;
    public int repeatTime = -1; //-1은 무한반복

    public bool fixRight;
    private AudioSource audioSource;
    private List<Cell> dialogue;

    private bool isLoaded;
    private bool isPlaying;
    private float maxPreferredWidth;
    private TypeWriter typeWriter;


    private void Start()
    {
        maxPreferredWidth = ballonMid.rect.width;
        //OnStartDefaultSetting();
        audioSource = transform.GetChild(0).GetComponent<AudioSource>();
        if (audioSource != null) audioSource.clip = SoundManager.instance.soundHolder.FindSE("bubble");
        StartConversation();
    }

    public void ChangeBallonText(string text, int repeatTime)
    {
        autoPlayID = text;
        this.repeatTime = repeatTime;

        if (gameObject.activeInHierarchy) StartCoroutine(IEChangeBallonText());
        else Debug.LogWarning("chat ballon is not active");
    }

    private IEnumerator IEChangeBallonText()
    {
        yield return new WaitUntil(() => isLoaded);

        if (isPlaying)
        {
            //
            StopCoroutine(nameof(PlayBallon));
            isPlaying = false;
            ballonMid.gameObject.SetActive(false);
        }

        StartConversation();
    }


    //override
    public void StartConversation()
    {
        isPlaying = false;
        ballonMid.gameObject.SetActive(false);
        text.text = "";
        typeWriter = GetComponent<TypeWriter>();
        //if (typeWriter.isTyping) typeWriter.StopTyping();
        if (!gameObject.activeInHierarchy)
            //
            Debug.Log("chat ballon is not active somehow....");
        //return;
        StartCoroutine(nameof(WaitStartConversation));
    }

    private IEnumerator WaitStartConversation()
    {
        isPlaying = true;

        while (dialogue == null)
        {
            dialogue = DialogueManager.instance.LoadCellsFromSheetData(autoPlayID);
            if (dialogue == null) yield return new WaitForSeconds(1f);
        }

        audioSource.Play();
        StartCoroutine(nameof(PlayBallon), repeatTime);
        isLoaded = true;
    }

    private IEnumerator PlayBallon(int repeating)
    {
        var repeatHowLong = 0;
        var currentIndex = 0;
        float beforeSizeX = 0;

        while (isPlaying)
        {
//            Debug.Log("currentIndex: " + currentIndex);
            if (fixRight)
                beforeSizeX = ballonMid.sizeDelta.x;

            var size = ballonMid.sizeDelta;
            size.x = text.preferredWidth < maxPreferredWidth ? text.preferredWidth : maxPreferredWidth;
            ballonMid.sizeDelta = size;
            if (fixRight)
                ballonMid.anchoredPosition = new Vector2(ballonMid.anchoredPosition.x - (size.x - beforeSizeX) / 2,
                    ballonMid.anchoredPosition.y);

            if (!typeWriter.isTyping)
            {
                yield return new WaitForSeconds(2f);
                ballonMid.gameObject.SetActive(false);

                yield return new WaitForSeconds(1f);
                ballonMid.gameObject.SetActive(true);

                audioSource.Play();

                typeWriter.StartTyping(dialogue[currentIndex].content, "");
                currentIndex++;

//                Debug.Log("currentIndex changed!: " + currentIndex);
            }


            if (currentIndex >= dialogue.Count)
            {
                if (!repeating.Equals(repeatHowLong))
                {
                    //-1인경우 무한반복, 양수인 경우 해당에 도달할때까지 반복 
                    currentIndex = 0;
                }
                else
                {
                    isPlaying = false;
                    ballonMid.gameObject.SetActive(false);
                    break;
                }
            }

            yield return new WaitForSeconds(0.02f);
        }

        yield return null;
    }
}
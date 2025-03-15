using System.Collections;
using TMPro;
using UnityEngine;

public class TypeWriter : MonoBehaviour
{

    [Header("Sentence")]
    public bool isOn = true;
    public int name_ch;
    public string sentence;//전체 타이핑해야하는 문장
    public string current;//현재 타이핑 중인 문장
                          // public List<Word> currentWord = new List<Word>();

    public TMP_Text content;

    private void Start()
    {
        sentence = content.text;
        StartTyping(sentence, 0, true);
    }
    public void StartTyping(int index)
    {
        StartTyping(sentence, index, true);
    }


    public void StartTyping(string sentence, int name_ch, bool isOn)
    {

        StopCoroutine("Typing");


        this.sentence = sentence;
        this.name_ch = name_ch;
        this.isOn = isOn;

        StartCoroutine("Typing", name_ch);
    }


    public void StopTyping()
    {
        if (sentence.Contains("<skip>")) return;
        StopCoroutine("Typing");

        current = this.sentence;


        content.text = current;

    }

    IEnumerator Typing(int index)
    {
        WaitForSeconds wait = new(0.07f);

        current = "";
        bool isSkip = false;

        content.text = current;
        yield return wait;

        if (index.Equals(0))
        {
            for (int i = 0; i < sentence.Length; i++)
            {
                current += sentence[i];
                content.text = current;

                yield return wait;
            }
        }
        if (index.Equals(1))
        {
            string[] words = sentence.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                current += words[i] + " ";
                content.text = current;

                yield return wait;
                yield return wait;
            }
        }

        yield return null;
    }

    /*
    public struct Word
    {
        public string word;
        public string cmd;
    }

    [Header("Thinking Animator")]
    public Animator thinkingAnim;
    public TMP_Text thinkingText;

    [Header("Sentence")]
    public bool isOn = true;
    public int name_ch;
    public string sentence;//전체 타이핑해야하는 문장
    public string current;//현재 타이핑 중인 문장
    public List<Word> currentWord = new List<Word>();

    [Header("Type Setting")]
    public GameObject dialogue;
    public TMP_Text content;
    public DialogueManager dialogueManager;

    public float autoDelay = 1.0f;
    public bool isTyping = false;


    [Header("Auto Click")]
    public RectTransform nextImg;
    public Image autoImg;
    RectTransform autoRect;
    public bool isAuto = false;//0: auto off, 1: text auto, 2: voice auto
    public float delay = 1.0f;


    public void ThinkingOn(string thinkingContent)
    {
        thinkingText.text = thinkingContent;
        thinkingAnim.gameObject.SetActive(false);
        thinkingAnim.gameObject.SetActive(true);
        //thinkingAnim.SetTrigger("thinkingOn");
    }

    public void ThinkingOff()
    {
        if (thinkingText.enabled) thinkingText.enabled = false;
    }



    void UpdateWordList()
    {
        for (int i = 0; i < sentence.Length; i++)
        {

        }
    }

    public void StartTyping(string sentence, int name_ch, bool isOn)
    {
        nextImg.gameObject.SetActive(false);
        if (autoRect == null) autoRect = autoImg.GetComponent<RectTransform>();
        autoRect.gameObject.SetActive(false);
        StopCoroutine("Typing");

        if (dialogueManager.currentBlock.ifEnd.Equals(if_end.CHOICE) && dialogueManager.currentBlock.block.Count - 1 == dialogueManager.index && !sentence.Contains("<skip>"))
            sentence += "<skip>";

        this.sentence = sentence;
        this.name_ch = name_ch;
        this.isOn = isOn;

        StartCoroutine("Typing");

    }


    public void StopTyping()
    {
        if (sentence.Contains("<skip>")) return;
        StopCoroutine("Typing");

        current = this.sentence;
    
    dialogueManager.RunAfterCMD();
        content.text = current;
        isTyping = false;
    }

    IEnumerator Typing()
    {
        WaitForSeconds wait = new(0.07f);
        isTyping = true;
        current = "";
        bool isSkip = false;

        for (int i = 0; i < sentence.Length; i++)
        {

            while (dialogueManager.isPause || dialogueManager.isViewPause || !dialogue.activeInHierarchy)
            {
                yield return wait;
            }


            if (sentence[i].Equals('<'))
            {
                if (sentence[i..(i + 6)].Equals("<skip>"))
                { //<skip>

                    isSkip = true;
                    break;//이 모든 for문에서 벗어남

                }
                else
                {
                    strInt stri = SkipBracket(sentence, current, i);
                    current = stri.str;
                    i = stri.iNum;

                    content.text = current;
                    //Debug.Log(current + "괄호 발견" + i);
                }

                yield return wait;
            }
            current += sentence[i];
            content.text = current;

            if (isOn && !sentence[i].Equals(" ") && !sentence[i].Equals("♡") && !sentence[i].Equals("?") && !sentence[i].Equals("!") && !sentence[i].Equals(".") && !sentence[i].Equals(","))
            {
                SoundManager.instance.PlayVoice(name_ch);
            }
            //

            yield return wait;
        }
        isTyping = false;
        nextImg.anchoredPosition = new Vector2(content.preferredWidth % 920 / 2 + 30, -content.preferredHeight + 98);
        autoRect.anchoredPosition = nextImg.anchoredPosition;
        if (!isAuto) nextImg.gameObject.SetActive(true);
        else autoRect.gameObject.SetActive(true);
        dialogueManager.RunAfterCMD();
        if (isSkip) dialogueManager.ChangeDialogue();
        if (isAuto) StartCoroutine("AutoDelay");
        yield return null;
    }

    struct strInt
    {
        public string str;
        public int iNum;

        public strInt(string str, int iNum)
        {
            this.str = str;
            this.iNum = iNum;
        }
    }

    strInt SkipBracket(string sentence, string current, int index)
    {
        //for (; !sentence[index].Equals('>'); index++)


        do
        {
            current = sentence[..index];
            index++;
        } while (!sentence[index].Equals('>'));

        current = sentence[..++index];

        return new strInt(current, index);
    }


    public void ChangeIsAuto()
    {
        this.isAuto = !isAuto;
        dialogueManager.canClickToNext = !this.isAuto;

    }

    public void ShowAutoDelayIcon(bool isShow)
    {
        autoImg.color = isShow ? Color.white : Color.clear;
    }


    IEnumerator AutoDelay()
    {
        autoImg.enabled = true;
        float delta = 0.05f;
        WaitForSeconds wait = new WaitForSeconds(delta);
        float time = 0.0f;
        while (time < delay)
        {
            time += delta;
            autoImg.fillAmount = time / delay;
            yield return wait;
        }
        if (dialogueManager != null) if (!dialogueManager.isNoNext) dialogueManager.ChangeDialogue();
        autoImg.enabled = false;
        yield return null;
    }

    private void Start()
    {
        autoRect = autoImg.GetComponent<RectTransform>();

    }*/
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypeWriter : MonoBehaviour
{
    [Header("Sentence")] public string name_ch;

    public string sentence; //��ü Ÿ�����ؾ��ϴ� ����
    public string current; //���� Ÿ���� ���� ����

    [Header("Type Setting")] public GameObject dialogue;
    public TMP_Text content;
    public DialogueManager dialogueManager;

    public float autoDelay = 1.0f;
    public bool isTyping;
    public List<Word> currentWord = new();

    private Color defaultColor;

    private void OnEnable()
    {
        defaultColor = content.color;
    }


    private void UpdateWordList()
    {
        for (var i = 0; i < sentence.Length; i++)
        {
            /*if (!sentence[i].Equals("<"))
            {
                Word w = new Word();
                w.word = sentence[i];
                w.cmd = "";
                currentWord.Add(w);
            }
            else {
                string tmp_cmd = sentence.Substring(i, sentence[i+1].Equals("/")?i+6:i + 5);
                if (tmp_cmd.Equals("<slow>")) {

                }
            }*/
        }
    }

    public void StartTyping(string sentence, string name_ch)
    {
        StopCoroutine("Typing");

        this.sentence = sentence;
        this.name_ch = name_ch;

        //if (name_ch.Equals("랜섬웨어"))
        //    content.color = new Color(1, 0, 0);
        //else content.color = defaultColor;


        StartCoroutine("Typing");
    }


    public void StopTyping()
    {
        if (sentence.Contains("<skip>")) return;
        StopCoroutine("Typing");

        current = sentence;

        content.text = current;
        isTyping = false;
    }

    private IEnumerator Typing()
    {
        var waitTime = 0.07f;
        if (name_ch.Equals("프로세서")) waitTime = 0.04f;
        else if (name_ch.Equals("램")) waitTime = 0.2f;

        WaitForSeconds wait = new(waitTime);
        isTyping = true;
        current = "";


        if (name_ch.Equals("램") || name_ch.Equals("프로세서"))
        {
            //단어 단위로 말하게 함
            var words = sentence.Split(' ');

            for (var i = 0; i < words.Length; i++)
            {
                if (dialogueManager != null)
                    while (dialogueManager.isPause || dialogueManager.isViewPause || !dialogue.activeInHierarchy)
                        yield return wait;
                var processedWord = "";
                var visibleCharCount = 0;
                var inTag = false;

                foreach (var c in words[i])
                {
                    if (c == '<') inTag = true;
                    if (!inTag) visibleCharCount++;
                    if (c == '>') inTag = false;

                    processedWord += c;
                }

                current += processedWord;
                content.text = current;

                if (visibleCharCount > 4)
                    yield return wait;
                else
                    yield return new WaitForSeconds(waitTime);

                if (i < words.Length - 1) current += " ";
            }
        }
        else
        {
            //문자 단위로 말하게 함
            for (var i = 0; i < sentence.Length; i++)
            {
                if (dialogueManager != null)
                    while (dialogueManager.isPause || dialogueManager.isViewPause || !dialogue.activeInHierarchy)
                        yield return wait;

                if (sentence[i].Equals('<'))
                {
                    var stri = SkipBracket(sentence, current, i);
                    current = stri.str;
                    i = stri.iNum;

                    content.text = current;

                    yield return wait;
                }

                if (i < sentence.Length) current += sentence[i];
                content.text = current;
                yield return wait;
            }
        }

        isTyping = false;
        yield return null;
    }

    private strInt SkipBracket(string sentence, string current, int index)
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

    public struct Word
    {
        public string word;
        public string cmd;
    }


    private struct strInt
    {
        public readonly string str;
        public readonly int iNum;

        public strInt(string str, int iNum)
        {
            this.str = str;
            this.iNum = iNum;
        }
    }
}
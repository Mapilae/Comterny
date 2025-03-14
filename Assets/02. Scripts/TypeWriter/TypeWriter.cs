using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TypeWriter : MonoBehaviour
{
    public struct Word
    {
        public string word;
        public string cmd;
    }

    [Header("Sentence")]
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



    void UpdateWordList()
    {
        for (int i = 0; i < sentence.Length; i++)
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

    public void StartTyping(string sentence, int name_ch)
    {


        StopCoroutine("Typing");

        this.sentence = sentence;
        this.name_ch = name_ch;

        StartCoroutine("Typing");

    }


    public void StopTyping()
    {
        if (sentence.Contains("<skip>")) return;
        StopCoroutine("Typing");

        current = this.sentence;

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


            yield return wait;
        }
        isTyping = false;



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


}
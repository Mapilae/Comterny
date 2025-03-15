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
    public string name_ch;
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

    public void StartTyping(string sentence, string name_ch)
    {


        StopCoroutine("Typing");

        this.sentence = sentence;
        this.name_ch = name_ch;

        if (name_ch.Equals("랜섬웨어"))
        {
            content.color = new Color(1, 0, 0);
        }
        else content.color = Color.white;

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

        float waitTime = 0.07f; //컴파일러, 유저, 랜섬웨어
        if (name_ch.Equals("프로세서")) waitTime = 0.02f;
        else if (name_ch.Equals("램")) waitTime = 0.2f;

        WaitForSeconds wait = new WaitForSeconds(waitTime);
        isTyping = true;
        current = "";
        bool isSkip = false;




        if (name_ch.Equals("램") || name_ch.Equals("프로세서"))
        {
            string[] words = sentence.Split(' '); // 문장을 공백 기준으로 단어 배열로 분리

            for (int i = 0; i < words.Length; i++)
            {
                while (dialogueManager.isPause || dialogueManager.isViewPause || !dialogue.activeInHierarchy)
                {
                    yield return wait;
                }

                string processedWord = "";
                int visibleCharCount = 0; // 실제 보이는 글자 수 (태그 제외)
                bool inTag = false;

                // 태그를 제외한 실제 글자 수 세기
                foreach (char c in words[i])
                {
                    if (c == '<') inTag = true; // 태그 시작
                    if (!inTag) visibleCharCount++; // 태그가 아닐 때만 카운트 증가
                    if (c == '>') inTag = false; // 태그 종료

                    processedWord += c; // 모든 문자(태그 포함)를 저장
                }

                current += processedWord;
                content.text = current;

                // 실제 보이는 글자 수가 4초과이면 즉시 출력
                if (visibleCharCount > 4)
                {
                    yield return wait;
                }
                else
                {
                    yield return new WaitForSeconds(waitTime);
                }

                if (i < words.Length - 1)
                {
                    current += " "; // 단어 사이에 공백 추가
                }
            }
        }
        else
        {
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
                        break; // 이 모든 for문에서 벗어남
                    }
                    else
                    {
                        strInt stri = SkipBracket(sentence, current, i);
                        current = stri.str;
                        i = stri.iNum;

                        content.text = current;
                    }

                    yield return wait;
                }

                current += sentence[i];
                content.text = current;
                yield return wait;
            }
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
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


    private void UpdateWordList()
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

        if (name_ch.Equals("��������"))
            content.color = new Color(1, 0, 0);
        else content.color = Color.white;

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
        float waitTime = 0.07f; //�����Ϸ�, ����, ��������
        if (name_ch.Equals("���μ���")) waitTime = 0.02f;
        else if (name_ch.Equals("��")) waitTime = 0.2f;

        WaitForSeconds wait = new(waitTime);
        isTyping = true;
        current = "";


        if (name_ch.Equals("��") || name_ch.Equals("���μ���"))
        {
            string[] words = sentence.Split(' '); // ������ ���� �������� �ܾ� �迭�� �и�

            for (int i = 0; i < words.Length; i++)
            {
                while (dialogueManager.isPause || dialogueManager.isViewPause || !dialogue.activeInHierarchy)
                    yield return wait;

                string processedWord = "";
                int visibleCharCount = 0; // ���� ���̴� ���� �� (�±� ����)
                bool inTag = false;

                // �±׸� ������ ���� ���� �� ����
                foreach (char c in words[i])
                {
                    if (c == '<') inTag = true; // �±� ����
                    if (!inTag) visibleCharCount++; // �±װ� �ƴ� ���� ī��Ʈ ����
                    if (c == '>') inTag = false; // �±� ����

                    processedWord += c; // ��� ����(�±� ����)�� ����
                }

                current += processedWord;
                content.text = current;

                // ���� ���̴� ���� ���� 4�ʰ��̸� ��� ���
                if (visibleCharCount > 4)
                    yield return wait;
                else
                    yield return new WaitForSeconds(waitTime);

                if (i < words.Length - 1) current += " "; // �ܾ� ���̿� ���� �߰�
            }
        }
        else
        {
            for (int i = 0; i < sentence.Length; i++)
            {
                while (dialogueManager.isPause || dialogueManager.isViewPause || !dialogue.activeInHierarchy)
                    yield return wait;

                if (sentence[i].Equals('<'))
                {
                    strInt stri = SkipBracket(sentence, current, i);
                    current = stri.str;
                    i = stri.iNum;

                    content.text = current;

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
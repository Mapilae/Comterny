using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class Quest
{
    private static readonly int Success = Animator.StringToHash("QuestSuccess");
    public Animator questObj;
    public string questName;
    public string content;

    private TMP_Text questContentText;
    private TMP_Text questNameText;

    public Quest(Animator questObj)
    {
        this.questObj = questObj;
        questName = "";
        content = "";
        questNameText = questObj.transform.GetChild(0).GetComponent<TMP_Text>();
        questContentText = questObj.transform.GetChild(1).GetComponent<TMP_Text>();
    }

    public void SetQuest(string questName, string content)
    {
        this.questName = questName;
        this.content = content;
        questContentText.text = content;
        questNameText.text = questName;
    }

    public void SuccessQuest()
    {
        questObj.SetTrigger(Success);
    }

    public void SetActive(bool active)
    {
        questObj.gameObject.SetActive(active);
    }
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    public Transform questWhole;
    public List<Quest> quests = new();
    private int questCount = -1;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (var i = 0; i < questWhole.childCount; i++)
            quests.Add(new Quest(questWhole.GetChild(i).gameObject.GetComponent<Animator>()));
    }

    public void AddQuest(string info)
    {
        var splitInfo = info.Split(':');
        for (int i = 0; i < quests.Count; i++)
        {
            if (!quests[i].questObj.gameObject.activeSelf)
            {
                quests[i].SetQuest(splitInfo[0], splitInfo[1]);
                quests[i].SetActive(true);
                questCount = Mathf.Max(questCount, i); // count 최신화
                Debug.Log($"퀘스트 추가됨: {i}");
                return;
            }
        }

        Debug.LogError("퀘스트 슬롯이 부족합니다. AddQuest 실패");
    }


    public void RemoveQuest(string questName)
    {
        Debug.Log("remove quest? [" + questName + "]");
        var index = -1;

        for (var i = 0; i <= questCount; i++)
        {
            Debug.Log($"checking quest {i}: [{quests[i].questName}]");
            if (questName.Trim().Equals(quests[i].questName.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                index = i;
                Debug.Log("remove quest found + " + i);
                break;
            }
        }

        if (index == -1)
        {
            Debug.LogWarning("Quest not found: " + questName);
            return;
        }

        StartCoroutine(IERemoveQuest(index));
    }


    private IEnumerator IERemoveQuest(int index)
    {
        quests[index].SuccessQuest();
        yield return new WaitForSeconds(2f);
        quests[index].SetActive(false);
        questCount--;
    }
}
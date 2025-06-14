using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    public Transform questWhole;
    public List<QuestCell> quests = new();
    private int questCount;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (var i = 0; i < questWhole.childCount; i++)
            quests.Add(questWhole.GetChild(i).GetComponent<QuestCell>());
    }

    public void AddQuest(string info)
    {
        Debug.Log("quest gave! " + info);
        var splitInfo = info.Split(':');
        questCount++;
        for (var i = 0; i < quests.Count; i++)
            if (!quests[i].gameObject.activeInHierarchy)
            {
                quests[i].gameObject.SetActive(true);
                quests[i].SetQuest(splitInfo[0], splitInfo[1]);

                //questCount = Mathf.Max(questCount, i); // count 최신화
                Debug.Log($"퀘스트 추가됨: {i}");
                return;
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

        quests[index].SuccessQuest();

        questCount--;
    }
}
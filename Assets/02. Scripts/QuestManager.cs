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


    /// <summary>진행 중인 퀘스트 목록 (완료 처리 중인 것 제외)</summary>
    public List<QuestState> CaptureQuests()
    {
        var list = new List<QuestState>();
        foreach (var q in quests)
            if (q != null && q.gameObject.activeSelf && !q.IsCompleting && !string.IsNullOrEmpty(q.questName))
                list.Add(new QuestState { name = q.questName, content = q.content });
        return list;
    }

    /// <summary>이어하기: 저장된 퀘스트를 앞쪽 슬롯부터 다시 채운다</summary>
    public void RestoreQuests(List<QuestState> saved)
    {
        foreach (var q in quests)
            if (q != null)
                q.gameObject.SetActive(false);
        questCount = 0;
        if (saved == null) return;

        foreach (var s in saved)
        {
            if (questCount >= quests.Count) break;
            var cell = quests[questCount];
            cell.gameObject.SetActive(true);
            cell.SetQuest(s.name, s.content);
            questCount++;
        }
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
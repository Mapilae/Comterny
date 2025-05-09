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
        questCount++;
        var splitInfo = info.Split(':');
        quests[questCount].SetQuest(splitInfo[0], splitInfo[1]);
        quests[questCount].SetActive(true);
    }

    public void RemoveQuest(string questName)
    {
        Debug.Log("remove quest? " + questName);
        var index = -1;
        for (var i = 0; i < questCount; i++)
            if (questName.Equals(quests[i].questName))
            {
                index = i;
                Debug.Log("remove quest found +" + i);
                break;
            }

        if (index == -1) return; //no quest found.

        //if quest found:
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
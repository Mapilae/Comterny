using System;
using TMPro;
using UnityEngine;

[Serializable]
public class QuestCell : MonoBehaviour
{
    private static readonly int Success = Animator.StringToHash("QuestSuccess");
    public Animator questObj;
    public string questName;
    public string content;

    private TMP_Text questContentText;
    private TMP_Text questNameText;

    private void Awake()
    {
        questObj = GetComponent<Animator>();
        questName = "";
        content = "";
        questNameText = transform.GetChild(0).GetComponent<TMP_Text>();
        questContentText = transform.GetChild(1).GetComponent<TMP_Text>();
    }

    public void SetQuest(string questName, string content)
    {
        this.questName = questName;
        this.content = content;

        if (questContentText == null)
        {
            questNameText = transform.GetChild(0).GetComponent<TMP_Text>();
            questContentText = transform.GetChild(1).GetComponent<TMP_Text>();
        }

        questContentText.text = content;
        questNameText.text = questName;
    }

    public void SuccessQuest()
    {
        questObj.SetTrigger(Success);
    }


    public void DisableThis()
    {
        gameObject.SetActive(false);
        //disable this in animation event
    }
}
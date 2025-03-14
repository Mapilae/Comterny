using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string charName;
    public string content;
    public Sprite sprite;
}
public class DialogueContent : MonoBehaviour
{

    public List<Dialogue> dialogue;

    public void StartDialogue()
    {
        DialogueManager.instance.StartDialogue(dialogue);
    }

    private void Start()
    {
        StartDialogue();
    }
}

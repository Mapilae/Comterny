using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dialogue
{
    public string charName;
    public string content;
    public Sprite sprite;

    public Dialogue(string charName, string content, Sprite sprite)
    {
        this.charName = charName;
        this.content = content;
        this.sprite = sprite;
    }

    public Dialogue()
    {
        charName = "";
        content = "";
        sprite = null;
    }
}

[Serializable]
public class DialogueContain
{
    public string id;
    public List<Dialogue> dialogue;
}

public class DialogueContent : MonoBehaviour
{
    public StandingSpriteManager sprManager;
    public List<DialogueContain> containers;
    private bool isLoaded;

    private void Start()
    {
    }


    public void LoadDialogueFromSheetData()
    {
        StartCoroutine(IELoadDialogueFromSheetData());
    }

    private IEnumerator IELoadDialogueFromSheetData()
    {
        Debug.Log("Sheet Data start.");
        for (int i = 0; i < containers.Count; i++)
        {
            PlayerMovement.instance.ChangeCanMove(false);
            containers[i].dialogue = new List<Dialogue>();
            yield return new WaitUntil(() => SheetLoader.isLoading == false);
            Debug.Log("Sheet Data loading. only for first time.");
            Block block = SheetData.instance.GetBlock(containers[i].id);
            foreach (Cell cell in block.cells)
            {
                Dialogue d = new();
                d.charName = cell.name;
                d.content = cell.content;
                d.sprite = sprManager.FindSprite(cell.name, cell.standingNum);
                containers[i].dialogue.Add(d);
            }

            yield return new WaitForSeconds(0.1f);
            //StartDialogueInManager();
            isLoaded = true;
        }
    }

    public void StartDialogueInManager(string idName)
    {
        if (!isLoaded || containers.Count == 0) LoadDialogueFromSheetData();
        StartCoroutine(StartDialogue(idName));
    }

    private IEnumerator StartDialogue(string idName)
    {
        Debug.Log("Start Dialogue waiting.");
        WaitUntil waitUntil = new(() => isLoaded);
        yield return waitUntil;
        Debug.Log("Start Dialogue waiting end.");

        int index = containers.FindIndex(x => x.id == idName);
        DialogueManager.instance.StartDialogue(containers[index].dialogue);
    }
}
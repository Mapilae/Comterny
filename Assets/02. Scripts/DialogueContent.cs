using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
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
        this.charName = "";
        this.content = "";
        this.sprite = null;
    }
}

[System.Serializable]
public class DialogueContain
{
    public string id;
    public List<Dialogue> dialogue;

}

public class DialogueContent : MonoBehaviour
{
    public StandingSpriteManager sprManager;
    public List<DialogueContain> containers;
    bool isLoaded = false;


    public void LoadDialogueFromSheetData()
    {
        StartCoroutine(IELoadDialogueFromSheetData());
    }

    IEnumerator IELoadDialogueFromSheetData()
    {
        for (int i = 0; i < containers.Count; i++)
        {
            PlayerMovement.instance.ChangeCanMove(false);
            containers[i].dialogue = new List<Dialogue>();
            yield return new WaitUntil(() => SheetLoader.isLoading == false);
            Block block = SheetData.instance.GetBlock(containers[i].id);
            foreach (Cell cell in block.cells)
            {
                Dialogue d = new Dialogue();
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

    void Start()
    {

    }
    public void StartDialogueInManager(string idName)
    {
        if (!isLoaded)
        {
            LoadDialogueFromSheetData();
        }
        StartCoroutine(StartDialogue(idName));
    }

    IEnumerator StartDialogue(string idName)
    {
        WaitUntil waitUntil = new WaitUntil(() => isLoaded);
        yield return waitUntil;


        int index = containers.FindIndex(x => x.id == idName);
        DialogueManager.instance.StartDialogue(containers[index].dialogue);

    }


}

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
}
public class DialogueContent : MonoBehaviour
{
    public StandingSpriteManager sprManager;
    public string id;
    public List<Dialogue> dialogue;

    public void LoadDialogueFromSheetData()
    {
        StartCoroutine(IELoadDialogueFromSheetData());
    }

    IEnumerator IELoadDialogueFromSheetData()
    {
        PlayerMovement.canMove = false;
        dialogue = new List<Dialogue>();
        yield return new WaitUntil(() => SheetLoader.isLoading == false);
        Block block = SheetData.instance.GetBlock(id);
        foreach (Cell cell in block.cells)
        {
            Dialogue d = new Dialogue();
            d.charName = cell.name;
            d.content = cell.content;
            d.sprite = sprManager.FindSprite(cell.name, cell.standingNum);
            dialogue.Add(d);
        }
        yield return new WaitForSeconds(0.1f);
        StartDialogueInManager();
    }

    public void StartDialogue()
    {
        LoadDialogueFromSheetData();
    }
    void StartDialogueInManager()
    {
        DialogueManager.instance.StartDialogue(dialogue);
    }

    private void Start()
    {
        //LoadDialogueFromSheetData();
        //StartDialogue();
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerCharacter : MovingFootPrint
{
    public string characterName;
    public List<DialogueContent> dialogueContent;

    public GameObject convObj;



    private void Start()
    {
        if (convObj == null)
            convObj = this.transform.GetChild(0).gameObject;
        convObj.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            convObj.SetActive(true);

        }

    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            convObj.SetActive(false);

        }
    }

    public void StartConversation(int index)
    {
        if (!convObj.activeInHierarchy) return;
        Debug.Log("말 걸음: " + dialogueContent[index].id);
        dialogueContent[index].StartDialogue();
        convObj.SetActive(false);
    }


}

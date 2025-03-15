using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerCharacter : MovingFootPrint
{
    public string characterName;
    public DialogueContent dialogueContent;

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

    private void Update()
    {
        //이 NPC의 Position 반경 1 이내에 있는 경우
        //Space 키를 누르면 대화 시작
        if (!convObj.activeInHierarchy) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("말 걸음");
            dialogueContent.StartDialogue();
            convObj.SetActive(false);
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[System.Serializable]
public enum InteractableType
{
    NPC,
    SELFDOOR,
    AUTODOOR,
    PUZZLE
}

[System.Serializable]
public class InteractableSetting
{


    public InteractableType interactableType;

    public List<int> needItemCode = new();

    public bool reverseNeedItem = false;
    
    [Header("After this worked, add variable?")]
    public List<string> varNameOrCodeToAdd;
    
    [Header("for NPC")]
    public string dialogueID;

    [Header("for PUZZLE")]
    public GameObject objToEnable; //미니게임 퍼즐 오브젝트.

}

public class InteractableObject : MonoBehaviour
{
    public List<InteractableSetting> interactList = new List<InteractableSetting>();
    [Header("interactList에서 true 조건을 달성한 인덱스")]
    int currentInteractIndex = 0;

    [Header("true when player is in trigger area")]
    public bool isClose = false;



    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isClose = true;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isClose = false;
        }
    }

    private void Update()
    {
        if (isClose && Input.GetKeyDown(KeyCode.Space))
        {
            isClose = false;
            //Debug.Log("Interacted with: " + gameObject.name);

            for (int num = 0; num < interactList.Count; num++)
            {

                bool isAllItemsInInventory = true;
                if (interactList[num].needItemCode.Count > 0)
                {
                    if (!interactList[num].reverseNeedItem) //존재하는지 체크
                    {
                        for (int i = 0; i < interactList[num].needItemCode.Count; i++)
                        {
                            if (interactList[num].needItemCode[i] >= 100)//Variable(not item)
                            {
                                if(!Inventory.instance.IsVariableExist(interactList[num].needItemCode[i])){
                                    isAllItemsInInventory = false;
                                    break;
                                }
                            }
                            else if (!Inventory.instance.CheckIfItemIsInInventory(interactList[num].needItemCode[i]))
                            {
                                isAllItemsInInventory = false;
                                break;
                            }


                        }
                    }
                    else
                    {//reverse
                        
                        for (int i = 0; i < interactList[num].needItemCode.Count; i++)
                        {
                            if (interactList[num].needItemCode[i] >= 100)//Variable(not item)
                            {
                                if(Inventory.instance.IsVariableExist(interactList[num].needItemCode[i])){
                                    isAllItemsInInventory = false;
                                    break;
                                }
                            }
                            else if (Inventory.instance.CheckIfItemIsInInventory(interactList[num].needItemCode[i]))
                            {
                                isAllItemsInInventory = false;
                                break;
                            }
                        }
                    }

                }
                if (isAllItemsInInventory)
                {
                    Debug.Log("All items are in inventory");
                    currentInteractIndex = num;
                    ItemTypeAct();

                    return;
                }
            }
        }
    }


    void ItemTypeAct()
    {

        isClose = true;
        for(int i=0;i<interactList[currentInteractIndex].varNameOrCodeToAdd.Count;i++){
            Inventory.instance.AddVariable(interactList[currentInteractIndex].varNameOrCodeToAdd[i]);
        }
        switch (interactList[currentInteractIndex].interactableType)
        {
            case InteractableType.NPC:
                Debug.Log("NPC");
                this.GetComponent<NonPlayerCharacter>().StartConversation(interactList[currentInteractIndex].dialogueID);
                break;

            case InteractableType.SELFDOOR://When Player holding the item
                Debug.Log("SELF DOOR");
                break;

            case InteractableType.AUTODOOR://When Player have the item(no need to hold)
                Debug.Log("AUTO DOOR");
                this.gameObject.gameObject.SetActive(false);
                break;
            case InteractableType.PUZZLE:
                Debug.Log("PUZZLE MINIGAME START");
                PlayerMovement.instance.ChangeCanMove(false);
                interactList[currentInteractIndex].objToEnable.SetActive(true);

                break;

            default:
                break;
        }
    }
}

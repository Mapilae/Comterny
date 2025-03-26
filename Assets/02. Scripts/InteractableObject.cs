using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum InteractableType
{
    NPC,
    SELFDOOR,
    AUTODOOR,
    PUZZLE
}

[Serializable]
public class InteractableSetting
{
    public InteractableType interactableType;

    public List<int> needItemCode = new();

    public bool reverseNeedItem;

    [Header("After this worked, add variable?")]
    public List<string> varNameOrCodeToAdd;

    [Header("for NPC")] public string dialogueID;

    [Header("for PUZZLE")] public GameObject objToEnable; //미니게임 퍼즐 오브젝트.

    public GameObject objToEnable2; //미니게임 퍼즐 오브젝트.

    [Header("for SelfDoor and AutoDoor")] public GameObject objToDisable;
}

public class InteractableObject : MonoBehaviour
{
    public List<InteractableSetting> interactList = new();

    [Header("true when player is in trigger area")]
    public bool isClose;

    [Header("interactList에서 true 조건을 달성한 인덱스")]
    private int currentInteractIndex;

    private void Update()
    {
        if (isClose && PlayerMovement.isSpace)
        {
            isClose = false;
            PlayerMovement.IsSpaceFalse();
            //Debug.Log("Interacted with: " + gameObject.name);

            for (var num = 0; num < interactList.Count; num++)
            {
                var isAllItemsInInventory = true;
                if (interactList[num].needItemCode.Count > 0)
                {
                    if (!interactList[num].reverseNeedItem) //존재하는지 체크
                    {
                        for (var i = 0; i < interactList[num].needItemCode.Count; i++)
                            if (interactList[num].needItemCode[i] >= 100) //Variable(not item)
                            {
                                if (!Inventory.instance.IsVariableExist(interactList[num].needItemCode[i]))
                                {
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
                    else
                    {
                        //reverse

                        for (var i = 0; i < interactList[num].needItemCode.Count; i++)
                            if (interactList[num].needItemCode[i] >= 100) //Variable(not item)
                            {
                                if (Inventory.instance.IsVariableExist(interactList[num].needItemCode[i]))
                                {
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) isClose = false;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) isClose = true;
    }


    private void ItemTypeAct()
    {
        isClose = true;
        for (var i = 0; i < interactList[currentInteractIndex].varNameOrCodeToAdd.Count; i++)
            Inventory.instance.AddVariable(interactList[currentInteractIndex].varNameOrCodeToAdd[i]);
        switch (interactList[currentInteractIndex].interactableType)
        {
            case InteractableType.NPC:
                Debug.Log("NPC");
                GetComponent<NonPlayerCharacter>().StartConversation(interactList[currentInteractIndex].dialogueID);
                break;

            case InteractableType.SELFDOOR: //When Player holding the item
                Debug.Log("SELF DOOR");
                interactList[currentInteractIndex].objToDisable.gameObject.gameObject.SetActive(false);
                break;

            case InteractableType.AUTODOOR: //When Player have the item(no need to hold)
                Debug.Log("AUTO DOOR");
                interactList[currentInteractIndex].objToDisable.gameObject.gameObject.SetActive(false);
                break;
            case InteractableType.PUZZLE:
                Debug.Log("PUZZLE MINIGAME START");
                PlayerMovement.instance.ChangeCanMove(false);
                if (interactList[currentInteractIndex].objToEnable2 != null)
                    interactList[currentInteractIndex].objToEnable2.SetActive(true);
                if (interactList[currentInteractIndex].objToEnable != null)
                    interactList[currentInteractIndex].objToEnable.SetActive(true);
                else
                    Debug.Log(name + "이 퍼즐임에도 퍼즐 오브젝트가 비어있습니다! Inspector에서 할당해주세요!");
                break;
        }
    }
}
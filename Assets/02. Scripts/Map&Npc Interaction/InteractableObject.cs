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

    [Header("After this worked, add variable?")]
    public List<int> varCodeToAdd;

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

    private PlayerData playerData;

    private void Awake()
    {
        playerData = GameObject.FindGameObjectWithTag("PlayerData").GetComponent<PlayerData>();
    }


    private void Update()
    {
        //플레이어가 근접 상태에서 + 스페이스바를 눌렀거나 or AUTODOOR인경우 실행
        if (isClose && PlayerMovement.isSpace)
        {
            isClose = false;
            PlayerMovement.instance.IsSpaceFalse(0.5f);
            //Debug.Log("Interacted with: " + gameObject.name);
            currentInteractIndex = -1;
            for (var num = 0; num < interactList.Count; num++)
            {
                var isAllItemsInInventory = true;
                if (interactList[num].needItemCode.Count > 0)
                    for (var i = 0; i < interactList[num].needItemCode.Count; i++)
                        if (interactList[num].needItemCode[i] >= 100) //Variable(not item)
                        {
                            if (!playerData.IsVariableExist(interactList[num].needItemCode[i]))
                            {
                                isAllItemsInInventory = false;
                                break;
                            }
                        }
                        else if (!playerData.IsItemExist(interactList[num].needItemCode[i]))
                        {
                            isAllItemsInInventory = false;
                            break;
                        }

                if (isAllItemsInInventory)
                {
//                    Debug.Log("All items are in inventory. ItemTypeActStart");
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
        //AUTO DOOR인경우 자동으로 문 열리도록하기
        //if (interactList[currentInteractIndex].interactableType.Equals(InteractableType.AUTODOOR))
        //   ItemTypeAct();
    }


    private void ItemTypeAct()
    {
        isClose = true;
        if (interactList.Count == 0) return;
        switch (interactList[currentInteractIndex].interactableType)
        {
            case InteractableType.NPC:
                //Debug.Log("NPC. Start: " + interactList[currentInteractIndex].dialogueID);
                if (DialogueManager.isPlayingDialogue) return;
                AddVariable();
                GetComponent<NonPlayerCharacter>().StartConversation(interactList[currentInteractIndex].dialogueID);

                if (interactList[currentInteractIndex].objToEnable2 != null)
                    interactList[currentInteractIndex].objToEnable2.SetActive(true);
                if (interactList[currentInteractIndex].objToEnable != null)
                    interactList[currentInteractIndex].objToEnable.SetActive(true);
                if (interactList[currentInteractIndex].objToDisable != null)
                    interactList[currentInteractIndex].objToDisable.SetActive(false);
                break;

            case InteractableType.SELFDOOR: //When Player holding the item
                // Debug.Log("SELF DOOR");

                interactList[currentInteractIndex].objToDisable.SetActive(false);
                break;

            case InteractableType.AUTODOOR: //When Player have the item(no need to hold)
                // Debug.Log("AUTO DOOR");
                //GetComponent<NonPlayerCharacter>().StartConversation(interactList[currentInteractIndex].dialogueID);
                if (interactList[currentInteractIndex].objToDisable != null)
                    interactList[currentInteractIndex].objToDisable.SetActive(false);
                break;
            case InteractableType.PUZZLE:
                //Debug.Log("PUZZLE MINIGAME START" + name);

                if (interactList[currentInteractIndex].objToEnable2 != null)
                    interactList[currentInteractIndex].objToEnable2.SetActive(true);
                if (interactList[currentInteractIndex].objToEnable != null)
                {
                    interactList[currentInteractIndex].objToEnable.SetActive(true);
                }
                else
                {
                    Debug.Log(name + "이 퍼즐임에도 퍼즐 오브젝트가 비어있습니다! Inspector에서 할당해주세요!");

                    AddVariable();
                    return;
                }

                PlayerMovement.instance.ChangeCanMove(false);
                break;
        }

        AddVariable();
    }

    public void AddVariable()
    {
        for (var i = 0; i < interactList[currentInteractIndex].varCodeToAdd.Count; i++)
            if (interactList[currentInteractIndex].varCodeToAdd[i] >= 100)
                playerData.AddVariable(interactList[currentInteractIndex].varCodeToAdd[i]);
            else
                playerData.AddItem(interactList[currentInteractIndex].varCodeToAdd[i]);
    }
}
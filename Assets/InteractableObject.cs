using System.Collections;
using System.Collections.Generic;
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


    [Header("for NPC")]
    public string dialogueID;

    [Header("for PUZZLE")]
    public GameObject objToEnable; //여기에다가 활성화할 미니게임을 끌어다 넣으면 됨.

}

public class InteractableObject : MonoBehaviour
{
    public List<InteractableSetting> interactList = new List<InteractableSetting>();
    [Header("현재 조건에 부합하는 인덱스")]
    int currentInteractIndex = 0;

    [Header("현재 감지 영역에 플레이어가 들어왔는지")]
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
                    if (!interactList[num].reverseNeedItem) //아이템이 있는 경우로 계산할때
                    {
                        for (int i = 0; i < interactList[num].needItemCode.Count; i++)
                        {


                            if (!Inventory.instance.CheckIfItemIsInInventory(interactList[num].needItemCode[i]))
                            {
                                isAllItemsInInventory = false;
                                break;
                            }


                        }
                    }
                    else
                    {
                        for (int i = 0; i < interactList[num].needItemCode.Count; i++)
                        {
                            if (Inventory.instance.CheckIfItemIsInInventory(interactList[num].needItemCode[i]))
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
        switch (interactList[currentInteractIndex].interactableType)
        {
            case InteractableType.NPC:
                Debug.Log("NPC");
                this.GetComponent<NonPlayerCharacter>().StartConversation(interactList[currentInteractIndex].dialogueID);
                break;

            case InteractableType.SELFDOOR://특정 아이템을 들고 있는 상태에서 스페이스 바를 눌러야 자동으로 열림
                Debug.Log("SELF DOOR");
                break;

            case InteractableType.AUTODOOR://특정 아이템을 인벤토리 칸에 가지고 있는 경우, 스페이스바를 누르면 자동으로 열림
                Debug.Log("AUTO DOOR");
                this.gameObject.gameObject.SetActive(false);
                break;
            case InteractableType.PUZZLE:
                Debug.Log("퍼즐 시작");
                PlayerMovement.instance.ChangeCanMove(false);
                interactList[currentInteractIndex].objToEnable.SetActive(true);

                break;

            default:
                break;
        }
    }
}

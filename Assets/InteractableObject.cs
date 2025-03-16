using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public enum InteractableType
    {
        NPC,
        ITEM,
        DOOR,
        CHEST
    }

    public InteractableType interactableType;
    public List<int> needItemCode = new();

    public bool reverseNeedItem = false;
    public int dialogueContentIndex = -1;
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
            Debug.Log("Interacted with: " + gameObject.name);
            bool isAllItemsInInventory = true;
            if (reverseNeedItem)
            {
                for (int i = 0; i < needItemCode.Count; i++)
                {


                    if (!Inventory.instance.CheckIfItemIsInInventory(needItemCode[i]))
                    {
                        isAllItemsInInventory = false;
                        break;
                    }


                }
            }
            else
            {
                for (int i = 0; i < needItemCode.Count; i++)
                {
                    if (Inventory.instance.CheckIfItemIsInInventory(needItemCode[i]))
                    {
                        isAllItemsInInventory = false;
                        break;
                    }
                }
            }

            if (isAllItemsInInventory)
            {
                Debug.Log("All items are in inventory");
                ItemTypeAct();
            }
            else
            {
                Debug.Log("Not all items are in inventory");
            }
        }
    }


    void ItemTypeAct()
    {
        switch (interactableType)
        {
            case InteractableType.NPC:
                Debug.Log("NPC");
                this.GetComponent<NonPlayerCharacter>().StartConversation(dialogueContentIndex);
                break;

            case InteractableType.ITEM:
                Debug.Log("ITEM");
                break;

            case InteractableType.DOOR:
                Debug.Log("DOOR");
                this.gameObject.gameObject.SetActive(false);
                break;

            case InteractableType.CHEST:
                Debug.Log("CHEST");
                break;

            default:
                break;
        }
    }
}

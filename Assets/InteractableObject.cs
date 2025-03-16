using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public List<int> needItemCode = new();


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
            for (int i = 0; i < needItemCode.Count; i++)
            {
                if (!Inventory.instance.CheckIfItemIsInInventory(needItemCode[i]))
                {
                    isAllItemsInInventory = false;
                    break;
                }


            }
            if (isAllItemsInInventory)
            {
                Debug.Log("All items are in inventory");
                this.gameObject.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Not all items are in inventory");
            }
        }
    }
}

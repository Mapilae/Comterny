using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : MonoBehaviour
{
    public int itemID;
    public string itemName;
    [TextArea] public string itemDesc;
    public Sprite itemSpr;

    public bool isClose = false;

    private void OnTriggerEnter2D(Collider2D collision)
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
            Debug.Log("Item picked up: " + itemName);
            Inventory.instance.AddItemInInventory(this);
            this.gameObject.SetActive(false);
        }
    }

}

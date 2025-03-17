using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

[System.Serializable]
public class Item : MonoBehaviour
{
    /// <summary>
    /// 아이템을 줍기 전, 줍고, 주운 후를 처리하는 코드.
    /// 
    /// </summary>

    SpriteRenderer sprRender;

    [Header("about items")]
    public int itemID;
    public string itemName;
    [TextArea] public string itemDesc;
    public Sprite itemSpr;

    [Header("When Player Come Close")]
    public bool isClose = false; //플레이어가 해당 아이템의 트리거 영역에 있는지 여부
    public GameObject objToEnable; //트리거 영역에 있는 경우, 활성화하는 오브젝트(ex.반짝이는 효과)

    [Header("When Player gets item")]
    [TextArea] public string msgToDialogue = "000을 얻었다.";


    [Header("After Find Item, to do?")]
    public Sprite sprToChange;
    public bool canUse = true; //중복 줍기 방지
    public List<string> varNameOrCodeToAdd;

    void Start()
    {
        sprRender = GetComponent<SpriteRenderer>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canUse) return;
        if (collision.CompareTag("Player"))
        {
            isClose = true;
            objToEnable.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isClose = false;
            objToEnable.SetActive(false);
        }
    }

    private void Update()
    {
        if (!canUse) return;
        if (isClose && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Item picked up: " + itemName);
            Inventory.instance.AddItemInInventory(this);
            objToEnable.SetActive(false);
            if (msgToDialogue != "") DialogueManager.instance.ItemGetDialogue(msgToDialogue);
            canUse = false;
            for(int i=0;i<varNameOrCodeToAdd.Count;i++){
                Inventory.instance.AddVariable(varNameOrCodeToAdd[i]);
            }
            if (!sprToChange.Equals(null)) sprRender.sprite = sprToChange;
            else this.gameObject.SetActive(false);
        }
    }

}

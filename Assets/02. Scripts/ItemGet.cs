using System.Collections.Generic;
using UnityEngine;

public class ItemGet : MonoBehaviour
{
    [Header("about items")] public int itemID;

    [Header("When Player Come Close")] public bool isClose; //플레이어가 해당 아이템의 트리거 영역에 있는지 여부

    public GameObject objToEnable; //트리거 영역에 있는 경우, 활성화하는 오브젝트(ex.반짝이는 효과)

    [Header("When Player gets item")] [TextArea]
    public string msgToDialogue = "000을 얻었다.";


    [Header("After Find Item, to do?")] public Sprite sprToChange;

    public bool canUse = true; //중복 줍기 방지
    public List<string> varNameOrCodeToAdd;
    private Item item;

    private PlayerData playerData;

    /// <summary>
    ///     아이템을 줍기 전, 줍고, 주운 후를 처리하는 코드.
    /// </summary>
    private SpriteRenderer sprRender;

    private void Awake()
    {
        playerData = GameObject.FindGameObjectWithTag("PlayerData").GetComponent<PlayerData>();
    }

    private void Start()
    {
        sprRender = GetComponent<SpriteRenderer>();

        item = playerData.GetItemFromID(itemID);
    }

    private void Update()
    {
        if (!canUse) return;
        if (isClose && PlayerMovement.isSpace)
        {
            PlayerMovement.instance.IsSpaceFalse(0.5f);
            Debug.Log("Item picked up: " + item.itemName);
            playerData.AddItem(item.itemID);
            objToEnable.SetActive(false);

            if (msgToDialogue != "")
            {
                DialogueManager.instance.ItemGetDialogue(msgToDialogue);
                PlayerMovement.instance.canMove = false;
            }

            canUse = false;
            for (int i = 0; i < varNameOrCodeToAdd.Count; i++) playerData.AddVariable(varNameOrCodeToAdd[i]);
            if (!sprToChange.Equals(null)) sprRender.sprite = sprToChange;
            else gameObject.SetActive(false);
        }
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
}
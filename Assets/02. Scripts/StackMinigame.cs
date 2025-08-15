/*
    동적으로 작동해야함 push 버튼이 눌렸을 때 그리드 레이아웃
    안에 이미지를 추가 하고 팝버튼이 눌러졌을 때 이미지를 하나 없에기
    일단 직접 큐를 구현 하기 해야될 듯
    버튼을 눌렀을 때 매인 자료구조 큐를 바꾸고 그걸 ui에 적용하기
    현제 문제가 동적으로 이미지를 생성하는 법을 모른다
    알아야 할 것 동적으로 이미지 생성하는 법
    그렇게 만들어진 이미지를 컨트롤 하는 법
 */


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StackBlock
{
    public GameObject block;
    public int itemCode = -1; // 아이템 코드, -1은 아이템이 없는 상태
    [HideInInspector] public SpriteRenderer sprite; //그리드 obj 이거 자식을 불러와야함
    public bool isBugBlock; //버그 블록인지 아닌지

    public void ChangeSprite(int itemID, Sprite newSprite)
    {
        GetSpriteRenderer();
        itemCode = itemID;
        sprite.sprite = newSprite;
    }

    public void GetSpriteRenderer()
    {
        if (sprite == null)
            sprite = block.GetComponent<SpriteRenderer>();
    }
}

public class StackMinigame : MonoBehaviour
{
    public Transform topTextObj;
    public Collider2D veryBottom;
    public GameObject bookCol;

    public List<StackBlock> stackBlock = new();
    public int top = -1; // 스택의 최상단 인덱스

    private readonly List<StackBlock> removedStackBlock = new(); // 제거된 블록을 저장하는 리스트
    private bool canPop = true; //블록을 팝할 수 있는지 여부
    private bool canPush = true;
    private Vector3 topLocalPos; //topTextObj의 로컬 포지션

    private void Start()
    {
        topLocalPos = topTextObj.localPosition;
        top = stackBlock.Count - 1;
        UpdateOrder();
    }

    private void UpdateOrder()
    {
        for (var i = 0; i < stackBlock.Count; i++)
        {
            stackBlock[i].GetSpriteRenderer();
            stackBlock[i].sprite.sortingOrder = i + 1;
            //현재 인덱스에 따른 레이어 오더 설정
        }
    }

    private void ChangeTopText()
    {
        if (topTextObj != null)
        {
            if (top == -1)
            {
                topTextObj.parent = veryBottom.transform;
                veryBottom.enabled = false;
                bookCol.SetActive(false);
                var newPos = topLocalPos;
                newPos.y = -0.2f;
                topTextObj.localPosition = newPos;
            }
            else
            {
                veryBottom.enabled = true;
                bookCol.SetActive(true);
                topTextObj.parent = stackBlock[top].block.transform;
                topTextObj.localPosition = topLocalPos;
            }
        }
    }

    public int PushButtonClick()
    {
        if (!canPush) return 0;

        var holdingItemID = Inventory.instance.currentHoldingItemIndex == -1
            ? -1
            : Inventory.instance.itemHoldList[Inventory.instance.currentHoldingItemIndex].itemID;
        if (holdingItemID >= 50 && holdingItemID <= 54)
        {
            //스택용 책인 경우
            PlayerData.instance.RemoveItem(holdingItemID);
            StartCoroutine(AddStackBlockAtTop(holdingItemID));
            return 0;
        }

        for (var i = 0; i < PlayerData.instance.items.Count; i++)
            if (PlayerData.instance.items[i] >= 50 && PlayerData.instance.items[i] <= 54)
            {
                //스택용 책인 경우
                var itemID = PlayerData.instance.RemoveItemAt(i);
                StartCoroutine(AddStackBlockAtTop(itemID));
                return 0;
            }

        return -1;

        // Debug.Log(CheckBug());
    }

    private IEnumerator AddStackBlockAtTop(int itemID)
    {
        canPush = false; //블록을 푸시할 수 없는 상태로 변경

        if (itemID == -1)
        {
            Debug.LogWarning("no item book found");
            yield break;
        }

        for (var i = 0; i < removedStackBlock.Count; i++)
            if (removedStackBlock[i].itemCode == itemID)
            {
                var newBlock = removedStackBlock[i];
                removedStackBlock.RemoveAt(i);
                stackBlock.Add(newBlock);
                UpdateOrder();
                var newPos = newBlock.block.transform.position;
                newPos.y = transform.position.y;
                newBlock.block.transform.position = newPos;
                newBlock.block.SetActive(true);
                top++;
                ChangeTopText();

                yield return new WaitForSeconds(0.5f);
                canPush = true; //이제 다시 블록을 푸시할 수 있음
                yield break;
            }
    }

    public int PopButtonClick()
    {
        if (!canPop) return 0;
        if (top == -1)
        {
            Debug.Log("stack is empty, cannot pop");
            return -1;
        }

        StartCoroutine(PopBlock());
        return 0;
    }

    private IEnumerator PopBlock()
    {
        canPop = false; //블록을 팝할 수 없는 상태로 변경
        var poppedBlock = stackBlock[top];
//        Debug.Log("the book itemcode is "+poppedBlock.itemCode);
        poppedBlock.block.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.65f);
        PlayerData.instance.AddItem(poppedBlock.itemCode);
        stackBlock.RemoveAt(top);
        removedStackBlock.Add(poppedBlock);

        if (poppedBlock.isBugBlock)
        {
            //bug가 있는 블럭이었던 경우, 성공!
            var bug = poppedBlock.block.transform.GetChild(0);
            bug.parent = null;
            SoundManager.instance.PlaySound("minigame clear");
            bug.GetComponent<MemoryBugObject>().MoveDown();
            poppedBlock.isBugBlock = false; //2번 방지
        }

        poppedBlock.block.SetActive(false);
        top--;

        UpdateOrder();
        ChangeTopText();
        canPop = true; //이제 다시 블록을 팝할 수 있음
    }
}
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
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class QueueBlock
{
    [SerializeField]
    public GameObject pushButton; // push 버튼
    public GameObject gridLayoutGroup; //그리드 obj 이거 자식을 불러와야함
    public List<GameObject> blocks; // queueBlock 모음 시작 할 때 기본적으로 2개 주기
    public GameObject initBlock; //복제될 블럭 push버튼을 누르면 이걸 복사함

}
    
public class Queue : MonoBehaviour
{
    public QueueBlock queueBlock = new();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(queueBlock.gridLayoutGroup.transform);
        queueBlock.initBlock = queueBlock.gridLayoutGroup.transform.GetChild(0).gameObject; // 가장 처음있는 블록을
        //제일 처음 초기화 기획한 대로 하면 2개임
        for (int i = 0; i < queueBlock.gridLayoutGroup.transform.childCount; i++)
        {
            queueBlock.blocks[i] = (queueBlock.gridLayoutGroup.transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pushButtonClick()
    {
        GameObject willAddBlock = Instantiate(queueBlock.initBlock, queueBlock.gridLayoutGroup.transform, true);
        willAddBlock.transform.localPosition = new Vector3(willAddBlock.transform.localPosition.x,willAddBlock.transform.localPosition.y,0f);//추가될 블럭
        willAddBlock.transform.localScale = new Vector2(1, 1);
    }
}

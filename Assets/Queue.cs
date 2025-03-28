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
using UnityEngine.UIElements;


[Serializable]
public class QueueBlock
{
    public GameObject pushButton; // push 버튼
    public GameObject gridLayoutGroup;
    public List<GameObject> block; // queueBlock 모음 시작 할 때 기본적으로 2개 주기

}
    
public class Queue : MonoBehaviour
{
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        QueueBlock queueBlock = new QueueBlock();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

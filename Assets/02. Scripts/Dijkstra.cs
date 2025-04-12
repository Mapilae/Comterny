using System;
using System.Collections.Generic;
using UnityEngine;
// 에초에 틀렸을 때 초기화 버튼을 만들어 다 활성화 되어야 하고
// 버튼을 누를 때마다 리스트에 값을 하나씩 추가한다
// 그다음 그 리스트가 정답 리스트랑 값이 같으면 성공 하게 한다.

// 각각의 노드를 구성하는 기본적인 정보를 가지는 클래스


public class Dijkstra : MonoBehaviour
{
    public List<NodeBlock> nodeBlocks = new();
    public bool neverClick = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {   /*
        for (var i = 0; i < binaryNodes.Count; i++)
        {
            var node = binaryNodes[i];
            Debug.Log(node.objNode.transform.position);
        }*/
    }

    public void CreateNode(int index)
    {
    }
    // 순서대로 클릭 하고 4번 이상 클릭 하면 초기화 되게함
    public void imgClick(int index)
    {
        //코드 추가 가이드: 정점을 1개만 제거하도록 4,5,6번 원중 하나를 지웠으면 나머지 원은 눌러도 삭제 안되는 코드 추가 필요

        Debug.Log(index);
        if ((index == 4 || index == 5 || index == 6) && neverClick)
        {
            
            neverClick = false;
            Debug.Log("성공!!!!!!");
            foreach (var arrow in nodeBlocks[index - 1].adjustArrows) arrow.SetActive(false);
            nodeBlocks[index - 1].vertexObj.SetActive(false);
            
        }
        else
        {
            foreach (var arrow in nodeBlocks[index - 1].adjustArrows) arrow.SetActive(false);

            nodeBlocks[index - 1].vertexObj.SetActive(false);
            Debug.Log("실패!!!!!!!");
        }
    }

    // Update is called once per frame
}
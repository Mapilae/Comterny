using System;
using System.Collections.Generic;
using UnityEngine;


// 각각의 노드를 구성하는 기본적인 정보를 가지는 클래스
[Serializable]
public class NodeBlock
{
    public GameObject vertexObj;
    public List<GameObject> adjustArrows;
}

public class BinaryTree : MonoBehaviour
{
    public List<NodeBlock> nodeBlocks = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Debug.Log("fddf");

        /*
        for (var i = 0; i < binaryNodes.Count; i++)
        {
            var node = binaryNodes[i];
            Debug.Log(node.objNode.transform.position);
        }*/
    }

    public void CreateNode(int index)
    {
    }

    public void imgClick(int index)
    {
        //코드 추가 가이드: 정점을 1개만 제거하도록 4,5,6번 원중 하나를 지웠으면 나머지 원은 눌러도 삭제 안되는 코드 추가 필요

        Debug.Log(index);
        if (index == 4 || index == 5 || index == 6)
        {
            Debug.Log("성공!!!!!!");
            foreach (var arrow in nodeBlocks[index - 1].adjustArrows) arrow.SetActive(false);
            nodeBlocks[index - 1].vertexObj.SetActive(false);
        }
        else
        {
            Debug.Log("실패!!!!!!!");
        }
    }

    // Update is called once per frame
}
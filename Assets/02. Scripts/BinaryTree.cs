using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


// 각각의 노드를 구성하는 기본적인 정보를 가지는 클래스
[System.Serializable]
public class NodeBlock
{
    public string binaryCode;
    public bool isNodeRight;
    public GameObject objNode;
    [HideInInspector] public GameObject obj;
    public TMP_Text text;
    
}
public class BinaryTree : MonoBehaviour
{
    public List<NodeBlock> binaryNodes = new List<NodeBlock>();
    public void CreateNode(int index)   
    {
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("fddf");
        
        for (int i = 0; i < binaryNodes.Count; i++)
        {
            NodeBlock node = binaryNodes[i];
            Debug.Log(node.objNode.transform.position);
        }
    }

    // Update is called once per frame
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;


// 각각의 노드를 구성하는 기본적인 정보를 가지는 클래스
public class Node
{
    public string binaryCode;
    public bool isNodeRight;
    public GameObject objNode;
    [HideInInspector] public GameObject obj;
    public TMP_Text text;
    
}
public class Binary : MonoBehaviour
{
    public List<Node> nodes = new List<Node>();

    public void CreateNode(int index)
    {
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}

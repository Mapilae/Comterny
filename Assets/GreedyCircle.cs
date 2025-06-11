using System;
using System.Collections.Generic;
using UnityEngine;

public class GreedyCircle : MonoBehaviour
{
    private SpriteRenderer render;
    bool isVisited = false; // 방문 여부
    public List<Collider2D> beforeNodes; // 방문 전 지나간 간선들
    List<SpriteRenderer> nearNodes;

    public bool isThisNodeGameOver;//틀린 답인가?
    

    private void Start()
    {
        nearNodes=new List<SpriteRenderer>();
        for (int i = 0; i < transform.childCount; i++)
        {
            nearNodes.Add(transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>());
        }
        render = GetComponent<SpriteRenderer>();
        ResetCircle(); // 초기화 시 방문하지 않은 상태로 설정
    }

    public void SetVisited(bool visited)
    {
        if(render==null)
            render = GetComponent<SpriteRenderer>();
        isVisited = visited;
        render.color = isVisited ? Color.yellow : Color.white;
        // 방문한 경우 색상을 노란색으로 변경, 방문하지 않은 경우 흰색으로 변경
        NearNodeActive(isVisited);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("deadzone"))//players new col
        {
            NearNodeActive(true);
            BeforeNodeActive(true);
            CheckIfGameOver();
        }
    }

    void CheckIfGameOver()
    {
        if (isThisNodeGameOver)
        {
            //game over!
            GreedyMinigame.instance.FailedGame();
        }
        else
        {
            SetVisited(true);
        }
    }
    
    public void ResetCircle()
    {
        SetVisited(false);
        NearNodeActive(false);
        BeforeNodeActive(false);
    }
    
    void NearNodeActive(bool active)
    {
        if(nearNodes==null||nearNodes.Count==0)
            return;
        foreach (var node in nearNodes)
        {
            if (node != null)
            {
                node.gameObject.SetActive(active);
            }
        }
    }

    void BeforeNodeActive(bool active)
    {
        foreach (var node in beforeNodes)
        {
            node.enabled = active;
        }
    }
    
}

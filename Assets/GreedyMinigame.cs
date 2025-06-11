using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GreedyMinigame : MonoBehaviour
{
    public CapsuleCollider2D playerDftCol;
    public Collider2D playerNewCol;
    public static GreedyMinigame instance;
    List<GreedyCircle> greedyCircles; // 그리디 서클 리스트
    
    public bool isClear = false; // 게임이 클리어 되었는지 여부
    
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        greedyCircles=GameObject.FindObjectsOfType<GreedyCircle>().ToList();
        
        ResetGame();
        SetPlayerNewCollider(true);
    }

    void SetPlayerNewCollider(bool isActive)
    {
        
            playerNewCol.enabled = isActive;
        
        playerDftCol.enabled = !isActive;
    }
    
    public void FailedGame()
    {
        SoundManager.instance.PlaySound("minigame fail");
        ResetGame();
    }

    void ResetGame()
    {
        GameObject.FindWithTag("Player").transform.position=this.transform.position;
        
        foreach (var circle in greedyCircles)
        {
            circle.ResetCircle();
        }
        
        
    }
}

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
        
        ResetGame(isPlayerTP:false);
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

    void ResetGame(bool isPlayerTP=true)
    {
        if(isPlayerTP)
            GameObject.FindWithTag("Player").transform.position=this.transform.position;
        
        foreach (var circle in greedyCircles)
        {
            circle.ResetCircle();
        }
    }
    
    public void SuccessGame()
    {
        if (isClear) return; // 이미 클리어된 경우는 무시
        isClear = true;
        SoundManager.instance.PlaySound("minigame success");
        SetPlayerNewCollider(false);
        // 미니게임 성공 후 처리할 로직 추가
        Debug.Log("Greedy Minigame Cleared!");
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GreedyMinigame : MonoBehaviour, ISaveableState
{
    public static GreedyMinigame instance;
    public CapsuleCollider2D playerDftCol;
    public Collider2D playerNewCol;

    public bool isClear; // 게임이 클리어 되었는지 여부
    public string clearCmd = "questRemove_가장 짧은 길로만";
    public GameObject collderWhenEnd;
    private List<GreedyCircle> greedyCircles; // 그리디 서클 리스트

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        greedyCircles = FindObjectsOfType<GreedyCircle>().ToList();

        ResetGame(false);
        SetPlayerNewCollider(true);
        // 다리 위 진행 상황(지나온 길 등)은 저장하지 않는다. 도중에 종료하면 미니게임 시작 전으로 이어진다.
        if (!isClear) SaveSystem.SetBusy(this, true);
    }

    private void OnDisable()
    {
        SaveSystem.SetBusy(this, false);
    }

    public string CaptureState(SceneStateTracker ids)
    {
        return isClear ? "clear" : null;
    }

    public void RestoreState(string data, SceneStateTracker ids)
    {
        if (data != "clear") return;
        isClear = true;
        SaveSystem.SetBusy(this, false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            if (isClear)
            {
                SetPlayerNewCollider(false);
                collderWhenEnd.SetActive(true);
            }
    }

    public void SetPlayerNewCollider(bool isActive)
    {
        playerNewCol.enabled = isActive;

        playerDftCol.enabled = !isActive;
    }

    public void FailedGame()
    {
        SoundManager.instance.PlaySound("minigame fail");
        ResetGame();
    }

    private void ResetGame(bool isPlayerTP = true)
    {
        if (isPlayerTP)
            GameObject.FindWithTag("Player").transform.position = transform.position;

        foreach (var circle in greedyCircles) circle.ResetCircle();
        //  SetPlayerNewCollider(false);
    }

    public void SuccessGame()
    {
        if (isClear) return; // 이미 클리어된 경우는 무시
        isClear = true;
        SaveSystem.SetBusy(this, false);
        SoundManager.instance.PlaySound("minigame success");
        DirectingManager.instance.RunCMD(clearCmd);
        // SetPlayerNewCollider(false);
        // 미니게임 성공 후 처리할 로직 추가
        Debug.Log("Greedy Minigame Cleared!");
    }
}
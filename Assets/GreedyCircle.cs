using System.Collections.Generic;
using UnityEngine;

public class GreedyCircle : MonoBehaviour
{
    public List<Collider2D> beforeNodes; // 방문 전 지나간 간선들
    public List<SpriteRenderer> nextVertex;
    public int thisType; //1: 시작점, 0: 중간점, 3: 끝점
    public Sprite[] circleSpr;
    public Color edgeColors;


    public bool isThisNodeGameOver; //틀린 답인가?
    public GameObject objToDisableWhenEnter;
    public GreedyCircle disableGameOver;
    private bool isVisited; // 방문 여부
    private List<SpriteRenderer> nearNodes;
    private List<GameObject> nearNodeWeightText;
    private SpriteRenderer render;

    private void Start()
    {
        nearNodes = new List<SpriteRenderer>();
        nearNodeWeightText = new List<GameObject>();
        for (var i = 0; i < 3; i++)
            if (transform.GetChild(i).gameObject.activeInHierarchy)
                nearNodes.Add(transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>());

        for (var i = 3; i < transform.childCount; i++)
            if (i < transform.childCount && transform.GetChild(i).gameObject.activeInHierarchy)
                nearNodeWeightText.Add(transform.GetChild(i).gameObject);

        render = GetComponent<SpriteRenderer>();
        ResetCircle(); // 초기화 시 방문하지 않은 상태로 설정
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GreedyMinigame.instance.isClear) return;
        if (other.CompareTag("deadzone") || other.CompareTag("Player")) //players new col
        {
            if (objToDisableWhenEnter != null) objToDisableWhenEnter.SetActive(false);
            NearNodeActive(true);
            BeforeNodeActive(true);
            CheckIfGameOver();
        }
    }

    public void SetVisited(bool visited)
    {
        if (GreedyMinigame.instance.isClear) return;
        if (render == null)
            render = GetComponent<SpriteRenderer>();


        isVisited = visited;
        render.sprite = isVisited && !disableGameOver ? circleSpr[1] : circleSpr[0];
        // 방문한 경우 색상을 노란색으로 변경, 방문하지 않은 경우 흰색으로 변경
        NearNodeActive(isVisited);
        if (disableGameOver != null) disableGameOver.isThisNodeGameOver = !isVisited;
    }


    private void CheckIfGameOver()
    {
        if (isThisNodeGameOver)
        {
            //game over!
            GreedyMinigame.instance.FailedGame();
        }
        else
        {
            SetVisited(true);

            if (thisType.Equals(2)) GreedyMinigame.instance.SuccessGame();
        }
    }

    public void ResetCircle()
    {
        if (GreedyMinigame.instance.isClear) return;
        SetVisited(false);
        NearNodeActive(false);
        BeforeNodeActive(false);
        if (thisType >= 1) render.enabled = true;
    }

    private void NearNodeActive(bool active)
    {
        if (GreedyMinigame.instance.isClear) active = true;
        if (nearNodes == null || nearNodes.Count == 0)
            return;
        foreach (var node in nearNodes)
            if (node != null)
            {
                node.gameObject.SetActive(active);
                node.color = edgeColors;
            }

        foreach (var vertex in nearNodeWeightText)
            if (vertex != null)
                vertex.SetActive(active);

        foreach (var vertex in nextVertex)
            if (vertex != null)
            {
                //vertex.enabled = active;
            }
    }

    private void BeforeNodeActive(bool active)
    {
        if (GreedyMinigame.instance.isClear) active = false;
        foreach (var node in beforeNodes) node.isTrigger = !active;
    }
}
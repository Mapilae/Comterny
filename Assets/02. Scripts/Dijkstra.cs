using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// 에초에 틀렸을 때 초기화 버튼을 만들어 다 활성화 되어야 하고
// 버튼을 누를 때마다 리스트에 값을 하나씩 추가한다
// 그다음 그 리스트가 정답 리스트랑 값이 같으면 성공 하게 한다.

// 각각의 노드를 구성하는 기본적인 정보를 가지는 클래스

[Serializable]
public class VertexBlock
{
    //정점 클래스
    public Button vertex;

    public VertexBlock(Button vertex)
    {
        this.vertex = vertex;
    }
}


public class Dijkstra : MonoBehaviour
{
    public List<int> clickList = new();
    public TMP_Text pathText;
    public Transform vertexBlockParent;
    public List<VertexBlock> vertexBlocks = new();
    public Transform nodeParent;
    public List<TMP_Text> nodeTexts = new();

    public GameObject demoEndObj;

    private readonly List<int> correctList = new() { 1, 3, 5, 7 };

    private readonly string strPath = "AABCDEFGG";
    //public int minSum = 1000000; //경로 최소값(정답)
    // public int playerSum = 1000000; //플레이어가 클릭한 간선 가중치 합. 나중에 minSum과 비교함. 

    private readonly int[] weight = new int[7];


    private void Start()
    {
        //자동으로 찾아서 할당함
        for (var i = 0; i < vertexBlockParent.childCount; i++)
            vertexBlocks.Add(new VertexBlock(vertexBlockParent.GetChild(i).GetComponent<Button>()));
        for (var i = 0; i < nodeParent.childCount; i++)
            nodeTexts.Add(nodeParent.GetChild(i).GetComponent<TMP_Text>());

        ResetVertexBlocks();
    }

    /*간선위치 노드위치는 고정 하지만 간선의 길이(가중치는 랜덤한 값으로 한다)
    문제는 1에서 7까지 가는 경로로 고정하고
    렌덤한 간선의 가중치 배열 [1,3,4,6,3,2,6] ( 렌덤 range(1,6))
    배열 값을 프론트에 반영하고 내부적으로 다익을 돌려서 답안을 받는다
    그후에는 기존과 같다.
    */

    private void OnEnable()
    {
        Pause.instance.SetCurrentPuzzle(gameObject);
    }

    private void OnDisable()
    {
        Pause.instance.SetCurrentPuzzle(null);
    }

    public void ResetVertexBlocks()
    {
        foreach (var vertexBlock in vertexBlocks)
        {
            vertexBlock.vertex.image.color = Color.white;
            vertexBlock.vertex.interactable = true;
        }

        for (var i = 0; i < nodeTexts.Count; i++) weight[i] = Random.Range(1, 6);

        var sumOf3n4 = weight[3] + weight[4]; //3번과 4번 인덱스 간선의 가중치 합 (3->5->7)
        var sumOf5n6 = weight[5] + weight[6]; //5,6번 인덱스 간선의 가중치 합 (3->4->7)

        if (sumOf3n4 >= sumOf5n6)
        {
            // 중복된 경우, 재생성
            weight[3] = weight[5];

            weight[4] = weight[6] = Random.Range(2, 6);
            weight[4] -= 1;
        }

        for (var i = 0; i < nodeTexts.Count; i++) nodeTexts[i].text = weight[i].ToString();

        pathText.text = "A";
        clickList.Clear();
        clickList.Add(1);
        vertexBlocks[0].vertex.image.color = Color.red;
        vertexBlocks[0].vertex.interactable = false;
    }

    public void ImgClick(int index)
    {
        Debug.Log(index);
        pathText.text += "→" + strPath[index];
        SoundManager.instance.PlaySound("keyboard");

        // foreach (var arrow in nodeBlocks[index - 1].adjustArrows) arrow.SetActive(false);
        clickList.Add(index);
        vertexBlocks[index - 1].vertex.image.color = Color.red;
        vertexBlocks[index - 1].vertex.interactable = false;

        Debug.Log(clickList.Count);
    }

    public void EndCheck()
    {
        //7번 버튼에 넣음
        if (clickList.SequenceEqual(correctList))
            Success();
        else Fail();
    }

    public void Success()
    {
        Debug.Log("성공!!!!!!");
        //if (objToDisable != null) objToDisable.gameObject.SetActive(false);
        // PlayerData.instance.AddVariable(varToAdd);
        PlayerMovement.instance.ChangeCanMove(true);
        //StartCoroutine(ClosePuzzleAfterDelay());
        SoundManager.instance.PlaySound("minigame clear");
        Invoke(nameof(DisableThis), 3f);
    }

    public void Fail()
    {
        SoundManager.instance.PlaySound("minigame fail");
        ResetVertexBlocks();
    }

    public void CloseTab()
    {
        PlayerMovement.instance.ChangeCanMove(true);
        gameObject.SetActive(false);
    }

    private void DisableThis()
    {
        Debug.Log("END");
        //temp
        // LodingScene.instance.LoadScene();
        demoEndObj.SetActive(true);
        gameObject.SetActive(false);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// 에초에 틀렸을 때 초기화 버튼을 만들어 다 활성화 되어야 하고
// 버튼을 누를 때마다 리스트에 값을 하나씩 추가한다
// 그다음 그 리스트가 정답 리스트랑 값이 같으면 성공 하게 한다.

// 각각의 노드를 구성하는 기본적인 정보를 가지는 클래스


public class Dijkstra : MonoBehaviour
{
    public List<NodeBlock> nodeBlocks = new();
    private bool neverClick = true;
    
    private List<int> correctList = new List<int>(){1,3,5,7} ;
    public List<int> clickList = new();

    public void ResetNodeBlocks()
    {
        foreach (var nodeBlock in nodeBlocks)
        {
            nodeBlock.vertexObj.SetActive(true);

        }

        neverClick = true;
    }

    // 순서대로 클릭 하고 4번 이상 클릭 하면 초기화 되게함
    public void imgClick(int index)
    {
        //코드 추가 가이드: 정점을 1개만 제거하도록 4,5,6번 원중 하나를 지웠으면 나머지 원은 눌러도 삭제 안되는 코드 추가 필요
        if (neverClick)
        {
            Debug.Log(index);

            // foreach (var arrow in nodeBlocks[index - 1].adjustArrows) arrow.SetActive(false);
            clickList.Add(index);
            nodeBlocks[index - 1].vertexObj.SetActive(false);
            if (clickList.SequenceEqual(correctList))
            {
                Debug.Log("성공!!!!!!");
                neverClick = false;
            }
            Debug.Log(clickList.Count);
            
        }
    }
}

    // Update is called once per frame

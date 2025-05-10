using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TreeNode
{
    public GameObject vertex;
    public GameObject text;
    public GameObject realBranch;
    [HideInInspector] public GameObject parent;
    public List<int> adjustedNode = new();
    private float currentScale;


    public void SetActive(bool active)
    {
        if (parent == null) parent = vertex.transform.parent.gameObject;
        //vertex.SetActive(active);
        parent.SetActive(active);
        text.SetActive(active);
        realBranch.SetActive(active);
    }


    public void OnPointer(bool isOn)
    {
        if (currentScale == 0f)
            currentScale = vertex.transform.localScale.x;
        vertex.transform.localScale = isOn
            ? new Vector3(currentScale * 1.2f, currentScale * 1.2f, 1)
            : new Vector3(currentScale, currentScale, 1);
    }
}

public class BinaryTree : MonoBehaviour
{
    public List<TreeNode> vertexObj = new();
    public bool neverClick = true;
    public GameObject barrier;
    public GameObject numTextWhole;
    public Transform pointer;
    [Header("When Success")] public int varToAdd;

    public bool isUsing = true; //false인경우 이진트리 관련해서는 사용하지 않음
    public int deletedNum = -1;

    public GameObject objToDisable; //puzzle open interactable Object
    private Animator anim;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        isUsing = true;
        /*
        for (var i = 0; i < binaryNodes.Count; i++)
        {
            var node = binaryNodes[i];
            Debug.Log(node.objNode.transform.position);
        }*/
    }

    private void OnEnable()
    {
        barrier.SetActive(true);
        Invoke("BarrierOff", 2f);
        if (isUsing) Pause.instance.SetCurrentPuzzle(gameObject);
    }

    private void OnDisable()
    {
        PlayerMovement.instance.ChangeCanMove(true);
    }

    private void BarrierOff()
    {
        barrier.SetActive(false);
        numTextWhole.SetActive(true);
    }

    public void ImgClick(int index)
    {
        if (!isUsing) return;
        //코드 추가 가이드: 정점을 1개만 제거하도록 6,7,8번 원중 하나를 지웠으면 나머지 원은 눌러도 삭제 안되는 코드 추가 필요

        Debug.Log(index);
        vertexObj[index - 1].SetActive(false);
        barrier.SetActive(true);

        if ((index == 6 || index == 7 || index == 8) && neverClick)
        {
            neverClick = false;

            // foreach (var arrow in vertexObj[index - 1].adjustArrows) arrow.SetActive(false);
            // nodeBlocks[index - 1].vertexObj.SetActive(false);
            RemoveDelNum(index);
            Success();
        }
        else
        {
            Fail(index);
        }
    }

    public void RemoveDelNum(int delNum)
    {
        vertexObj[delNum - 1].SetActive(false);
        // vertexObj.RemoveAt(delNum - 1);
        deletedNum = delNum;
        vertexObj[2].adjustedNode.Remove(delNum); //이거의 부모 정점에서 인접노드로 해당 인덱스 삭제하기
    }

    public void Success()
    {
        //end animation
        anim.SetTrigger("Success");

        if (isUsing)
        {
            if (deletedNum.Equals(-1))
                RemoveDelNum(7);
            //변수 추가
            PlayerData.instance.AddVariable(varToAdd);
            objToDisable.SetActive(false);
        }

        Invoke("DisableThis", 3f);
    }

    public void Fail(int index)
    {
        //reset game
        StartCoroutine(IEFail(index));
    }

    private IEnumerator IEFail(int index)
    {
        anim.SetTrigger("Fail");
        yield return new WaitForSeconds(4.75f);
        //gameObject.transform.GetChild(0).gameObject.SetActive(false);
        if (isUsing)
        {
            vertexObj[index - 1].SetActive(true);
            //gameObject.transform.GetChild(0).gameObject.SetActive(true);
            neverClick = true;
        }

        barrier.SetActive(false);
    }

    private void DisableThis()
    {
        Debug.Log("END");
        barrier.SetActive(false);
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void PointerOn(int index)
    {
        pointer.parent = vertexObj[index - 1].text.transform;
        pointer.transform.localPosition = Vector3.zero;
        pointer.gameObject.SetActive(true);
        vertexObj[index - 1].OnPointer(true);
    }

    public void PointerOff(int index)
    {
        pointer.parent = transform;
        pointer.gameObject.SetActive(false);
        vertexObj[index - 1].OnPointer(false);
    }
}
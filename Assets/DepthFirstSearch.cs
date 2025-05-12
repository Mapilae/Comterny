using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DepthFirstSearch : MonoBehaviour
{
    public BinaryTree binaryTree;
    public RectTransform treeWhole;
    public TMP_Text titleText;
    public TMP_Text contentText;
    public TMP_Text pathText;
    public List<int> userPath;

    //1 -> 2 > [ ] -> 5 -> [ ] -> 10 -> [ ] -> 6?7 -> 7?8
    //1  2  4   5  9  10  3  6?7  7?8
    public bool isUsing;

    public List<int> correctPath = new() { 1, 2, 4, 5, 9, 10, 3, 67, 78 };

    public GameObject hideWhole;
    public TMP_Text failText;
    public GameObject objToDisable;
    private readonly int[] userPathIndexOfBlank = { 4, 6 };
    private int userPathIndex;
    private List<TreeNode> vertexObj = new();
    public int varToAdd = 115;

    private void OnEnable()
    {
        Pause.instance.SetCurrentPuzzle(gameObject);
        Debug.Log("Depth First Search Started");
        binaryTree.gameObject.SetActive(true);
        binaryTree.isUsing = false;
        failText.text = "왠지 이 경로가 아닌 것 같다...";
        isUsing = true;

        vertexObj = binaryTree.vertexObj;
        treeWhole.localScale = Vector3.one * 0.9f;
        treeWhole.localPosition = new Vector3(treeWhole.localPosition.x, treeWhole.localPosition.y + 50,
            treeWhole.localPosition.z);

        if (binaryTree.deletedNum.Equals(-1)) binaryTree.RemoveDelNum(7);
        correctPath = new List<int> { 1, 2, 4, 5, 9, 10, 3, 67, 78 };
        switch (binaryTree.deletedNum)
        {
            case 6:
                correctPath[7] = 7;
                correctPath[8] = 8;
                break;
            case 7:
                correctPath[7] = 6;
                correctPath[8] = 8;
                break;
            case 8:
                correctPath[7] = 6;
                correctPath[8] = 7;
                break;
        }

        userPath = correctPath.ToList();
        //1 -> 2 -> 4 -> 5 -> [ ] -> 10 -> [ ] -> 6 -> 8
        ResetPath();

        titleText.text = "DFS 버스 경로 입력";
        contentText.text = "깊이우선탐색 규칙에 따라 빈 정류장 번호를 채워 넣어 버스 노선을 완성하세요.";
    }

    private void ResetPath()
    {
        userPath[4] = userPath[6] = -1;
        userPathIndex = 0;
        SetPathText();
    }

    private void SetPathText()
    {
        var isFinishedFilling = true;
        pathText.text = "";
        for (var i = 0; i <= correctPath.Count - 1; ++i)
        {
            if (userPath[i] == -1)
            {
                isFinishedFilling = false;
                if (i.Equals(userPathIndexOfBlank[userPathIndex]))
                    pathText.text += "<color=yellow>\u25a1</color>"; //□
                else pathText.text += "\u25a1"; //□
            }
            else
            {
                pathText.text += userPath[i].ToString();
            }

            //마지막이 아닌 경우 화살표 추가
            if (i != correctPath.Count - 1) pathText.text += " -> ";
        }

        if (isFinishedFilling) CheckAnswer();
    }

    private void CheckAnswer()
    {
        if (userPath.SequenceEqual(correctPath))
            Success();
        else
            Fail();
    }

    public void Success()
    {
        hideWhole.SetActive(false);
        objToDisable.SetActive(false);
        Debug.Log("성공!");
        binaryTree.Success();
        PlayerData.instance.AddVariable(varToAdd);
        //버스 오는 animator에 트리거해서 버스오는거 만들기
        //또는 버스 오는 애니메이션 스크립트 작성해서 "버스가 곧 옵니다.." 같은거 만들기
    }

    private void Fail()
    {
        binaryTree.Fail(0);
        Invoke(nameof(ResetPath), 4.75f);
    }

    public void ImgClick(int index)
    {
        if (!isUsing) return;
        if (userPathIndex >= userPathIndexOfBlank.Length) return;
        userPath[userPathIndexOfBlank[userPathIndex]] = index;
        userPathIndex++;
        SetPathText();
    }
}
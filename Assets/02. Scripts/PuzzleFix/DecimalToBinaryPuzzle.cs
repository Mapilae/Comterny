using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DecimalToBinaryPuzzle : MonoBehaviour
{
    public UISwitcher.UISwitcher[] toggleSwitches; // 8개 스위치
    public TextMeshProUGUI[] toggleLabels; // 각 토글의 Label
    public RectTransform[] tipRects; // 각 토글의 Tip

    public TextMeshProUGUI decimalNumberText; // 문제로 제시되는 십진수
    public TextMeshProUGUI resultText; // 플레이어 입력의 변환 결과
    public TextMeshProUGUI successText; // 성공 메시지
    public GameObject puzzlePanel; // 퍼즐 전체 패널

    public ChatBallon chatBallon; // 대화 풍선
    public GameObject objToHide;
    public GameObject objToHide2;

    private int targetNumber;

    private void Start()
    {
        GenerateQuestion();

        for (var i = 0; i < toggleSwitches.Length; i++)
        {
            var index = i;
            toggleSwitches[i].onValueChanged.AddListener(_ => SetToggleVisuals(index));
        }

        UpdateAllToggles(); // 초기 상태 반영
    }

    private void OnEnable()
    {
        Pause.instance.SetCurrentPuzzle(gameObject);
    }

    private void OnDisable()
    {
        Pause.instance.SetCurrentPuzzle(null);
    }


    public void ClosePuzzle()
    {
        gameObject.SetActive(false);
        PlayerMovement.instance.ChangeCanMove(true);
    }

    private void GenerateQuestion()
    {
        targetNumber = Random.Range(1, 32); // 1~255 --> 1~31로 변경
        decimalNumberText.text = targetNumber.ToString();
        successText.gameObject.SetActive(false);
    }

    public void SetToggleVisuals(int index)
    {
        var isOn = toggleSwitches[index].isOn;
        SoundManager.instance.PlaySound("switch button");
        if (toggleLabels.Length > index)
        {
            toggleLabels[index].text = isOn ? "1" : "0";

            // Label 위치: Tip 반대 방향 (X는 13 고정)
            toggleLabels[index].rectTransform.anchoredPosition =
                isOn ? new Vector2(13f, -23f) : new Vector2(13f, 13f);
        }

        if (tipRects.Length > index)
            tipRects[index].anchoredPosition =
                isOn ? new Vector2(0f, 15f) : new Vector2(0f, -15f);

        UpdateResult();
    }

    private void UpdateAllToggles()
    {
        for (var i = 0; i < toggleSwitches.Length; i++) SetToggleVisuals(i); // 현재 상태 강제 반영
    }

    private void UpdateResult()
    {
        var binary = "";
        foreach (var toggle in toggleSwitches) binary += toggle.isOn ? "1" : "0";

        try
        {
            var userValue = Convert.ToInt32(binary, 2);
            resultText.text = "= " + userValue;

            if (userValue == targetNumber) Success();
        }
        catch (Exception e)
        {
            Debug.LogWarning("Binary conversion failed: " + binary + " → " + e.Message);
        }
    }

    public void Success()
    {
        successText.text = "성공입니다!";
        successText.gameObject.SetActive(true);
        PlayerData.instance.AddVariable(141);
        PlayerMovement.instance.ChangeCanMove(true);
        chatBallon.ChangeBallonText("컴파일러번역한뒤", 2);
        objToHide.SetActive(false);
        objToHide2.SetActive(false);
        StartCoroutine(ClosePuzzleAfterDelay());
    }

    private IEnumerator ClosePuzzleAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        puzzlePanel.SetActive(false);
    }
}
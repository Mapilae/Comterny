using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UISwitcher;
using System;

public class DecimalToBinaryPuzzle : MonoBehaviour
{
    public UISwitcher.UISwitcher[] toggleSwitches;         // 8개 스위치
    public TextMeshProUGUI[] toggleLabels;                  // 각 토글의 Label
    public RectTransform[] tipRects;                        // 각 토글의 Tip

    public TextMeshProUGUI decimalNumberText;               // 문제로 제시되는 십진수
    public TextMeshProUGUI resultText;                      // 플레이어 입력의 변환 결과
    public TextMeshProUGUI successText;                     // 성공 메시지
    public GameObject puzzlePanel;                          // 퍼즐 전체 패널

    private int targetNumber;

    void Start()
    {
        GenerateQuestion();

        for (int i = 0; i < toggleSwitches.Length; i++)
        {
            int index = i;
            toggleSwitches[i].onValueChanged.AddListener(_ => SetToggleVisuals(index));
        }

        UpdateAllToggles(); // 초기 상태 반영
    }

    void GenerateQuestion()
    {
        targetNumber = UnityEngine.Random.Range(1, 256); // 1~255
        decimalNumberText.text = targetNumber.ToString();
        successText.gameObject.SetActive(false);
    }

    void SetToggleVisuals(int index)
    {
        bool isOn = toggleSwitches[index].isOn;

        if (toggleLabels.Length > index)
        {
            toggleLabels[index].text = isOn ? "1" : "0";

            // Label 위치: Tip 반대 방향 (X는 13 고정)
            toggleLabels[index].rectTransform.anchoredPosition =
                isOn ? new Vector2(13f, -23f) : new Vector2(13f, 13f);
        }

        if (tipRects.Length > index)
        {
            tipRects[index].anchoredPosition =
                isOn ? new Vector2(0f, 15f) : new Vector2(0f, -15f);
        }

        UpdateResult();
    }

    void UpdateAllToggles()
    {
        for (int i = 0; i < toggleSwitches.Length; i++)
        {
            SetToggleVisuals(i); // 현재 상태 강제 반영
        }
    }

    void UpdateResult()
    {
        string binary = "";
        foreach (var toggle in toggleSwitches)
        {
            binary += toggle.isOn ? "1" : "0";
        }

        try
        {
            int userValue = Convert.ToInt32(binary, 2);
            resultText.text = "= " + userValue.ToString();

            if (userValue == targetNumber)
            {
                Success();
            }
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
        StartCoroutine(ClosePuzzleAfterDelay());
    }

    private System.Collections.IEnumerator ClosePuzzleAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        puzzlePanel.SetActive(false);
    }
}

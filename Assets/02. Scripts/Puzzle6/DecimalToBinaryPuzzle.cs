using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// ← 코루틴 사용을 위해 추가

public class DecimalToBinaryPuzzle : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI decimalNumberText;
    public Button[] answerButtons;

    public TextMeshProUGUI resultText;
    public GameObject PuzzlePanel;

    [Header("When Success")] public int varToAdd;
    public TMP_Text guideText;
    public ChatBallon chatBallon;
    public string chatBallonTextToChange;
    public int repeatTime;
    public GameObject objToDisable;

    private int correctAnswer;
    private bool isOpened;


    private void OnEnable()
    {
        GenerateQuestion();
        Pause.instance.SetCurrentPuzzle(gameObject);
    }

    private void OnDisable()
    {
        Pause.instance.SetCurrentPuzzle(null);
    }

    public void CloseTab()
    {
        PlayerMovement.instance.ChangeCanMove(true);

        gameObject.SetActive(false);
    }


    private void GenerateQuestion()
    {
        if (isOpened) return;
        isOpened = true;
        var number = Random.Range(1, 256); // 1~255
        correctAnswer = number;
        var binaryAnswer = Convert.ToString(number, 2);
        decimalNumberText.text = number.ToString();

        // 보기 배열 생성
        var choices = new string[4];
        var correctIndex = Random.Range(0, 4);
        choices[correctIndex] = binaryAnswer;

        for (var i = 0; i < 4; i++)
        {
            if (i == correctIndex) continue;
            var wrong = Random.Range(1, 256);
            while (wrong == number) wrong = Random.Range(1, 256);
            choices[i] = Convert.ToString(wrong, 2);
        }

        for (var i = 0; i < 4; i++)
        {
            var index = i; // 캡처 변수
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[i];
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => CheckAnswer(choices[index]));
        }
    }

    private void CheckAnswer(string selected)
    {
        var correct = Convert.ToString(correctAnswer, 2);
        if (selected == correct)
        {
            Debug.Log("정답!");
            resultText.text = "정답입니다!";
            Success();
        }
        else
        {
            Debug.Log("오답!");
            resultText.text = "오답입니다!";

            // 선택된 버튼을 찾아서 흔들기 실행
            for (var i = 0; i < answerButtons.Length; i++)
                if (answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text == selected)
                {
                    StartCoroutine(ShakeButton(answerButtons[i]));
                    break;
                }
        }
    }

    public void Success()
    {
        if (objToDisable != null) objToDisable.gameObject.SetActive(false);
        PlayerData.instance.AddVariable(varToAdd);
        guideText.text = "컴파일러 대리";
        PlayerMovement.instance.ChangeCanMove(true);
        if (chatBallon != null) chatBallon.ChangeBallonText(chatBallonTextToChange, repeatTime);
        StartCoroutine(ClosePuzzleAfterDelay());
    }

    // 1초 후 퀴즈 끄기
    private IEnumerator ClosePuzzleAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        PuzzlePanel.SetActive(false);
        resultText.text = ""; // 문구 초기화
    }

    // 버튼 흔들기 효과
    private IEnumerator ShakeButton(Button button)
    {
        var rect = button.GetComponent<RectTransform>();
        Vector3 originalPos = rect.anchoredPosition;

        var duration = 0.3f;
        var elapsed = 0f;
        var magnitude = 5f;

        while (elapsed < duration)
        {
            var x = Random.Range(-1f, 1f) * magnitude;
            rect.anchoredPosition = originalPos + new Vector3(x, 0, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rect.anchoredPosition = originalPos;
    }
}
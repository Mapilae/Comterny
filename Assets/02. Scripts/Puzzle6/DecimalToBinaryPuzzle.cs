using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections; // ← 코루틴 사용을 위해 추가

public class DecimalToBinaryPuzzle : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI decimalNumberText;
    public Button[] answerButtons;

    public TextMeshProUGUI resultText;
    public GameObject PuzzlePanel;

    private int correctAnswer;

    void Start()
    {
        GenerateQuestion();
    }

    void GenerateQuestion()
    {
        int number = UnityEngine.Random.Range(1, 256); // 1~255
        correctAnswer = number;
        string binaryAnswer = System.Convert.ToString(number, 2);
        decimalNumberText.text = number.ToString();

        // 보기 배열 생성
        string[] choices = new string[4];
        int correctIndex = UnityEngine.Random.Range(0, 4);
        choices[correctIndex] = binaryAnswer;

        for (int i = 0; i < 4; i++)
        {
            if (i == correctIndex) continue;
            int wrong = UnityEngine.Random.Range(1, 256);
            while (wrong == number) wrong = UnityEngine.Random.Range(1, 256);
            choices[i] = System.Convert.ToString(wrong, 2);
        }

        for (int i = 0; i < 4; i++)
        {
            int index = i; // 캡처 변수
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[i];
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => CheckAnswer(choices[index]));
        }
    }

    void CheckAnswer(string selected)
    {
        string correct = System.Convert.ToString(correctAnswer, 2);
        if (selected == correct)
        {
            Debug.Log("정답!");
            resultText.text = "정답입니다!";
            StartCoroutine(ClosePuzzleAfterDelay());
        }
        else
        {
            Debug.Log("오답!");
            resultText.text = "오답입니다!";

            // 선택된 버튼을 찾아서 흔들기 실행
            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text == selected)
                {
                    StartCoroutine(ShakeButton(answerButtons[i]));
                    break;
                }
            }
        }
    }

    // 1초 후 퀴즈 끄기
    IEnumerator ClosePuzzleAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        PuzzlePanel.SetActive(false);
        resultText.text = ""; // 문구 초기화
    }

    // 버튼 흔들기 효과
    IEnumerator ShakeButton(Button button)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        Vector3 originalPos = rect.anchoredPosition;

        float duration = 0.3f;
        float elapsed = 0f;
        float magnitude = 5f;

        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            rect.anchoredPosition = originalPos + new Vector3(x, 0, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rect.anchoredPosition = originalPos;
    }
}

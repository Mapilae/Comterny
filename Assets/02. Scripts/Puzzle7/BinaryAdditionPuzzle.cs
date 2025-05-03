using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BinaryAdditionPuzzle : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI number1Text;
    public TextMeshProUGUI number2Text;
    public UISwitcher.UISwitcher[] toggleSwitches;
    public GameObject puzzlePanel;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI[] toggleLabels;
    public RectTransform[] tipRects;
    public RectTransform[] labelRects;
    public Button submitButton;

    [Header("When Success")] public int varToAdd;
    public Collider2D colToDisable;

    private int answer;

    private void Start()
    {
        GenerateQuestion();
        submitButton.onClick.AddListener(SubmitAnswer);
    }

    private void GenerateQuestion()
    {
        int num1, num2;

        // 캐리가 반드시 발생하는 두 수를 고르기
        while (true)
        {
            num1 = Random.Range(0, 16);
            num2 = Random.Range(0, 16);
            var result = num1 + num2;

            // 덧셈 결과가 16 이상이면 캐리가 발생 (5비트 필요)
            if (result >= 16)
            {
                answer = result;
                break;
            }
        }

        titleText.text = "이진수 덧셈 퀴즈";
        questionText.text = "다음 두 이진수를 더하세요";
        number1Text.text = Convert.ToString(num1, 2).PadLeft(4, '0');
        number2Text.text = Convert.ToString(num2, 2).PadLeft(4, '0');

        for (var i = 0; i < toggleLabels.Length; i++) toggleLabels[i].text = "0";

        foreach (var toggle in toggleSwitches) toggle.isOn = false;

        resultText.text = "";
    }


    public void SubmitAnswer()
    {
        var userBinary = "";
        foreach (var toggle in toggleSwitches) userBinary += toggle.isOn ? "1" : "0";

        var userAnswer = Convert.ToInt32(userBinary, 2);
        var correctBinary = Convert.ToString(answer, 2).PadLeft(5, '0'); // 5자리로 맞추기

        if (userBinary == correctBinary)
        {
            resultText.text = "정답입니다!";
            PlayerData.instance.AddVariable(varToAdd);
            PlayerMovement.instance.ChangeCanMove(true);
            if (colToDisable != null) colToDisable.enabled = false;
            StartCoroutine(ClosePuzzleAfterDelay());
        }
        else
        {
            resultText.text = "오답입니다!";
            StartCoroutine(ShakeText(resultText)); // ✅ 흔들림 효과 추가
        }

        Debug.Log("SubmitAnswer() 호출됨");
    }

    public void SetToggleText(int index)
    {
        var isOn = toggleSwitches[index].isOn;

        toggleLabels[index].text = isOn ? "1" : "0";

        if (tipRects.Length > index)
            tipRects[index].anchoredPosition = isOn ? new Vector2(0, 15f) : new Vector2(0, -15f);

        if (labelRects.Length > index)
            labelRects[index].anchoredPosition = isOn ? new Vector2(13f, -23f) : new Vector2(13f, 13f);
    }

    public void ClosePuzzle()
    {
        puzzlePanel.SetActive(false);
        resultText.text = "";
    }

    private IEnumerator ClosePuzzleAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        puzzlePanel.SetActive(false);
    }

    // ✅ 흔들림 코루틴 추가됨
    private IEnumerator ShakeText(TextMeshProUGUI text, float duration = 0.3f, float magnitude = 5f)
    {
        var originalPos = text.rectTransform.localPosition;
        var elapsed = 0f;

        while (elapsed < duration)
        {
            var x = Random.Range(-1f, 1f) * magnitude;
            text.rectTransform.localPosition = originalPos + new Vector3(x, 0, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        text.rectTransform.localPosition = originalPos;
    }
}
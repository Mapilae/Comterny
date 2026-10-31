using TMPro;
using UnityEngine;

/// <summary>
/// 말풍선 (Boss_Chat Canvas 루트에 붙는다). 말하는 쪽이 누구든 Say만 부르면 된다.
/// - 말풍선 너비는 대사의 preferred width를 따라가고, 높이는 프리팹 값 그대로다.
/// - 일정 시간이 지나면 캔버스 오브젝트를 끈다.
/// </summary>
public class SpeechBubble : MonoBehaviour
{
    public TMP_Text text;
    [Tooltip("너비가 바뀌는 말풍선 본체 (자식 Ballon)")]
    public RectTransform balloon;
    [Tooltip("대사를 띄워두는 기본 시간(초)")]
    public float defaultDuration = 2.2f;
    [Tooltip("글자가 줄바꿈되지 않도록 preferred width에 더하는 여유")]
    public float widthPadding = 2f;
    public float minWidth = 0f;
    [Tooltip("말풍선 본체의 최대 너비. 이보다 긴 대사는 줄바꿈된다")]
    public float maxWidth = 240f;

    private float hideAt; // 처음엔 0이라 첫 Update에서 꺼진다

    /// <summary>대사를 띄운다. 말하는 중이면 대사를 바꾸고 시간을 다시 센다. duration이 음수면 defaultDuration.</summary>
    public void Say(string line, float duration = -1f)
    {
        if (string.IsNullOrEmpty(line)) return;
        hideAt = Time.time + (duration >= 0f ? duration : defaultDuration);
        gameObject.SetActive(true);
        GetComponent<Canvas>().enabled = true; // NPC 프리팹에서는 Canvas 컴포넌트를 꺼 둔 채로 저장되어 있다

        text.text = line;
        // 글자 영역은 Ballon 크기에 stretch로 묶여 있어서 sizeDelta.x만큼 차이가 난다
        float width = text.GetPreferredValues(line).x - text.rectTransform.sizeDelta.x + widthPadding;
        balloon.sizeDelta = new Vector2(Mathf.Clamp(width, minWidth, maxWidth), balloon.sizeDelta.y);
    }

    public void Hide() => gameObject.SetActive(false);

    private void Update()
    {
        if (Time.time >= hideAt) Hide();
    }
}

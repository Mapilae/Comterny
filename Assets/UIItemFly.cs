using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemFly : MonoBehaviour
{
    [Header("이동할 UI Image (RectTransform)")]
    public RectTransform itemRect;

    [Header("도착할 타겟 UI (RectTransform)")] public RectTransform targetRect;

    [Header("애니메이션 지속시간 (초)")] public float duration = 0.8f;

    [Header("곡선 제어점 높이 (월드 단위)")] public float curveHeight = 100f;

    [Header("최종 스케일")] public Vector3 endScale = Vector3.zero;

    [Header("회전 속도 (도/초)")] public float rotationSpeed = 360f;

    private Vector3 controlPoint;
    private Image image;
    private TMP_Text imgText;
    private Vector3 startScale;

    private Vector3 startWorldPos;

    private void Awake()
    {
        image = itemRect.GetComponent<Image>();
        imgText = itemRect.GetComponentInChildren<TMP_Text>(true);
        itemRect.gameObject.SetActive(false);
    }

    /// <summary>
    ///     외부에서 호출: 아이콘·이름·타겟 지정 후 실행
    /// </summary>
    public void BeginFly(Sprite icon, string itemName, RectTransform target)
    {
        targetRect = target;
        image.sprite = icon;
        imgText.text = itemName;
        itemRect.gameObject.SetActive(true);

        // 시작 시점에 매번 갱신
        startWorldPos = itemRect.position;
        startScale = itemRect.localScale;

        var endWorldPos = targetRect.position;
        // 제어점: 시작과 끝 중간 + 위쪽 오프셋
        controlPoint = (startWorldPos + endWorldPos) * 0.5f + Vector3.up * curveHeight;

        StopAllCoroutines();
        StartCoroutine(FlyRoutine(startWorldPos, endWorldPos));
    }

    private IEnumerator FlyRoutine(Vector3 p0, Vector3 p2)
    {
        var elapsed = 0f;
        //날아가기 전에 대기
        yield return new WaitForSeconds(1.2f);
        while (elapsed < duration)
        {
            var t = elapsed / duration;

            // Quadratic Bezier
            var p1 = controlPoint;
            var worldPos =
                Mathf.Pow(1 - t, 2) * p0 +
                2 * (1 - t) * t * p1 +
                Mathf.Pow(t, 2) * p2;

            itemRect.position = worldPos;
            itemRect.localScale = Vector3.Lerp(startScale, endScale, t);
            itemRect.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime, Space.Self);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 최종 보정
        itemRect.position = p2;
        itemRect.localScale = endScale;
        // 필요하면: itemRect.rotation = targetRect.rotation;

        // 애니메이션 끝났으니 비활성화
        itemRect.gameObject.SetActive(false);
    }
}
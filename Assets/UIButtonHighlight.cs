using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler,
    ISubmitHandler
{
    public Color textHighlightColor = Color.black;
    public Color textNormalColor = Color.white;

    private Button btn;
    private TMP_Text btnText;
    private Vector2 highlightScale;
    private Vector2 normalScale;

    private void Start()
    {
        btn = GetComponent<Button>();
        normalScale = transform.localScale;
        highlightScale = normalScale * 1.1f; // Highlight 시 크기를 10% 증가시킴

        if (transform.childCount > 0) btnText = transform.GetChild(0).GetComponent<TMP_Text>();
        if (btnText == null) Debug.Log(gameObject.name + " UIButtonHighlight: No TMP_Text found in child 0 objects.");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Deselect();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        BtnTextHighlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BtnTextNormal();
    }

    // ISubmitHandler
    public void OnSubmit(BaseEventData eventData)
    {
        Deselect();
    }

    private void BtnTextHighlight()
    {
        if (!btn.interactable) return;
        if (btnText != null) btnText.color = textHighlightColor;
        transform.localScale = highlightScale; // 버튼 크기 증가
    }

    private void BtnTextNormal()
    {
        if (btnText != null)
            btnText.color = textNormalColor;
        transform.localScale = normalScale; // 버튼 크기 원래대로
    }

    // 실제 선택 해제 처리
    private void Deselect()
    {
        // 1) 현재 선택된 GameObject를 null로 설정
        EventSystem.current.SetSelectedGameObject(null);
        BtnTextNormal();

        // 2) (선택적으로) 다른 버튼을 선택하고 싶다면
        //    EventSystem.current.SetSelectedGameObject(anotherButton.gameObject);
    }
}
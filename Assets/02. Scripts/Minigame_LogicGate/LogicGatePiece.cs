using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LogicGatePiece : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public LogicGateOne.GateType gateType;

    public bool isDrag;

    private Outline outline;

    private RectTransform rectTransform;
    private TMP_Text text;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        SetGateColor();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        isDrag = true;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
    }


    private void SetGateColor()
    {
        outline = GetComponent<Outline>();
        text = transform.GetChild(0).GetComponent<TMP_Text>();

        switch (gateType)
        {
            case LogicGateOne.GateType.NONE:
                outline.effectColor = Color.black;
                text.text = "-";
                break;
            case LogicGateOne.GateType.AND:
                outline.effectColor = Color.green;
                text.text = "AND";
                break;
            case LogicGateOne.GateType.OR:
                outline.effectColor = Color.yellow;
                text.text = "OR";
                break;
            case LogicGateOne.GateType.NOT:
                outline.effectColor = Color.red;
                text.text = "NOT";
                break;
        }
    }
}
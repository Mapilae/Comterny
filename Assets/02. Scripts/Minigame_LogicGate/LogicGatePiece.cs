using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LogicGatePiece : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("Item Info")] public int itemID;
    public Transform holder;
    [Header("Minigame Info")] public LogicGateMinigame minigame;

    public LogicGateOne.GateType gateType;
    public LogicGateOne logicGateOne;
    private bool isStay;

    // private Outline outline;

    private TMP_Text text;

    private void Start()
    {
        StartSetting();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("LogicGate"))
        {
            logicGateOne = other.gameObject.GetComponent<LogicGateOne>();

            isStay = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("LogicGate"))
        {
            logicGateOne = other.gameObject.GetComponent<LogicGateOne>();
            isStay = false;
            logicGateOne.LogicGatePieceChange(this, isStay);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("LogicGate"))
        {
            logicGateOne = other.gameObject.GetComponent<LogicGateOne>();

            isStay = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!LogicGateMinigame.canUse) return;
        if (transform.parent != minigame.transform) transform.parent = minigame.transform;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (logicGateOne != null) logicGateOne.LogicGatePieceChange(this, isStay);
        // minigame.CheckGates();
    }

    private void StartSetting()
    {
        transform.parent = minigame.transform;
        transform.parent = holder;
        SetGateColor();
    }


    private void SetGateColor()
    {
        //outline = GetComponent<Outline>();
        text = transform.GetChild(0).GetComponent<TMP_Text>();

        switch (gateType)
        {
            case LogicGateOne.GateType.NONE:
                //outline.effectColor = Color.black;
                text.text = "-";
                break;
            case LogicGateOne.GateType.AND:
                // outline.effectColor = Color.green;
                text.text = "AND";
                break;
            case LogicGateOne.GateType.OR:
                //outline.effectColor = Color.yellow;
                text.text = "OR";
                break;
            case LogicGateOne.GateType.NOT:
                //outline.effectColor = Color.red;
                text.text = "NOT";
                break;
        }
    }
}
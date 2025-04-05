using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogicGateOne : MonoBehaviour
{
    [Serializable]
    public enum GateType
    {
        NONE,
        AND,
        OR,
        NOT
    }

    public GateType gateType;
    public bool isForNot;
    public LogicGateWire[] inputGate;

    public LogicGateWire output;
    public bool isStay;

    public LogicGateMinigame logicGateMinigame;
    public Image wire;
    private bool isIn;
    private LogicGatePiece logicGatePiece;

    private Outline outline;
    private int passCount;
    private TMP_Text text;

    private void Start()
    {
        SetGateColor();
    }


    private void Update()
    {
        if (logicGatePiece == null) return;
        if (logicGatePiece.isDrag || !isStay) return;
        if (logicGatePiece.gateType.Equals(GateType.NOT) ^ isForNot) return;

        gateType = logicGatePiece.gateType;
        logicGatePiece.transform.position = transform.position;
        SetGateColor();
        isStay = false;
        //logicGateMinigame.StartTest();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("LogicGatePiece"))
        {
            logicGatePiece = other.gameObject.GetComponent<LogicGatePiece>();
            isStay = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("LogicGatePiece"))
        {
            isStay = false;
            gateType = GateType.NONE;
            SetGateColor();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("LogicGatePiece"))
        {
            logicGatePiece = other.gameObject.GetComponent<LogicGatePiece>();
            isStay = true;
        }
    }

    private void SetGateColor()
    {
        Debug.Log("check: " + name);
        outline = GetComponent<Outline>();
        text = transform.GetChild(0).GetComponent<TMP_Text>();


        switch (gateType)
        {
            case GateType.NONE:
                outline.effectColor = Color.black;
                text.text = "-";

                break;
            case GateType.AND:
                outline.effectColor = Color.green;
                text.text = "AND";
                break;
            case GateType.OR:
                outline.effectColor = Color.yellow;
                text.text = "OR";
                break;
            case GateType.NOT:
                outline.effectColor = Color.red;
                text.text = "NOT";
                break;
        }

        //output.ResetWire();
        // PassGate();
    }

    public void PassGate()
    {
        if (passCount != inputGate.Length - 1)
        {
            passCount++;
            return;
        }

        bool value = true;

        switch (gateType)
        {
            case GateType.NONE:

                value = isForNot ? inputGate[0].value : false;
                break;
            case GateType.AND:

                value = inputGate[0].value && inputGate[1];
                break;
            case GateType.OR:

                value = inputGate[0].value || inputGate[1];
                break;
            case GateType.NOT:

                value = !inputGate[0].value;
                break;
        }

        wire.color = value ? Color.yellow : Color.white;

        wire.enabled = isForNot && gateType == GateType.NONE;

        output.value = value;
        output.PassWire();
    }
}
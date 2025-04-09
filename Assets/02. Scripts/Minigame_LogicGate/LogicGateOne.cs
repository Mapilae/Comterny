using System;
using System.Collections;
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
    public LogicGateOne[] preGates;

    public LogicGateWire output;

    public LogicGateMinigame logicGateMinigame;
    public Image wire;
    public LogicGatePiece logicGatePiece;
    public int isConnected;
    private Image backImg; //OR게이트일때 hide되면서 전선 보이게함
    private bool isCoroutineRunning;

    private bool isPlaced;

    //  private Outline outline;
    private int passCount;
    private TMP_Text text;

    private void Start()
    {
        isConnected = -1;
        SetGateColor(gateType);
    }

    public void LogicGatePieceChange(LogicGatePiece piece, bool isEnter)
    {
        if (piece == null)
        {
            Debug.Log("Piece is null!");

            return;
        }

        if (isEnter && !isPlaced)
        {
            //isForNot인 경우, 
            if (isForNot)
            {
                if (piece.gateType == GateType.NOT) SetPiecePosition(piece);
                else return;
            }
            else
            {
                if (piece.gateType == GateType.NOT) return;
                SetPiecePosition(piece);
            }
        }
        else if (!isEnter)
        {
            if (piece != logicGatePiece) return;
            //나가는 경우
            piece.logicGateOne = null;
            logicGatePiece = null;
            isPlaced = false;
            isConnected = -1;
            SetGateColorAndPass(GateType.NONE);
            wire.transform.parent.gameObject.SetActive(isForNot);
            wire.enabled = isForNot;
            for (int i = 0; i < output.wireImg.Count; i++) output.wireImg[i].color = Color.white;
        }
    }

    private void SetPiecePosition(LogicGatePiece piece)
    {
        logicGatePiece = piece;
        isPlaced = true;
        piece.gameObject.transform.position = transform.position;
        isConnected = 1;

        SetGateColorAndPass(logicGatePiece.gateType);
    }

    private void SetGateColor(GateType newGateType)
    {
        //outline = GetComponent<Outline>();
        text = transform.GetChild(0).GetComponent<TMP_Text>();
        backImg = transform.GetChild(1).GetComponent<Image>();
        gateType = newGateType;
        backImg.enabled = true;
        switch (gateType)
        {
            case GateType.NONE:
                //outline.effectColor = Color.black;
                text.text = "-";

                break;
            case GateType.AND:
                //outline.effectColor = Color.green;
                text.text = "AND";
                break;
            case GateType.OR:
                backImg.enabled = false;
                //outline.effectColor = Color.yellow;
                text.text = "OR";
                break;
            case GateType.NOT:
                //outline.effectColor = Color.red;
                text.text = "NOT";
                break;
        }

        GetComponent<Image>().enabled = gateType == GateType.NONE;
    }


    private void SetGateColorAndPass(GateType newGateType)
    {
        SetGateColor(newGateType);
        logicGateMinigame.StartTest();
        //logicGateMinigame.StartTest();
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

                value = isForNot && inputGate[0].value;
                break;
            case GateType.AND:

                value = inputGate[0].value && inputGate[1].value;
                break;
            case GateType.OR:

                value = inputGate[0].value || inputGate[1].value;
                break;
            case GateType.NOT:

                //value = logicGateMinigame.CheckGates() && inputGate[0].value;
                value = CheckPregates() && !inputGate[0].value;

                break;
        }

        if (isCoroutineRunning) StopCoroutine("ThisWireDelay");
        StartCoroutine("ThisWireDelay", value);
    }

    public bool CheckPregates()
    {
        if (preGates.Length == 0) return true;
        bool isTrue = true;
        for (int i = 0; i < preGates.Length; i++)
            if (preGates[i].isConnected.Equals(-1))
            {
                isTrue = false;
                break;
            }

        return isTrue;
    }

    private IEnumerator ThisWireDelay(bool value)
    {
        isCoroutineRunning = true;
        wire.color = value ? Color.yellow : Color.white;
        bool isActive = (isForNot && gateType == GateType.NONE) || !isConnected.Equals(1);
        wire.transform.parent.gameObject.SetActive(isActive);
        wire.enabled = isForNot;

        if (isActive)
        {
            if (value)
            {
                wire.fillAmount = 0;
                float time = 0;

                while (time < 1)
                {
                    float add = Time.deltaTime;
                    time += add * 2f;
                    wire.fillAmount = time;
                    yield return new WaitForSeconds(add);
                }
            }
            else
            {
                wire.fillAmount = 1;
            }
        }

        yield return new WaitForSeconds(0.1f);
        output.PassWire(value);
        isCoroutineRunning = false;
    }

    public void EndPass()
    {
        wire.transform.parent.gameObject.SetActive(true);
        backImg.enabled = false;
        GetComponent<Image>().enabled = false;
        output.EndPass();
    }
}
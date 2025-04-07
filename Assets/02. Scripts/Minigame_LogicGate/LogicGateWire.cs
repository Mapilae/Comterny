using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicGateWire : MonoBehaviour
{
    [Serializable]
    public enum WireType
    {
        WIRE,
        INPUT,
        OUTPUT
    }

    public WireType wireType;
    public bool value;
    public LogicGateOne nextGate;

    [Header("If Output F")] public LogicGateMinigame logicGateMinigame;
    public List<Image> wireImg;

    private void Start()
    {
        GetWireImage();
    }

    public void GetWireImage()
    {
        wireImg.Add(GetComponent<Image>());
        if (transform.childCount > 0)
            for (int i = 0; i < transform.childCount; i++)
            {
                Image img = transform.GetChild(i).GetComponent<Image>();
                if (img != null)
                    wireImg.Add(img);
            }
    }

    public void PassWire()
    {
        SetWire();
        if (wireType != WireType.OUTPUT) nextGate.PassGate();
        else logicGateMinigame.EndTest(value);
    }

    private void SetWire()
    {
        if (wireImg.Count == 0) GetWireImage();
        for (int i = 0; i < wireImg.Count; i++)
            wireImg[i].color = value ? Color.yellow : Color.white;
    }

    public void ResetWire()
    {
        value = false;
        SetWire();
    }
}
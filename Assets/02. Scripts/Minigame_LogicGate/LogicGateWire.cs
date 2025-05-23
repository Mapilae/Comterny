using System;
using System.Collections;
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

    public void GetWireImage()
    {
        if (wireImg.Count != 0) return;

        if (transform.childCount > 1)
            for (var i = 1; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).childCount == 0) Debug.Log(transform.name + " dont have child");

                var img = transform.GetChild(i).GetChild(0).GetComponent<Image>();
                if (img != null)
                    wireImg.Add(img);
            }

        wireImg.Add(transform.GetChild(0).GetComponent<Image>());
    }

    public void PassWire(bool newValue)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(PassWireDelay(newValue));
    }

    private void SetWire()
    {
        if (wireImg.Count == 0) GetWireImage();
        for (var i = 0; i < wireImg.Count; i++)
        {
            wireImg[i].fillAmount = 0;
            wireImg[i].color = value ? Color.yellow : Color.white;
        }
    }

    private IEnumerator PassWireDelay(bool newValue)
    {
        value = newValue;
        SetWire();

        if (newValue)
        {
            for (var i = 0; i < wireImg.Count; i++)
            {
                float time = 0;

                while (time < 1)
                {
                    var add = Time.deltaTime;
                    time += add * 2f;
                    wireImg[i].fillAmount = time;
                    yield return new WaitForSeconds(add);
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

        if (wireType != WireType.OUTPUT)
            nextGate.PassGate();
        else
            logicGateMinigame.EndTest(value);

        yield return null;
    }

    public void EndPass()
    {
        if (wireType != WireType.OUTPUT) nextGate.EndPass();
        else return;
    }
}
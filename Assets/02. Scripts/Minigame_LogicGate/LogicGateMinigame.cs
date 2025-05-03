using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LogicGateMinigame : MonoBehaviour
{
    public static bool canUse = true;
    public int varToAdd = 124;
    public GameObject thisWhole;

    public LogicGateWire[] inputs;

    [Header("After")] public Volume volume;
    public VolumeProfile volumeProfile;
    public GameObject[] objToDisable;
    public GameObject[] objToEnable;
    public Collider2D colToDisable;
    public FanSpinning fanSpin;

    // public List<LogicGateOne> gates = new();
    public bool[] inputValues;

    [Header("Item ID List")] public List<LogicGatePiece> itemIDList;

    private bool isConnectedAll;
    private PlayerData playerData;


    private void Start()
    {
        playerData = GameObject.FindGameObjectWithTag("PlayerData").GetComponent<PlayerData>();
        /*GameObject[] objs = GameObject.FindGameObjectsWithTag("LogicGate");

        for (int i = 0; i < objs.Length; i++)
        {
            LogicGateOne gate = objs[i].GetComponent<LogicGateOne>();
            if (!gate.isForNot) gates.Add(gate);
        }*/
    }

    private void OnEnable()
    {
        StartTest();
        PlayerMovement.canMove = false;
        Inventory.instance.anything = gameObject;
        Pause.instance.SetCurrentPuzzle(gameObject);
    }

    public void CloseThis()
    {
        PlayerMovement.canMove = true;
        Inventory.instance.anything = null;
        Pause.instance.SetCurrentPuzzle(null);
        StopAllCoroutines();
        thisWhole.SetActive(false);
    }

    public void StartTest()
    {
        for (var i = 0; i < inputs.Length; i++) inputs[i].PassWire(false);

        for (var i = 0; i < inputs.Length; i++)
            inputs[i].PassWire(inputValues[i]);
    }

    public void EndTest(bool result)
    {
        if (!result) return;
        playerData.AddVariable(varToAdd);
        //---만약 스킵한 경우 대비---
        playerData.RemoveVariable(10); //소지중인 AND게이트1번 삭제
        playerData.RemoveVariable(11); //AND게이트2번 삭제
        playerData.RemoveVariable(12); //OR게이트 삭제
        playerData.RemoveVariable(13); //NOT게이트 삭제
        //--------
        canUse = false;
        volume.profile = volumeProfile;

        if (objToDisable.Length > 0)
            for (var i = 0; i < objToDisable.Length; i++)
                objToDisable[i].SetActive(false);
        if (objToEnable.Length > 0)
            for (var i = 0; i < objToEnable.Length; i++)
                objToEnable[i].SetActive(true);
        if (colToDisable != null)
            colToDisable.enabled = false;

        if (fanSpin != null) fanSpin.PlaySpinPan(true);
        Invoke("CloseThis", 3f);
        //for (int i = 0; i < inputs.Length; i++) inputs[i].EndPass();
    }

    /*
    public bool CheckGates()
    {
        return true;
        bool isConnectedAll = true;
        for (int i = 0; i < gates.Count; i++)
            if (gates[i].isConnected.Equals(-1))
            {
                isConnectedAll = false;
                break;
            }

        this.isConnectedAll = isConnectedAll;
        return isConnectedAll;
    }*/


    public void InventoryToReal(int invItemID)
    {
        for (var i = 0; i < itemIDList.Count; i++)
            if (itemIDList[i].itemID.Equals(invItemID))
            {
                itemIDList[i].gameObject.SetActive(true);
                Debug.Log(itemIDList[i].itemID + " set true");
                PieceEnabledAnimation(itemIDList[i].GetComponent<RectTransform>());
                playerData.RemoveVariable(invItemID); //소지중인 게이트 삭제
                PlayerMovement.instance.UnholdItem();


                break;
            }
    }

    private void PieceEnabledAnimation(RectTransform rect)
    {
        StartCoroutine(IEPieceEnabledAnimation(rect));
    }

    private IEnumerator IEPieceEnabledAnimation(RectTransform rect)
    {
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * 5f;
            rect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time);
            yield return null;
        }

        rect.localScale = Vector3.one;
        yield return null;
    }
}
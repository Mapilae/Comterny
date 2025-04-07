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
    public GameObject objToDisable;
    public FanSpinning fanSpin;

    // public List<LogicGateOne> gates = new();
    public bool[] inputValues;
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
    }

    public void CloseThis()
    {
        PlayerMovement.canMove = true;

        thisWhole.SetActive(false);
    }

    public void StartTest()
    {
        for (int i = 0; i < inputs.Length; i++) inputs[i].PassWire(false);

        for (int i = 0; i < inputs.Length; i++)
            inputs[i].PassWire(inputValues[i]);
    }

    public void EndTest(bool result)
    {
        if (!result) return;
        playerData.AddVariable(varToAdd);
        canUse = false;
        volume.profile = volumeProfile;

        if (objToDisable != null) objToDisable.SetActive(false);
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
}
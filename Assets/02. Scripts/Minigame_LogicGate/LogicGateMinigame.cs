using UnityEngine;

public class LogicGateMinigame : MonoBehaviour
{
    public GameObject thisWhole;
    public LogicGateWire[] inputs;
    public bool[] inputValues;


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

        for (int i = 0; i < inputs.Length; i++) inputs[i].PassWire(inputValues[i]);
    }

    public void EndTest(bool result)
    {
        Debug.Log("EndTest : " + result);
    }
}
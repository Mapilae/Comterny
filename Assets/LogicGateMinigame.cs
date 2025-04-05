using UnityEngine;

public class LogicGateMinigame : MonoBehaviour
{
    public LogicGateWire[] inputs;
    public bool[] inputValues;


    private void Start()
    {
        StartTest();
    }

    public void StartTest()
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i].value = inputValues[i];
            inputs[i].ResetWire();
        }

        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i].value = inputValues[i];
            inputs[i].PassWire();
        }
    }

    public void EndTest(bool result)
    {
        Debug.Log("EndTest : " + result);
    }
}
using System.Collections;
using UnityEngine;

public class FanSpinning : MonoBehaviour
{
    public Transform fanObj;


    public void StartSpin()
    {
        GetComponent<Animator>().SetTrigger("StartSpin");
    }

    public void PlaySpinFan()
    {
        //Animation Trigger
        PlaySpinPan(true);
    }

    private void PlaySpinPan(bool isSpin)
    {
        if (!isSpin)
        {
            StopCoroutine(SpinFan());
            return;
        }

        if (fanObj != null)
            StartCoroutine(SpinFan());
    }

    private IEnumerator SpinFan()
    {
        GetComponent<Animator>().enabled = false;
        while (true)
        {
            fanObj.Rotate(Vector3.forward * 1000f * Time.deltaTime);
            yield return null;
        }
    }
}
using System.Collections;
using UnityEngine;

public class FanSpinning : MonoBehaviour
{
    public Transform fanObj;

    public void PlaySpinPan(bool isSpin)
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
        while (true)
        {
            fanObj.Rotate(Vector3.forward * 1000f * Time.deltaTime);
            yield return null;
        }
    }
}
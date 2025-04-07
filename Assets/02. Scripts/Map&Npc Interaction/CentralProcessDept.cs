using UnityEngine;
using UnityEngine.Rendering;

public class CentralProcessDept : MonoBehaviour
{
    public GameObject beforeMap;
    public GameObject afterMap;
    public Volume volume;
    public VolumeProfile[] volumeProfiles;

    public void ChangeBeforeAfterState(bool isAfter)
    {
        beforeMap.SetActive(!isAfter);
        afterMap.SetActive(isAfter);
        volume.profile = volumeProfiles[!isAfter ? 0 : 1];
    }

    public void AfterPowerOn()
    {
        ChangeBeforeAfterState(true);
    }
}
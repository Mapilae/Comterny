using UnityEngine;

public class MobileUICheck : MonoBehaviour
{
    
    void Start()
    {
        #if UNITY_WEBGL
        this.GetComponent<Canvas>().enabled = true;
#else
        this.GetComponent<Canvas>().enabled = false;
#endif
    }

    public void CheckMobile(bool isUseJoy)
    {
        MobileUI.isJoyOn = isUseJoy;
        MobileUI.instance.mobileOnlyObj.SetActive(isUseJoy);
        MobileUI.instance.pcObj.SetActive(true);
        this.GetComponent<Canvas>().enabled = false;
    }
}

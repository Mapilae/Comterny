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
        this.GetComponent<Canvas>().enabled = false;

        // MainMenu has Mobile Canvas inactive, so MobileUI.instance can be null here.
        // Later scenes read isJoyOn in MobileUI.Start().
        var ui = MobileUI.instance;
        if (ui == null) return;
        if (ui.mobileOnlyObj != null) ui.mobileOnlyObj.SetActive(isUseJoy);
        if (ui.pcObj != null) ui.pcObj.SetActive(true);
    }
}

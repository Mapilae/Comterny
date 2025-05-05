using UnityEngine;

public class UI_Setting : MonoBehaviour
{
    public void OnClickSettingButton()
    {
        Pause.instance.OpenPauseMenu();
    }
}
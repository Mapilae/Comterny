using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Setting : MonoBehaviour
{
    [SerializeField] private Button settingButton;

    private void Start()
    {
        settingButton.onClick.AddListener(() =>
        {
            // 버튼 클릭 후 선택 해제
            EventSystem.current.SetSelectedGameObject(null);
        });
    }

    public void OnClickSettingButton()
    {
        Pause.instance.OpenPauseMenu();
    }
}
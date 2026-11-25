using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button loadButton;
    public TMP_Text loadText;

    private bool isStarting;
    private SaveFile save;

    private void Start()
    {
        save = SaveSystem.Load();

        if (save == null)
        {
            loadButton.interactable = false;
            loadText.text = "";
        }
        else
        {
            loadButton.interactable = true;
            loadText.text = $"{save.playerName}, {SaveSystem.GetStageLabel(save)}";
        }

        // 씬의 이어하기 버튼 OnClick이 비어 있어도 동작하도록 코드에서 연결한다.
        // 인스펙터에서도 연결돼 두 번 불리더라도 isStarting으로 한 번만 실행된다.
        loadButton.onClick.AddListener(LoadDataAndStartGame);
    }

    public void LoadDataAndStartGame()
    {
        //on button click
        if (isStarting || save == null) return;
        isStarting = SaveSystem.ContinueGame();
    }
}

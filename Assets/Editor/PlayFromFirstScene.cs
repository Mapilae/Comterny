using UnityEditor;
using UnityEditor.SceneManagement;

// Play 버튼을 누르면 현재 열린 씬과 관계없이 Build Settings 의 첫 번째 씬(MainMenu)부터 시작
[InitializeOnLoad]
public class PlayFromFirstScene
{
    private const string PREF_KEY = "PlayFromFirstScene_Enabled"; // 설정 저장 키
    private const string MENU_PATH = "Tools/Play From First Scene";

    static PlayFromFirstScene()
    {
        EditorApplication.delayCall += Apply; // 에디터 로드가 끝난 뒤 적용
    }

    private static void Apply()
    {
        bool isEnabled = EditorPrefs.GetBool(PREF_KEY, true);
        Menu.SetChecked(MENU_PATH, isEnabled);

        if (!isEnabled || EditorBuildSettings.scenes.Length == 0)
        {
            EditorSceneManager.playModeStartScene = null;
            return;
        }

        string firstScenePath = EditorBuildSettings.scenes[0].path;
        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(firstScenePath);
    }

    [MenuItem(MENU_PATH)]
    public static void Toggle()
    {
        EditorPrefs.SetBool(PREF_KEY, !EditorPrefs.GetBool(PREF_KEY, true));
        Apply();
    }
}

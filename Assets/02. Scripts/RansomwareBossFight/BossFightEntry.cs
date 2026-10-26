using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 본편 씬 → 랜섬웨어 보스전 씬 입장 스크립트.
///
/// 사용법 (셋 중 아무거나):
///  1) 본편 씬에 빈 오브젝트 + Collider2D(Is Trigger) + 이 컴포넌트 → Player 태그가 닿으면 입장
///     (requireSpace 켜면 범위 안에서 Space를 눌러야 입장)
///  2) 버튼/UnityEvent/다이얼로그 연출에서 Enter() 호출
///  3) 코드 어디서든 BossFightEntry.EnterBossFight("RansomwareBoss", "다음씬")
///  ※ 다이얼로그 명령 movescene 에 "RansomwareBoss" 를 넣어도 그대로 입장된다 (이 경우 클리어 후 씬은 보스 씬 인스펙터 값).
///
/// 이름/난이도는 PlayerData(DontDestroyOnLoad)를 통해 보스 씬으로 그대로 넘어간다.
/// </summary>
public class BossFightEntry : MonoBehaviour
{
    public const string DefaultBossScene = "RansomwareBoss";

    [Header("입장")]
    [Tooltip("빌드 설정에 등록된 보스 씬 이름")]
    public string bossSceneName = DefaultBossScene;
    [Tooltip("Player 태그가 트리거에 닿으면 입장")]
    public bool enterOnTrigger = true;
    [Tooltip("트리거 안에서 Space(모바일: 상호작용 버튼)를 눌러야 입장")]
    public bool requireSpace;
    [Tooltip("입장 시 PlayerData.crtStage 를 이 값으로 바꾼다 (-1 = 안 바꿈)")]
    public int setStageOnEnter = -1;

    [Header("클리어 후")]
    [Tooltip("보스 클리어 후 이동할 씬. 비우면 보스 씬 인스펙터의 nextSceneOnClear 사용")]
    public string nextSceneOnClear = "";

    /// <summary>보스 클리어 후 갈 씬 (입장 시 세팅)</summary>
    public static string PendingNextScene { get; private set; }
    /// <summary>보스전으로 들어오기 직전 씬</summary>
    public static string ReturnScene { get; private set; }
    /// <summary>이번 플레이에서 보스를 깼는지</summary>
    public static bool LastFightCleared { get; private set; }

    private bool playerInside;
    private bool entering;

    public void Enter()
    {
        if (entering) return;
        entering = true;
        if (setStageOnEnter >= 0 && PlayerData.instance != null) PlayerData.instance.ChangeCrtStage(setStageOnEnter);
        EnterBossFight(bossSceneName, nextSceneOnClear);
    }

    public static void EnterBossFight(string bossScene = DefaultBossScene, string nextScene = "")
    {
        ReturnScene = SceneManager.GetActiveScene().name;
        PendingNextScene = nextScene;
        LastFightCleared = false;

        // 본편 플레이어 멈춤 (로딩 중 움직이지 않게)
        if (PlayerMovement.instance != null) PlayerMovement.instance.ChangeCanMove(false);

        Debug.Log($"[BossFightEntry] {ReturnScene} → {bossScene} (클리어 후: {(string.IsNullOrEmpty(nextScene) ? "보스 씬 설정" : nextScene)}, 이름: {PlayerData.playerName})");
        LoadScene(bossScene);
    }

    /// <summary>보스 씬에서 나갈 때 (RansomwareBossFight가 클리어 후 호출)</summary>
    public static void ExitBossFight(string sceneName, bool cleared)
    {
        LastFightCleared = cleared;
        PendingNextScene = null;
        PlayerMovement.canMove = true; // static: 다음 씬 플레이어가 멈춘 채로 시작하지 않도록
        Debug.Log($"[BossFightEntry] 보스전 종료 (cleared={cleared}) → {sceneName}");
        LoadScene(sceneName);
    }

    /// <summary>본편 로딩 화면(LodingScene)이 있으면 그걸로, 없으면 바로 로드</summary>
    public static void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"[BossFightEntry] 씬 '{sceneName}' 이(가) 빌드 설정에 없습니다. File > Build Profiles 에 추가하세요.");
            return;
        }

        var loader = LodingScene.instance;
        if (loader != null)
        {
            // 이전 씬에서 DontDestroyOnLoad로 남은 로딩 오브젝트는 로딩 후 비활성화돼 있으므로 다시 켠다
            if (!loader.gameObject.activeSelf) loader.gameObject.SetActive(true);
            loader.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private void Update()
    {
        if (!enterOnTrigger || !requireSpace || !playerInside || entering) return;
        if (Input.GetKeyDown(KeyCode.Space) || PlayerMovement.isSpace)
        {
            PlayerMovement.isSpace = false;
            Enter();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enterOnTrigger || !other.CompareTag("Player")) return;
        playerInside = true;
        if (!requireSpace) Enter();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInside = false;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///     세이브 슬롯 1개의 내용. JsonUtility로 직렬화해서 PlayerPrefs 키 하나에 저장한다.
/// </summary>
[Serializable]
public class SaveFile
{
    public int version;
    public string playerName;
    public int difficulty;
    public int crtStage;
    public string sceneName; // 이어하기 시 불러올 씬

    // 스테이지에 들어온 순간의 아이템·변수. 중간 진행을 복원할 수 없을 때(씬이 수정됨 등) 여기서 다시 시작한다.
    public List<int> items = new();
    public List<int> variables = new();

    public bool skippedPuzzle; // Pause.isSkippedOnce (도전과제 판정용)
    public string savedAt; // 표시용 저장 시각

    // 스테이지 중간 진행 상황. JsonUtility는 null을 저장하지 못하므로 hasProgress로 유무를 구분한다.
    public bool hasProgress;
    public StageProgress progress = new();
}

/// <summary>
///     세이브/로드(이어하기) 시스템. 슬롯 1개, 자동 저장.
///     게임 시작 시 자동으로 생성되어 DontDestroyOnLoad로 유지된다. 씬에 배치할 필요 없음.
///     <para>
///         저장 시점: 스테이지 씬에 들어올 때(= 스테이지 이동), 타이틀로 돌아갈 때, 게임 종료 시,
///         모바일/웹에서 앱이 백그라운드로 가거나 포커스를 잃을 때, 그리고 플레이 중 10초마다(내용이 바뀐 경우만).
///     </para>
///     <para>
///         스테이지 중간 진행: 대사·컷신·퍼즐·연출이 진행 중이지 않은 "안정된 순간"마다 씬 상태를 기억해 두고(SceneStateTracker),
///         저장할 때 그것을 함께 기록한다. 대사 도중에 종료하면 그 대사를 시작하기 직전 상태로 이어진다.
///         보스전은 중간 저장 없이 보스전 처음부터 다시 시작한다.
///     </para>
///     저장소: PlayerPrefs (PC 레지스트리/plist, Android/iOS 기본 저장소, WebGL IndexedDB).
/// </summary>
public class SaveSystem : MonoBehaviour
{
    public const string TitleSceneName = "MainMenu";
    public const string DefaultPlayerName = "유저 디버그";

    private const string SaveKey = "Comterny.Save";
    private const int FormatVersion = 2;

    // 패리티 비트 미니게임 클리어 변수. CPUWarp.canWarp(static)가 이 변수와 함께 켜진다.
    private const int ParitySolvedVariable = 135;

    private const float SettleSeconds = 1f; // 안정된 상태가 이만큼 이어져야 기억한다 (대사 종료 직후 연출 등이 끝나도록)
    private const float CaptureInterval = 1f;
    private const float WriteInterval = 10f;
    private const float RestoreGuardSeconds = 0.5f; // 복원 직후 제자리 트리거로 자동 대사 등이 다시 실행되지 않게 막는 시간

    // 예전 SaveData가 쓰던 키. 더 이상 읽지 않고, 세이브 삭제 시 같이 지운다.
    private static readonly string[] LegacyKeys =
        { "IsDataExists", "PlayerName", "CrtStage", "Difficulty", "Items", "Variables" };

    // 스테이지 번호(crtStage) ↔ 씬 이름 ↔ 표시 이름
    private static readonly (int stage, string scene, string label)[] Stages =
    {
        (0, "ComternyEntry", "컴터니 입성"),
        (1, "MemoryDept", "주기억부서"),
        (2, "BusStop", "시스템버스정류장"),
        (3, "CentralProcessingDept", "중앙처리부서"),
        (4, "SystemSoftware", "시스템소프트웨어부서"),
        (5, "GoToOS", "운영체제부서 가는 길"),
        (6, "OperatingSystem", "운영체제부서"),
        (7, "RansomwareBoss", "랜섬웨어 보스전")
    };

    // 진행 중인 연출(벌레 비행, 컷신 마무리, 스택 블록 이동 등). 하나라도 있으면 씬 상태를 기억하지 않는다.
    private static readonly HashSet<Behaviour> busy = new();

    private static SaveSystem runner;

    // 현재 스테이지에 들어온 순간의 상태 + 저장 정보. 저장할 때 progress를 붙여서 쓴다.
    private static SaveFile checkpoint;

    // 현재 스테이지 씬의 오브젝트 목록 (타이틀/보스전/세션 밖에서는 null)
    private static SceneStateTracker tracker;

    // 현재 스테이지에서 마지막으로 기억한 안정된 상태 (없으면 스테이지 처음부터)
    private static StageProgress progress;

    // 이어하기로 불러올 중간 진행. 씬이 로드되면 적용된다.
    private static StageProgress pendingRestore;

    // 같은 내용을 반복해서 디스크에 쓰지 않기 위한 비교용 (savedAt 제외)
    private static string lastWrittenJson;

    private float nextCaptureTime;
    private float nextWriteTime;
    private float stableTime;

    /// <summary>
    ///     새로 시작/이어하기로 게임에 들어온 상태. 타이틀에 돌아오면 false.
    ///     에디터에서 스테이지 씬을 바로 실행해 테스트할 때는 false라서 실제 세이브를 덮어쓰지 않는다.
    /// </summary>
    public static bool IsSessionActive { get; private set; }

    /// <summary>
    ///     이어하기로 씬 상태를 되돌리는 중. 이 동안에는 자동 대사·자동 문·컷신 등이 새로 시작되지 않아야 한다.
    /// </summary>
    public static bool IsRestoring { get; private set; }

    /// <summary>
    ///     진행 중인 연출이 있어 지금 상태를 저장하면 안 될 때 true로 등록하고, 끝나면 false로 해제한다.
    ///     오브젝트가 꺼지거나 파괴되면 자동으로 해제된 것으로 본다.
    /// </summary>
    public static void SetBusy(Behaviour owner, bool isBusy)
    {
        if (owner == null) return;
        if (isBusy) busy.Add(owner);
        else busy.Remove(owner);
    }

    #region 앱 수명주기 (자동 생성된 오브젝트에서 호출됨)

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticsOnPlay()
    {
        // 에디터에서 Domain Reload를 끈 경우에도 이전 플레이의 상태가 남지 않게
        IsSessionActive = false;
        IsRestoring = false;
        checkpoint = null;
        tracker = null;
        progress = null;
        pendingRestore = null;
        lastWrittenJson = null;
        busy.Clear();
        SceneStateTracker.ClearSpriteCache();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        var go = new GameObject("[SaveSystem]");
        DontDestroyOnLoad(go);
        runner = go.AddComponent<SaveSystem>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 모바일: 홈 버튼/앱 전환 시. iOS/Android는 이후 종료 콜백 없이 프로세스가 죽을 수 있다.
    private void OnApplicationPause(bool paused)
    {
        if (paused) SaveCheckpoint();
    }

    // WebGL 탭 전환, PC 창 전환
    private void OnApplicationFocus(bool focused)
    {
        if (!focused) SaveCheckpoint();
    }

    private void OnApplicationQuit()
    {
        SaveCheckpoint();
    }

    // 안정된 순간마다 씬 상태를 기억하고, 주기적으로 디스크에 쓴다 (WebGL은 탭을 닫을 때 종료 콜백이 오지 않는다)
    private void LateUpdate()
    {
        if (!IsSessionActive || tracker == null || IsRestoring || !IsStable())
        {
            stableTime = 0f;
            return;
        }

        stableTime += Time.unscaledDeltaTime;
        if (stableTime < SettleSeconds) return;

        var now = Time.unscaledTime;
        if (now >= nextCaptureTime)
        {
            nextCaptureTime = now + CaptureInterval;
            CaptureProgress();
        }

        if (now >= nextWriteTime)
        {
            nextWriteTime = now + WriteInterval;
            WriteCurrent(false); // 주기 저장은 로그를 남기지 않는다
        }
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode != LoadSceneMode.Single) return;

        // 이전 씬의 기록은 버린다
        tracker = null;
        progress = null;
        IsRestoring = false;
        SceneStateTracker.ClearSpriteCache();

        if (scene.name == TitleSceneName)
        {
            OnTitleLoaded();
            return;
        }

        if (!IsSessionActive) return;

        var restore = pendingRestore;
        pendingRestore = null;
        if (restore != null && restore.sceneName == scene.name)
            BeginRestore(scene, restore);
        else
            EnterStage(scene);
    }

    #endregion

    #region 게임 흐름

    /// <summary>새로 시작: 이전 플레이 데이터를 비운다. 기존 세이브는 첫 스테이지에 들어갈 때 덮어써진다.</summary>
    public static void StartNewGame()
    {
        if (PlayerData.instance != null) PlayerData.instance.ResetForNewGame();
        Pause.isSkippedOnce = false;
        checkpoint = null;
        progress = null;
        pendingRestore = null;
        IsSessionActive = true;
    }

    /// <summary>이어하기: 세이브를 PlayerData에 적용하고 저장된 스테이지를 불러온다.</summary>
    public static bool ContinueGame()
    {
        var data = Load();
        if (data == null || PlayerData.instance == null) return false;

        var sceneName = ResolveScene(data);
        var resume = data.hasProgress && data.progress != null && data.progress.sceneName == sceneName;

        PlayerData.instance.ApplySave(data);
        // 중간 진행이 있으면 그 시점의 아이템·변수로 씬을 연다 (씬 오브젝트들이 Awake/Start에서 참고함)
        if (resume) PlayerData.instance.ApplyProgress(data.progress.items, data.progress.variables);

        Pause.isSkippedOnce = data.skippedPuzzle;
        checkpoint = data;
        progress = null;
        pendingRestore = resume ? data.progress : null;
        IsSessionActive = true;

        Debug.Log($"[SaveSystem] 이어하기 → {sceneName} ({GetStageLabel(data)}{(resume ? ", 중간부터" : "")})");
        LoadingScene.Load(sceneName);
        return true;
    }

    /// <summary>
    ///     지금까지의 진행을 디스크에 저장. 타이틀 복귀/게임 종료/앱 전환 시 호출.
    ///     지금이 안정된 순간이면 현재 상태를, 대사·컷신 도중이면 그 직전에 기억한 상태를 저장한다.
    /// </summary>
    public static void SaveCheckpoint()
    {
        if (!IsSessionActive || checkpoint == null) return;
        if (runner != null && tracker != null && !IsRestoring && runner.stableTime >= SettleSeconds && IsStable())
            CaptureProgress();
        WriteCurrent();
    }

    // 스테이지 씬에 새로 들어옴 (씬 오브젝트들의 Awake 이후, Start 이전).
    // 이 시점의 아이템·변수가 이전 스테이지에서 그대로 들고 온 "스테이지 시작 상태"다.
    private static void EnterStage(Scene scene)
    {
        var pd = PlayerData.instance;
        if (pd == null)
        {
            Debug.LogWarning($"[SaveSystem] PlayerData가 없어 '{scene.name}' 진입을 저장하지 못했습니다.");
            return;
        }

        var stage = GetStageForScene(scene.name);
        if (stage >= 0) pd.crtStage = stage;

        checkpoint = new SaveFile
        {
            playerName = PlayerData.playerName,
            difficulty = pd.difficulty,
            crtStage = pd.crtStage,
            sceneName = scene.name,
            items = new List<int>(pd.items ?? new List<int>()),
            variables = new List<int>(pd.variables ?? new List<int>()),
            skippedPuzzle = Pause.isSkippedOnce
        };
        progress = null;
        tracker = CreateTracker(scene);
        UpdateDerivedStatics();
        WriteCurrent();
    }

    private static SceneStateTracker CreateTracker(Scene scene)
    {
        return SceneStateTracker.IsBossScene(scene) ? null : new SceneStateTracker(scene);
    }

    // 이어하기로 들어온 스테이지에 저장된 중간 상태를 적용한다. (씬 오브젝트들의 Awake 이후, Start 이전)
    private static void BeginRestore(Scene scene, StageProgress restore)
    {
        var t = CreateTracker(scene);
        if (t == null || !t.Matches(restore))
        {
            // 업데이트로 씬이 수정되면 오브젝트 번호가 달라져 안전하게 되돌릴 수 없다 → 스테이지 처음부터
            Debug.LogWarning($"[SaveSystem] '{scene.name}' 씬 구성이 저장 당시와 달라 중간 진행을 복원하지 못했습니다. 스테이지 처음부터 시작합니다.");
            if (PlayerData.instance != null) PlayerData.instance.ApplySave(checkpoint);
            EnterStage(scene);
            return;
        }

        tracker = t;
        progress = restore; // 바로 종료해도 같은 상태가 다시 저장되도록
        IsRestoring = true;
        UpdateDerivedStatics();

        // 맵 타이틀 카드와 그 뒤에 나오는 인트로 대사는 이미 본 것이므로 띄우지 않는다
        if (MapOveray.instance != null) MapOveray.instance.gameObject.SetActive(false);
        MapOveray.isOFF = false;

        // Start 전에 꺼 둬야 처음에 켜져 있던 인트로 컷신/연출이 시작되지 않는다
        t.ApplyObjects(restore);
        ResetControlStatics();

        runner.StartCoroutine(FinishRestore(t, restore));
    }

    private static IEnumerator FinishRestore(SceneStateTracker t, StageProgress restore)
    {
        // 씬 오브젝트들의 Start가 모두 끝난 뒤 다시 맞춘다 (Start에서 상태를 초기화하는 스크립트가 있음)
        yield return null;
        yield return null;
        if (tracker != t) yield break; // 그 사이 다른 씬으로 이동함

        t.ApplyComponents(restore);
        t.ApplyObjects(restore);
        t.ApplyAnimators(restore);
        if (QuestManager.instance != null) QuestManager.instance.RestoreQuests(restore.quests);

        CameraFollow.doFollowPlayer = restore.cameraFollowsPlayer;
        UpdateDerivedStatics();
        ResetControlStatics();

        try
        {
            // 들고 있던 아이템 표시는 새 씬의 인벤토리와 맞지 않으므로 내려놓은 상태로 시작
            if (Inventory.instance != null)
            {
                Inventory.instance.UpdateInventory();
                Inventory.instance.UnholdItem();
            }

            if (MobileUI.instance != null) MobileUI.instance.HideThis(false);
            if (CameraFollow.instance != null) CameraFollow.instance.ExcludeLayerInCamera(false);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        Debug.Log($"[SaveSystem] '{restore.sceneName}' 중간 진행 복원 완료");

        yield return new WaitForSecondsRealtime(RestoreGuardSeconds);
        if (tracker == t) IsRestoring = false;
    }

    private static void OnTitleLoaded()
    {
        IsSessionActive = false;
        IsRestoring = false;
        checkpoint = null;
        tracker = null;
        progress = null;
        pendingRestore = null;
        busy.Clear();
        ResetGameplayStatics();
    }

    /// <summary>
    ///     씬이 바뀌어도 남는 static 상태를 초기화한다.
    ///     대사/컷신/일시정지 도중에 타이틀로 나간 뒤 이어하기를 하면 이 값들이 남아 조작이 막힐 수 있다.
    /// </summary>
    private static void ResetGameplayStatics()
    {
        ResetControlStatics();
        MapOveray.isOFF = false;
        CameraFollow.doFollowPlayer = true;
        CPUWarp.canWarp = false;
        LogicGateMinigame.canUse = true;
    }

    private static void ResetControlStatics()
    {
        PlayerMovement.canMove = true;
        PlayerMovement.isCutscene = false;
        PlayerMovement.isSpace = false;
        DialogueManager.isPlayingDialogue = false;
        Pause.isPaused = false;
    }

    /// <summary>
    ///     변수에서 다시 계산할 수 있는 static 상태. 원래는 이전 스테이지에서 켜진 값이 그대로 이어지는데,
    ///     이어하기로 바로 들어오면 그 과정이 없으므로 여기서 맞춘다.
    ///     (예: 중앙처리부서에서 패리티 비트를 풀면 켜지는 워프를 시스템소프트웨어부서에서도 사용)
    /// </summary>
    private static void UpdateDerivedStatics()
    {
        var pd = PlayerData.instance;
        if (pd == null) return;
        CPUWarp.canWarp = pd.IsVariableExist(ParitySolvedVariable);
    }

    #endregion

    #region 중간 진행 기억

    /// <summary>대사·컷신·퍼즐·연출 없이 플레이어가 자유롭게 움직일 수 있는 상태인지</summary>
    private static bool IsStable()
    {
        if (tracker == null || SceneManager.GetActiveScene() != tracker.scene) return false;
        if (Pause.isPaused || LoadingScene.IsLoading) return false;

        var pm = PlayerMovement.instance;
        if (pm == null || !pm.isActiveAndEnabled) return false;
        if (!PlayerMovement.canMove || PlayerMovement.isCutscene || DialogueManager.isPlayingDialogue) return false;

        if (Pause.instance != null && Pause.instance.currentPuzzle != null &&
            Pause.instance.currentPuzzle.activeInHierarchy) return false;
        if (Inventory.instance != null && Inventory.instance.anything != null &&
            Inventory.instance.anything.activeInHierarchy) return false; // 휴지통/논리게이트 창
        if (MapOveray.instance != null && MapOveray.instance.isActiveAndEnabled) return false; // 맵 타이틀 카드

        busy.RemoveWhere(b => b == null || !b.isActiveAndEnabled);
        return busy.Count == 0;
    }

    private static void CaptureProgress()
    {
        var pd = PlayerData.instance;
        if (tracker == null || pd == null) return;

        try
        {
            var p = tracker.Capture();
            p.items = new List<int>(pd.items ?? new List<int>());
            p.variables = new List<int>(pd.variables ?? new List<int>());
            if (QuestManager.instance != null) p.quests = QuestManager.instance.CaptureQuests();
            p.cameraFollowsPlayer = CameraFollow.doFollowPlayer;
            progress = p;
        }
        catch (Exception ex)
        {
            // 저장 실패가 게임 진행을 막으면 안 된다. 이전에 기억한 상태를 그대로 쓴다.
            Debug.LogException(ex);
        }
    }

    private static void WriteCurrent(bool log = true)
    {
        if (!IsSessionActive || checkpoint == null) return;
        checkpoint.skippedPuzzle |= Pause.isSkippedOnce; // 한 번이라도 스킵했다는 기록은 되돌리지 않는다
        checkpoint.hasProgress = progress != null;
        checkpoint.progress = progress ?? new StageProgress();
        Write(checkpoint, log);
    }

    #endregion

    #region 저장소

    public static bool HasSave()
    {
        return Load() != null;
    }

    /// <summary>저장된 데이터를 읽는다. 없거나 손상됐거나 불러올 씬이 없으면 null.</summary>
    public static SaveFile Load()
    {
        var json = PlayerPrefs.GetString(SaveKey, "");
        if (string.IsNullOrEmpty(json)) return null;

        SaveFile data;
        try
        {
            data = JsonUtility.FromJson<SaveFile>(json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SaveSystem] 세이브 데이터를 읽지 못했습니다: {e.Message}\n{json}");
            return null;
        }

        if (data == null) return null;
        data.items ??= new List<int>();
        data.variables ??= new List<int>();
        data.progress ??= new StageProgress();
        data.progress.items ??= new List<int>();
        data.progress.variables ??= new List<int>();
        data.progress.transforms ??= new List<TransformState>();
        data.progress.sprites ??= new List<SpriteState>();
        data.progress.animators ??= new List<AnimatorState>();
        data.progress.components ??= new List<ComponentState>();
        data.progress.quests ??= new List<QuestState>();
        if (data.version < 2) data.hasProgress = false;

        if (string.IsNullOrEmpty(ResolveScene(data)))
        {
            Debug.LogWarning($"[SaveSystem] 저장된 씬 '{data.sceneName}'(stage {data.crtStage})을 빌드 설정에서 찾을 수 없습니다.");
            return null;
        }

        return data;
    }

    public static void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        foreach (var key in LegacyKeys) PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
        checkpoint = null;
        progress = null;
        lastWrittenJson = null;
        Debug.Log("[SaveSystem] 로컬 세이브 데이터를 삭제했습니다.");
    }

    private static void Write(SaveFile data, bool log = true)
    {
        data.version = FormatVersion;

        var prevSavedAt = data.savedAt;
        data.savedAt = null;
        var compare = JsonUtility.ToJson(data);
        if (compare == lastWrittenJson)
        {
            data.savedAt = prevSavedAt;
            return;
        }

        data.savedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save(); // 모바일/웹은 종료 콜백이 오지 않을 수 있으므로 바로 디스크에 쓴다
        lastWrittenJson = compare;

        if (log)
            Debug.Log($"[SaveSystem] 저장됨: {data.playerName}, {data.sceneName}(stage {data.crtStage})" +
                  (data.hasProgress ? $", 중간 진행 포함(바뀐 위치 {data.progress.transforms.Count}개)" : ", 스테이지 시작 지점") +
                  $", items=[{string.Join(",", data.hasProgress ? data.progress.items : data.items)}]");
    }

    #endregion

    #region 스테이지 정보

    /// <summary>세이브에서 불러올 씬. 씬 이름이 빌드에 없으면 스테이지 번호로 찾는다.</summary>
    public static string ResolveScene(SaveFile data)
    {
        if (!string.IsNullOrEmpty(data.sceneName) && data.sceneName != TitleSceneName &&
            Application.CanStreamedLevelBeLoaded(data.sceneName))
            return data.sceneName;

        var byStage = GetSceneNameForStage(data.crtStage);
        if (!string.IsNullOrEmpty(byStage) && Application.CanStreamedLevelBeLoaded(byStage))
            return byStage;

        return null;
    }

    public static string GetSceneNameForStage(int stage)
    {
        foreach (var s in Stages)
            if (s.stage == stage)
                return s.scene;
        return null;
    }

    public static int GetStageForScene(string sceneName)
    {
        foreach (var s in Stages)
            if (s.scene == sceneName)
                return s.stage;
        return -1;
    }

    public static string GetStageLabel(SaveFile data)
    {
        foreach (var s in Stages)
            if (s.scene == data.sceneName)
                return s.label;
        foreach (var s in Stages)
            if (s.stage == data.crtStage)
                return s.label;
        return data.sceneName;
    }

    #endregion

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Save Data/Print")]
    private static void EditorPrintSave()
    {
        var json = PlayerPrefs.GetString(SaveKey, "");
        Debug.Log(string.IsNullOrEmpty(json) ? "[SaveSystem] 저장된 데이터 없음" : $"[SaveSystem] {json}");
    }

    [UnityEditor.MenuItem("Tools/Save Data/Delete")]
    private static void EditorDeleteSave()
    {
        DeleteSave();
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// [Scene 6. Puzzle 9. 포인터] 운영체제 부서 회장실 앞 방화벽 - 랜섬웨어 보스전 (프로토타입)
///
/// - 랜섬웨어가 탄막을 쏜다. 플레이어 생명력은 3칸:
///   렘 → 레0x124521 → ㄹ0x13452 → 한 번 더 맞으면 게임 오버.
/// - 맵에 생기는 복호화 아이템을 먹으면 한 단계 회복한다.
/// - 탄막을 피하면서 코드 조각(*(fw+i))을 주워 주소가 맞는 무너진 방화벽 자리(0x....)에 끼운다.
///   방화벽을 모두 복구하면 랜섬웨어가 도망갈 곳이 없어져 검거 → 클리어.
///
/// 빈 씬에 이 컴포넌트 하나만 두면 카메라/아레나/플레이어/보스/HUD를 전부 런타임에 만든다.
/// </summary>
public class RansomwareBossFight : MonoBehaviour
{
    [Header("리소스 (비워두면 기본 도형/폰트 사용)")]
    public TMP_FontAsset font;
    [Tooltip("Spine 캐릭터가 없을 때만 쓰는 정지 이미지")]
    public Sprite playerSprite;
    public Sprite bossSprite;
    public AudioClip bgm;

    [Header("플레이어 캐릭터 (본편 Player 프리팹과 같은 Spine)")]
    [Tooltip("User Debug/새 폴더/skeleton_SkeletonData - 본편 Player 프리팹이 쓰는 것과 동일")]
    public SkeletonDataAsset playerSkeleton;
    public float playerSpineScale = 1.3f;
    public Vector2 playerSpineOffset = Vector2.zero;
    public string idleAnim = "idle", walkAnim = "walk", runAnim = "run";

    [Header("본편 연결")]
    [Tooltip("클리어 후 이동할 씬 (BossFightEntry로 들어오면 그쪽 설정이 우선). 비우면 그 자리에서 R로 다시하기")]
    public string nextSceneOnClear = "";
    [Tooltip("클리어 후 자동으로 넘어가기까지 시간 (Space/Enter로 바로 넘어감)")]
    public float clearAutoContinue = 5f;

    [Header("테스트")]
    [Tooltip("PlayerData가 없는 테스트 씬(보스 씬 단독 실행)에서 쓸 이름")]
    public string testPlayerName = "유저 디버그";
    [Tooltip("컴공모드: 방화벽 자리에 (fw+i) 힌트를 숨긴다. PlayerData.difficulty==1 이면 자동으로 켜짐")]
    public bool hardMode;

    [Header("아레나 / 플레이어")]
    [Tooltip("세로로 긴 방 (아이작 메가 사탄 방처럼 보스는 맨 위에 고정)")]
    public Vector2 arenaHalfSize = new(5.5f, 9f);
    public float playerSpeed = 5f;
    public float playerHitRadius = 0.16f;
    public float invincibleTime = 2f;

    [Header("카메라 (버스정류장 씬처럼 플레이어를 따라 세로 스크롤)")]
    public float cameraOrthoSize = 4f;
    public float cameraFollowSmooth = 0.12f;

    [Header("퍼즐")]
    [Range(2, 6)] public int fragmentCount = 4;
    public float pickupRange = 0.6f;
    public float slotRange = 0.95f;

    [Header("복호화 아이템")]
    public float decryptSpawnInterval = 5f;
    public float decryptLifeTime = 10f;

    [Header("보스")]
    public float bossContactRadius = 0.55f;

    [Header("디버그 (테스트용 단축키)")]
    [Tooltip("F1: 조각 하나 자동 배치 / F2: 무적 토글 / F3: 피격 테스트")]
    public bool debugKeys = true;
    public bool godMode;

    [Header("탄막 난이도 (1 = 기본, 낮을수록 쉬움)")]
    [Range(0.3f, 2f)] public float bulletSpeedScale = 1f;
    [Range(0.3f, 2f)] public float bulletCountScale = 1f;
    [Range(0.5f, 3f)] public float patternRestScale = 1f;

    [Header("이벤트 (본편 씬 연결용)")]
    public UnityEvent onClear;
    public UnityEvent onGameOver;

    private enum FightState { Intro, Fight, Clear, GameOver }

    private class Bullet
    {
        public Vector2 pos, vel;
        public float radius;
        public SpriteRenderer sr;
        public bool alive;
    }

    private class Laser
    {
        public Vector2 a, b;
        public float width;
        public bool active;
        public SpriteRenderer sr;
    }

    private class Fragment
    {
        public int index;          // fw+index
        public Vector2 pos;
        public bool carried, placed;
        public Transform tr;
        public TextMeshPro label;
    }

    private class Slot
    {
        public int index;          // 이 자리의 주소 = baseAddr + index*4
        public Vector2 pos;
        public bool horizontal;    // 위/아래 벽이면 true
        public bool filled;
        public SpriteRenderer sr;
        public TextMeshPro label;
    }

    // ---- 런타임 상태 ----
    private FightState state;
    private Transform root;
    private Camera cam;
    private Vector2 camFollow, camVel;
    private readonly List<(Transform target, TextMeshPro marker)> offscreenMarkers = new();
    private float shakeTime;
    private Sprite squareSprite, circleSprite;
    private Material spriteMat;

    private Transform player;
    private SpriteRenderer playerSr;
    private SkeletonAnimation playerSpine;
    private string playerAnim;
    private float clearAt;
    private bool leaving;
    private TextMeshPro playerNameLabel, carryLabel;
    private Vector2 playerPos;
    private float invincibleUntil;
    private int corruption;          // 0~2
    private string baseName, shownName;
    private Fragment carrying;

    private Transform boss;
    private SpriteRenderer bossSr;
    private Vector2 bossPos, bossHome;

    private readonly List<Bullet> bullets = new();
    private readonly List<Laser> lasers = new();
    private readonly List<Fragment> fragments = new();
    private readonly List<Slot> slots = new();
    private int baseAddr;
    private int filledCount;
    private float wrongSlotCooldown;

    private Transform decryptItem;
    private float decryptSpawnTimer, decryptDespawnAt;

    private TextMeshPro hpText, progressText, hintText, msgText, subMsgText, helpText;
    private float msgUntil;
    private AudioSource bgmSource;

    private Rect Arena => new(-arenaHalfSize, arenaHalfSize * 2f);

    // =====================================================================
    #region 초기화
    private void Start()
    {
        baseName = ResolvePlayerName();
        if (PlayerData.instance != null && PlayerData.instance.difficulty == 1) hardMode = true;
        Debug.Log($"[RansomwareBossFight] player name = '{baseName}' (PlayerData {(PlayerData.instance != null ? "연결됨" : "없음 → 테스트 이름")})");

        ResolveFont();
        CreateBaseSprites();
        root = new GameObject("[BossFight Runtime]").transform;

        SetupCamera();
        BuildArena();
        BuildPuzzle();
        BuildPlayer();
        BuildBoss();
        BuildHud();
        camFollow = CameraTarget();
        LateUpdate();

        if (bgm != null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.clip = bgm;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        StartCoroutine(IntroRoutine());
    }

    /// <summary>
    /// 본편에서 넘어오면 PlayerData(DontDestroyOnLoad)에 저장된 이름(이름 입력 화면/세이브)을 그대로 쓴다.
    /// 보스 씬만 단독으로 틀면 PlayerData가 없으므로 testPlayerName.
    /// </summary>
    private string ResolvePlayerName()
    {
        string n = PlayerData.instance != null ? PlayerData.playerName : testPlayerName;
        if (string.IsNullOrWhiteSpace(n)) n = PlayerData.playerName; // static 기본값 "유저 디버그"
        if (string.IsNullOrWhiteSpace(n)) n = "유저 디버그";
        return n.Trim();
    }

    private void ResolveFont()
    {
#if UNITY_EDITOR
        if (font == null)
            font = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/DungGeunMo SDF.asset");
#endif
        if (font == null) font = TMP_Settings.defaultFontAsset;
    }

    private void CreateBaseSprites()
    {
        var sq = new Texture2D(4, 4) { filterMode = FilterMode.Point };
        var px = new Color[16];
        for (int i = 0; i < px.Length; i++) px[i] = Color.white;
        sq.SetPixels(px);
        sq.Apply();
        squareSprite = Sprite.Create(sq, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);

        const int n = 64;
        var circ = new Texture2D(n, n) { filterMode = FilterMode.Bilinear };
        var cpx = new Color[n * n];
        for (int y = 0; y < n; y++)
        for (int x = 0; x < n; x++)
        {
            float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(n / 2f, n / 2f)) / (n / 2f);
            float a = Mathf.Clamp01((1f - d) * 8f);              // 부드러운 가장자리
            float core = d < 0.55f ? 1f : Mathf.Lerp(1f, 0.75f, (d - 0.55f) / 0.45f);
            cpx[y * n + x] = new Color(core, core, core, a);
        }
        circ.SetPixels(cpx);
        circ.Apply();
        circleSprite = Sprite.Create(circ, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), n);

        var sh = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (sh == null) sh = Shader.Find("Sprites/Default");
        if (sh != null) spriteMat = new Material(sh);
    }

    private void SetupCamera()
    {
        cam = Camera.main;
        if (cam == null)
        {
            var go = new GameObject("Main Camera") { tag = "MainCamera" };
            cam = go.AddComponent<Camera>();
            go.AddComponent<AudioListener>();
        }
        cam.orthographic = true;
        cam.orthographicSize = cameraOrthoSize;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.02f, 0.03f, 0.07f);
        cam.transform.position = new Vector3(0f, 0.25f, -10f);

    }
    #endregion

    // =====================================================================
    #region 생성 헬퍼
    private SpriteRenderer MakeSprite(string name, Sprite s, Vector2 pos, Vector2 size, Color c, int order, Transform parent = null)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent != null ? parent : root, false);
        go.transform.position = pos;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = s;
        if (spriteMat != null) sr.sharedMaterial = spriteMat;
        sr.color = c;
        sr.sortingOrder = order;
        Vector2 b = s.bounds.size;
        go.transform.localScale = new Vector3(size.x / b.x, size.y / b.y, 1f);
        return sr;
    }

    private TextMeshPro MakeText(string name, string text, Vector2 pos, float size, Color c, int order,
        Transform parent = null, TextAlignmentOptions align = TextAlignmentOptions.Center)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent != null ? parent : root, false);
        go.transform.position = pos;
        var t = go.AddComponent<TextMeshPro>();
        if (font != null) t.font = font;
        t.text = text;
        t.fontSize = size;
        t.color = c;
        t.alignment = align;
        t.textWrappingMode = TextWrappingModes.NoWrap;
        t.richText = true;
        t.rectTransform.sizeDelta = new Vector2(18f, 1.5f);
        t.sortingOrder = order;
        return t;
    }
    #endregion

    // =====================================================================
    #region 아레나 / 퍼즐
    private static readonly Color WallColor = new(1f, 0.42f, 0.18f);
    private static readonly Color FixedColor = new(0.3f, 0.95f, 1f);
    private static readonly Color BrokenColor = new(1f, 0.15f, 0.3f);
    private const float WallThick = 0.3f;
    private const float GapWidth = 1.7f;

    private void BuildArena()
    {
        var r = Arena;
        MakeSprite("Floor", squareSprite, Vector2.zero, r.size, new Color(0.06f, 0.08f, 0.14f), 0);

        // 회로 느낌의 격자
        var grid = new Color(0.12f, 0.2f, 0.32f, 0.35f);
        for (float x = Mathf.Ceil(r.xMin); x < r.xMax; x += 1f)
            MakeSprite("GridV", squareSprite, new Vector2(x, 0), new Vector2(0.02f, r.height), grid, 1);
        for (float y = Mathf.Ceil(r.yMin); y < r.yMax; y += 1f)
            MakeSprite("GridH", squareSprite, new Vector2(0, y), new Vector2(r.width, 0.02f), grid, 1);
    }

    private void BuildPuzzle()
    {
        var r = Arena;
        // 무너진 방화벽 자리 후보 (위/아래 벽 4곳 + 좌/우 벽 2곳)
        // 무너진 방화벽 자리 후보: 좌우 벽을 따라 위아래로 흩어져 있음 (+ 맨 아래 벽)
        var spots = new List<(Vector2 pos, bool horizontal)>
        {
            (new Vector2(r.xMin, r.yMax - r.height * 0.38f), false),
            (new Vector2(r.xMax, r.yMax - r.height * 0.55f), false),
            (new Vector2(r.xMin, r.yMax - r.height * 0.75f), false),
            (new Vector2(r.xMax, r.yMax - r.height * 0.88f), false),
            (new Vector2(r.xMax, r.yMax - r.height * 0.22f), false),
            (new Vector2(r.xMin, r.yMax - r.height * 0.58f), false),
        };
        int count = Mathf.Clamp(fragmentCount, 2, spots.Count);
        spots = spots.GetRange(0, count);

        // 주소 순서를 섞어서 배치 → 위치만 보고는 못 맞히게
        var order = new List<int>();
        for (int i = 0; i < count; i++) order.Add(i);
        Shuffle(order);

        baseAddr = 0x1000 * Random.Range(1, 15);

        for (int i = 0; i < count; i++)
        {
            var (pos, horizontal) = spots[i];
            var slot = new Slot { index = order[i], pos = pos, horizontal = horizontal };
            Vector2 size = horizontal ? new Vector2(GapWidth, WallThick) : new Vector2(WallThick, GapWidth);
            slot.sr = MakeSprite($"Slot_{slot.index}", squareSprite, pos, size, BrokenColor, 6);

            Vector2 inward = horizontal ? new Vector2(0, pos.y > 0 ? -0.55f : 0.55f) : new Vector2(pos.x > 0 ? -1.05f : 1.05f, 0);
            slot.label = MakeText($"SlotLabel_{slot.index}", SlotLabel(slot), pos + inward, 3.0f, BrokenColor, 20);
            slots.Add(slot);
        }

        BuildWalls();
        MakeText("CEO Room", "<color=#FC6>▲ 회장실</color>", new Vector2(0f, r.yMax + 0.5f), 2.6f, Color.white, 20);

        // 코드 조각: 아레나 안쪽 무작위 위치 (보스/플레이어 시작점 근처는 피함)
        for (int i = 0; i < count; i++)
        {
            Vector2 p = RandomInteriorPoint(1.2f, avoid: new[] { PlayerStart, BossSpot }, avoidRadius: 2.4f);
            var f = new Fragment { index = i, pos = p };
            var sr = MakeSprite($"Fragment_{i}", squareSprite, p, new Vector2(1.25f, 0.5f), new Color(0.1f, 0.35f, 0.15f, 0.95f), 8);
            f.tr = sr.transform;
            f.label = MakeText("Code", FragmentCode(i), p, 2.4f, new Color(0.55f, 1f, 0.55f), 9);
            f.label.transform.SetParent(f.tr, true);
            fragments.Add(f);
        }
    }

    private string SlotLabel(Slot s)
    {
        string addr = $"0x{baseAddr + s.index * 4:X4}";
        if (s.filled) return $"<color=#7FF>{FragmentCode(s.index)}</color>";
        return hardMode ? addr : $"{addr}\n<size=70%>(fw+?)</size>";
    }

    private static string FragmentCode(int i) => $"*(fw+{i})";

    /// <summary>테두리 벽을 슬롯 자리만 비우고 그린다.</summary>
    private void BuildWalls()
    {
        var r = Arena;
        DrawEdge(new Vector2(r.xMin, r.yMax), new Vector2(r.xMax, r.yMax), true);
        DrawEdge(new Vector2(r.xMin, r.yMin), new Vector2(r.xMax, r.yMin), true);
        DrawEdge(new Vector2(r.xMin, r.yMin), new Vector2(r.xMin, r.yMax), false);
        DrawEdge(new Vector2(r.xMax, r.yMin), new Vector2(r.xMax, r.yMax), false);
    }

    private void DrawEdge(Vector2 a, Vector2 b, bool horizontal)
    {
        float from = horizontal ? a.x : a.y;
        float to = horizontal ? b.x : b.y;
        float fixedCoord = horizontal ? a.y : a.x;
        var gaps = new List<float>();
        foreach (var s in slots)
        {
            if (s.horizontal != horizontal) continue;
            float sc = horizontal ? s.pos.y : s.pos.x;
            if (Mathf.Abs(sc - fixedCoord) < 0.01f) gaps.Add(horizontal ? s.pos.x : s.pos.y);
        }
        gaps.Sort();

        float cursor = from - WallThick * 0.5f;
        float end = to + WallThick * 0.5f;
        foreach (float g in gaps)
        {
            AddWallPiece(cursor, g - GapWidth * 0.5f, fixedCoord, horizontal);
            cursor = g + GapWidth * 0.5f;
        }
        AddWallPiece(cursor, end, fixedCoord, horizontal);
    }

    private void AddWallPiece(float from, float to, float fixedCoord, bool horizontal)
    {
        float len = to - from;
        if (len <= 0.01f) return;
        float mid = (from + to) * 0.5f;
        Vector2 pos = horizontal ? new Vector2(mid, fixedCoord) : new Vector2(fixedCoord, mid);
        Vector2 size = horizontal ? new Vector2(len, WallThick) : new Vector2(WallThick, len);
        MakeSprite("Firewall", squareSprite, pos, size, WallColor, 5);
    }

    private Vector2 RandomInteriorPoint(float margin, Vector2[] avoid = null, float avoidRadius = 0f)
    {
        var r = Arena;
        for (int tries = 0; tries < 60; tries++)
        {
            var p = new Vector2(Random.Range(r.xMin + margin, r.xMax - margin), Random.Range(r.yMin + margin, r.yMax - margin));
            bool ok = true;
            if (avoid != null)
                foreach (var a in avoid)
                    if (Vector2.Distance(a, p) < avoidRadius) { ok = false; break; }
            foreach (var f in fragments)
                if (Vector2.Distance(f.pos, p) < 1.6f) { ok = false; break; }
            if (ok) return p;
        }
        return Vector2.zero;
    }

    private Vector2 RandomPointIn(Rect r, float margin, Vector2[] avoid = null, float avoidRadius = 0f)
    {
        for (int tries = 0; tries < 40; tries++)
        {
            var p = new Vector2(Random.Range(r.xMin + margin, r.xMax - margin), Random.Range(r.yMin + margin, r.yMax - margin));
            bool ok = true;
            if (avoid != null)
                foreach (var a in avoid)
                    if (Vector2.Distance(a, p) < avoidRadius) { ok = false; break; }
            if (ok) return p;
        }
        return r.center;
    }

    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
    #endregion

    // =====================================================================
    #region 플레이어 / 보스 / HUD 생성
    private void BuildPlayer()
    {
        playerPos = PlayerStart;
        var sr = MakeSprite("Player", playerSprite != null ? playerSprite : circleSprite, playerPos,
            playerSprite != null ? new Vector2(0.95f, 0.95f) : new Vector2(0.6f, 0.6f),
            playerSprite != null ? Color.white : new Color(0.6f, 0.9f, 1f), 12);
        player = sr.transform;
        playerSr = sr;

        if (playerSkeleton != null)
        {
            // 본편 Player 프리팹과 같은 Spine 캐릭터 (idle / walk / run 모션)
            playerSpine = SkeletonAnimation.NewSkeletonAnimationGameObject(playerSkeleton);
            playerSpine.name = "PlayerSpine";
            var st = playerSpine.transform;
            st.SetParent(player, false);
            // 부모(Player 스프라이트)의 스케일 영향 제거
            var ps = player.lossyScale;
            st.localScale = new Vector3(playerSpineScale / ps.x, playerSpineScale / ps.y, 1f);
            st.localPosition = new Vector3(playerSpineOffset.x / ps.x, playerSpineOffset.y / ps.y, 0f);
            var mr = playerSpine.GetComponent<MeshRenderer>();
            mr.sortingOrder = 12;
            sr.enabled = false; // 판정/위치용으로만 남김
            SetPlayerAnim(idleAnim);
        }

        // 판정점(작은 코어) 표시 - 탄막 게임처럼 실제 피격 범위를 보여준다
        var core = MakeSprite("HitCore", circleSprite, playerPos, Vector2.one * playerHitRadius * 2f, new Color(1f, 1f, 1f, 0.9f), 13);
        core.transform.SetParent(player, true);

        playerNameLabel = MakeText("NameLabel", baseName, playerPos + new Vector2(0, 0.75f), 2.8f, Color.white, 21);
        carryLabel = MakeText("CarryLabel", "", playerPos + new Vector2(0, 1.1f), 2.4f, new Color(0.55f, 1f, 0.55f), 21);
        shownName = baseName;
    }

    private Vector2 PlayerStart => new(0f, -arenaHalfSize.y + 1.5f);
    private Vector2 BossSpot => new(0f, arenaHalfSize.y - 1.9f);

    private void BuildBoss()
    {
        bossHome = BossSpot;
        bossPos = bossHome;
        bossSr = MakeSprite("Ransomware", bossSprite != null ? bossSprite : circleSprite, bossPos,
            bossSprite != null ? new Vector2(1.7f, 1.7f) : new Vector2(1.2f, 1.2f),
            bossSprite != null ? Color.white : new Color(0.9f, 0.2f, 0.5f), 10);
        boss = bossSr.transform;
        var bossLabel = MakeText("BossName", "<color=#F5A>RANSOMWARE</color>", bossPos + new Vector2(0, 1.15f), 2.4f, Color.white, 21);
        bossLabel.transform.SetParent(boss, true);
    }

    private TextMeshPro MakeHudText(string name, string text, Vector2 local, float size, Color c, int order,
        TextAlignmentOptions align = TextAlignmentOptions.Center)
    {
        var t = MakeText(name, text, Vector2.zero, size, c, order, cam.transform, align);
        t.transform.localPosition = new Vector3(local.x, local.y, 10f);
        return t;
    }

    private void BuildHud()
    {
        float halfH = cam.orthographicSize, halfW = halfH * cam.aspect;
        float top = halfH - 0.35f;

        // HUD 뒷배경 띠 (스크롤되는 맵 위에서도 글자가 잘 보이도록)
        foreach (float by in new[] { halfH - 0.35f, -halfH + 0.25f })
        {
            var band = MakeSprite("HUD_Band", squareSprite, Vector2.zero, new Vector2(halfW * 2f + 1f, 0.7f), new Color(0.01f, 0.02f, 0.05f, 0.8f), 29, cam.transform);
            band.transform.localPosition = new Vector3(0f, by, 10f);
        }
        hpText = MakeHudText("HUD_HP", "", new Vector2(-halfW + 0.3f + 9f, top), 3.2f, Color.white, 30, TextAlignmentOptions.Left);
        hintText = MakeHudText("HUD_Hint", $"int* fw = 0x{baseAddr:X4};  <size=80%>// sizeof(int)=4</size>",
            new Vector2(0f, top), 3f, new Color(0.75f, 0.9f, 1f), 30);
        progressText = MakeHudText("HUD_Progress", "", new Vector2(halfW - 0.3f - 9f, top), 3.2f, Color.white, 30, TextAlignmentOptions.Right);
        helpText = MakeHudText("HUD_Help", "WASD/방향키 이동   Shift 천천히   R 재시작" + (debugKeys ? "   <color=#556>[F1 조각자동 F2 무적 F3 피격]</color>" : ""),
            new Vector2(0f, -halfH + 0.25f), 2.2f, new Color(0.6f, 0.65f, 0.75f), 30);
        msgText = MakeHudText("HUD_Msg", "", new Vector2(0f, 0.6f), 5.5f, Color.white, 40);
        subMsgText = MakeHudText("HUD_SubMsg", "", new Vector2(0f, -0.5f), 3f, new Color(0.85f, 0.85f, 0.9f), 40);
        msgText.rectTransform.sizeDelta = new Vector2(18f, 3f);
        subMsgText.rectTransform.sizeDelta = new Vector2(18f, 3f);

        // 화면 밖에 있는 조각(초록)/구멍(빨강) 방향 표시
        foreach (var f in fragments)
            offscreenMarkers.Add((f.tr, MakeHudText("Marker", "", Vector2.zero, 3.4f, new Color(0.55f, 1f, 0.55f), 35)));
        foreach (var sl in slots)
            offscreenMarkers.Add((sl.sr.transform, MakeHudText("Marker", "", Vector2.zero, 3.4f, BrokenColor, 35)));
        RefreshHud();
    }

    private void RefreshHud()
    {
        int hp = BossNameCorruptor.MaxLevel + 1 - corruption;
        string bars = "";
        for (int i = 0; i < BossNameCorruptor.MaxLevel + 1; i++) bars += i < hp ? "<color=#6F6>■</color>" : "<color=#555>□</color>";
        hpText.text = $"무결성 {bars}";
        progressText.text = $"방화벽 복구 <color=#7FF>{filledCount}</color>/{slots.Count}";
    }

    private void ShowMessage(string main, string sub = "", float duration = 2f)
    {
        msgText.text = main;
        subMsgText.text = sub;
        msgUntil = duration < 0 ? float.MaxValue : Time.time + duration;
    }
    #endregion

    // =====================================================================
    #region 진행
    private IEnumerator IntroRoutine()
    {
        state = FightState.Intro;
        ShowMessage("<color=#F5A>랜섬웨어</color>", $"\"방화벽은 이미 부서졌어. {baseName}, 네 파일도 곧 내 거야!\"", 2.6f);
        yield return new WaitForSeconds(2.6f);
        ShowMessage("코드 조각을 맞는 주소에 끼워 방화벽을 복구하라!",
            hardMode ? "포인터 산술: fw + i 의 주소는?" : "*(fw+i) 는 주소 fw + i*4 에 있다", 2.4f);
        state = FightState.Fight;
        decryptSpawnTimer = decryptSpawnInterval;
        StartCoroutine(BossPatternLoop());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && (debugKeys || state == FightState.GameOver || state == FightState.Clear))
        {
            Restart();
            return;
        }

        if (state == FightState.Clear && !leaving && !string.IsNullOrEmpty(NextScene) &&
            (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) ||
             (clearAutoContinue > 0 && Time.time - clearAt > clearAutoContinue)))
        {
            leaving = true;
            BossFightEntry.ExitBossFight(NextScene, cleared: true);
        }

        if (debugKeys && state == FightState.Fight)
        {
            if (Input.GetKeyDown(KeyCode.F1)) DebugSolveNext();
            if (Input.GetKeyDown(KeyCode.F2))
            {
                godMode = !godMode;
                ShowMessage(godMode ? "[DEBUG] 무적 ON" : "[DEBUG] 무적 OFF", "", 1f);
            }
            if (Input.GetKeyDown(KeyCode.F3)) DebugHit();
        }

        float dt = Time.deltaTime;
        if (Time.time > msgUntil) { msgText.text = ""; subMsgText.text = ""; }
        if (wrongSlotCooldown > 0) wrongSlotCooldown -= dt;

        if (state == FightState.Intro || state == FightState.Fight)
        {
            UpdatePlayer(dt);
            UpdateBoss(dt);
        }
        if (state == FightState.Fight)
        {
            UpdateBullets(dt);
            UpdateLasers();
            UpdatePuzzle();
            UpdateDecryptItem(dt);
        }
        UpdateVisuals(dt);
    }

    private void Restart()
    {
        var scene = SceneManager.GetActiveScene();
        if (scene.buildIndex >= 0) { SceneManager.LoadScene(scene.buildIndex); return; }
#if UNITY_EDITOR
        // 빌드 세팅에 없는 테스트 씬도 에디터에서는 바로 재시작
        UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(scene.path,
            new LoadSceneParameters(LoadSceneMode.Single));
#endif
    }

    private void UpdatePlayer(float dt)
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (DebugMoveInput != Vector2.zero) input = DebugMoveInput; // 자동 테스트용
        if (input.sqrMagnitude > 1f) input.Normalize();
        bool slow = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        playerPos += input * (playerSpeed * (slow ? 0.45f : 1f) * dt);

        var r = Arena;
        const float m = 0.4f;
        playerPos.x = Mathf.Clamp(playerPos.x, r.xMin + m, r.xMax - m);
        playerPos.y = Mathf.Clamp(playerPos.y, r.yMin + m, r.yMax - m);
        player.position = playerPos;
        if (input.x != 0 && playerSprite != null) playerSr.flipX = input.x > 0;
        UpdatePlayerAnim(input, slow);
    }

    // 본편 PlayerMovement와 같은 규칙: 정지 idle / 이동 run(보스전은 기본 달리기, PlayerBossCombat.forceRun과 동일)
    // Shift(정밀 이동) 중에는 walk. 좌우 반전도 본편과 동일하게 spine localScale.x 부호로 처리.
    private void UpdatePlayerAnim(Vector2 input, bool slow)
    {
        if (playerSpine == null) return;
        bool moving = input.sqrMagnitude > 0.01f;
        SetPlayerAnim(!moving ? idleAnim : slow ? walkAnim : runAnim,
            moving ? 1f + Mathf.Abs(input.y) * 0.5f : 1f);
        if (input.x != 0)
        {
            var sc = playerSpine.transform.localScale;
            sc.x = Mathf.Abs(sc.x) * (input.x < 0 ? 1f : -1f);
            playerSpine.transform.localScale = sc;
        }
    }

    private void SetPlayerAnim(string anim, float timeScale = 1f)
    {
        if (playerSpine == null || string.IsNullOrEmpty(anim)) return;
        if (playerSpine.AnimationState == null) playerSpine.Initialize(false);
        if (playerAnim != anim)
        {
            playerAnim = anim;
            playerSpine.AnimationState.SetAnimation(0, anim, true);
        }
        var cur = playerSpine.AnimationState.GetCurrent(0);
        if (cur != null) cur.TimeScale = timeScale;
    }

    private void UpdateBoss(float dt)
    {
        // 메가 사탄처럼 방 맨 위에 고정. 살짝 둥실거리는 연출만.
        bossPos = bossHome + new Vector2(0f, Mathf.Sin(Time.time * 1.6f) * 0.08f);
        boss.position = bossPos;

        if (state == FightState.Fight && Vector2.Distance(bossPos, playerPos) < bossContactRadius + playerHitRadius)
            OnPlayerHit();
    }

    private Vector2 CameraTarget()
    {
        float halfH = cam.orthographicSize, halfW = halfH * cam.aspect;
        var r = Arena;
        // 클리어하면 검거된 랜섬웨어 쪽으로 카메라 이동
        var target = state == FightState.Clear ? bossPos + new Vector2(0f, -2.2f) : new Vector2(playerPos.x, playerPos.y + 0.8f);
        // 방 폭이 화면보다 좁으면 X 고정 (버스정류장 '세로 카메라 고정'과 같은 방식)
        target.x = halfW >= r.width * 0.5f + 0.5f ? r.center.x
            : Mathf.Clamp(target.x, r.xMin - 0.5f + halfW, r.xMax + 0.5f - halfW);
        float lo = r.yMin - 0.7f + halfH, hi = r.yMax + 1.4f - halfH;
        target.y = lo > hi ? r.center.y : Mathf.Clamp(target.y, lo, hi);
        return target;
    }

    private void LateUpdate()
    {
        if (cam == null || player == null) return;
        camFollow = Vector2.SmoothDamp(camFollow, CameraTarget(), ref camVel,
            state == FightState.Clear ? 0.6f : cameraFollowSmooth);
        Vector2 shake = Vector2.zero;
        if (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;
            shake = Random.insideUnitCircle * 0.12f;
        }
        cam.transform.position = new Vector3(camFollow.x + shake.x, camFollow.y + shake.y, -10f);
        UpdateOffscreenMarkers();
    }

    private Rect ViewRect
    {
        get
        {
            float halfH = cam.orthographicSize, halfW = halfH * cam.aspect;
            return new Rect(camFollow.x - halfW, camFollow.y - halfH, halfW * 2f, halfH * 2f);
        }
    }

    /// <summary>화면에 보이는 영역 ∩ 방</summary>
    private Rect VisibleArena
    {
        get
        {
            Rect v = ViewRect, a = Arena;
            float xMin = Mathf.Max(v.xMin, a.xMin), xMax = Mathf.Min(v.xMax, a.xMax);
            float yMin = Mathf.Max(v.yMin + 0.8f, a.yMin), yMax = Mathf.Min(v.yMax - 0.8f, a.yMax);
            return Rect.MinMaxRect(xMin, yMin, xMax, Mathf.Max(yMin + 0.5f, yMax));
        }
    }

    private void UpdateOffscreenMarkers()
    {
        float halfH = cam.orthographicSize, halfW = halfH * cam.aspect;
        bool show = state == FightState.Fight || state == FightState.Intro;
        for (int i = 0; i < offscreenMarkers.Count; i++)
        {
            var (target, marker) = offscreenMarkers[i];
            bool done = i < fragments.Count
                ? fragments[i].placed || fragments[i].carried
                : slots[i - fragments.Count].filled;
            Vector2 rel = (Vector2)target.position - camFollow;
            bool off = Mathf.Abs(rel.y) > halfH - 0.6f || Mathf.Abs(rel.x) > halfW - 0.3f;
            if (!show || done || !off) { marker.text = ""; continue; }
            bool up = rel.y > 0;
            marker.text = up ? "▲" : "▼";
            float x = Mathf.Clamp(rel.x, -halfW + 0.6f, halfW - 0.6f);
            float y = up ? halfH - 0.95f : -halfH + 0.85f;
            marker.transform.localPosition = new Vector3(x, y + Mathf.Sin(Time.time * 5f + i) * 0.06f, 10f);
        }
    }

    private void UpdateVisuals(float dt)
    {
        if (state == FightState.GameOver) return;

        // 이름 라벨 (손상 단계일수록 글리치)
        Vector2 jitter = corruption > 0 && state != FightState.Clear
            ? Random.insideUnitCircle * (0.02f * corruption)
            : Vector2.zero;
        playerNameLabel.transform.position = playerPos + new Vector2(0, 0.78f) + jitter;
        carryLabel.transform.position = playerPos + new Vector2(0, 1.12f);
        if (corruption > 0 && state == FightState.Fight && Random.value < 0.015f * corruption)
            playerNameLabel.text = BossNameCorruptor.Corrupt(baseName, corruption, randomHex: true); // 주소가 순간 깨지는 연출
        else if (corruption > 0 && state == FightState.Fight && Random.value < 0.02f)
            playerNameLabel.text = shownName;

        // 무적 깜빡임 + 손상 단계별 색
        Color tint = corruption switch
        {
            0 => Color.white,
            1 => new Color(0.85f, 0.6f, 1f),
            _ => new Color(1f, 0.45f, 0.45f),
        };
        if (playerSprite == null) tint *= new Color(0.6f, 0.9f, 1f);
        bool blink = Time.time < invincibleUntil && Mathf.FloorToInt(Time.time * 14f) % 2 == 0;
        tint.a = blink ? 0.25f : 1f;
        playerSr.color = tint;
        if (playerSpine != null) playerSpine.Skeleton.SetColor(tint);

        // 깨진 방화벽 깜빡임
        foreach (var s in slots)
        {
            if (s.filled) continue;
            float k = 0.55f + 0.45f * Mathf.Sin(Time.time * 6f + s.index);
            s.sr.color = new Color(BrokenColor.r, BrokenColor.g, BrokenColor.b, k);
        }

    }
    #endregion

    // =====================================================================
    #region 피격 / 회복
    private void OnPlayerHit()
    {
        if (state != FightState.Fight || Time.time < invincibleUntil || godMode) return;

        shakeTime = 0.25f;
        if (corruption >= BossNameCorruptor.MaxLevel)
        {
            corruption = BossNameCorruptor.MaxLevel + 1;
            RefreshHud();
            GameOver();
            return;
        }

        corruption++;
        shownName = BossNameCorruptor.Corrupt(baseName, corruption);
        playerNameLabel.text = shownName;
        invincibleUntil = Time.time + invincibleTime;
        ClearBulletsNear(playerPos, 1.6f);
        DropCarried();
        RefreshHud();
        ShowMessage("<color=#F55>파일이 암호화되고 있다!</color>",
            corruption >= BossNameCorruptor.MaxLevel ? "한 번 더 맞으면 끝이다... 복호화 아이템을 찾자" : "복호화 아이템을 먹으면 원래대로 돌아온다", 1.6f);
    }

    private void Decrypt()
    {
        corruption = Mathf.Max(0, corruption - 1);
        shownName = BossNameCorruptor.Corrupt(baseName, corruption);
        playerNameLabel.text = shownName;
        RefreshHud();
        ShowMessage("<color=#7FF>복호화 성공!</color>", corruption == 0 ? $"{baseName}(으)로 완전히 돌아왔다" : "조금 더 복구해야 한다", 1.4f);
    }

    private void UpdateDecryptItem(float dt)
    {
        if (decryptItem == null)
        {
            if (corruption == 0) return; // 멀쩡할 땐 안 나옴
            decryptSpawnTimer -= dt;
            if (decryptSpawnTimer > 0) return;

            Vector2 p = RandomPointIn(VisibleArena, 0.8f, new[] { bossPos, playerPos }, 2.2f);
            var sr = MakeSprite("DecryptItem", circleSprite, p, new Vector2(0.55f, 0.55f), new Color(0.3f, 1f, 1f), 8);
            var t = MakeText("Label", "복호화", p + new Vector2(0, 0.5f), 2.2f, new Color(0.6f, 1f, 1f), 9);
            t.transform.SetParent(sr.transform, true);
            decryptItem = sr.transform;
            decryptDespawnAt = Time.time + decryptLifeTime;
            return;
        }

        float remain = decryptDespawnAt - Time.time;
        decryptItem.localScale = Vector3.one * (0.55f / circleSprite.bounds.size.x) * (1f + 0.12f * Mathf.Sin(Time.time * 8f));
        var isr = decryptItem.GetComponent<SpriteRenderer>();
        isr.enabled = remain > 2f || Mathf.FloorToInt(Time.time * 10f) % 2 == 0;

        if (Vector2.Distance(decryptItem.position, playerPos) < 0.55f)
        {
            Destroy(decryptItem.gameObject);
            decryptItem = null;
            decryptSpawnTimer = decryptSpawnInterval;
            Decrypt();
        }
        else if (remain <= 0)
        {
            Destroy(decryptItem.gameObject);
            decryptItem = null;
            decryptSpawnTimer = decryptSpawnInterval * 0.5f;
        }
    }
    #endregion

    // =====================================================================
    #region 퍼즐 (코드 조각 ↔ 방화벽 주소)
    private void UpdatePuzzle()
    {
        if (carrying == null)
        {
            foreach (var f in fragments)
            {
                if (f.carried || f.placed) continue;
                if (Vector2.Distance(f.pos, playerPos) < pickupRange)
                {
                    carrying = f;
                    f.carried = true;
                    f.tr.gameObject.SetActive(false);
                    carryLabel.text = $"[{FragmentCode(f.index)}]";
                    break;
                }
            }
            return;
        }

        foreach (var s in slots)
        {
            if (s.filled || Vector2.Distance(s.pos, playerPos) > slotRange) continue;

            if (s.index == carrying.index)
            {
                s.filled = true;
                s.sr.color = FixedColor;
                s.label.text = SlotLabel(s);
                s.label.color = FixedColor;
                carrying.placed = true;
                carrying.carried = false;
                carrying = null;
                carryLabel.text = "";
                filledCount++;
                RefreshHud();
                shakeTime = 0.1f;
                if (filledCount >= slots.Count) { Clear(); return; }
                ShowMessage($"<color=#7FF>0x{baseAddr + s.index * 4:X4} 복구!</color>",
                    $"남은 구멍 {slots.Count - filledCount}개 - 랜섬웨어가 초조해진다", 1.4f);
            }
            else if (wrongSlotCooldown <= 0)
            {
                wrongSlotCooldown = 1.2f;
                int need = baseAddr + carrying.index * 4;
                ShowMessage("<color=#F77>주소 불일치!</color>",
                    hardMode ? $"{FragmentCode(carrying.index)} 는 여기가 아니다"
                             : $"{FragmentCode(carrying.index)} → fw + {carrying.index}×4 = 0x{need:X4}", 1.4f);
            }
            break;
        }
    }

    /// <summary>디버그: 남은 조각 하나를 맞는 자리에 바로 끼운다.</summary>
    public void DebugSolveNext()
    {
        if (state != FightState.Fight) return;
        var f = carrying ?? fragments.Find(x => !x.placed);
        if (f == null) return;
        var s = slots.Find(x => x.index == f.index);
        if (carrying == null) { carrying = f; f.carried = true; f.tr.gameObject.SetActive(false); }
        playerPos = s.pos + (s.horizontal ? new Vector2(0, s.pos.y > 0 ? -0.5f : 0.5f) : new Vector2(s.pos.x > 0 ? -0.5f : 0.5f, 0));
        player.position = playerPos;
        UpdatePuzzle();
    }

    /// <summary>디버그: 무적/무적시간 무시하고 한 대 맞는다.</summary>
    public void DebugHit()
    {
        invincibleUntil = 0; bool g = godMode; godMode = false; OnPlayerHit(); godMode = g;
    }

    public int Corruption => corruption;
    public string ShownName => shownName;
    public bool IsCleared => state == FightState.Clear;
    /// <summary>자동 테스트용 가상 입력 (0이면 키보드)</summary>
    public Vector2 DebugMoveInput { get; set; }
    public string PlayerName => baseName;
    public string CurrentPlayerAnim => playerAnim;
    /// <summary>클리어 후 갈 씬: BossFightEntry로 들어왔으면 그 설정, 아니면 인스펙터 값</summary>
    public string NextScene => !string.IsNullOrEmpty(BossFightEntry.PendingNextScene) ? BossFightEntry.PendingNextScene : nextSceneOnClear;
    public bool IsGameOver => state == FightState.GameOver;

    private void DropCarried()
    {
        if (carrying == null) return;
        carrying.carried = false;
        carrying.pos = playerPos + Random.insideUnitCircle.normalized * 0.9f;
        var r = Arena;
        carrying.pos.x = Mathf.Clamp(carrying.pos.x, r.xMin + 0.8f, r.xMax - 0.8f);
        carrying.pos.y = Mathf.Clamp(carrying.pos.y, r.yMin + 0.8f, r.yMax - 0.8f);
        carrying.tr.position = carrying.pos;
        carrying.tr.gameObject.SetActive(true);
        carrying = null;
        carryLabel.text = "";
    }
    #endregion

    // =====================================================================
    #region 클리어 / 게임오버
    private void Clear()
    {
        state = FightState.Clear;
        StopAllCoroutines();
        ClearAllBullets();
        if (decryptItem != null) Destroy(decryptItem.gameObject);
        bossSr.color = new Color(0.55f, 0.55f, 0.6f);
        playerNameLabel.text = baseName;
        clearAt = Time.time;
        SetPlayerAnim(idleAnim);
        ShowMessage("<color=#7FF>방화벽 복구 완료!</color>",
            $"도망갈 곳이 없어진 랜섬웨어를 검거했다.\n<size=80%>{(string.IsNullOrEmpty(NextScene) ? "R: 다시하기" : "Space: 계속   R: 다시하기")}</size>", -1f);
        Debug.Log("[RansomwareBossFight] CLEAR");
        onClear?.Invoke();
    }

    private void GameOver()
    {
        state = FightState.GameOver;
        StopAllCoroutines();
        playerNameLabel.text = BossNameCorruptor.RandomHex(8);
        playerSr.color = new Color(1f, 0.2f, 0.2f, 0.5f);
        if (playerSpine != null)
        {
            SetPlayerAnim(idleAnim);
            playerSpine.Skeleton.SetColor(new Color(1f, 0.2f, 0.2f, 0.5f));
        }
        ShowMessage("<color=#F33>GAME OVER</color>", "파일이 완전히 암호화되었습니다...\nR: 다시하기", -1f);
        Debug.Log("[RansomwareBossFight] GAME OVER");
        onGameOver?.Invoke();
    }
    #endregion

    // =====================================================================
    #region 탄막
    private IEnumerator BossPatternLoop()
    {
        yield return new WaitForSeconds(0.6f);
        int last = -1;
        while (state == FightState.Fight)
        {
            int p;
            do p = Random.Range(0, 4); while (p == last);
            last = p;
            switch (p)
            {
                case 0: yield return RadialBurst(); break;
                case 1: yield return Spiral(); break;
                case 2: yield return AimedFan(); break;
                case 3: yield return ChainLasers(); break;
            }
            yield return new WaitForSeconds(Mathf.Max(0.9f, 1.8f - filledCount * 0.2f) * patternRestScale);
        }
    }

    private float Phase => filledCount; // 방화벽을 복구할수록 랜섬웨어가 발악 (완만하게)

    private int Cnt(float n) => Mathf.Max(1, Mathf.RoundToInt(n * bulletCountScale));

    private IEnumerator RadialBurst()
    {
        int waves = 2;
        for (int w = 0; w < waves; w++)
        {
            int n = Cnt(9 + Phase * 1.5f);
            float off = Random.Range(0f, 360f);
            for (int i = 0; i < n; i++)
                Fire(bossPos, off + 360f / n * i, 2.0f + Phase * 0.12f, new Color(1f, 0.3f, 0.55f));
            yield return new WaitForSeconds(0.8f);
        }
    }

    private IEnumerator Spiral()
    {
        float dur = 2.0f, t = 0f, ang = Random.Range(0f, 360f);
        int arms = Cnt(2 + (Phase >= 3 ? 1 : 0));
        float dir = Random.value < 0.5f ? 1f : -1f;
        while (t < dur)
        {
            for (int a = 0; a < arms; a++)
                Fire(bossPos, ang + 360f / arms * a, 2.2f + Phase * 0.1f, new Color(1f, 0.75f, 0.2f));
            ang += 19f * dir;
            t += 0.15f;
            yield return new WaitForSeconds(0.15f);
        }
    }

    private IEnumerator AimedFan()
    {
        int volleys = 2 + (Phase >= 3 ? 1 : 0);
        for (int v = 0; v < volleys; v++)
        {
            Vector2 d = playerPos - bossPos;
            float baseAng = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
            int n = Mathf.Max(2, Cnt(3 + Phase * 0.5f));
            float spread = 36f;
            for (int i = 0; i < n; i++)
                Fire(bossPos, baseAng - spread / 2f + spread / (n - 1) * i, 2.8f + Phase * 0.15f, new Color(0.75f, 0.4f, 1f));
            yield return new WaitForSeconds(0.7f);
        }
    }

    /// <summary>소은 초안(체인 패턴)을 계승: 회색 예고선 → 실체화.</summary>
    private IEnumerator ChainLasers()
    {
        var r = VisibleArena;
        int n = Cnt(2 + Mathf.Min(1, (int)Phase / 2));
        var made = new List<Laser>();
        for (int i = 0; i < n; i++)
        {
            Vector2 a, b;
            int kind = Random.Range(0, 3);
            if (i == 0) // 첫 줄은 플레이어를 지나가게
            {
                if (kind == 0) { a = new Vector2(r.xMin, playerPos.y); b = new Vector2(r.xMax, playerPos.y); }
                else if (kind == 1) { a = new Vector2(playerPos.x, r.yMin); b = new Vector2(playerPos.x, r.yMax); }
                else { a = playerPos + new Vector2(-12, -12); b = playerPos + new Vector2(12, 12); }
            }
            else if (kind == 0) { float y = Random.Range(r.yMin + 0.5f, r.yMax - 0.5f); a = new Vector2(r.xMin, y); b = new Vector2(r.xMax, y); }
            else if (kind == 1) { float x = Random.Range(r.xMin + 0.5f, r.xMax - 0.5f); a = new Vector2(x, r.yMin); b = new Vector2(x, r.yMax); }
            else { var c = RandomPointIn(r, 0.5f); a = c + new Vector2(-12, 12); b = c + new Vector2(12, -12); }

            ClipToRect(r, ref a, ref b);
            var L = new Laser { a = a, b = b, width = 0.26f };
            L.sr = MakeSprite("Chain", squareSprite, (a + b) / 2f, new Vector2(Vector2.Distance(a, b), 0.06f), new Color(0.7f, 0.7f, 0.7f, 0.5f), 14);
            L.sr.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg);
            made.Add(L);
            lasers.Add(L);
        }

        yield return new WaitForSeconds(Mathf.Max(1.0f, 1.4f - Phase * 0.08f)); // 예고선 충분히 보여주기
        foreach (var L in made)
        {
            L.active = true;
            L.sr.color = new Color(1f, 0.25f, 0.35f, 0.95f);
            var s = L.sr.transform.localScale;
            L.sr.transform.localScale = new Vector3(s.x, L.width / squareSprite.bounds.size.y, 1);
        }
        shakeTime = Mathf.Max(shakeTime, 0.12f);
        yield return new WaitForSeconds(0.45f);
        foreach (var L in made)
        {
            lasers.Remove(L);
            if (L.sr != null) Destroy(L.sr.gameObject);
        }
    }

    private static void ClipToRect(Rect r, ref Vector2 a, ref Vector2 b)
    {
        // 선분을 사각형으로 잘라냄 (Liang–Barsky)
        float t0 = 0f, t1 = 1f;
        Vector2 d = b - a;
        float[] p = { -d.x, d.x, -d.y, d.y };
        float[] q = { a.x - r.xMin, r.xMax - a.x, a.y - r.yMin, r.yMax - a.y };
        for (int i = 0; i < 4; i++)
        {
            if (Mathf.Approximately(p[i], 0f)) { if (q[i] < 0) return; continue; }
            float t = q[i] / p[i];
            if (p[i] < 0) t0 = Mathf.Max(t0, t); else t1 = Mathf.Min(t1, t);
        }
        if (t0 > t1) return;
        Vector2 na = a + d * t0, nb = a + d * t1;
        a = na; b = nb;
    }

    private void UpdateLasers()
    {
        foreach (var L in lasers)
        {
            if (!L.active) continue;
            if (DistancePointSegment(playerPos, L.a, L.b) < L.width * 0.5f + playerHitRadius * 0.6f)
            {
                OnPlayerHit();
                break;
            }
        }
    }

    private static float DistancePointSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / Mathf.Max(ab.sqrMagnitude, 1e-5f));
        return Vector2.Distance(p, a + ab * t);
    }

    private void Fire(Vector2 from, float angleDeg, float speed, Color color, float radius = 0.11f)
    {
        Bullet b = null;
        foreach (var x in bullets)
            if (!x.alive) { b = x; break; }
        if (b == null)
        {
            b = new Bullet { sr = MakeSprite("Bullet", circleSprite, from, Vector2.one * 0.3f, color, 15) };
            bullets.Add(b);
        }
        float rad = angleDeg * Mathf.Deg2Rad;
        b.pos = from;
        b.vel = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * (speed * bulletSpeedScale);
        b.radius = radius;
        b.alive = true;
        b.sr.color = color;
        b.sr.transform.position = from;
        b.sr.transform.localScale = Vector3.one * (radius * 2.4f / circleSprite.bounds.size.x);
        b.sr.gameObject.SetActive(true);
    }

    private void UpdateBullets(float dt)
    {
        var r = Arena;
        foreach (var b in bullets)
        {
            if (!b.alive) continue;
            b.pos += b.vel * dt;
            b.sr.transform.position = b.pos;
            if (b.pos.x < r.xMin || b.pos.x > r.xMax || b.pos.y < r.yMin || b.pos.y > r.yMax
                || Mathf.Abs(b.pos.y - camFollow.y) > cam.orthographicSize * 2.5f)
            {
                Kill(b);
                continue;
            }
            if ((b.pos - playerPos).sqrMagnitude < (b.radius + playerHitRadius) * (b.radius + playerHitRadius))
            {
                Kill(b);
                OnPlayerHit();
            }
        }
    }

    private static void Kill(Bullet b)
    {
        b.alive = false;
        b.sr.gameObject.SetActive(false);
    }

    private void ClearBulletsNear(Vector2 p, float radius)
    {
        foreach (var b in bullets)
            if (b.alive && Vector2.Distance(b.pos, p) < radius) Kill(b);
    }

    private void ClearAllBullets()
    {
        foreach (var b in bullets) if (b.alive) Kill(b);
        foreach (var L in lasers) if (L.sr != null) Destroy(L.sr.gameObject);
        lasers.Clear();
    }
    #endregion
}

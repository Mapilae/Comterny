using System.Collections.Generic;
using System.IO;
using Spine.Unity;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 보스전 씬에 실제 오브젝트(카메라/바닥/컴퓨터/보스/플레이어)를 배치한다.
/// 그 전에는 전부 플레이할 때만 생성돼서 씬 뷰가 비어 보였음.
/// 메뉴: Comterny/보스전/씬 오브젝트 생성(갱신)
/// 배치 후에는 씬에서 컴퓨터·플레이어 위치를 옮기면 플레이에 그대로 반영된다.
/// </summary>
public static class RansomwareBossSceneBuilder
{
    const string RootName = "[BossFight Scene]";
    const string SpriteDir = "Assets/03. Image/BossFight";
    const string SquarePath = SpriteDir + "/BossFx_Square.png";
    const string CirclePath = SpriteDir + "/BossFx_Circle.png";
    const string DefaultBgmPath = "Assets/05. Sounds/BGM/Comterny01(120) (1).mp3";
    const string DefaultHitSfxPath = "Assets/05. Sounds/error-9-206494.mp3";
    const string DefaultGlowMaterialPath = "Assets/03. Image/Emmision.mat";
    const string DefaultScreenGlowMaterialPath = "Assets/03. Image/Emmision_light.mat";
    const string PlayerPrefabPath = "Assets/04. Prefabs/!!이거 쓰세요/!!필수로 넣어야함!! 씬 하나당 꼭 1개씩만 두세요/Player.prefab";
    const string UnlitMatPath = "Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Unlit-Default.mat";

    [MenuItem("Comterny/보스전/씬 오브젝트 생성(갱신)")]
    public static void BuildMenu()
    {
        var fight = Object.FindAnyObjectByType<RansomwareBossFight>();
        if (fight == null)
        {
            EditorUtility.DisplayDialog("보스전", "열린 씬에 RansomwareBossFight 컴포넌트가 없습니다.", "확인");
            return;
        }
        Build(fight);
        EditorSceneManager.MarkSceneDirty(fight.gameObject.scene);
    }

    public static void Build(RansomwareBossFight f)
    {
        EnsureSprites(out var square, out var circle);
        var unlit = AssetDatabase.LoadAssetAtPath<Material>(UnlitMatPath);
        var scene = f.gameObject.scene;

        // 기존 생성물 제거 후 다시 생성
        foreach (var go in scene.GetRootGameObjects())
            if (go.name == RootName) Object.DestroyImmediate(go);
        var root = new GameObject(RootName).transform;
        SceneManager.MoveGameObjectToScene(root.gameObject, scene);

        SpriteRenderer Spr(string name, Sprite s, Transform parent, Vector2 pos, Vector2 size, Color c, int order)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = s;
            if (unlit != null) sr.sharedMaterial = unlit;
            sr.color = c;
            sr.sortingOrder = order;
            Vector2 b = s.bounds.size;
            go.transform.localScale = new Vector3(size.x / b.x, size.y / b.y, 1f);
            return sr;
        }

        TextMeshPro Txt(string name, string text, Transform parent, Vector2 pos, float size, Color c, int order)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            var t = go.AddComponent<TextMeshPro>();
            if (f.font != null) t.font = f.font;
            t.text = text;
            t.fontSize = size;
            t.color = c;
            t.alignment = TextAlignmentOptions.Center;
            t.textWrappingMode = TextWrappingModes.NoWrap;
            t.rectTransform.sizeDelta = new Vector2(18f, 1.5f);
            t.sortingOrder = order;
            return t;
        }

        Vector2 half = f.arenaHalfSize;
        var arena = new Rect(-half, half * 2f);

        // ---- 카메라 (본편 씬처럼 Main Camera가 씬에 있어야 Game 뷰가 보임)
        var cam = Camera.main;
        if (cam == null)
        {
            var cgo = new GameObject("Main Camera") { tag = "MainCamera" };
            SceneManager.MoveGameObjectToScene(cgo, scene);
            cam = cgo.AddComponent<Camera>();
            cgo.AddComponent<AudioListener>();
        }
        cam.orthographic = true;
        cam.orthographicSize = f.cameraOrthoSize;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = RansomwareBossFight.BgColor;
        cam.transform.position = new Vector3(0f, f.DefaultPlayerStart.y + 0.8f, -10f);

        // ---- 바닥 + 회로 격자
        var floor = Spr("Floor", square, root, Vector2.zero, arena.size, RansomwareBossFight.FloorColor, 0);
        var gridCol = RansomwareBossFight.GridColor;
        for (float x = Mathf.Ceil(arena.xMin); x < arena.xMax; x += 1f)
            Spr("GridV", square, floor.transform, new Vector2(x, 0), new Vector2(0.02f, arena.height), gridCol, 1);
        for (float y = Mathf.Ceil(arena.yMin); y < arena.yMax; y += 1f)
            Spr("GridH", square, floor.transform, new Vector2(0, y), new Vector2(arena.width, 0.02f), gridCol, 1);

        // ---- 에디터 미리보기: 방화벽 외곽 (실제 방화벽/구멍 위치는 플레이 때 무작위)
        var preview = new GameObject("EditorPreview (플레이하면 꺼짐)");
        preview.transform.SetParent(root, false);
        var wallCol = RansomwareBossFight.WallColor * new Color(1, 1, 1, 0.6f);
        const float t = 0.3f;
        Spr("Wall_Top", square, preview.transform, new Vector2(0, arena.yMax + t / 2), new Vector2(arena.width + t * 2, t), wallCol, 5);
        Spr("Wall_Bottom", square, preview.transform, new Vector2(0, arena.yMin - t / 2), new Vector2(arena.width + t * 2, t), wallCol, 5);
        Spr("Wall_Left", square, preview.transform, new Vector2(arena.xMin - t / 2, 0), new Vector2(t, arena.height), wallCol, 5);
        Spr("Wall_Right", square, preview.transform, new Vector2(arena.xMax + t / 2, 0), new Vector2(t, arena.height), wallCol, 5);
        Txt("Note", "방화벽 구멍 / 코드 조각 / 탄막은 플레이할 때 생성", preview.transform,
            new Vector2(0, arena.yMin + 0.5f), 2f, new Color(1f, 1f, 1f, 0.5f), 21);

        // ---- 컴퓨터
        var computers = new List<Transform>();
        var pcRoot = new GameObject("Computers").transform;
        pcRoot.SetParent(root, false);
        var spots = f.DefaultComputerSpots();
        var body = new Color(0.22f, 0.22f, 0.23f);
        for (int i = 0; i < spots.Count; i++)
        {
            var desk = spots[i] - RansomwareBossFight.ComputerBossOffset;
            var pc = new GameObject(i == 0 ? "PC_0 (보스 시작)" : $"PC_{i}").transform;
            pc.SetParent(pcRoot, false);
            pc.position = desk;
            Spr("Body", square, pc, desk, new Vector2(0.95f, 0.65f), body, 3);
            Spr("Screen", square, pc, desk + new Vector2(0, 0.04f), new Vector2(0.78f, 0.45f), RansomwareBossFight.ScreenOk, 4);
            Spr("Stand", square, pc, desk + new Vector2(0, -0.42f), new Vector2(0.4f, 0.12f), body, 3);
            Txt("Label", $"PC-{0x20 + i * 0x17:X2}", pc, desk + new Vector2(0, -0.62f), 1.6f, RansomwareBossFight.DullTextColor, 4);
            computers.Add(pc);
        }

        // ---- 보스
        var bossPos = spots[0];
        var bossSr = Spr("Ransomware (Boss)", f.bossSprite != null ? f.bossSprite : circle, root, bossPos,
            f.bossSprite != null ? new Vector2(1.7f, 1.7f) : new Vector2(1.2f, 1.2f),
            f.bossSprite != null ? Color.white : new Color(0.9f, 0.2f, 0.5f), 10);
        var bl = Txt("BossName", "<color=#C88>RANSOMWARE</color>", root, bossPos + new Vector2(0, 1.15f), 2.4f, Color.white, 21);
        bl.transform.SetParent(bossSr.transform, true);

        // ---- 플레이어 (본편 Player와 같은 Spine)
        var pStart = f.DefaultPlayerStart;
        var existingPlayer = Object.FindAnyObjectByType<PlayerMovement>();
        var player = existingPlayer != null ? existingPlayer.transform
            : ((GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath), scene)).transform;
        player.position = pStart;

        // ---- 컴포넌트에 연결
        Undo.RecordObject(f, "Build Boss Scene");
        if (f.bgm == null) f.bgm = AssetDatabase.LoadAssetAtPath<AudioClip>(DefaultBgmPath);
        if (f.hitSfx == null) f.hitSfx = AssetDatabase.LoadAssetAtPath<AudioClip>(DefaultHitSfxPath);
        if (f.glowMaterial == null) f.glowMaterial = AssetDatabase.LoadAssetAtPath<Material>(DefaultGlowMaterialPath);
        if (f.screenGlowMaterial == null) f.screenGlowMaterial = AssetDatabase.LoadAssetAtPath<Material>(DefaultScreenGlowMaterialPath);
        f.sceneFloor = floor;
        f.scenePlayer = player;
        f.sceneBoss = bossSr.transform;
        f.sceneComputers = computers;
        f.editorPreview = preview;
        EditorUtility.SetDirty(f);
        Debug.Log($"[BossSceneBuilder] '{scene.name}' 에 보스전 오브젝트 배치 완료 (컴퓨터 {computers.Count}대)");
    }

    static void EnsureSprites(out Sprite square, out Sprite circle)
    {
        if (!Directory.Exists(SpriteDir)) Directory.CreateDirectory(SpriteDir);
        if (!File.Exists(SquarePath))
        {
            var tex = new Texture2D(4, 4);
            var px = new Color[16];
            for (int i = 0; i < 16; i++) px[i] = Color.white;
            tex.SetPixels(px);
            File.WriteAllBytes(SquarePath, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(SquarePath);
            SetupImporter(SquarePath, 4, FilterMode.Point);
        }
        if (!File.Exists(CirclePath))
        {
            const int n = 64;
            var tex = new Texture2D(n, n);
            var px = new Color[n * n];
            for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(n / 2f, n / 2f)) / (n / 2f);
                float a = Mathf.Clamp01((1f - d) * 8f);
                float core = d < 0.55f ? 1f : Mathf.Lerp(1f, 0.75f, (d - 0.55f) / 0.45f);
                px[y * n + x] = new Color(core, core, core, a);
            }
            tex.SetPixels(px);
            File.WriteAllBytes(CirclePath, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(CirclePath);
            SetupImporter(CirclePath, n, FilterMode.Bilinear);
        }
        square = AssetDatabase.LoadAssetAtPath<Sprite>(SquarePath);
        circle = AssetDatabase.LoadAssetAtPath<Sprite>(CirclePath);
    }

    static void SetupImporter(string path, int ppu, FilterMode filter)
    {
        var imp = (TextureImporter)AssetImporter.GetAtPath(path);
        imp.textureType = TextureImporterType.Sprite;
        imp.spriteImportMode = SpriteImportMode.Single;
        imp.spritePixelsPerUnit = ppu;
        imp.filterMode = filter;
        imp.alphaIsTransparency = true;
        imp.mipmapEnabled = false;
        imp.textureCompression = TextureImporterCompression.Uncompressed;
        imp.SaveAndReimport();
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Assets/07. Voice 폴더를 훑어 Resources/VoiceDatabase.asset을 만든다.
/// 07. Voice 바로 아래 폴더 이름 = 캐릭터 이름, 그 안(하위 폴더 포함)의 오디오 파일 = 보이스.
/// 직접 적어 둔 별칭(aliases)은 갱신해도 유지된다.
/// </summary>
public class VoiceDatabaseBuilder : AssetPostprocessor
{
    private const string VoiceRoot = "Assets/07. Voice";
    private const string OutputPath = "Assets/Resources/VoiceDatabase.asset";

    // 새 캐릭터 폴더가 생겼을 때 기본으로 넣을 별칭 (시트 Name 칸 / 보스전 표시 이름)
    private static readonly Dictionary<string, string[]> DefaultAliases = new()
    {
        { "랜섬웨어", new[] { "랜섬웨어빌런" } },
        { "램", new[] { "램 주임" } },
        { "프로세서", new[] { "프로세서 팀장" } },
        { "컴파일러", new string[0] },
        { "유저 디버그", new[] { "디버그", "유저" } },
    };

    private static bool rebuildQueued;

    [MenuItem("Tools/Voice/보이스 DB 갱신")]
    public static void Build()
    {
        if (!AssetDatabase.IsValidFolder(VoiceRoot))
        {
            Debug.LogWarning($"[Voice] {VoiceRoot} 폴더가 없습니다.");
            return;
        }

        var db = AssetDatabase.LoadAssetAtPath<VoiceDatabase>(OutputPath);
        if (db == null)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources")) AssetDatabase.CreateFolder("Assets", "Resources");
            db = ScriptableObject.CreateInstance<VoiceDatabase>();
            AssetDatabase.CreateAsset(db, OutputPath);
        }

        var oldAliases = db.characters.ToDictionary(c => c.name, c => c.aliases ?? new List<string>());
        var result = new List<VoiceDatabase.VoiceCharacter>();
        var total = 0;

        foreach (var folder in AssetDatabase.GetSubFolders(VoiceRoot).OrderBy(f => f))
        {
            var charName = Path.GetFileName(folder);
            var character = new VoiceDatabase.VoiceCharacter { name = charName };

            if (oldAliases.TryGetValue(charName, out var aliases)) character.aliases = new List<string>(aliases);
            else if (DefaultAliases.TryGetValue(charName, out var defaults)) character.aliases = defaults.ToList();

            var seen = new HashSet<string>();
            foreach (var guid in AssetDatabase.FindAssets("t:AudioClip", new[] { folder }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (clip == null) continue;
                if (!seen.Add(clip.name.ToLowerInvariant()))
                    Debug.LogWarning($"[Voice] {charName} 폴더에 같은 파일명이 여러 개 있습니다: {clip.name} ({path})");
                character.clips.Add(clip);
            }

            character.clips = character.clips.OrderBy(c => c.name).ToList();
            total += character.clips.Count;
            result.Add(character);
        }

        db.characters = result;
        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        Debug.Log($"[Voice] VoiceDatabase 갱신 완료: 캐릭터 {result.Count}명, 보이스 {total}개");
    }

    [MenuItem("Tools/Voice/시트용 보이스 파일명 목록 복사")]
    public static void CopyListForSheet()
    {
        var db = AssetDatabase.LoadAssetAtPath<VoiceDatabase>(OutputPath);
        if (db == null)
        {
            Build();
            db = AssetDatabase.LoadAssetAtPath<VoiceDatabase>(OutputPath);
        }

        var lines = new List<string> { "Name\tVoice" };
        foreach (var c in db.characters)
            lines.AddRange(c.clips.Where(x => x != null).Select(x => $"{c.name}\t{x.name}"));
        EditorGUIUtility.systemCopyBuffer = string.Join("\n", lines);
        Debug.Log($"[Voice] {lines.Count - 1}개 보이스 파일명을 클립보드에 복사했습니다 (Name<TAB>Voice).");
    }

    private static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom)
    {
        if (rebuildQueued) return;
        var changed = imported.Concat(deleted).Concat(moved).Concat(movedFrom)
            .Any(p => p.StartsWith(VoiceRoot + "/") && !p.EndsWith(".meta"));
        if (!changed) return;

        rebuildQueued = true;
        EditorApplication.delayCall += () =>
        {
            rebuildQueued = false;
            Build();
        };
    }
}

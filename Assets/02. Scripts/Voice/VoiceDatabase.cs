using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 캐릭터별 보이스 클립 목록. Assets/07. Voice/&lt;캐릭터 이름&gt;/ 아래의 오디오 파일로 에디터가 자동 생성한다
/// (메뉴: Tools/Voice/보이스 DB 갱신, 07. Voice 폴더가 바뀌면 자동 갱신).
/// 시트에는 캐릭터 폴더를 뺀 "파일명"만 적고, 캐릭터는 대사의 Name 칸(또는 별칭)으로 찾는다.
/// </summary>
[CreateAssetMenu(fileName = "VoiceDatabase", menuName = "Dialogue/VoiceDatabase", order = 2)]
public class VoiceDatabase : ScriptableObject
{
    public const string ResourcePath = "VoiceDatabase"; // Assets/Resources/VoiceDatabase.asset

    [Serializable]
    public class VoiceCharacter
    {
        [Tooltip("07. Voice 아래 캐릭터 폴더 이름")] public string name;

        [Tooltip("시트의 Name 칸이 이 값들이어도 이 캐릭터로 본다 (예: 랜섬웨어빌런 → 랜섬웨어)")]
        public List<string> aliases = new();

        public List<AudioClip> clips = new();
    }

    public List<VoiceCharacter> characters = new();

    private static VoiceDatabase loaded;
    private static bool triedLoad;

    public static VoiceDatabase Instance
    {
        get
        {
            if (loaded == null && !triedLoad)
            {
                triedLoad = true;
                loaded = Resources.Load<VoiceDatabase>(ResourcePath);
                if (loaded == null) Debug.LogWarning("[Voice] Resources/VoiceDatabase.asset이 없습니다. Tools/Voice/보이스 DB 갱신을 실행하세요.");
            }

            return loaded;
        }
    }

    /// <summary>
    /// 캐릭터 이름과 보이스 파일명으로 클립을 찾는다. 파일명은 확장자/폴더가 붙어 있어도 된다.
    /// 해당 캐릭터에서 못 찾으면 전체 캐릭터에서 같은 파일명이 하나뿐일 때 그걸 쓴다.
    /// </summary>
    public AudioClip Find(string characterName, string voiceFile)
    {
        var key = NormalizeKey(voiceFile);
        if (key.Length == 0) return null;

        var character = FindCharacter(characterName);
        if (character != null)
        {
            var clip = FindIn(character, key);
            if (clip != null) return clip;
        }

        AudioClip found = null;
        foreach (var c in characters)
        {
            var clip = FindIn(c, key);
            if (clip == null) continue;
            if (found != null)
            {
                Debug.LogWarning($"[Voice] '{voiceFile}' 파일이 여러 캐릭터에 있어 고를 수 없습니다. Name 칸('{characterName}')을 확인하세요.");
                return null;
            }

            found = clip;
        }

        if (found == null) Debug.LogWarning($"[Voice] 보이스를 찾을 수 없습니다: {characterName} / {voiceFile}");
        return found;
    }

    public VoiceCharacter FindCharacter(string characterName)
    {
        if (string.IsNullOrWhiteSpace(characterName)) return null;
        characterName = characterName.Trim();
        foreach (var c in characters)
        {
            if (c.name == characterName) return c;
            if (c.aliases != null && c.aliases.Contains(characterName)) return c;
        }

        return null;
    }

    private static AudioClip FindIn(VoiceCharacter character, string key)
    {
        foreach (var clip in character.clips)
            if (clip != null && string.Equals(clip.name, key, StringComparison.OrdinalIgnoreCase))
                return clip;
        return null;
    }

    /// <summary>"램/1앗혹시.WAV" → "1앗혹시"</summary>
    public static string NormalizeKey(string voiceFile)
    {
        if (string.IsNullOrWhiteSpace(voiceFile)) return "";
        var key = voiceFile.Trim().Replace('\\', '/');
        var slash = key.LastIndexOf('/');
        if (slash >= 0) key = key.Substring(slash + 1);
        var ext = Path.GetExtension(key).ToLowerInvariant();
        if (ext is ".wav" or ".mp3" or ".ogg" or ".aif" or ".aiff") key = key.Substring(0, key.Length - ext.Length);
        return key.Trim();
    }
}

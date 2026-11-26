using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터 보이스 재생기. 씬에 따로 배치할 필요 없이 처음 쓸 때 스스로 만들어진다(DontDestroyOnLoad).
/// 채널마다 AudioSource를 하나씩 두어, 같은 채널에서 새 대사가 나오면 이전 보이스는 끊긴다.
/// - 다이얼로그: DialogueChannel 하나를 공유
/// - 말풍선: 말하는 오브젝트마다 각자 채널 (서로 겹쳐도 끊지 않음)
/// </summary>
public class VoicePlayer : MonoBehaviour
{
    public const string DialogueChannel = "dialogue";

    public const float DefaultVolume = 0.7f;

    /// <summary>보이스 전체 볼륨 (설정 UI에서 바꿔 쓸 수 있도록 열어둠)</summary>
    public static float Volume
    {
        get => PlayerPrefs.GetFloat("VoiceVolume", DefaultVolume);
        set
        {
            PlayerPrefs.SetFloat("VoiceVolume", Mathf.Clamp01(value));
            if (instance != null)
                foreach (var s in instance.sources.Values)
                    if (s != null) s.volume = Volume;
        }
    }

    private static VoicePlayer instance;
    private readonly Dictionary<string, AudioSource> sources = new();

    private static VoicePlayer Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("[VoicePlayer]");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<VoicePlayer>();
            }

            return instance;
        }
    }

    /// <summary>캐릭터 이름 + 보이스 파일명으로 찾아 재생한다. 비어 있거나 못 찾으면 해당 채널만 멈춘다. 재생한 클립(없으면 null)을 돌려준다.</summary>
    public static AudioClip Play(string channel, string characterName, string voiceFile)
    {
        AudioClip clip = null;
        if (!string.IsNullOrWhiteSpace(voiceFile) && VoiceDatabase.Instance != null)
            clip = VoiceDatabase.Instance.Find(characterName, voiceFile);
        Play(channel, clip);
        return clip;
    }

    public static void Play(string channel, AudioClip clip)
    {
        if (clip == null)
        {
            Stop(channel);
            return;
        }

        var source = Instance.GetSource(channel);
        source.Stop();
        source.clip = clip;
        source.volume = Volume;
        source.Play();
#if UNITY_EDITOR
        Debug.Log($"[Voice] 재생: {clip.name} (채널 {channel}, 볼륨 {Volume:0.##})");
#endif
    }

    public static void Stop(string channel)
    {
        if (instance != null && instance.sources.TryGetValue(channel, out var s) && s != null) s.Stop();
    }

    public static void StopAll()
    {
        if (instance == null) return;
        foreach (var s in instance.sources.Values)
            if (s != null) s.Stop();
    }

    public static bool IsPlaying(string channel)
    {
        return instance != null && instance.sources.TryGetValue(channel, out var s) && s != null && s.isPlaying;
    }

    private AudioSource GetSource(string channel)
    {
        if (sources.TryGetValue(channel, out var s) && s != null) return s;
        s = gameObject.AddComponent<AudioSource>();
        s.playOnAwake = false;
        s.loop = false;
        s.spatialBlend = 0f;
        sources[channel] = s;
        return s;
    }
}

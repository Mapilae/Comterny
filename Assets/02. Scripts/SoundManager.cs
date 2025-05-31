using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    /// <summary>
    ///     다른 게임에서 쓰던 코드를 복사해와서 안쓰는 기능이 끼어있을 수 있습니다.
    ///     https://m.blog.naver.com/bluebell2018/221319730428
    ///     https://m.blog.naver.com/bluebell2018/221321787597
    ///     https://newsroom.daewoong.co.kr/archives/13336
    /// </summary>
    public static SoundManager instance;

    public SoundHolder soundHolder;

    //public List<AudioClip> voiceClips;
    //  public AudioSource typingAudio;
    public AudioSource soundAudio;

    public List<AudioSource> ambienceAudio;
    //앰비언스와 효과음의 차이는 루프 여부임.

    public AudioSource bgmAudio;

    [Space] public AudioSource uiAudio;

    private readonly float bgmVolume = 0.4f;

    public void ChangeBGMVolume(float value)
    {
        
            bgmAudio.volume = value;
        
    }

    private void Awake()
    {
        instance = this;
    }

    public void PlayVoice(int index)
    {
        //typingAudio.PlayOneShot(voiceClips[index]);
        /*typingAudio.clip = voiceClips[index];
        typingAudio.Stop();
        typingAudio.Play();*/
    }

    public void StopSound()
    {
        soundAudio.Stop();
    }

    public void PlaySound(string name)
    {
        var clip = soundHolder.FindSE(name);
        //Debug.Log("PlaySE : " + name + " (" + clip.name + ")");
        PlaySound(clip, "");
    }

    public void PlaySound(AudioClip clip)
    {
        PlaySound(clip, "");
    }

    public void PlaySound(AudioClip clip, string subtitle)
    {
        if (soundAudio.isPlaying)
        {
            soundAudio.PlayOneShot(clip);
            return;
        }

        soundAudio.clip = clip;
        soundAudio.Stop();
        soundAudio.Play();
    }

    public void PlayUISound(AudioClip clip)
    {
        uiAudio.PlayOneShot(clip);
    }

    public void PlayBGM(string name)
    {
        var clip = soundHolder.FindBGM(name);
        //Debug.Log("PlayBGM : " + name + " (" + clip.name + ")");
        PlayBGM(clip, "", true);
    }

    public void PlayBGM(AudioClip clip, string subtitle, bool isLoop)
    {
        if (clip == null) return;
        if (clip.Equals(bgmAudio.clip)) return;
        StartCoroutine(CoroutinePlayBGM(clip));
    }

    private IEnumerator CoroutinePlayBGM(AudioClip clip)
    {
        var wait = new WaitForSeconds(0.1f);

        if (bgmAudio.isPlaying)
        {
            var minus = bgmVolume / 10;
            while (bgmAudio.volume > 0)
            {
                bgmAudio.volume -= minus;
                yield return wait;
            }

            bgmAudio.Stop();
            bgmAudio.volume = 0;
            bgmAudio.clip = null;
        }

        bgmAudio.clip = clip;

        var plus = bgmVolume / 10;
        bgmAudio.Play();
        while (bgmAudio.volume < bgmVolume)
        {
            bgmAudio.volume += plus;
            yield return wait;
        }

        bgmAudio.volume = bgmVolume;
        yield return null;
    }

    public void PlayAmbience(AudioClip clip, string subtitle)
    {
        if (clip == null)
        {
            Debug.Log("클립이 null입니다!");
            return;
        }

        var emptyIndex = -1;
        for (var i = 0; i < ambienceAudio.Count; i++)
            if (!ambienceAudio[i].isPlaying || ambienceAudio[i].clip == null)
            {
                emptyIndex = i;
                break;
            }
            else if (ambienceAudio[i].clip.name.Equals(clip.name))
            {
                //중복되는 것이 이미 재생 중인 경우
                return;
            }

        if (emptyIndex == -1)
        {
            Debug.Log("자리가 없음! 새 인스턴스 생성합니다.");
            var newAudio = Instantiate(ambienceAudio[0], ambienceAudio[0].transform.parent);
            ambienceAudio.Add(newAudio);
            emptyIndex = ambienceAudio.Count - 1;
        }

        Debug.Log("play amb track: " + ambienceAudio[emptyIndex].name);
        ambienceAudio[emptyIndex].clip = clip;
        ambienceAudio[emptyIndex].loop = true;
        ambienceAudio[emptyIndex].Play();
    }

    public void PlayAmbience(string clipName)
    {
        var clip = soundHolder.FindSE(clipName);
        PlayAmbience(clip, "");
    }


    public void EndBGM()
    {
        StartCoroutine(CoroutineEndBGM());
    }

    private IEnumerator CoroutineEndBGM()
    {
        var wait = new WaitForSeconds(0.1f);
        var minus = bgmVolume / 10;
        while (bgmAudio.volume > 0)
        {
            bgmAudio.volume -= minus;
            yield return wait;
        }

        bgmAudio.Stop();
        bgmAudio.volume = bgmVolume;
        bgmAudio.clip = null;
    }

    public void StopBGM()
    {
        if (bgmAudio.isPlaying)
            bgmAudio.Stop();
    }

    public void PauseBGM()
    {
        bgmAudio.Pause();
    }

    public void StopAmbience(string clipName)
    {
        var clip = soundHolder.FindSE(clipName);
        for (var i = 0; i < ambienceAudio.Count; i++)
            if (ambienceAudio[i].clip.name.Equals(clip.name))
                ambienceAudio[i].Stop();
    }


    public void ReplayBGM()
    {
        bgmAudio.Play();
    }
}
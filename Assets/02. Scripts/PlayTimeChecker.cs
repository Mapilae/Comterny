using System.Collections;
using TMPro;
using UnityEngine;

public class PlayTimeChecker : MonoBehaviour
{
    public static float firstPlayTime;
    public static bool isFirstTime = true;
    public static float totalPlayTime;
    public TMP_Text timeText;

    private static PlayTimeChecker instance;

    private void Awake()
    {
        // 타이틀로 돌아올 때마다 메인메뉴 씬의 사본이 또 생기므로 처음 것만 남긴다
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        if (instance != this) return;
        DontDestroyOnLoad(this);
        StartCoroutine(Stopwatch());
        LoadPlayTime();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        SavePlayTime();
    }

    //save play time when application quits
    private void OnApplicationQuit()
    {
        SavePlayTime();
    }

    private IEnumerator Stopwatch()
    {
        var wait = new WaitForSeconds(1f);
        while (true)
        {
            if (isFirstTime) firstPlayTime += 1f;
            totalPlayTime += 1f;
            timeText.text = "First Play Time: " + firstPlayTime.ToString("F0") + "s";
            timeText.text += "<br>Total Play Time: " + totalPlayTime.ToString("F0") + "s";
            yield return wait;
        }
        //yield return null;
    }

    public void SavePlayTime()
    {
        PlayerPrefs.SetFloat("PlayTime", totalPlayTime);
        PlayerPrefs.SetFloat("FirstPlayTime", firstPlayTime);
        PlayerPrefs.Save();
    }

    public void LoadPlayTime()
    {
        if (PlayerPrefs.HasKey("PlayTime") && PlayerPrefs.HasKey("FirstPlayTime"))
        {
            totalPlayTime = PlayerPrefs.GetFloat("PlayTime", 0f);
            firstPlayTime = PlayerPrefs.GetFloat("FirstPlayTime", 0f);
            isFirstTime = PlayerPrefs.GetInt("isFirstTime", 1) == 1;
        }
        else
        {
            totalPlayTime = 0f;
            firstPlayTime = 0f;
            isFirstTime = true;
            PlayerPrefs.SetInt("isFirstTime", 1);
        }
    }
}
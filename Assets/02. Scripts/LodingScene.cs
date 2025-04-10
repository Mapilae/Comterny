using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LodingScene : MonoBehaviour
{
    public static LodingScene instance;
    
    /**
     * 불러올 씬의 이름
     */
    public string LoadSceneName;

    /**
     * * 씬을 변화 시킬 버튼 임시로 Button으로 설정 했고 버튼 함수를
     * 바탕으로 구현 하거나 LoadSceneAsync를 명시적으로 실행
     */
    
    /**
     * 로딩 씬을 비동기로 로드할 때 사용할 텍스트 오브젝트
    */
    public TMP_Text HowLoading;

    public Image fill;

    /**
     * 비동기 로딩을 위한 코루틴
     */
    private void Awake()
    {
        instance = this;
    }

    public void LoadScene()
    {
        this.transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(LoadSceneAsync());
    }
    
    private IEnumerator LoadSceneAsync()
    {
        string LoadingSessionName = LoadSceneName == "" ? "Third Scene" : LoadSceneName;
        // 로딩 씬을 비동기로 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(LoadingSessionName);
        while (!asyncLoad.isDone)
        {
            HowLoading.text = asyncLoad.progress+"%";
            fill.fillAmount = asyncLoad.progress;
            yield return null;
        }
    }

    private IEnumerator LoadSceneAsync(GameObject text)
    {
        string LoadingSessionName = LoadSceneName == "" ? "Third Scene" : LoadSceneName;
        // 로딩 씬을 비동기로 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(LoadingSessionName);
        while (!asyncLoad.isDone)
        {
            text.GetComponent<TMP_Text>().text = asyncLoad.progress+"%";
            fill.fillAmount = asyncLoad.progress;
            yield return null;
        }
    }


    /**
     * 버튼 클릭 시 호출되는 함수
     * Text는 로딩 수치를 표시하는 TextMeshProUGUI 오브젝트
     * 사용 안해도 된다
     */
    public void ClickButton(GameObject text)
    {
        StartCoroutine(LoadSceneAsync(text));
    }
}
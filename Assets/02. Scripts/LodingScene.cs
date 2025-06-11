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
     * 불러올 씬의 이름
     */
    [Header("불러올 씬의 이름을 적어주세요 씬이름이 중복 될 수도 있습니다 빌드 설정을 해주세요")]
    public string LoadSceneName;

    /**
     * * 씬을 변화 시킬 버튼 임시로 Button으로 설정 했고 버튼 함수를
     * 바탕으로 구현 하거나 LoadSceneAsync를 명시적으로 실행
     */
    /* 씬을 변화 시킬 버튼 임시로 Button으로 설정 했고 버튼 함수를
    바탕으로 구현 하거나 LoadSceneAsync를 명시적으로 실행*/
    [Header("버튼을 클릭하면 씬이 바뀝니다 Button 오브젝트는 Button컴포넌트가 필요합니다.")]
    public GameObject Button;

    /**
     * 로딩 씬을 비동기로 로드할 때 사용할 텍스트 오브젝트
    */
    public TMP_Text HowLoading;

    public RectTransform charSD;
    public Image fill;

    private readonly string[] msgs =
    {
        "프로그램 오류인 버그라는 단어는 컴퓨터의 오작동 원인이 벌레였던 건에서 유래되었습니다.",
        "CPU는 RAM보다 성능 면에서 수십에서 수백배 빠릅니다.",
        "단어(word)는 컴퓨터에서 연산의 기본 단위로 사용되는 정보의 양입니다.",
        "시스템 버스는 제어 버스, 주소 버스, 데이터 버스로 구성되어 있습니다.",
        "랜섬웨어는 컴퓨터 시스템을 감염시켜 접근을 제한하고 몸값을 요구하는 악성 코드입니다.",
        "컴파일러는 특정 프로그래밍 언어로 쓰여 있는 문서를 다른 프로그래밍 언어로 옮기는 언어 번역 프로그램을 말합니다."
    };

    /**
     * 비동기 로딩을 위한 코루틴
     */
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (Button != null)
            Button.GetComponent<Button>().onClick.AddListener(ClickButton);
    }

    public void LoadScene(string sceneName)
    {
        LoadSceneName = sceneName;
        LoadScene();
    }

    public void LoadScene()
    {
        DontDestroyOnLoad(gameObject);
        transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        var LoadingSessionName = LoadSceneName == "" ? "Third Scene" : LoadSceneName;
        // 로딩 씬을 비동기로 로드
        var asyncLoad = SceneManager.LoadSceneAsync(LoadingSessionName);
        HowLoading.text = msgs[Random.Range(0, msgs.Length)];
        var time = 0;
        while (!asyncLoad.isDone)
        {
            fill.fillAmount = asyncLoad.progress;
            time++;
            //Debug.Log(time);
            charSD.rotation = Quaternion.Euler(0, 0, time % 2 == 0 ? -5 : 5);
            yield return null;
        }

        yield return null;
        gameObject.SetActive(false);
        //Destroy(this.gameObject);
    }

    private IEnumerator LoadSceneAsync(GameObject text)
    {
        var LoadingSessionName = LoadSceneName == "" ? "Third Scene" : LoadSceneName;
        // 로딩 씬을 비동기로 로드
        var asyncLoad = SceneManager.LoadSceneAsync(LoadingSessionName);
        while (!asyncLoad.isDone)
        {
            text.GetComponent<TMP_Text>().text = asyncLoad.progress + "%";
            fill.fillAmount = asyncLoad.progress;
            // fill.material
            yield return null;
        }
    }

    public void LoadSceneDependOnCrtStage(int crtStage)
    {
        var sceneName = "";
        switch (crtStage)
        {
            case 0:
                sceneName = "ComternyEntry";
                break;
            case 1:
                sceneName = "MainMemory";
                break;
            case 2:
                sceneName = "BusStop";
                break;
            case 3:
                sceneName = "CentralProcessingDept";
                break;
            case 4:
                sceneName = "SystemSoftware";
                break;
            case 5:
                sceneName = "GoToOS";
                break;
            case 6:
                sceneName = "OperatingSystem";
                break;
        }

        LoadScene(sceneName);
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

    public void ClickButton()
    {
        StartCoroutine(LoadSceneAsync(HowLoading.gameObject));
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
        
    }
    
}
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public static Pause instance;
    public static bool isPaused;

    //한번이라도 스킵한적 있는지 체크하는 용. 스킵한 적 있으면 도전과제 달성 안됨.
    public static bool isSkippedOnce;
    public GameObject pauseMenu;

    public Button skipBtn;
    
    public Slider bgmSlider;

    [Header("Current Puzzle Minigame(for skip)")]
    public GameObject currentPuzzle;

    private bool isPlayerObjExist;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //pauseMenu = this.transform.GetChild(0).gameObject;
        isPlayerObjExist = GameObject.FindWithTag("Player");
        skipBtn.interactable = currentPuzzle != null;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) OpenPauseMenu();
    }

    public void SetCurrentPuzzle(GameObject puzzle)
    {
        //현재 미니게임을 등록한다. skipPuzzle()에서 사용하기 위함.
        //미니게임을 닫는 경우, 해당 미니게임스크립트에서 오브젝트를 null로 바꿔줘야 한다.
        currentPuzzle = puzzle;
        skipBtn.interactable = currentPuzzle != null;
    }

    public void SkipPuzzle()
    {
        //버튼에서 사용함
        //currentPuzzle에서 미니게임 스크립트를 꺼내서 미니게임의 클리어 후의 메소드를 여기서 실행시킨다.(종료. 즉 스킵)
        if (currentPuzzle == null) return;
        //이 아래부터 존재하는 미니게임을 스킵한 경우임
        isSkippedOnce = true;
        if (currentPuzzle.GetComponent<BinaryTree>())
            currentPuzzle.GetComponent<BinaryTree>().Success();
        if (currentPuzzle.GetComponent<DepthFirstSearch>())
            currentPuzzle.GetComponent<DepthFirstSearch>().Success();
        else if (currentPuzzle.GetComponent<LogicGateMinigame>())
            currentPuzzle.GetComponent<LogicGateMinigame>().EndTest(true);
        else if (currentPuzzle.GetComponent<ParityBit>())
            currentPuzzle.GetComponent<ParityBit>().EndGame();
        else if (currentPuzzle.GetComponent<BinaryAdditionPuzzle>())
            currentPuzzle.GetComponent<BinaryAdditionPuzzle>().Success();
        else if (currentPuzzle.GetComponent<DecimalToBinaryPuzzle>())
            currentPuzzle.GetComponent<DecimalToBinaryPuzzle>().Success();
        //스킵후 창 닫기
        OpenPauseMenu();
    }

    public void ChangeBGMVolume()
    {
        //슬라이더에서 사용함
        //Debug.Log("BGM Volume Changed: " + value);
        float value = bgmSlider.value;
        SoundManager.instance.ChangeBGMVolume(value);
    }
    
    public void OpenPauseMenu()
    {
        //button click or key press
        //Debug.Log("Opening Pause Menu");
        isPaused = !isPaused;

        //일시정지 상태인 경우 플레이어 움직임도 멈추게함. 일시정지 해제 상태인 경우 플레이어 움직임도 재개함.
        if (isPaused && isPlayerObjExist) PlayerMovement.instance.ChangeCanMove(isPaused);
        //메뉴 닫기 또는 열기
        pauseMenu.SetActive(isPaused);
    }

    public void GoToMainMenu()
    {
        //버튼에서 사용함
        SaveData.instance.Save();
        LodingScene.instance.LoadScene("MainMenu");
    }

    public void ResetSaveData()
    {
        SaveData.instance.ResetData();
    }
}
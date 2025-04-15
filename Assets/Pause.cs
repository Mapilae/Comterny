using System;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public static Pause instance;
    public GameObject pauseMenu;
    public static bool isPaused = false;
    bool isPlayerObjExist = false;

    public Button skipBtn;
    
    //한번이라도 스킵한적 있는지 체크하는 용. 스킵한 적 있으면 도전과제 달성 안됨.
    public static bool isSkippedOnce;
    [Header("Current Puzzle Minigame(for skip)")]
    public GameObject currentPuzzle;
    
    void Awake()
    {
        instance = this;
    }
    
    private void Start()
    {
        //pauseMenu = this.transform.GetChild(0).gameObject;
        isPlayerObjExist= GameObject.FindWithTag("Player");
        skipBtn.interactable = (currentPuzzle != null);
    }

    public void SetCurrentPuzzle(GameObject puzzle)
    {//현재 미니게임을 등록한다. skipPuzzle()에서 사용하기 위함.
        //미니게임을 닫는 경우, 해당 미니게임스크립트에서 오브젝트를 null로 바꿔줘야 한다.
        currentPuzzle = puzzle;
        skipBtn.interactable = (currentPuzzle != null);
    }

    public void SkipPuzzle()
    {//버튼에서 사용함
        //currentPuzzle에서 미니게임 스크립트를 꺼내서 미니게임의 클리어 후의 메소드를 여기서 실행시킨다.(종료. 즉 스킵)
        if (currentPuzzle == null) return;
        //이 아래부터 존재하는 미니게임을 스킵한 경우임
        isSkippedOnce= true;
        if (currentPuzzle.GetComponent<LogicGateMinigame>())
        {
            currentPuzzle.GetComponent<LogicGateMinigame>().EndTest(true);
        }
        else if (currentPuzzle.GetComponent<ParityBit>())
        {
            currentPuzzle.GetComponent<ParityBit>().EndGame();
        }
        //스킵후 창 닫기
        OpenPauseMenu();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenPauseMenu();
        }
    }

    public void OpenPauseMenu()
    {//button click or key press
        //Debug.Log("Opening Pause Menu");
        isPaused = !isPaused;
        
        //일시정지 상태인 경우 플레이어 움직임도 멈추게함. 일시정지 해제 상태인 경우 플레이어 움직임도 재개함.
        if (isPaused&&isPlayerObjExist)
        {
            PlayerMovement.instance.ChangeCanMove(isPaused);
        }
        //메뉴 닫기 또는 열기
        pauseMenu.SetActive(isPaused);

    }

    public void GoToMainMenu()
    {//버튼에서 사용함
        LodingScene.instance.LoadScene("MainMenu");
    }
    
}

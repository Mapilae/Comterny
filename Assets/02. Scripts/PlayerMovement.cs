using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MovingFootPrint
{
    public static bool isSpace;
    public static PlayerMovement instance;

    public static bool canMove = true;
    public static bool isCutscene = false;
    public Transform spineTrans;

    public float moveSpeed = 5f;
    public float moveSpeed2x = 5f;

    public int currentHoldingItem = -1; //-1: no item holding, else: item code
    public Image currentHoldingImg;

    public LayerMask obstacleLayer;

    private FixedJoystick fixedJoystick;
    private Vector2 input;
    private bool isRunning;
    private float moveSpeed1x;

    private SpriteRenderer spriteRender;
    private Vector2 targetPos;


    private void Awake()
    {
        instance = this;

        fixedJoystick = GameObject.FindWithTag("Mobile").transform.GetChild(0).GetComponent<FixedJoystick>();
    }

    private void Start()
    {
        moveSpeed1x = moveSpeed;

        spriteRender = GetComponent<SpriteRenderer>();
        if (footprintObj == null) footprintObj = GameObject.FindWithTag("FootPrint").transform.GetChild(0).gameObject;
        StartFootPrint();
        targetPos = transform.position; // 초기 위치 설정
    }

    private void Update()
    {
        //if (isMoving) return;
        FootStep(); //MovingFootPrint: 바닥 환경에 따라 발소리 나게함


//#if UNITY_EDITOR
        //달리기 탐지 목적(걷는WASD와는 관련 없음)
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftControl))
            SetIfRunning(true);
        else
            SetIfRunning(false);
        if (Input.GetKeyDown(KeyCode.Space)) SetIsSpace(true);
//#endif
#if UNITY_ANDROID || UNITY_IOS
        if (fixedJoystick.CurrentMovementState.Equals(Joystick.MovementState.RUN))
            SetIfRunning(true);
        else if (fixedJoystick.CurrentMovementState.Equals(Joystick.MovementState.WALK))
            SetIfRunning(false);
#endif


        if (canMove && !isCutscene)
            input = GetInput(); //WASD가 클릭되고 있거나, 조이스틱이 사용되고 있는지 그 값
        else
            input = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (!canMove || isCutscene) return;

        rb.linearVelocity = input * moveSpeed;

        if (input.x != 0)
        {
            if (spriteRender != null) spriteRender.flipX = input.x < 0;
            spineTrans.localScale = input.x < 0 ? Vector3.one : new Vector3(-1, 1, 1);
        }

        CheckIfWalking();
        if (!isWalking)
        {
            playSpineAnim.SetAnim("idle", true);
        }
        else
        {
            if (!isRunning) //걷고 있는 경우
                playSpineAnim.SetAnim("walk", true);
            else
                playSpineAnim.SetAnim("run", true);
        }
    }

    public void SetIsSpace(bool isButtonClicked)
    {
        if (!canMove || isWalking || isCutscene) return;
        isSpace = isButtonClicked;
    }

    public void IsSpaceFalse(float delay)
    {
        Invoke("IsSpaceFalseDelay", delay);
    }

    private void IsSpaceFalseDelay()
    {
        isSpace = false;
    }


    public void SetIfRunning(bool isRunning)
    {
        if (isRunning)
        {
            moveSpeed = moveSpeed2x;
            this.isRunning = true;
        }
        else
        {
            moveSpeed = moveSpeed1x;
            this.isRunning = false;
        }
    }


    private Vector2 GetInput()
    {
        var inputVec = Vector2.zero;
#if UNITY_ANDROID
        //모바일인 경우
        inputVec.x = fixedJoystick.Horizontal;
        inputVec.y = fixedJoystick.Vertical;
        // Debug.Log(inputVec.x + ",  " + inputVec.y);

        return inputVec;

#endif
#if UNITY_EDITOR
        //PC인 경우
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
        inputVec.Normalize();
        return inputVec;
#endif
    }

    public void ChangeCanMove(bool canMoveb)
    {
        canMove = canMoveb;
        if (!canMoveb)
            StopMoving();
        else rb.WakeUp();
    }

    public void UnholdItem()
    {
        currentHoldingItem = -1;
        currentHoldingImg.sprite = null;
        currentHoldingImg.enabled = false;
        currentHoldingImg.gameObject.SetActive(false);
    }

    public void HoldItem(int holdItem, Sprite holdSpr)
    {
        currentHoldingItem = holdItem;
        currentHoldingImg.sprite = holdSpr;
        currentHoldingImg.enabled = true;
        currentHoldingImg.gameObject.SetActive(true);
    }

    public void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.linearVelocity = Vector2.zero;

        rb.Sleep();

        playSpineAnim.SetAnim("idle", true);
    }
}
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MovingFootPrint
{
    public static bool isSpace;
    public static PlayerMovement instance;
    public static string playerName = "유저 디버그";
    public static bool canMove = true;
    public Transform spineTrans;

    public float moveSpeed = 5f;
    public float moveSpeed2x = 5f;

    public int currentHoldingItem = -1; //-1: no item holding, else: item code
    public Image currentHoldingImg;

    public LayerMask obstacleLayer;
    public PlaySpineAnim playSpineAnim;
    private readonly bool isMoving = false;

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
        if (isMoving) return;
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftControl))
            SetIfRunning(true);
        else
            SetIfRunning(false);
        if (Input.GetKeyDown(KeyCode.Space)) SetIsSpace(true);
#endif


        if (canMove)
            input = GetInput();
        else
            input = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (!canMove) return;
        
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
            if (!isRunning)//걷고 있는 경우
                playSpineAnim.SetAnim("walk", true);
            else
                playSpineAnim.SetAnim("run", true);
        }
    }

    public void SetIsSpace(bool isButtonClicked)
    {
        if (!canMove) return;
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
        Vector2 inputVec = Vector2.zero;
#if UNITY_ANDROID
        //모바일인 경우
        inputVec.x = fixedJoystick.Horizontal;
        inputVec.y = fixedJoystick.Vertical;
        Debug.Log(inputVec.x + ",  " + inputVec.y);
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
    }

    public void HoldItem(int holdItem, Sprite holdSpr)
    {
        currentHoldingItem = holdItem;
        currentHoldingImg.sprite = holdSpr;
        currentHoldingImg.enabled = true;
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
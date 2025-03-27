using UnityEngine;

public class PlayerMovement : MovingFootPrint
{
    public static bool isSpace;
    public static PlayerMovement instance;
    public static string playerName = "유저 디버그";
    public Transform spineTrans;

    public float moveSpeed = 5f;
    public float moveSpeed2x = 5f;
    public bool canMove = true;

    public int currentHoldingItem = -1; //-1: no item holding, else: item code
    public SpriteRenderer currentHoldingImg;

    public LayerMask obstacleLayer;
    private readonly bool isMoving = false;

    private FixedJoystick fixedJoystick;
    private Vector2 input;
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
    }

    public void SetIsSpace(bool isButtonClicked)
    {
        isSpace = isButtonClicked;
    }

    public static void IsSpaceFalse()
    {
        isSpace = false;
    }

    public void SetIfRunning(bool isRunning)
    {
        if (isRunning) moveSpeed = moveSpeed2x;
        else moveSpeed = moveSpeed1x;
    }


    private Vector2 GetInput()
    {
        var inputVec = Vector2.zero;
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

    public void ChangeCanMove(bool canMove)
    {
        this.canMove = canMove;
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
}
/*
public class PlayerMovement : MovingFootPrint
{

    public static PlayerMovement instance;

    private SpriteRenderer spriteRender;
    private Vector2 input;
    private bool isMoving = false;
    private Vector2 targetPos;

    public float moveSpeed = 5f;
    private float moveSpeed1x;
    public float moveSpeed2x = 5f;
    public bool canMove = true;
    public static string playerName = "유저 디버그";

    public int currentHoldingItem = -1; //-1: no item holding, else: item code
    public SpriteRenderer currentHoldingImg;

    public LayerMask obstacleLayer;

    public void ChangeCanMove(bool canMove)
    {
        this.canMove = canMove;
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


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        moveSpeed1x=moveSpeed;

        spriteRender = GetComponent<SpriteRenderer>();
        if(footprintObj==null) footprintObj=GameObject.FindWithTag("FootPrint").transform.GetChild(0).gameObject;
        StartFootPrint();
        targetPos = transform.position; // 초기 위치 설정
    }

    private void Update()
    {

        if (!canMove) return;
        if (isMoving) return;

        if (Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift)||Input.GetKey(KeyCode.LeftControl))
            moveSpeed = moveSpeed2x;
        else
            moveSpeed = moveSpeed1x;

        Vector2 moveInput = Vector2.zero;

        float horizontal = Input.GetAxisRaw("Horizontal")*0.5f;
        float vertical = Input.GetAxisRaw("Vertical")*0.5f;

        Vector2 currentPosition = transform.position;

        // 가로 방향 이동 검사
        if (horizontal != 0)
        {

            spriteRender.flipX = horizontal < 0;
            Vector2 nextPosX = currentPosition + new Vector2(horizontal, 0);
            if (!Physics2D.OverlapCircle(nextPosX, 0.1f, obstacleLayer))
            {
                moveInput.x = horizontal;
            }

        }

        // 세로 방향 이동 검사
        if (vertical != 0)
        {
            Vector2 nextPosY = currentPosition + new Vector2(0, vertical);
            if (!Physics2D.OverlapCircle(nextPosY, 0.1f, obstacleLayer))
            {
                moveInput.y = vertical;
            }
        }

        // 최종 이동
        if (moveInput != Vector2.zero)
        {
            targetPos = (Vector2)transform.position + moveInput;
            isMoving = true;
        }
    }

    private void FixedUpdate()
    {
        if (!canMove) return;
        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime);

            if ((Vector2)transform.position == targetPos)
            {
                isMoving = false;
            }
        }

        isWalking = isMoving;

        //CheckIfWalking();
    }
}*/

/*
public class PlayerMovement : MovingFootPrint
{
    public static PlayerMovement instance;

    SpriteRenderer spriteRender;

    public float moveSpeed = 5f;
    public static bool canMove = true;
    public static string playerName = "유저 디버그";

    private Vector2 input;

    public int currentHoldingItem = -1; //-1: no item holding, else: item code
    public SpriteRenderer currentHoldingImg;

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

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        StartFootPrint();
    }

    private void Update()
    {
        if (canMove)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            input.Normalize(); // �밢�� �̵� �ӵ� ����ȭ
        }
        else
        {
            input = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = input * moveSpeed;

        if (input.x != 0)
            spriteRender.flipX = input.x < 0;
        CheckIfWalking();
    }
}
*/
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool isMoving;
    public Vector2 input;
    public static bool canMove = true;


    void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input != Vector2.zero)
            {
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                StartCoroutine(Move(targetPos));

            }
        }

    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
    }


}
*/
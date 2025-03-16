using UnityEngine;
using UnityEngine.UI;


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
using UnityEngine;

public class PlayerMovement : MovingFootPrint
{
    SpriteRenderer spriteRender;

    public float moveSpeed = 5f;
    public static bool canMove = true;
    public static string playerName = "�÷��̾�";

    private Vector2 input;


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
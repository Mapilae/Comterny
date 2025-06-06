using System.Collections;
using UnityEngine;

public class MemoryBugObject : MonoBehaviour
{
    public bool colEnableAtStart; // 시작할 때 충돌체를 활성화할지 여부

    public Transform goPos1; // 도달할 목표 위치

    public Transform goPos2; // 도달
    public float duration = 1.0f; // 이동에 걸리는 시간
    public float height = 2.0f; // 포물선 최고 높이
    private Animator anim;

    private Collider2D col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        col.enabled = colEnableAtStart; //cannot catch bug at the start
        // MoveDown();
    }

    public void MoveDown()
    {
        transform.parent = null;
        col.enabled = true;
        Debug.Log("bug is ready to Catch");
        StartParabola(goPos1);
    }


    public void FlyUp()
    {
        col.enabled = false;
        Debug.Log("bug is flying up");
        StartParabola(goPos2);
    }

    public void StartParabola(Transform target)
    {
        StartCoroutine(MoveParabola(target.position, duration, height));
    }

    private IEnumerator MoveParabola(Vector2 targetPos, float duration, float height)
    {
        Vector2 startPos = transform.position;
        var time = 0f;

        while (time < duration)
        {
            var t = time / duration;

            // 수평 보간
            var linear = Vector2.Lerp(startPos, targetPos, t);

            // 포물선 y값 추가
            var parabolicY = 4 * height * t * (1 - t);
            var newPos = new Vector2(linear.x, linear.y + parabolicY);

            transform.position = newPos;

            time += Time.deltaTime;
            yield return null;
        }

        // 최종 위치 보정
        transform.position = targetPos;
    }
}
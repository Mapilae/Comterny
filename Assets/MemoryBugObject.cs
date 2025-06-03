using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryBugObject : MonoBehaviour
{
    private Animator anim;

    private Collider2D col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        col.enabled = false; //cannot catch bug at the start
       // MoveDown();
    }

    public void MoveDown()
    {
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
    
    public Transform goPos1;         // 도달할 목표 위치
    
    public Transform goPos2;         // 도달
    public float duration = 1.0f;    // 이동에 걸리는 시간
    public float height = 2.0f;      // 포물선 최고 높이

    public void StartParabola(Transform target)
    {
        StartCoroutine(MoveParabola(target.position, duration, height));
    }

    private IEnumerator MoveParabola(Vector2 targetPos, float duration, float height)
    {
        Vector2 startPos = transform.position;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;

            // 수평 보간
            Vector2 linear = Vector2.Lerp(startPos, targetPos, t);

            // 포물선 y값 추가
            float parabolicY = 4 * height * t * (1 - t);
            Vector2 newPos = new Vector2(linear.x, linear.y + parabolicY);

            transform.position = newPos;

            time += Time.deltaTime;
            yield return null;
        }

        // 최종 위치 보정
        transform.position = targetPos;
    }

    
}

using System.Collections;
using UnityEngine;

public class MemoryBugObject : MonoBehaviour
{
    public bool colEnableAtStart; // 시작할 때 충돌체를 활성화할지 여부

    public Transform goPos1; // 도달할 목표 위치

    public Transform goPos2; // 도달
    public float duration = 1.0f; // 이동에 걸리는 시간
    public float height = 2.0f; // 포물선 최고 높이
    Animator anim;

    private Collider2D col;

    public Transform endParent;

    [Header("Attack effect")] public GameObject attackEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        col.enabled = colEnableAtStart; //cannot catch bug at the start
        // MoveDown();
        
        //anim.SetTrigger("die");
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
        anim.SetBool("fly", true);
        StartCoroutine(MoveParabola(target.position, duration, height));
    }

    private IEnumerator MoveParabola(Vector2 targetPos, float duration, float height)
    {
        Vector2 startPos = transform.position;
        float time = 0f;
        float finalAngle = Mathf.Atan2(targetPos.y - startPos.y, targetPos.x - startPos.x) * Mathf.Rad2Deg;

        while (time < duration)
        {
            float t = time / duration;

            // 수평 보간
            Vector2 linear = Vector2.Lerp(startPos, targetPos, t);

            // 포물선 y값 추가
            float parabolicY = 4f * height * t * (1f - t);
            Vector2 newPos = new Vector2(linear.x, linear.y + parabolicY);

            // 회전: 현재 위치(newPos)에서 목표(targetPos)를 바라보도록
            Vector2 direction = (targetPos - newPos).normalized;
            if (direction.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + -135f;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }

            transform.position = newPos;
            time += Time.deltaTime;
            yield return null;
        }

        // 마지막에는 시작→도착 직선 방향으로 고정
        transform.position = targetPos;
        transform.rotation = Quaternion.Euler(0f, 0f, finalAngle);

        transform.parent = endParent;
        anim.SetBool("fly", false);
    }


    public void StartRotation(Vector3 target)
    {
        anim.SetBool("fly", true);
        StartCoroutine(nameof(ChangeRotation), target);
    }

    public void EndRotation()
    {
        StopCoroutine(nameof(ChangeRotation));
        anim.SetBool("fly", false);
    }
    
    IEnumerator ChangeRotation(Vector3 target)
    {
        while (true)
        {
            Vector2 direction = (target - transform.position).normalized;
            if (direction.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + -135f;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
            yield return null;
        
        }
    }

    public void BugAttack()
    {
        if (attackEffect != null)
        {
            attackEffect.SetActive(true);
        }
    }
    
    public void BugDisappear()
    {
        StartCoroutine(nameof(IEBugGlitch));
    }

    IEnumerator IEBugGlitch()
    {
        Material mat = GetComponent<Renderer>().sharedMaterial;
        mat.SetFloat("NoiseAmount", 5f);
        mat.SetFloat("GlitchStrength", 2f);
        
        Debug.Log("play! bug glitch and disappear");
        
        float time = 0f;
        while (time < 3f)
        {
            time += Time.deltaTime;
            transform.localScale+= new Vector3(0.03f, 0.03f, 0f);
            yield return null;
        }

        time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            mat.SetFloat("AlphaFloat", Mathf.Lerp(1f, 0f, time));
            yield return null;
        }
        mat.SetFloat("GlitchStrength", 30f);
        mat.SetFloat("NoiseAmount", 2f);
        
        gameObject.SetActive(false);
    }

}
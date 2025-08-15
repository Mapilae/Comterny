using System.Collections;
using UnityEngine;

public class MemoryBugObject : MonoBehaviour
{
    public bool colEnableAtStart; // 시작할 때 충돌체를 활성화할지 여부

    public Transform rotationTarget;
    public Transform goPos1; // 도달할 목표 위치

    public Transform goPos2; // 도달
    public float duration = 1.0f; // 이동에 걸리는 시간
    public float height = 2.0f; // 포물선 최고 높이

    public Transform endParent;

    [Header("Attack effect")] public GameObject attackEffect;
    public GameObject objToEnableAfterFly;
    public Animator anim;
    private Collider2D col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        col = GetComponent<Collider2D>();

        if (col != null) col.enabled = colEnableAtStart; //cannot catch bug at the start
        // MoveDown();

        //anim.SetTrigger("die");

        Debug.Log("bug is flying up");
        var mat = anim.GetComponent<Renderer>().sharedMaterial;
        mat.SetFloat("_GlitchStrength", 30f);
        mat.SetFloat("_NoiseAmount", 2f);

        mat.SetFloat("_AlphaFloat", 1f);
        mat.SetFloat("_IsBugWhite", 0f);
    }

    public void MoveDown()
    {
        transform.parent = null;
        col.enabled = true;
        //       Debug.Log("bug is ready to Catch");
        StartParabola(goPos1);
    }


    public void FlyUp()
    {
        col.enabled = false;
        Debug.Log("bug is flying up");
        var mat = anim.GetComponent<Renderer>().sharedMaterial;
        mat.SetFloat("_GlitchStrength", 30f);
        mat.SetFloat("_NoiseAmount", 2f);

        mat.SetFloat("_AlphaFloat", 1f);
        mat.SetFloat("_IsBugWhite", 0f);
        //gameObject.SetActive(false);
        StartParabola(goPos2);
    }

    public void StartParabola(Transform target)
    {
        anim.SetBool("fly", true);
        // SoundManager.instance.PlaySound("bug flying");
        StartCoroutine(MoveParabola(target.position, duration, height));
    }

    private IEnumerator MoveParabola(Vector2 targetPos, float duration, float height)
    {
        Vector2 startPos = transform.position;
        var time = 0f;
        var finalAngle = Mathf.Atan2(targetPos.y - startPos.y, targetPos.x - startPos.x) * Mathf.Rad2Deg;

        while (time < duration)
        {
            var t = time / duration;

            // 수평 보간
            var linear = Vector2.Lerp(startPos, targetPos, t);

            // 포물선 y값 추가
            var parabolicY = 4f * height * t * (1f - t);
            var newPos = new Vector2(linear.x, linear.y + parabolicY);

            // 회전: 현재 위치(newPos)에서 목표(targetPos)를 바라보도록
            var direction = (targetPos - newPos).normalized;
            if (direction.sqrMagnitude > 0.0001f)
            {
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + -135f;
                rotationTarget.rotation = Quaternion.Euler(0f, 0f, angle);
            }

            transform.position = newPos;
            time += Time.deltaTime;
            yield return null;
        }

        // 마지막에는 시작→도착 직선 방향으로 고정
        transform.position = targetPos;
        rotationTarget.rotation = Quaternion.Euler(0f, 0f, finalAngle);

        transform.parent = endParent;
        anim.SetBool("fly", false);
        objToEnableAfterFly.SetActive(true);

        //SoundManager.instance.StopSound();
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

    private IEnumerator ChangeRotation(Vector3 target)
    {
        while (true)
        {
            Vector2 direction = (target - transform.position).normalized;
            if (direction.sqrMagnitude > 0.0001f)
            {
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + -135f;
                rotationTarget.rotation = Quaternion.Euler(0f, 0f, angle);
            }

            yield return null;
        }
    }

    public void BugAttack()
    {
        if (attackEffect != null) attackEffect.SetActive(true);
    }

    public void BugDisappear()
    {
        StartCoroutine(nameof(IEBugGlitch));
        col.enabled = false;
    }

    private IEnumerator IEBugGlitch()
    {
        var mat = anim.GetComponent<Renderer>().sharedMaterial;
        mat.SetFloat("_NoiseAmount", 5f);
        mat.SetFloat("_GlitchStrength", 2f);
        mat.SetFloat("_IsBugWhite", 1f); // ⬅ 반드시 언더스코어 포함해야 Shader Graph가 인식

        Debug.Log("play! bug glitch and disappear");

        var time = 0f;
        while (time < 3f)
        {
            time += Time.deltaTime;
            rotationTarget.localScale += new Vector3(0.06f, 0.06f, 0f);
            yield return null;
        }

        time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            mat.SetFloat("_AlphaFloat", Mathf.Lerp(1f, 0f, time));
            yield return null;
        }

        gameObject.transform.localScale = Vector3.zero;
        gameObject.GetComponent<Collider2D>().enabled = false;
        gameObject.GetComponent<InteractableObject>().enabled = false;
        mat.SetFloat("_GlitchStrength", 30f);
        mat.SetFloat("_NoiseAmount", 2f);

        mat.SetFloat("_AlphaFloat", 1f);
        mat.SetFloat("_IsBugWhite", 0f);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
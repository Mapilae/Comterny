using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StandingAnim : MonoBehaviour
{
    [HideInInspector] public Image image;
    private float maskAmount;
    private Material mat;

    private void Awake()
    {
        image = GetComponent<Image>();
        mat = new Material(image.material); // 새 인스턴스 생성
        image.material = mat; // 변경 적용
    }

    public void StandingAlpha(bool isFadeOn)
    {
        if (!gameObject.activeInHierarchy) return;
        if (image.sprite == null)
        {
            image.enabled = false;
            return;
        }

        StartCoroutine(nameof(StandingAlphaRoutine), isFadeOn);
    }

    private IEnumerator StandingAlphaRoutine(bool isOn)
    {
        image.enabled = true;
        var isOnMask = isOn ? 1 : -1;
        maskAmount = isOn ? 0 : 1;
        image.material = mat;

        while (true)
        {
            maskAmount += isOnMask * Time.deltaTime * 3f;
            mat.SetFloat("_AlphaFloat", maskAmount);

            if (isOn && maskAmount >= 1) break;
            if (!isOn && maskAmount <= 0) break;
            yield return null;
        }

        image.material = null;
        image.enabled = isOn;
    }
}
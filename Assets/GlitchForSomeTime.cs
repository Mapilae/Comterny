using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GlitchForSomeTime : MonoBehaviour
{
    public float coolTime = 3f;
    public float noiseAmount = 50f;
    private Graphic graphic;
    private Material mat;
    private Renderer rend;

    private void Start()
    {
        //graphic = GetComponent<Graphic>();

        graphic = GetComponent<Graphic>();
        rend = GetComponent<Renderer>();

        if (graphic != null)
            mat = Instantiate(graphic.material);
        else if (rend != null)
            mat = Instantiate(rend.material);

        StartGlitch();
    }

    private IEnumerator Glitch()
    {
        while (true)
        {
            //glitch 없는 상태
            mat.SetFloat("_GlitchStart", 0f);
            mat.SetFloat("_GlitchStrength", 0f);
            mat.SetFloat("_NoiseAmount", 0f);

            if (graphic != null)
                graphic.material = mat;
            else if (rend != null)
                rend.material = mat;
            yield return new WaitForSeconds(coolTime);
            //glitch 효과를 줌
            mat.SetFloat("_GlitchStart", 0.2f);
            mat.SetFloat("_GlitchStrength", 3);
            mat.SetFloat("_NoiseAmount", noiseAmount);
            if (graphic != null)
                graphic.material = mat;
            else if (rend != null)
                rend.material = mat;
            yield return new WaitForSeconds(1f);
        }
    }

    private void StartGlitch()
    {
        StartCoroutine(Glitch());
    }
}
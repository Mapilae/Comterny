using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GlitchForSomeTime : MonoBehaviour
{
   public float coolTime=3f;
   private Graphic graphic;
   private Material mat;
   public float noiseAmount = 50f;
    void Start()
    {
        graphic= GetComponent<Graphic>();
        mat = Instantiate(graphic.material);
        graphic.material = mat;

        StartGlitch();
    }
    IEnumerator Glitch()
    {
        while (true)
        {
            //glitch 없는 상태
            mat.SetFloat("_GlitchStart", 0f);
            mat.SetFloat("_GlitchStrength", 0f);
            mat.SetFloat("_NoiseAmount", 0f);
            graphic.material = mat;
            yield return new WaitForSeconds(coolTime);
            //glitch 효과를 줌
            mat.SetFloat("_GlitchStart", 0.2f);
            mat.SetFloat("_GlitchStrength", 3);
            mat.SetFloat("_NoiseAmount", noiseAmount);
            graphic.material = mat;
            yield return new WaitForSeconds(1f);
        }
        
    }

    void StartGlitch()
    {
        StartCoroutine(Glitch());
    }
}

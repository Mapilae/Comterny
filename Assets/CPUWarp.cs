using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CPUWarp : MonoBehaviour
{
    private float stayingTime = 0f;
    bool isStaying = false;

    public Image waitTimer;
    public TMP_Text waitTxt;
    public static bool canWarp = false;
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canWarp) return;
        if (other.CompareTag("Player"))
        {
            isStaying = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (!canWarp) return;
        if (other.CompareTag("Player"))
        {
            isStaying = false;
            stayingTime = 0f;
            waitTimer.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!canWarp) return;
        if (isStaying)
        {
            stayingTime += Time.deltaTime;

            if (stayingTime >= 1.5f)
            {
                waitTimer.gameObject.SetActive(true);
                waitTimer.fillAmount = (stayingTime-1.5f) / 3f;
                //3 2 1
                //
                waitTxt.text = Math.Ceiling(3f-(stayingTime-1.5f)).ToString();
              
                if (waitTimer.fillAmount >= 1)
                {
                    
                    waitTimer.fillAmount = 1;
                    isStaying = false;
                    MoveScene();
                    return;
                }
            }
        }
    }

    void MoveScene()
    {
        //move scene call
        LodingScene.instance.LoadScene();
    }
}

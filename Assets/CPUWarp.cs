using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CPUWarp : MonoBehaviour
{
    public enum DftWarpState
    {
        OFF,
        ON
    }

    public static bool canWarp = false;

    public DftWarpState warpState = DftWarpState.OFF;

    public Image waitTimer;
    public TMP_Text waitTxt;
    private bool isStaying;

    private float stayingTime;

    private void Start()
    {
        if (warpState.Equals(DftWarpState.ON))
            GetComponent<Animator>().SetTrigger("WarpIdle");
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
                waitTimer.fillAmount = (stayingTime - 1.5f) / 3f;
                //3 2 1
                //
                waitTxt.text = Math.Ceiling(3f - (stayingTime - 1.5f)).ToString();

                if (waitTimer.fillAmount >= 1)
                {
                    waitTimer.fillAmount = 1;
                    isStaying = false;
                    MoveScene();
                }
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canWarp) return;
        if (other.CompareTag("Player")) isStaying = true;
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

    private void MoveScene()
    {
        //move scene call
        LodingScene.instance.LoadScene();
    }
}
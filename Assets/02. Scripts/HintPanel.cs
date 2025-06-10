using System;
using UnityEngine;
using UnityEngine.UI;

public class HintPanel : MonoBehaviour
{
    
    public Image hintImage;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SetHintImage(Sprite sprite)
    {
        hintImage.sprite = sprite;
    }


    public void HideHint()
    {
        anim.SetTrigger("off");
        
    }

    public void DisableHint()
    {
        hintImage.transform.parent.gameObject.SetActive(false); 
        hintImage.sprite = null;
    }
}

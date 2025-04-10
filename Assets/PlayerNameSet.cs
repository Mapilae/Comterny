using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameSet : MonoBehaviour
{
    public string strName;
    public TMP_InputField nameField;
     Animator anim;
   
    public TMP_Text dateText;
    public GameObject nextTab;
    
    private void Start()
    {
        anim = GetComponent<Animator>();
        dateText.text = DateTime.Today.ToString("yyyy. M. d.");
    }

    public void SetName()
    {
        if (nameField.text.Replace(" ", "").Length == 0)
        {
            Debug.Log("name is not filled!");
            nameField.text = "유저 디버그";
        }

        nameField.interactable = false;
        
        strName = nameField.text;
            //PlayerPrefs.SetString("PlayerName", strName);
            //PlayerPrefs.Save();
       
        PlayerData.instance.SetName(strName);
        Debug.Log("Player Name Set: " + strName);
        anim.SetTrigger("move");
        StartCoroutine(PlayNextTab());
    }
    
    IEnumerator PlayNextTab()
    {
        yield return new WaitForSeconds(1.5f);
        nextTab.SetActive(true);
    }
    
    public void SetDifficulty(int diff)
    {
        PlayerData.instance.SetDifficulty(diff);
        Debug.Log("Player Difficulty Set: " + diff);
        
        LodingScene.instance.LoadScene();
    }
    
}

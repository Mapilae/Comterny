using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

[System.Serializable]
public class ParityBlock
{
    public string parityCode;
    public bool isParityRight;
    public Button objBtn;
    [HideInInspector] public GameObject obj;
    public TMP_Text bitText;
}

public class ParityBit : MonoBehaviour
{
    public List<ParityBlock>  parityBlocks = new List<ParityBlock>();
    
    public void CreateParity(int index)
    {
        int rand = Random.Range(0, 2);

        parityBlocks[index].isParityRight = rand == 0;
        char[] code= "00000000".ToCharArray();

        int randInt = Random.Range(0, 128);
        int parityCount = 0;
        
        if (randInt / 64 > 1)
        {
            code[1] = '1';
            randInt -= 64;
            parityCount++;
        }
        if (randInt / 32 > 1)
        {
            code[0] = '1';
            randInt -= 32;
            parityCount++;
        }
        if (randInt / 16 > 1)
        {
            code[2] = '1';
            randInt -= 16;
            parityCount++;
        }
        if (randInt / 8 > 1)
        {
            code[4] = '1';
            randInt -= 8;
            parityCount++;
        }
        if (randInt / 4 > 1)
        {
            code[7] = '1';
            randInt -= 4;
            parityCount++;
        }
        if (randInt / 2 > 1)
        {
            code[6] = '1';
            randInt -= 2;
            parityCount++;
        }
        if (randInt / 1 > 1)
        {
            code[5] = '1';
            parityCount++;
        }

        if (parityBlocks[index].isParityRight)
            code[3]=parityCount%2==0?'0':'1';
        else
            code[3]=parityCount%2==0?'1':'0';


        parityBlocks[index].bitText.text = parityBlocks[index].parityCode = code.ArrayToString();
        parityBlocks[index].obj.SetActive(true);
    }
    
    public void ResetParity()
    {
  
        for (int i = 0; i < parityBlocks.Count; i++)
        {
            parityBlocks[i].obj=parityBlocks[i].objBtn.gameObject;
            CreateParity(i);
            parityBlocks[i].objBtn.interactable = true;
        }
    }

    public void ParityCheck(int index)
    {

        if (parityBlocks[index].isParityRight)
        {//패리티 맞음. 오류 없음. 잘못 클릭함!
            ResetParity();
        }
        else{//패리티 안맞음. 오류 있음. 사라짐.
            parityBlocks[index].obj.SetActive(false);
            CreateParity(index);
            if (CheckIfFinished())
            {
                EndGame();
            }
        }
        
    }

    public void EndGame()
    {
        for(int i=0;i<parityBlocks.Count;i++)
            parityBlocks[i].objBtn.interactable = false;
       
    }

    bool CheckIfFinished()
    {
        for (int i = 0; i < parityBlocks.Count; i++)
        {
            if (!parityBlocks[i].isParityRight)
            {
                return false;
            }
        }
        return true;
    }

    private void Start()
    {
        ResetParity();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class ParityBlock
{
    public string parityCode;
    public bool isParityRight; //올바른 패리티인지 여부. false인경우 눌러야하는 버튼이 됨.
    public Button objBtn;
    [HideInInspector] public GameObject obj;
    public TMP_Text bitText;
}

public class ParityBit : MonoBehaviour
{
    public List<ParityBlock> parityBlocks = new();
    public GameObject closeABtn;

    [Header("Potal")] public Animator anim;
    public GameObject objToDisable; //퍼즐 오브젝트를 비활성화 시키는 오브젝트

    //varToAdd=135
    private void Start()
    {
        ResetParity();
    }

    private void OnEnable()
    {
        PlayerMovement.canMove = false;
        Pause.instance.SetCurrentPuzzle(gameObject);
    }

    public void Close()
    {
        PlayerMovement.canMove = true;
        Pause.instance.SetCurrentPuzzle(null);
        gameObject.SetActive(false);
    }


    public void CreateParity(int index)
    {
        var rand = Random.Range(0, 5); //0,1,2,3,4
        parityBlocks[index].isParityRight = rand >= 2; //패리티 에러났을 확률 40%

        bool isSolo;
        string code;
        var loopCount = 0; // 추가: 루프 제한용 카운터

        do
        {
            isSolo = true;
            code = CreateParityCode(Random.Range(0, 128), parityBlocks[index].isParityRight);

            for (var i = 0; i < parityBlocks.Count; i++)
                if (i != index && code.Equals(parityBlocks[i].parityCode))
                {
                    isSolo = false;
                    break;
                }

            loopCount++;
            if (loopCount > 50) // 50번 넘게 반복되면 강제 탈출
            {
                Debug.LogWarning("CreateParity() 무한 루프 방지: 강제 탈출");
                break;
            }
        } while (!isSolo);

        parityBlocks[index].bitText.text = parityBlocks[index].parityCode = code;
        parityBlocks[index].obj.SetActive(true);
    }


    private string CreateParityCode(int randInt, bool isParityRight)
    {
        var code = "00000000".ToCharArray();

        var parityCount = 0;

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


        if (isParityRight)
            code[3] = parityCount % 2 == 0 ? '0' : '1';
        else
            code[3] = parityCount % 2 == 0 ? '1' : '0';

        return code.ArrayToString();
    }

    public void ResetParity()
    {
        for (var i = 0; i < parityBlocks.Count; i++)
        {
            parityBlocks[i].obj = parityBlocks[i].objBtn.gameObject;
            CreateParity(i);
            parityBlocks[i].objBtn.interactable = true;
        }
    }

    public void ParityCheck(int index)
    {
        if (parityBlocks[index].isParityRight)
        {
            SoundManager.instance.PlaySound("minigame fail");
            ResetParity();
        }
        else
        {
            SoundManager.instance.PlaySound("signal get");
            StartCoroutine(AnotherBlock(index));
        }
    }

    private IEnumerator AnotherBlock(int index)
    {
        parityBlocks[index].obj.SetActive(false);
        CreateParity(index);

        yield return new WaitForSeconds(1f);
        if (CheckIfFinished())
            EndGame();
        else
            Debug.Log("Not finished");
    }

    public void EndGame()
    {
        //Debug.Log("패리티비트 클리어!");

        for (var i = 0; i < parityBlocks.Count; i++)
            parityBlocks[i].objBtn.interactable = false;
        PlayerData.instance.AddVariable(135);
        StartCoroutine(EndClose());
    }

    private IEnumerator EndClose()
    {
        closeABtn.SetActive(false);
        objToDisable.SetActive(false);
        Debug.Log("Close Parity Bit Minigame...");
        yield return new WaitForSeconds(2.5f);
        anim.SetTrigger("Success");
        SoundManager.instance.PlaySound("minigame clear");
        CPUWarp.canWarp = true;
        Close();
    }

    private bool CheckIfFinished()
    {
        for (var i = 0; i < parityBlocks.Count; i++)
            if (!parityBlocks[i].isParityRight)
                return false;

        return true;
    }
}
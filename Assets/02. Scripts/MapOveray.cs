using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class MapSet
{
    public enum MapBuildingType
    {
        HARDWARE,
        SOFTWARE,
        PROLOGUE,
        EPILOGUE
    }

    public string mapName;
    public string mapENG;
    public MapBuildingType mapBuildingType;
    public Color mapBuildingColor;
    [Range(0, 1)] public float blackAlpha;
    public Sprite sprOveray;
}

public class MapOveray : MonoBehaviour
{
    private static readonly int FadeOut = Animator.StringToHash("FadeOut");
    public static bool isOFF;
    public static MapOveray instance;
    public int thisMapIndex;
    public List<MapSet> mapSets;
    public TMP_Text mapNameTxt;
    public TMP_Text mapENGTxt;
    public TMP_Text mapBuildingTypeTxt;
    public Image overlayImg;
    public Image blackAlphaImg;

    private Animator anim;

    private float multiplier = 1f;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        PlayerMovement.canMove = false;
        PlayerMovement.isCutscene = false;
        anim = GetComponent<Animator>();
        GetComponent<Canvas>().enabled = true;
        SetMap();
        Glitch();
        StartCoroutine(nameof(IEDisableThis)); //auto
        PlayerData.instance.crtStage = thisMapIndex;
    }

    private void SetMap()
    {
        mapNameTxt.text = mapSets[thisMapIndex].mapName;
        mapENGTxt.text = mapSets[thisMapIndex].mapENG;
        mapBuildingTypeTxt.text = mapSets[thisMapIndex].mapBuildingType.ToString();
        overlayImg.sprite = mapSets[thisMapIndex].sprOveray;
        var newCol = blackAlphaImg.color;
        newCol.a = mapSets[thisMapIndex].blackAlpha;
        blackAlphaImg.color = newCol;
        if (thisMapIndex == 2)
        {
            PlayerData.instance.RemoveItem(27);
            PlayerData.instance.RemoveItem(27);
            PlayerData.instance.RemoveItem(27);
            PlayerData.instance.RemoveItem(27);
            PlayerData.instance.RemoveItem(27);
            PlayerData.instance.RemoveItem(27);
            PlayerData.instance.RemoveItem(50);
            PlayerData.instance.RemoveItem(51);
            PlayerData.instance.RemoveItem(52);
            PlayerData.instance.RemoveItem(53);
            PlayerData.instance.RemoveItem(54);
        }
    }

    public void OnClicked()
    {
        //Button click
        multiplier = 2.5f;
        DisableThis();
    }

    private IEnumerator IEDisableThis()
    {
        yield return new WaitForSeconds(3f);
        DisableThis();
    }

    private void DisableThis()
    {
        if (isOFF) return;
        anim.SetTrigger(FadeOut);
        anim.speed = multiplier;
        isOFF = true;
        StartCoroutine(IEFadeOut());
    }

    private void Glitch()
    {
        StartCoroutine(nameof(IEGlitch));
    }

    private IEnumerator IEGlitch()
    {
        while (true)
        {
            //move random position
            var newPos = new Vector2(Random.Range(-7f, 7f), Random.Range(-5f, 5f));
            overlayImg.rectTransform.anchoredPosition = newPos;
            yield return new WaitForSeconds(Random.Range(0.04f, 0.2f));
        }
    }

    private IEnumerator IEFadeOut()
    {
        //overlayImg.rectTransform.anchoredPosition = Vector2.zero;
        var alphaB = blackAlphaImg.color.a;
        var alphaOv = overlayImg.color.a;
        while (alphaB > 0f || alphaOv > 0f)
        {
            alphaB -= Time.deltaTime * 0.5f * multiplier;
            alphaOv -= Time.deltaTime * 0.5f * multiplier;

            overlayImg.color = new Color(1, 1, 1, alphaOv);
            blackAlphaImg.color =
                new Color(blackAlphaImg.color.r, blackAlphaImg.color.g, blackAlphaImg.color.b, alphaB);
            yield return null;
        }

        StopCoroutine(nameof(IEGlitch));
        overlayImg.rectTransform.anchoredPosition = Vector2.zero;
        PlayerMovement.canMove = true;
        SheetLoader.instance.StartDialogue();
        isOFF = false;
        gameObject.SetActive(false);
    }
}
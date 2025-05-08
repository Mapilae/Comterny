using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public int thisMapIndex;
    public List<MapSet> mapSets;

    public TMP_Text mapNameTxt;
    public TMP_Text mapENGTxt;
    public TMP_Text mapBuildingTypeTxt;
    public Image overlayImg;
    public Image blackAlphaImg;

    private Animator anim;
    private bool isOFF;

    private void Start()
    {
        anim = GetComponent<Animator>();
        Invoke("OnClicked", 3f); //auto
        SetMap();
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
    }

    public void OnClicked()
    {
        isOFF = true;
        //Button click
        anim.SetTrigger("FadeOut");
        Invoke("DisableThis", 2f);
    }

    private void DisableThis()
    {
        if (isOFF) return;
        gameObject.SetActive(false);
    }
}
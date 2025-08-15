using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class SortClass
{
    public SortingGroup sortingGroup;
    public int layerOrderDft;
    public int layerOrderEnter;

    public SortClass()
    {
        sortingGroup = new SortingGroup();
        layerOrderDft = 0;
        layerOrderEnter = 0;
    }
}

public class ChangeLayerOrder : MonoBehaviour
{
    [Header("변경이 영구적인지 여부")] public bool isPermanent; //변경이 영구적인가

    public SortClass[] sorts;

    public GameObject[] objToEnableWhenEnter;
    public GameObject[] objToDisableWhenEnter;

    public Collider2D[] colliderToEnableWhenEnter;
    public Collider2D[] colliderToDisableWhenEnter;
    public Canvas[] canvasToDisableWhenEnter;


    private void OnTriggerEnter2D(Collider2D other)
    {
        SetSorting(other, true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        SetSorting(other, false);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        SetSorting(other, true);
    }

    private void SetSorting(Collider2D other, bool isEnter)
    {
        if (!gameObject.activeInHierarchy) return;

        if (other.CompareTag("Player") || other.CompareTag("NPC Follow"))
        {
            if (isEnter)
            {
                if (sorts != null)
                    for (var i = 0; i < sorts.Length; i++)
                        sorts[i].sortingGroup.sortingOrder = sorts[i].layerOrderEnter;
            }
            else
            {
                if (sorts != null)
                    for (var i = 0; i < sorts.Length; i++)
                        if (sorts[i] != null && sorts[i].sortingGroup.gameObject.activeInHierarchy)
                            sorts[i].sortingGroup.sortingOrder = sorts[i].layerOrderDft;
            }

            if (other.CompareTag("Player"))
            {
                if (objToDisableWhenEnter != null)
                {
                    foreach (var obj in objToDisableWhenEnter) obj.SetActive(!isEnter);

                    if (isPermanent) objToDisableWhenEnter = null;
                }

                if (objToEnableWhenEnter != null)
                {
                    foreach (var obj in objToEnableWhenEnter)
                        obj.SetActive(isEnter);
                    if (isPermanent) objToEnableWhenEnter = null;
                }

                if (colliderToEnableWhenEnter != null)
                {
                    foreach (var obj in colliderToEnableWhenEnter)
                        obj.enabled = isEnter;
                    if (isPermanent) colliderToEnableWhenEnter = null;
                }

                if (colliderToDisableWhenEnter != null)
                {
                    foreach (var obj in colliderToDisableWhenEnter)
                        obj.enabled = !isEnter;
                    if (isPermanent) colliderToDisableWhenEnter = null;
                }

                if (canvasToDisableWhenEnter != null)
                {
                    foreach (var obj in canvasToDisableWhenEnter)
                        obj.enabled = !isEnter;
                    if (isPermanent) canvasToDisableWhenEnter = null;
                }
            }
        }
    }
}
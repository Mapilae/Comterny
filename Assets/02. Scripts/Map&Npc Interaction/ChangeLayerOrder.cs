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
    public SortClass[] sorts;

    public GameObject[] objToEnableWhenEnter;
    public GameObject[] objToDisableWhenEnter;

    public Collider2D[] colliderToEnableWhenEnter;
    public Collider2D[] colliderToDisableWhenEnter;


    private void OnTriggerEnter2D(Collider2D other)
    {
        SetSorting(other, true);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        SetSorting(other, true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        
            SetSorting(other, false);
    }
    void SetSorting(Collider2D other, bool isEnter)
     {
         if (other.CompareTag("Player")||other.CompareTag("NPC"))
         {
             if (isEnter)
             {
                 if (sorts != null)
                 {
                    for (int i = 0; i < sorts.Length; i++)
                     sorts[i].sortingGroup.sortingOrder = sorts[i].layerOrderEnter;
                 }

             }
             else
             {
                 for (int i = 0; i < sorts.Length; i++)
                        if (sorts[i].sortingGroup.gameObject.activeInHierarchy)
                            sorts[i].sortingGroup.sortingOrder = sorts[i].layerOrderDft;
             }
             if(other.CompareTag("Player")){
                 if (objToDisableWhenEnter.Length > 0)
                     foreach (GameObject obj in objToDisableWhenEnter)
                         obj.SetActive(!isEnter);
                 if (objToEnableWhenEnter.Length > 0)
                     foreach (GameObject obj in objToEnableWhenEnter)
                         obj.SetActive(isEnter);
                 if (colliderToEnableWhenEnter.Length > 0)
                     foreach (Collider2D obj in colliderToEnableWhenEnter)
                         obj.enabled = isEnter;
                 if (colliderToDisableWhenEnter.Length > 0)
                     foreach (Collider2D obj in colliderToDisableWhenEnter)
                         obj.enabled = !isEnter;
                 
             }
         }
     }
}


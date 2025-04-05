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
        if (other.CompareTag("Player"))
        {
            if (sorts != null)
                for (int i = 0; i < sorts.Length; i++)
                    sorts[i].sortingGroup.sortingOrder = sorts[i].layerOrderEnter;
            Debug.Log("sort order changed!");
            if (objToDisableWhenEnter.Length > 0)
                foreach (GameObject obj in objToDisableWhenEnter)
                    obj.SetActive(false);
            if (objToEnableWhenEnter.Length > 0)
                foreach (GameObject obj in objToEnableWhenEnter)
                    obj.SetActive(true);
            if (colliderToEnableWhenEnter.Length > 0)
                foreach (Collider2D obj in colliderToEnableWhenEnter)
                    obj.enabled = true;
            if (colliderToDisableWhenEnter.Length > 0)
                foreach (Collider2D obj in colliderToDisableWhenEnter)
                    obj.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < sorts.Length; i++)
                if (sorts[i].sortingGroup.gameObject.activeInHierarchy)
                    sorts[i].sortingGroup.sortingOrder = sorts[i].layerOrderDft;

            if (objToDisableWhenEnter.Length > 0)
                foreach (GameObject obj in objToDisableWhenEnter)
                    obj.SetActive(true);
            if (objToEnableWhenEnter.Length > 0)
                foreach (GameObject obj in objToEnableWhenEnter)
                    obj.SetActive(false);
            if (colliderToEnableWhenEnter.Length > 0)
                foreach (Collider2D obj in colliderToEnableWhenEnter)
                    obj.enabled = false;
            if (colliderToDisableWhenEnter.Length > 0)
                foreach (Collider2D obj in colliderToDisableWhenEnter)
                    obj.enabled = true;
        }
    }
}
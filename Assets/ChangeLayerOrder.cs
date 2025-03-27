using UnityEngine;
using UnityEngine.Rendering;

public class ChangeLayerOrder : MonoBehaviour
{
    SortingGroup sortGroup;
    public int layerOrderDft = 0;
    public int layerOrderEnter = 0;
    
    public GameObject[] objToEnableWhenEnter;
    public GameObject[] objToDisableWhenEnter;
    
    
    void Start()
    {
        sortGroup = GetComponent<SortingGroup>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")){
            if(sortGroup!=null) sortGroup.sortingOrder = layerOrderEnter;
            Debug.Log("sort order changed!");
            if(objToDisableWhenEnter.Length>0) foreach(GameObject obj in objToDisableWhenEnter) obj.SetActive(false);
            if(objToEnableWhenEnter.Length>0) foreach(GameObject obj in objToEnableWhenEnter) obj.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(sortGroup!=null) sortGroup.sortingOrder = layerOrderDft;
          
            if(objToDisableWhenEnter.Length>0) foreach(GameObject obj in objToDisableWhenEnter) obj.SetActive(true);
            if(objToEnableWhenEnter.Length>0) foreach(GameObject obj in objToEnableWhenEnter) obj.SetActive(false);
        }
    }
}

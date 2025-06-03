using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QueueMinigame : MonoBehaviour
{
    private BoxCollider2D blockBoxCol;
    bool isSuccess = false;
    private List<GameObject> disableObjs;
    public Transform spawnPoint;
    
    
    void Start()
    {
        disableObjs = new List<GameObject>();
        blockBoxCol = transform.GetChild(0).GetComponent<BoxCollider2D>();
    }

    public void PushOne()
    {
        if (disableObjs.Count == 0) return;
        if (PlayerData.instance.IsItemExist(27))
        {
            PlayerData.instance.RemoveItem(27);
            GameObject obj = disableObjs[0];
            disableObjs.RemoveAt(0);
            obj.transform.position = spawnPoint.position;
            obj.GetComponent<Rigidbody2D>().AddForce(Vector2.right*2, ForceMode2D.Impulse);
            obj.SetActive(true);
        }
    }
    
    public void PopOne()
    {
        if(isSuccess)return;
        blockBoxCol.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("box"))
        {
//            Debug.Log("pop");
            disableObjs.Add(other.gameObject);
            other.gameObject.SetActive(false);
            PlayerData.instance.AddItem(27);//data box item
            blockBoxCol.enabled = true;
        }
    }

}

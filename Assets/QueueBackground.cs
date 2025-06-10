using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueBackground : MonoBehaviour
{
    public Transform spawnPoint;
    private GameObject blockBoxCol;
   // private List<GameObject> disableObjs;
    private List<GameObject> waitToBeSpawn;
    
    private void Start()
    {
        waitToBeSpawn = new List<GameObject>();
        blockBoxCol = transform.GetChild(0).gameObject;
        StartCoroutine(IEPushObjInList());
        //blockBoxCol.SetActive(true);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("box"))
        {
            other.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            waitToBeSpawn.Add(other.gameObject);
            other.gameObject.SetActive(false);
            blockBoxCol.SetActive(true);
        }
    }
    
    IEnumerator IEPushObjInList()
    {
        while (true)
        {
            if(waitToBeSpawn.Count>0){
                blockBoxCol.SetActive(false);
                var obj = waitToBeSpawn[0];
                waitToBeSpawn.RemoveAt(0);
                obj.transform.position = spawnPoint.position;
                obj.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 2, ForceMode2D.Impulse);
                obj.SetActive(true);
            }
            yield return new WaitForSeconds(4f);
        }
    }
}

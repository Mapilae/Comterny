using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueMinigame : MonoBehaviour
{
    public Transform spawnPoint;
    private GameObject blockBoxCol;
    private List<GameObject> disableObjs;
    private readonly bool isSuccess = false;

    private List<GameObject> waitToBeSpawn;
    

    private void Start()
    {
        disableObjs = new List<GameObject>();
        waitToBeSpawn = new List<GameObject>();
        blockBoxCol = transform.GetChild(0).gameObject;
        StartCoroutine(IEPushObjInList());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("box"))
        {
//            Debug.Log("pop");
            disableObjs.Add(other.gameObject);

            if (other.gameObject.transform.childCount > 0)
            {
                //if box has a bug obj, it is a success
                Debug.Log("bug 2 caught");
                PlayerData.instance.AddVariable(108);
                other.gameObject.transform.GetChild(0).GetComponent<MemoryBugObject>().MoveDown();
            }


            PlayerData.instance.AddItem(27); //data box item
            other.gameObject.SetActive(false);
            blockBoxCol.SetActive(true);
        }
    }

    public void PushOne()
    {
        if (disableObjs.Count == 0) return;
        
        int holdingItemID =Inventory.instance.currentHoldingItemIndex==-1?
            -1: Inventory.instance.itemHoldList[Inventory.instance.currentHoldingItemIndex].itemID;
        if (holdingItemID.Equals(27))
        {
            //if is box item holding
            PlayerData.instance.RemoveItemAt(Inventory.instance.currentHoldingItemIndex);
            PushObj();
        }
        else if (PlayerData.instance.IsItemExist(27))
        {
            PlayerData.instance.RemoveItem(27);
            PushObj();
        }
    }

    void PushObj()
    {
        var obj = disableObjs[0];
        disableObjs.RemoveAt(0);
        waitToBeSpawn.Add(obj);
    }

    IEnumerator IEPushObjInList()
    {
        while (true)
        {
            if(waitToBeSpawn.Count>0){
                var obj = waitToBeSpawn[0];
                waitToBeSpawn.RemoveAt(0);

                obj.transform.position = spawnPoint.position;
                obj.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 2, ForceMode2D.Impulse);
                obj.SetActive(true);
            }
            yield return new WaitForSeconds(4f);
        }
    }



    public void PopOne()
    {
        if (isSuccess) return;
        blockBoxCol.SetActive(false);
    }
}
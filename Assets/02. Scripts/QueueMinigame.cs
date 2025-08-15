using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueMinigame : MonoBehaviour
{
    public Transform spawnPoint;
    public List<GameObject> disableObjs;

    public List<GameObject> waitToBeSpawn;
    private readonly bool isSuccess = false;
    private GameObject blockBoxCol;


    private void Awake()
    {
        disableObjs = new List<GameObject>();
        waitToBeSpawn = new List<GameObject>();
    }

    private void Start()
    {
        blockBoxCol = transform.GetChild(0).gameObject;
    }

    private void OnEnable()
    {
        StartCoroutine(IEPushObjInList());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("box"))
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


    public int PushOne()
    {
        if (disableObjs.Count == 0) return 0;

        var holdingItemID = Inventory.instance.currentHoldingItemIndex == -1
            ? -1
            : Inventory.instance.itemHoldList[Inventory.instance.currentHoldingItemIndex].itemID;
        if (holdingItemID.Equals(27))
        {
            //if is box item holding
            PlayerData.instance.RemoveItemAt(Inventory.instance.currentHoldingItemIndex);
            PushObj();
            return 0;
        }

        if (PlayerData.instance.IsItemExist(27))
        {
            PlayerData.instance.RemoveItem(27);
            PushObj();
            return 0;
        }

        return -1;
    }

    private void PushObj()
    {
        var obj = disableObjs[0];
        waitToBeSpawn.Add(obj);
        disableObjs.RemoveAt(0);
    }

    private IEnumerator IEPushObjInList()
    {
        while (true)
            if (waitToBeSpawn.Count > 0)
            {
                Debug.Log("waitToBeSpawn");
                var obj = waitToBeSpawn[0];
                obj.transform.position = spawnPoint.position;
                obj.SetActive(true);
                obj.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 2, ForceMode2D.Impulse);
                waitToBeSpawn.RemoveAt(0);
                yield return new WaitForSeconds(4f);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
    }

    public int PopOne()
    {
        if (isSuccess) return 0;
        blockBoxCol.SetActive(false);
        return 0;
    }
}
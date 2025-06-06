using System.Collections.Generic;
using UnityEngine;

public class QueueMinigame : MonoBehaviour
{
    public Transform spawnPoint;
    private BoxCollider2D blockBoxCol;
    private List<GameObject> disableObjs;
    private readonly bool isSuccess = false;


    private void Start()
    {
        disableObjs = new List<GameObject>();
        blockBoxCol = transform.GetChild(0).GetComponent<BoxCollider2D>();
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
            blockBoxCol.enabled = true;
        }
    }

    public void PushOne()
    {
        if (disableObjs.Count == 0) return;
        if (PlayerData.instance.IsItemExist(27))
        {
            PlayerData.instance.RemoveItem(27);
            var obj = disableObjs[0];
            disableObjs.RemoveAt(0);
            obj.transform.position = spawnPoint.position;
            obj.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 2, ForceMode2D.Impulse);
            obj.SetActive(true);
        }
    }

    public void PopOne()
    {
        if (isSuccess) return;
        blockBoxCol.enabled = false;
    }
}
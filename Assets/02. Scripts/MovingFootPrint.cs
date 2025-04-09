using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingFootPrint : MonoBehaviour
{
    public bool isWalking;
    public Rigidbody2D rb;
    public GameObject footprintObj;
    public Transform[] footPos=new Transform[2];
    int poolSize = 2; // object pooling object size
    private int footIsLeft = 0;
    private List<GameObject> footPrintList = new List<GameObject>();
    private int poolIndex = 0;

    private void InitializeFootprintPool()
    {
        if (footprintObj == null) return;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(footprintObj, footprintObj.transform.parent);
            obj.SetActive(false);
            footPrintList.Add(obj);
        }
    }

    public void StartFootPrint()
    {
        rb = GetComponent<Rigidbody2D>();
        InitializeFootprintPool();

        if (rb != null)
            StartCoroutine(CreateFootPrint());
    }

    public void CheckIfWalking()
    {
        if (rb == null) return;
            isWalking = rb.linearVelocity.sqrMagnitude > 0.01f;
    }

    IEnumerator CreateFootPrint()
    {
        while (true)
        {
            if (isWalking)
            {
                GameObject obj = GetPooledFootprint();
                obj.transform.position = footPos[footIsLeft].position;
                obj.transform.rotation = Quaternion.identity;
                obj.SetActive(true);
                footIsLeft = footIsLeft == 0 ? 1 : 0;
            }

            float randomTime = Random.Range(0.05f, 0.1f);
            yield return new WaitForSeconds(randomTime);
        }
    }

    private GameObject GetPooledFootprint()
    {
        if (poolIndex < footPrintList.Count)
        {
            return footPrintList[poolIndex++];
        }
        else
        {
            poolIndex = 0;
            return footPrintList[poolIndex++];
        }

    }
}

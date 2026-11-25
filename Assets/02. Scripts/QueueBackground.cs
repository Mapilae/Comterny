using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueBackground : MonoBehaviour, ISaveableState
{
    public Transform spawnPoint;
    private GameObject blockBoxCol;
   // private List<GameObject> disableObjs;
    public List<GameObject> waitToBeSpawn;
    private bool isRestored; // 꺼진 채로 복원된 경우 나중에 켜질 때 Awake가 비우지 않게

    private void Awake()
    {
        if (isRestored) return;
        waitToBeSpawn = new List<GameObject>();

        //blockBoxCol.SetActive(true);
    }

    // 뒤로 돌아가 다시 나오기를 기다리는 상자들. 저장하지 않으면 이어하기 후 영영 꺼진 채로 남는다.
    public string CaptureState(SceneStateTracker ids)
    {
        if (waitToBeSpawn == null || waitToBeSpawn.Count == 0) return null;
        return string.Join(",", ids.IdsOf(waitToBeSpawn));
    }

    public void RestoreState(string data, SceneStateTracker ids)
    {
        var idList = new List<int>();
        foreach (var s in data.Split(','))
            if (int.TryParse(s, out var id))
                idList.Add(id);
        waitToBeSpawn = ids.ObjectsAt(idList);
        isRestored = true;
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
            if (waitToBeSpawn.Count > 0)
            {
                Debug.Log("queue background push obj");
                blockBoxCol.SetActive(false);
                var obj = waitToBeSpawn[0];
                waitToBeSpawn.RemoveAt(0);
                obj.transform.position = spawnPoint.position;
                obj.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 2, ForceMode2D.Impulse);
                obj.SetActive(true);
                yield return new WaitForSeconds(4f);
            }
            else yield return new WaitForSeconds(1f);

        }
    }
}

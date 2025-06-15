using System.Collections.Generic;
using UnityEngine;

public class ChangeFloor : MonoBehaviour
{
    public static int floor = 1;
    public int numToChange = 1; // 변경할 층 수

    public GameObject floor1;
    public GameObject floor2;
    public GameObject floor3;

    public List<Collider2D> floor1Col;
    public List<Collider2D> floor2Col;
    public List<Collider2D> floor3Col;

    private void Awake()
    {
        floor = 1;
    }

    private void Start()
    {
        UpdateFloor();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            ChangeFloorNum(numToChange);
    }

    public void ChangeFloorNum(int newFloor)
    {
        floor = newFloor;
        UpdateFloor();
    }

    private void UpdateFloor()
    {
//        Debug.Log(floor+"층으로 변경됨");
        if (floor1 != null) floor1.SetActive(floor == 1);
        if (floor2 != null) floor2.SetActive(floor >= 2);
        if (floor3 != null) floor3.SetActive(floor == 3);

        if (floor1Col != null)
            for (var i = 0; i < floor1Col.Count; i++)
                floor1Col[i].enabled = floor == 1;
        if (floor2Col != null)
            for (var i = 0; i < floor2Col.Count; i++)
                floor2Col[i].enabled = floor == 1;
        if (floor3Col != null)
            for (var i = 0; i < floor3Col.Count; i++)
                floor3Col[i].enabled = floor == 1;
    }
}
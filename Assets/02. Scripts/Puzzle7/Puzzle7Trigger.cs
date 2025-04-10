using System.Diagnostics;
using UnityEngine;

public class Puzzle7Trigger : MonoBehaviour
{
    public GameObject puzzleUI;
    private bool isPlayerNear = false;
    private bool isSolved = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Space))
        {
            if (!isSolved)
                puzzleUI.SetActive(true);
            else
                UnityEngine.Debug.Log("문이 열려 있습니다!");
        }
    }

    public void SetSolved()
    {
        isSolved = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = false;
    }
}

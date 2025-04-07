using Unity.VisualScripting;
using UnityEngine;

public class BackPanel : MonoBehaviour
{
    void OnDisable()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        PlayerMovement.instance.ChangeCanMove(true);
    }
}

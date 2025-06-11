using System;
using UnityEngine;

public class GreedyBeforeCol : MonoBehaviour
{
    private SpriteRenderer parentRender;

    private void Start()
    {
        parentRender = transform.parent.GetComponent<SpriteRenderer>();
        SetColor(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("deadzone"))
            SetColor(true);
    }
    
    

    void SetColor(bool isVisited)
    {
        if (parentRender == null) return;
        
        // 방문 여부에 따라 부모 오브젝트의 색상을 변경
        parentRender.color = isVisited ? Color.yellow : Color.white;
    }
  
}

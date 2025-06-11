using System;
using UnityEngine;

public class GreedyBeforeCol : MonoBehaviour
{
    private SpriteRenderer parentRender;
    public Color[] edgeColors;

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
        parentRender.color = isVisited ? edgeColors[1] : edgeColors[0];
    }
  
}

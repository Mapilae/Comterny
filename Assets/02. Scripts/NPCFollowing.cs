using System;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class NPCFollowing : MonoBehaviour
{
    AIPath aiPath;
    AIDestinationSetter aiDestinationSetter;
    public Transform target;
    SpriteRenderer spriteRenderer;
    SortingGroup sortingGroup;
    int sortingOrder;
    
    private void Start()
    {
        aiPath = GetComponent<AIPath>();
        aiDestinationSetter= GetComponent<AIDestinationSetter>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        sortingGroup = GetComponent<SortingGroup>();
        sortingOrder=sortingGroup.sortingOrder;
        target = aiDestinationSetter.target;
    }

    void Update()
    {
        if (target != null)
        {
            //플레이어의 위치에 따라 보는방향 변경
            spriteRenderer.flipX = target.position.x - transform.position.x>=0;
            
            sortingGroup.sortingOrder = (sortingOrder+(target.position.y-transform.position.y>=0?0:-2));
        }
        
    }

}

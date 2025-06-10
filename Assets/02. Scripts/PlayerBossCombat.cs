using System;
using UnityEngine;

public class PlayerBossCombat : MonoBehaviour
{
    public Collider2D beforeCollider;//플레이어가 예전에 썼던 상호작용 목적의 콜라이더.
                                     //이걸 그대로 쓰면 판정이 이상해지기 때문에 보다 정확한 콜라이더를 새로 만들어서 쓴다.
    
         
    void Start()
    {
        beforeCollider.enabled = false;
        PlayerMovement.instance.forceRun= true; //플레이어가 달리게 한다.
        CameraFollow.instance.enabled = false;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("deadzone"))
        {
            Dead();   
        }
    }
    
    

    void Dead()
    {
        Debug.Log("You are dead!");
    }

}

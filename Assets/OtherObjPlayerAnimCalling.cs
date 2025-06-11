using System.Collections;
using UnityEngine;

public class OtherObjPlayerAnimCalling : MonoBehaviour
{
    public void PlayerSpineAnimBug()
    {
        // 플레이어 애니메이션을 버그 애니메이션으로 변경
        PlayerMovement.instance.PlayerSpineAnimBug();
    }

    public void PlayerSpineAnimIdle()
    {
        // 플레이어 애니메이션을 대기 애니메이션으로 변경
        PlayerMovement.instance.PlayerSpineAnimIdle();
    }
}

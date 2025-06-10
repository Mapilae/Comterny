using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StandingSprite")]
public class StandingSpriteManager : ScriptableObject
{
    public Sprite nullSprite;
    public List<CharacterSpr> userSpr;
    public List<CharacterSpr> ramSpr;
    public List<CharacterSpr> processorSpr;
    public List<CharacterSpr> compilerSpr;
    public List<CharacterSpr> ransomwareSpr;

    public string[] StandingNumParse(string chName, string standingNum)
    {
        if (standingNum.Contains(':')) return standingNum.Split(':');

        return new[]
        {
            chName, standingNum
        };
    }

    public Sprite FindSprite(string chName, string standingNum)
    {
        if (chName.Equals("")) return nullSprite;

        List<CharacterSpr> sprListWhole;

        switch (chName)
        {
            case "유저 디버그": //user debug
                sprListWhole = userSpr;
                break;
            case "램": //RAM
                sprListWhole = ramSpr;
                break;
            case "프로세서": //Processor
                sprListWhole = processorSpr;
                break;
            case "컴파일러":
                sprListWhole = compilerSpr;
                break;
            case "랜섬웨어":
            case "랜섬웨어빌런":
                sprListWhole = ransomwareSpr;
                break;


            default:
                sprListWhole = null;
                break;
        }

        /* switch (iChapter)
         {
             case 1:
                 sprList = sprListWhole.chapter1;
                 break;
             case 2:
                 sprList = sprListWhole.chapter1;//2
                 break;
             default:
                 sprList = new();
                 break;
         }*/

        if (sprListWhole == null) return nullSprite;
        
        //Parse 가능하면 하고, 불가 ㄴ시 0을 디폴트로 넘기기
        if (!int.TryParse(standingNum, out int result)) result = 0;
        
        var spr = sprListWhole[result].sprite;

        return spr;
    }

    [Serializable]
    public class CharacterSpr
    {
        public string name;
        public Sprite sprite;
    }
}
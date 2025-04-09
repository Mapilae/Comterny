using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "StandingSprite")]
public class StandingSpriteManager : ScriptableObject
{

    [System.Serializable]
    public class CharacterSpr
    {
        public string name;
        public Sprite sprite;
    }

    public List<CharacterSpr> userSpr;
    public List<CharacterSpr> ramSpr;
    public List<CharacterSpr> processorSpr;
    public List<CharacterSpr> compilerSpr;
    public List<CharacterSpr> ransomwareSpr;


    public Sprite FindSprite(string chName, int standingNum)
    {
        if (chName.Equals("")) return null;


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

        if (sprListWhole == null) return null;
        Sprite spr = sprListWhole[standingNum].sprite;

        return spr;
    }


}

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

    [System.Serializable]
    public class CharacterSprList
    {
        //public string characterName;
        public List<CharacterSpr> chapter1;
    }

    //public List<CharacterSpr> enjolrasSpr;
    //public List<CharacterSpr> grantaireSpr;

    public bool isShortHair = false;

    public CharacterSprList enjolrasSpr;
    public CharacterSprList grantaireSpr;
    public CharacterSprList combeferreSpr;
    public CharacterSprList courfeyracSpr;
    public CharacterSprList jolySpr;
    public CharacterSprList lamarqueSpr;
    public CharacterSprList musichettaSpr;

    int iChapter = 1;//나중에 findsprite의 파라미터로 옮기기
    public Sprite FindSprite(string chName, string face)
    {
        if (chName.Equals("")) return null;

        Sprite spr = null;
        CharacterSprList sprListWhole;
        List<CharacterSpr> sprList;

        switch (chName)
        {
            case "R":
                sprListWhole = grantaireSpr;
                break;
            case "ENJ":
                sprListWhole = enjolrasSpr;
                break;
            case "COM":
                sprListWhole = combeferreSpr;
                break;
            case "X":
                return null;
            case "CUF":
                sprListWhole = courfeyracSpr;
                break;
            case "LMQ":
                sprListWhole = lamarqueSpr;
                break;

            case "JOL":
                sprListWhole = jolySpr;
                break;
            case "MUS":
                sprListWhole = musichettaSpr;
                break;
            default:
                sprListWhole = new();
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
        sprList = sprListWhole.chapter1;
        if (face.Replace(" ", "").Equals("")) face = "DEFAULT";

        if (isShortHair && chName.Equals("ENJOLRAS"))
        {
            face += "2";
        }
        for (int i = 0; i < sprList.Count; i++)
        {
            if (sprList[i].name.Equals(face))
            {
                spr = sprList[i].sprite;
                break;
            }
        }

        return spr;
    }


}

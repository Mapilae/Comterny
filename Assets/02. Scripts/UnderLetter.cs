using UnityEngine;

public class UnderLetter
{
    /// <summary>
    /// 가갸거겨고교구규그기게계개걔까꺄꺼껴꼬꾜꾸뀨끄끼께꼐깨꺠
    /// 나냐너녀노뇨누뉴느니네녜내냬다댜더뎌도됴두듀드디데뎨대댸
    /// 따땨떠뗘또뚀뚜뜌뜨띠떼뗴때떄라랴러려로료루류르리레례래럐
    /// 마먀머며모묘무뮤므미메몌매먜바뱌버벼보뵤부뷰브비베볘배뱨
    /// 빠뺘뻐뼈뽀뾰뿌쀼쁘삐뻬뼤빼뺴사샤서셔소쇼수슈스시세셰새섀
    /// 싸쌰써쎠쏘쑈쑤쓔쓰씨쎄쎼쌔썌아야어여오요우유으이에예애얘
    /// 자쟈저져조죠주쥬즈지제졔재쟤짜쨔쩌쪄쪼쬬쭈쮸쯔찌쩨쪠째쨰
    /// 차챠처쳐초쵸추츄츠치체쳬채챼카캬커켜코쿄쿠큐크키케켸캐컈
    /// 타탸터텨토툐투튜트티테톄태턔파퍄퍼펴포표푸퓨프피페폐패퍠
    /// 하햐허혀호효후휴흐히헤혜해햬
    /// </summary>
    static string no_underletter = "AaDdEeFfGgHhIiJjKkOoQqRrSsUuVvWwXxYyZz가갸거겨고교구규그기게계개걔까꺄꺼껴꼬꾜꾸뀨끄끼께꼐깨꺠나냐너녀노뇨누뉴느니네녜내냬다댜더뎌도됴두듀드디데뎨대댸따땨떠뗘또뚀뚜뜌뜨띠떼뗴때떄라랴러려로료루류르리레례래럐마먀머며모묘무뮤므미메몌매먜바뱌버벼보뵤부뷰브비베볘배뱨빠뺘뻐뼈뽀뾰뿌쀼쁘삐뻬뼤빼뺴사샤서셔소쇼수슈스시세셰새섀싸쌰써쎠쏘쑈쑤쓔쓰씨쎄쎼쌔썌아야어여오요우유으이에예애얘자쟈저져조죠주쥬즈지제졔재쟤짜쨔쩌쪄쪼쬬쭈쮸쯔찌쩨쪠째쨰차챠처쳐초쵸추츄츠치체쳬채챼카캬커켜코쿄쿠큐크키케켸캐컈타탸터텨토툐투튜트티테톄태턔파퍄퍼펴포표푸퓨프피페폐패퍠하햐허혀호효후휴흐히헤혜해햬";


    public static bool HasUnderLetter(string str)
    {
        bool hasUnderletter = true;
        char c = str[^1];
        for (int i = 0; i < no_underletter.Length; i++)
        {
            if (c.Equals(no_underletter[i]))
            {
                hasUnderletter = false;
                // Debug.Log(str + "가~~  " + str + "는~~"); 
                break;
            }

        }
        if (hasUnderletter) Debug.Log(str + "이~~  " + str + "은~~");

        return hasUnderletter;
    }

    public static char SetUnderLetterEnd(string str, char endLetter)
    {
        bool hasUnderletter = HasUnderLetter(str);//받침이 있는가? ex. 쿠르페락

        switch (endLetter)
        {
            case '이':
            case '가':
                return hasUnderletter ? '이' : '가';
            case '을':
            case '를':
                return hasUnderletter ? '을' : '를';
            case '은':
            case '는':
                return hasUnderletter ? '은' : '는';
            case '과':
            case '와':
                return hasUnderletter ? '과' : '와';


        }

        return ' ';
    }

    public static string SetUnderLetter(string str, char endLetter)
    {
        return str + SetUnderLetterEnd(str, endLetter);
    }
}

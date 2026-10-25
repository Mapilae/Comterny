using System.Text;
using UnityEngine;

/// <summary>
/// 보스전 피격 시 플레이어 이름이 암호화(손상)되는 표기를 만든다.
/// 0단계: 렘  →  1단계: 레0x124521  →  2단계: ㄹ0x13452  →  (한 번 더 맞으면 게임 오버)
/// </summary>
public static class BossNameCorruptor
{
    public const int MaxLevel = 2; // 이 단계에서 한 번 더 맞으면 게임 오버

    private static readonly char[] Choseong = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ".ToCharArray();
    private const string HexDigits = "0123456789ABCDEF";

    private static bool IsHangulSyllable(char c) => c >= 0xAC00 && c <= 0xD7A3;

    public static string RandomHex(int digits)
    {
        var sb = new StringBuilder("0x");
        for (int i = 0; i < digits; i++) sb.Append(HexDigits[Random.Range(0, HexDigits.Length)]);
        return sb.ToString();
    }

    public static string Corrupt(string name, int level)
    {
        if (string.IsNullOrEmpty(name)) name = "USER";
        if (level <= 0) return name;

        if (level == 1)
        {
            // 마지막 글자의 받침을 떼어낸다 (렘 → 레). 받침이 없으면 초성만 남긴다 (오지 → 오ㅈ).
            char last = name[name.Length - 1];
            string head = name.Substring(0, name.Length - 1);
            string damaged = "";
            if (IsHangulSyllable(last))
            {
                int code = last - 0xAC00;
                int jong = code % 28;
                damaged = jong > 0 ? ((char)(last - jong)).ToString() : Choseong[code / 588].ToString();
            }
            return head + damaged + RandomHex(6);
        }

        // 2단계 이상: 첫 글자의 초성만 남는다 (렘 → ㄹ).
        char first = name[0];
        string f = IsHangulSyllable(first) ? Choseong[(first - 0xAC00) / 588].ToString() : first.ToString();
        return f + RandomHex(5);
    }
}

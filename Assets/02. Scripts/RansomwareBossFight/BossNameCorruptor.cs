using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 보스전 피격 시 플레이어 이름이 암호화(손상)되어 보이는 표기를 만든다.
/// 기획 예시: 렘 → 레0x124521 → ㄹ0x13452 → (한 번 더 맞으면 게임 오버)
///
/// 임의의 닉네임에 일반화한 규칙
///  0단계: 원래 이름 그대로.
///  1단계: 앞쪽 2/3(올림)만 남기고, 남은 마지막 글자를 한 겹 깨뜨린 뒤 6자리 주소를 붙인다.
///         - 받침이 있으면 받침이 떨어짐   (렘 → 레, 철 → 처)
///         - 받침이 없으면 초성만 남음      (버 → ㅂ, 지 → ㅈ)
///         - 한글이 아니면 '_' 로 깨짐
///  2단계: 첫 글자의 초성(한글이 아니면 첫 글자)만 남기고 5자리 주소를 붙인다.
///
///  예) 렘          → 레0x……        → ㄹ0x……
///      유저 디버그 → 유저 디ㅂ0x……  → ㅇ0x……
///      오지철      → 오ㅈ0x……      → ㅇ0x……
///      Rem         → R_0x……        → R0x……
///
/// 주소(16진수)는 이름+단계로 시드를 잡아 항상 같은 값이 나온다.
/// 글리치 연출용으로 randomHex=true 를 주면 주소만 매번 바뀐다.
/// </summary>
public static class BossNameCorruptor
{
    public const int MaxLevel = 2; // 이 단계에서 한 번 더 맞으면 게임 오버

    private static readonly char[] Choseong = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ".ToCharArray();
    private const string HexDigits = "0123456789ABCDEF";

    private static bool IsHangulSyllable(char c) => c >= 0xAC00 && c <= 0xD7A3;

    /// <summary>음절의 초성 자모 (렘 → ㄹ). 한글 음절이 아니면 그대로.</summary>
    public static char InitialOf(char c) => IsHangulSyllable(c) ? Choseong[(c - 0xAC00) / 588] : c;

    /// <summary>글자를 한 겹 깨뜨린다: 받침 제거 → 없으면 초성만 → 한글 아니면 '_'.</summary>
    public static string DamageOnce(char c)
    {
        if (!IsHangulSyllable(c)) return "_";
        int code = c - 0xAC00;
        int jong = code % 28;
        return jong > 0 ? ((char)(c - jong)).ToString() : InitialOf(c).ToString();
    }

    public static string Hex(int digits, System.Random rng = null)
    {
        var sb = new StringBuilder("0x");
        for (int i = 0; i < digits; i++)
            sb.Append(HexDigits[rng != null ? rng.Next(16) : Random.Range(0, 16)]);
        return sb.ToString();
    }

    public static string RandomHex(int digits) => Hex(digits);

    public static string Corrupt(string name, int level, bool randomHex = false)
    {
        name = string.IsNullOrWhiteSpace(name) ? "USER" : name.Trim();
        if (level <= 0) return name;

        var rng = randomHex ? null : new System.Random(StableHash(name) * 31 + level);

        // 공백을 뺀 실제 글자들의 위치
        var letterIdx = new List<int>();
        for (int i = 0; i < name.Length; i++)
            if (!char.IsWhiteSpace(name[i])) letterIdx.Add(i);

        if (level == 1)
        {
            int keep = Mathf.Max(1, Mathf.CeilToInt(letterIdx.Count * 2f / 3f));
            int lastPos = letterIdx[keep - 1];
            return name.Substring(0, lastPos) + DamageOnce(name[lastPos]) + Hex(6, rng);
        }

        return InitialOf(name[letterIdx[0]]) + Hex(5, rng);
    }

    private static int StableHash(string s)
    {
        unchecked
        {
            int h = 17;
            foreach (char c in s) h = h * 31 + c;
            return h & 0x3FFFFFF;
        }
    }
}

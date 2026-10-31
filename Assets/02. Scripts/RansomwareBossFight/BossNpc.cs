using TMPro;
using UnityEngine;

public enum NpcKind { None, Ram, Processor }

/// <summary>
/// 보스전 구출 NPC (boss_램 / boss_프로세서 프리팹에 붙는다).
/// 대사와 말풍선 출력, 이름표 글리치, 구출 시 외형 전환, 플레이어 추적을 맡는다.
/// 구출 판정과 패시브 효과는 RansomwareBossFight가 처리한다.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class BossNpc : MonoBehaviour
{
    [Header("대사")]
    [TextArea] public string rescueLine;
    [TextArea] public string actionLine;
    [Tooltip("갇혀 있을 때 맞는 열쇠 없이 플레이어가 닿으면 하는 말")]
    [TextArea] public string lockedLine;
    [Tooltip("구출되지 못한 채로 게임을 클리어해 풀려날 때 하는 말")]
    [TextArea] public string clearLine;

    public NpcKind Kind { get; private set; }
    public int KeyIndex { get; private set; } // 이 NPC를 푸는 열쇠의 주소 = baseAddr + index*4
    public bool Rescued { get; private set; }
    public string DisplayName => Kind == NpcKind.Ram ? "램 주임" : "프로세서 팀장";

    private SpeechBubble bubble;
    private SpriteRenderer sr;
    private TextMeshPro tagLabel, addressLabel;
    private Color color;
    private string corruptedName;
    private Vector2 tagOffset;   // 크기에 맞춘 이름표 위치
    private float followSide;    // 플레이어 옆으로 떨어지는 거리
    private bool usesPrefab;     // 프리팹을 쓰면 구출 시 스케일을 다시 계산하지 않는다
    private bool playerTouching;
    private GameObject guide;    // 갇혀 있는 동안 보이는 표시 (Guide Star 또는 주소 라벨)

    public void Setup(NpcKind kind, int keyIndex, Color npcColor, TextMeshPro tag, TextMeshPro address, GameObject guideMarker, bool fromPrefab)
    {
        Kind = kind;
        KeyIndex = keyIndex;
        color = npcColor;
        tagLabel = tag;
        addressLabel = address;
        guide = guideMarker;
        usesPrefab = fromPrefab;
        sr = GetComponent<SpriteRenderer>();
        bubble = GetComponentInChildren<SpeechBubble>(true);
        corruptedName = BossNameCorruptor.Corrupt(DisplayName, BossNameCorruptor.MaxLevel);
        tagLabel.text = corruptedName;
        RefreshLayout();
    }

    public void SetAddressText(string address) => addressLabel.text = address;

    /// <summary>말풍선으로 대사를 띄운다. 말풍선이 없는 NPC(프리팹 미사용)는 말하지 않는다.</summary>
    public void Speak(string line, float duration = -1f)
    {
        if (bubble != null) bubble.Say(line, duration);
    }

    public void SpeakAction() => Speak(actionLine);

    /// <summary>갇혀 있는 동안 플레이어와 닿았는지 알려준다. 닿는 순간에만 말하고, 떨어졌다가 다시 닿으면 또 말한다.</summary>
    public void SetPlayerTouching(bool touching)
    {
        if (touching && !playerTouching) Speak(lockedLine);
        playerTouching = touching;
    }

    public void Rescue(Sprite freeSprite)
    {
        Free(freeSprite);
        Speak(rescueLine);
    }

    /// <summary>클리어 시 아직 묶여 있으면 풀려난다. 이미 구출된 NPC는 아무 말도 하지 않는다.</summary>
    public void FreeAtClear(Sprite freeSprite)
    {
        if (Rescued) return;
        Free(freeSprite);
        Speak(clearLine);
    }

    private void Free(Sprite freeSprite)
    {
        Rescued = true;
        guide.SetActive(false);
        sr.color = color;
        ChangeAppearance(freeSprite);
        tagLabel.text = DisplayName;
        tagLabel.transform.position = (Vector2)transform.position + tagOffset; // 스프라이트가 그대로인 경우(위 함수가 return)를 위해 재배치
    }

    /// <summary>갇혀 있으면 이름표 글리치, 구출됐으면 플레이어 옆을 따라다닌다.</summary>
    public void Tick(Vector2 playerPos, float dt)
    {
        if (!Rescued)
        {
            // 갇혀 있는 동안은 손상된 플레이어 이름처럼 글리치
            int level = BossNameCorruptor.MaxLevel;
            tagLabel.transform.position = (Vector2)transform.position + tagOffset + Random.insideUnitCircle * (0.02f * level);
            if (Random.value < 0.015f * level) tagLabel.text = BossNameCorruptor.Corrupt(DisplayName, level, randomHex: true);
            else if (Random.value < 0.02f) tagLabel.text = corruptedName;
            return;
        }
        Vector2 target = playerPos + new Vector2(Kind == NpcKind.Ram ? -followSide : followSide, 0.35f);
        transform.position = Vector2.Lerp(transform.position, target, 1f - Mathf.Exp(-6f * dt));
    }

    /// <summary>NPC 스프라이트를 바꾼다. 프리팹 사용 시엔 스케일은 그대로 두고 SpriteRenderer.sprite만 교체한다(프리팹의 Scale은 크기 조정용).
    /// 스프라이트만 바꾸는 경우(비-프리팹)엔 비율이 달라도 화면상 높이를 유지하도록 스케일을 재계산한다. 이름표/추적 거리는 항상 새 크기에 맞춘다.</summary>
    private void ChangeAppearance(Sprite sprite)
    {
        if (sprite == null) return;
        if (usesPrefab)
        {
            sr.sprite = sprite;
        }
        else
        {
            float worldHeight = sr.sprite != null ? sr.sprite.bounds.size.y * transform.localScale.y : transform.localScale.y;
            sr.sprite = sprite;
            float newHeight = sprite.bounds.size.y;
            if (newHeight > 0.0001f)
            {
                float scale = worldHeight / newHeight;
                transform.localScale = new Vector3(scale, scale, 1f);
            }
        }
        RefreshLayout();
    }

    private void RefreshLayout()
    {
        Vector2 pos = transform.position;
        tagOffset = new Vector2(0f, sr.bounds.max.y - pos.y + 0.2f);
        followSide = sr.bounds.extents.x + 0.55f;
        tagLabel.transform.position = pos + tagOffset;
    }
}

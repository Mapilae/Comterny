using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/// <summary>
///     오브젝트 켜고 끄기만으로는 복원되지 않는 내부 상태(미니게임 진행도, static 값 등)를 가진 컴포넌트가 구현한다.
///     <para>
///         CaptureState: 저장할 내용이 없으면 null 또는 빈 문자열을 반환.
///         RestoreState: 이어하기 때 씬의 Start가 모두 끝난 뒤 호출된다. 오브젝트가 꺼져 있어 Awake/Start가 아직 안 돌았을 수도 있다.
///     </para>
/// </summary>
public interface ISaveableState
{
    string CaptureState(SceneStateTracker ids);
    void RestoreState(string data, SceneStateTracker ids);
}

[Serializable]
public class TransformState
{
    public int id;
    public int parent; // -1: 씬 최상위, -2: 추적하지 않는 부모(부모 변경 안 함)
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;
    public bool hasVelocity;
    public Vector2 velocity;
    public float angularVelocity;
}

[Serializable]
public class SpriteState
{
    public int comp;
    public string sprite;
    public string texture;
}

[Serializable]
public class AnimatorState
{
    public int comp;
    public List<int> states = new(); // 레이어별 현재 상태 (fullPathHash)
    public List<float> times = new(); // 레이어별 normalizedTime
    public List<int> paramHashes = new(); // Bool/Int/Float 파라미터 (Trigger 제외)
    public List<float> paramValues = new();
}

[Serializable]
public class ComponentState
{
    public int comp;
    public string data;
}

[Serializable]
public class QuestState
{
    public string name;
    public string content;
}

/// <summary>스테이지 중간 진행 상황 (마지막으로 안정된 순간의 씬 상태)</summary>
[Serializable]
public class StageProgress
{
    public string sceneName;
    public int signature;
    public int objectCount;
    public int componentCount;

    public List<int> items = new();
    public List<int> variables = new();

    public string active; // 오브젝트별 activeSelf ('0'/'1')
    public string enabled; // 컴포넌트별 enabled ('0'/'1')
    public List<TransformState> transforms = new(); // 씬 처음과 달라진 것만
    public List<SpriteState> sprites = new(); // 씬 처음과 달라진 것만
    public List<AnimatorState> animators = new(); // 켜져 있던 애니메이터의 상태 (컷신이 바꾼 버스·쿨러 등)
    public List<ComponentState> components = new(); // ISaveableState
    public List<QuestState> quests = new();
    public bool cameraFollowsPlayer = true;
}

/// <summary>
///     스테이지 씬 하나의 오브젝트 목록. 씬이 로드된 직후(Start 이전)에 만들고, 그 시점을 기준(처음 상태)으로 삼는다.
///     <para>
///         오브젝트는 계층 순서대로 번호를 매긴다. 같은 씬 파일이면 번호가 항상 같으므로,
///         번호와 이름으로 만든 서명(signature)이 저장할 때와 다르면 씬이 수정된 것으로 보고 복원하지 않는다.
///     </para>
/// </summary>
public class SceneStateTracker
{
    // 씬마다 놓이는 시스템 오브젝트(UI, 매니저, 카메라 등)는 저장 대상에서 뺀다. 이 컴포넌트가 들어 있는 최상위 오브젝트는 통째로 제외.
    private static readonly Type[] SystemTypes =
    {
        typeof(PlayerData), typeof(SoundManager), typeof(LoadingScene), typeof(Pause), typeof(SheetLoader),
        typeof(SheetData), typeof(MobileUI), typeof(DialogueManager), typeof(Inventory), typeof(QuestManager),
        typeof(MapOveray), typeof(CameraFollow), typeof(EventSystem), typeof(PlayTimeChecker), typeof(SaveSystem)
    };

    private static Dictionary<string, Sprite> spriteLookup;

    public readonly Scene scene;
    private readonly List<Tracked> comps = new();
    private readonly List<Entry> entries = new();
    private readonly Dictionary<GameObject, int> ids = new();
    private readonly Dictionary<int, ISaveableState> saveables = new();

    public SceneStateTracker(Scene scene)
    {
        this.scene = scene;
        var hash = 2166136261u;
        var buffer = new List<Component>();
        foreach (var root in scene.GetRootGameObjects())
        {
            if (IsSystemRoot(root)) continue;
            Add(root.transform, -1, 0, buffer, ref hash);
        }

        Signature = (int)hash;
    }

    public int Signature { get; }
    public int ObjectCount => entries.Count;

    #region 번호 ↔ 오브젝트 (ISaveableState에서 사용)

    /// <summary>오브젝트의 저장용 번호. 추적하지 않는 오브젝트면 -1.</summary>
    public int IdOf(GameObject go)
    {
        return go != null && ids.TryGetValue(go, out var id) ? id : -1;
    }

    public GameObject ObjectAt(int id)
    {
        return id >= 0 && id < entries.Count ? entries[id].go : null;
    }

    public List<int> IdsOf(IEnumerable<GameObject> objects)
    {
        var list = new List<int>();
        if (objects == null) return list;
        foreach (var go in objects)
        {
            var id = IdOf(go);
            if (id >= 0) list.Add(id);
        }

        return list;
    }

    public List<GameObject> ObjectsAt(IEnumerable<int> idList)
    {
        var list = new List<GameObject>();
        if (idList == null) return list;
        foreach (var id in idList)
        {
            var go = ObjectAt(id);
            if (go != null) list.Add(go);
        }

        return list;
    }

    #endregion

    private static bool IsSystemRoot(GameObject root)
    {
        if (root.tag == "FootPrint") return true; // 발자국 풀
        foreach (var type in SystemTypes)
            if (root.GetComponentInChildren(type, true) != null)
                return true;
        return false;
    }

    public static bool IsBossScene(Scene scene)
    {
        foreach (var root in scene.GetRootGameObjects())
            if (root.GetComponentInChildren<RansomewareBoss>(true) != null ||
                root.GetComponentInChildren<PlayerBossCombat>(true) != null ||
                root.GetComponentInChildren<RansomwareBossFight>(true) != null)
                return true;
        return false;
    }

    private void Add(Transform t, int parent, int depth, List<Component> buffer, ref uint hash)
    {
        var go = t.gameObject;
        // 런타임에 생기는 숨김 오브젝트(TMP 서브메시 등)는 매번 달라질 수 있어 제외
        if ((go.hideFlags & (HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy)) != 0) return;
        if (go.GetComponent<TMP_SubMesh>() != null || go.GetComponent<TMP_SubMeshUI>() != null) return;

        var index = entries.Count;
        var body = go.GetComponent<Rigidbody2D>();
        entries.Add(new Entry
        {
            go = go,
            tr = t,
            isRect = t is RectTransform,
            parent = parent,
            pos = t.localPosition,
            rot = t.localRotation,
            scale = t.localScale,
            // 플레이어 속도는 입력으로 정해지므로 저장하지 않는다
            body = body != null && go.GetComponent<PlayerMovement>() == null ? body : null
        });
        ids[go] = index;

        hash = Hash(hash, depth);
        hash = Hash(hash, go.name);

        go.GetComponents(buffer);
        foreach (var c in buffer)
        {
            if (!(c is Behaviour) && !(c is Renderer)) continue;
            comps.Add(new Tracked
            {
                comp = c,
                baseSprite = c is SpriteRenderer sr ? sr.sprite : null
            });
            hash = Hash(hash, c.GetType().FullName);
            if (c is ISaveableState s) saveables[comps.Count - 1] = s;
        }

        hash = Hash(hash, -1);

        for (var i = 0; i < t.childCount; i++)
            Add(t.GetChild(i), index, depth + 1, buffer, ref hash);
    }

    #region 저장

    public StageProgress Capture()
    {
        var p = new StageProgress
        {
            sceneName = scene.name,
            signature = Signature,
            objectCount = entries.Count,
            componentCount = comps.Count
        };

        var sb = new StringBuilder(Mathf.Max(entries.Count, comps.Count));
        foreach (var e in entries) sb.Append(e.go != null && e.go.activeSelf ? '1' : '0'); // 파괴된 오브젝트는 꺼진 것으로
        p.active = sb.ToString();

        sb.Clear();
        foreach (var c in comps) sb.Append(IsEnabled(c.comp) ? '1' : '0');
        p.enabled = sb.ToString();

        for (var i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            if (e.go == null || e.isRect) continue; // UI 배치는 레이아웃이 다시 계산하므로 제외

            var t = e.tr;
            var parent = ParentId(t);
            var changed = parent != e.parent ||
                          (t.localPosition - e.pos).sqrMagnitude > 1e-8f ||
                          Quaternion.Angle(t.localRotation, e.rot) > 0.01f ||
                          (t.localScale - e.scale).sqrMagnitude > 1e-8f;
            var moving = e.body != null && e.body.bodyType == RigidbodyType2D.Dynamic && e.body.simulated &&
                         (e.body.linearVelocity.sqrMagnitude > 1e-6f || Mathf.Abs(e.body.angularVelocity) > 1e-3f);
            if (!changed && !moving) continue;

            var s = new TransformState
            {
                id = i,
                parent = parent,
                pos = t.localPosition,
                rot = t.localRotation,
                scale = t.localScale
            };
            if (moving)
            {
                s.hasVelocity = true;
                s.velocity = e.body.linearVelocity;
                s.angularVelocity = e.body.angularVelocity;
            }

            p.transforms.Add(s);
        }

        for (var j = 0; j < comps.Count; j++)
        {
            if (!(comps[j].comp is SpriteRenderer sr) || sr == null) continue;
            var sprite = sr.sprite;
            if (sprite == comps[j].baseSprite) continue;
            p.sprites.Add(new SpriteState
            {
                comp = j,
                sprite = sprite != null ? sprite.name : "",
                texture = sprite != null && sprite.texture != null ? sprite.texture.name : ""
            });
        }

        for (var j = 0; j < comps.Count; j++)
        {
            if (!(comps[j].comp is Animator a) || a == null) continue;
            if (!a.isActiveAndEnabled || a.runtimeAnimatorController == null || !a.isInitialized) continue;

            var s = new AnimatorState { comp = j };
            for (var layer = 0; layer < a.layerCount; layer++)
            {
                var info = a.GetCurrentAnimatorStateInfo(layer);
                s.states.Add(info.fullPathHash);
                s.times.Add(info.normalizedTime);
            }

            foreach (var prm in a.parameters)
                switch (prm.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        s.paramHashes.Add(prm.nameHash);
                        s.paramValues.Add(a.GetBool(prm.nameHash) ? 1f : 0f);
                        break;
                    case AnimatorControllerParameterType.Int:
                        s.paramHashes.Add(prm.nameHash);
                        s.paramValues.Add(a.GetInteger(prm.nameHash));
                        break;
                    case AnimatorControllerParameterType.Float:
                        s.paramHashes.Add(prm.nameHash);
                        s.paramValues.Add(a.GetFloat(prm.nameHash));
                        break;
                }

            p.animators.Add(s);
        }

        foreach (var pair in saveables)
        {
            if (pair.Value as Object == null) continue;
            try
            {
                var data = pair.Value.CaptureState(this);
                if (!string.IsNullOrEmpty(data)) p.components.Add(new ComponentState { comp = pair.Key, data = data });
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        return p;
    }

    private int ParentId(Transform t)
    {
        if (t.parent == null) return -1;
        return ids.TryGetValue(t.parent.gameObject, out var id) ? id : -2;
    }

    #endregion

    #region 복원

    /// <summary>저장 당시와 씬 구성이 같은지</summary>
    public bool Matches(StageProgress p)
    {
        return p != null && p.sceneName == scene.name && p.signature == Signature &&
               p.objectCount == entries.Count && p.componentCount == comps.Count &&
               p.active != null && p.active.Length == entries.Count &&
               p.enabled != null && p.enabled.Length == comps.Count;
    }

    /// <summary>오브젝트 켜짐/꺼짐, 컴포넌트 enabled, 위치, 스프라이트를 저장 당시로 되돌린다.</summary>
    public void ApplyObjects(StageProgress p)
    {
        // 끄는 것부터: OnDisable에서 자식을 끄는 스크립트(BackPanel 등)가 있어도 켜는 단계에서 다시 맞춰진다
        for (var i = 0; i < entries.Count; i++)
        {
            var go = entries[i].go;
            if (go != null && p.active[i] == '0' && go.activeSelf) go.SetActive(false);
        }

        for (var i = 0; i < entries.Count; i++)
        {
            var go = entries[i].go;
            if (go != null && p.active[i] == '1' && !go.activeSelf) go.SetActive(true);
        }

        for (var j = 0; j < comps.Count; j++)
        {
            var c = comps[j].comp;
            if (c == null) continue;
            var on = p.enabled[j] == '1';
            if (c is Behaviour b && b.enabled != on) b.enabled = on;
            else if (c is Renderer r && r.enabled != on) r.enabled = on;
        }

        foreach (var s in p.transforms)
        {
            if (s.id < 0 || s.id >= entries.Count) continue;
            var e = entries[s.id];
            if (e.go == null) continue;
            var t = e.tr;

            if (s.parent == -1)
            {
                if (t.parent != null) t.SetParent(null, false);
            }
            else if (s.parent >= 0 && s.parent < entries.Count && entries[s.parent].go != null)
            {
                var target = entries[s.parent].tr;
                if (t.parent != target) t.SetParent(target, false);
            }

            t.localPosition = s.pos;
            t.localRotation = s.rot;
            t.localScale = s.scale;

            if (e.body != null)
            {
                e.body.position = t.position;
                e.body.linearVelocity = s.hasVelocity ? s.velocity : Vector2.zero;
                e.body.angularVelocity = s.hasVelocity ? s.angularVelocity : 0f;
            }
        }

        foreach (var s in p.sprites)
        {
            if (s.comp < 0 || s.comp >= comps.Count) continue;
            if (!(comps[s.comp].comp is SpriteRenderer sr) || sr == null) continue;
            if (string.IsNullOrEmpty(s.sprite))
            {
                sr.sprite = null;
                continue;
            }

            var sprite = FindSprite(s.sprite, s.texture);
            if (sprite != null) sr.sprite = sprite;
            else Debug.LogWarning($"[SaveSystem] 스프라이트 '{s.sprite}'를 찾지 못해 복원하지 못했습니다.");
        }
    }

    /// <summary>애니메이터를 저장 당시 상태로 이동. 오브젝트가 켜지고 Start가 끝난 뒤에 호출해야 한다.</summary>
    public void ApplyAnimators(StageProgress p)
    {
        foreach (var s in p.animators)
        {
            if (s.comp < 0 || s.comp >= comps.Count) continue;
            if (!(comps[s.comp].comp is Animator a) || a == null) continue;
            if (!a.isActiveAndEnabled || a.runtimeAnimatorController == null) continue;

            try
            {
                var paramTypes = new Dictionary<int, AnimatorControllerParameterType>();
                foreach (var prm in a.parameters) paramTypes[prm.nameHash] = prm.type;

                for (var i = 0; i < s.paramHashes.Count && i < s.paramValues.Count; i++)
                {
                    if (!paramTypes.TryGetValue(s.paramHashes[i], out var type)) continue;
                    var value = s.paramValues[i];
                    if (type == AnimatorControllerParameterType.Bool) a.SetBool(s.paramHashes[i], value > 0.5f);
                    else if (type == AnimatorControllerParameterType.Int) a.SetInteger(s.paramHashes[i], Mathf.RoundToInt(value));
                    else if (type == AnimatorControllerParameterType.Float) a.SetFloat(s.paramHashes[i], value);
                }

                for (var layer = 0; layer < a.layerCount && layer < s.states.Count; layer++)
                    if (a.HasState(layer, s.states[layer]))
                        a.Play(s.states[layer], layer, layer < s.times.Count ? s.times[layer] : 0f);

                a.Update(0f); // 위치/스프라이트를 바로 반영
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    /// <summary>ISaveableState 컴포넌트들의 내부 상태 복원</summary>
    public void ApplyComponents(StageProgress p)
    {
        foreach (var s in p.components)
        {
            if (!saveables.TryGetValue(s.comp, out var target) || target as Object == null) continue;
            try
            {
                target.RestoreState(s.data, this);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    private static Sprite FindSprite(string spriteName, string textureName)
    {
        if (spriteLookup == null)
        {
            spriteLookup = new Dictionary<string, Sprite>();
            // 씬이 참조하는 스프라이트는 씬과 함께 메모리에 올라와 있다
            foreach (var sprite in Resources.FindObjectsOfTypeAll<Sprite>())
            {
                var tex = sprite.texture != null ? sprite.texture.name : "";
                spriteLookup.TryAdd(sprite.name + "|" + tex, sprite);
                spriteLookup.TryAdd(sprite.name, sprite);
            }
        }

        if (spriteLookup.TryGetValue(spriteName + "|" + textureName, out var found) && found != null) return found;
        if (spriteLookup.TryGetValue(spriteName, out found) && found != null) return found;

        spriteLookup = null; // 언로드된 것이 섞여 있었을 수 있으니 다음에 다시 만든다
        return null;
    }

    public static void ClearSpriteCache()
    {
        spriteLookup = null;
    }

    #endregion

    private static bool IsEnabled(Component c)
    {
        if (c == null) return false;
        if (c is Behaviour b) return b.enabled;
        if (c is Renderer r) return r.enabled;
        return false;
    }

    private static uint Hash(uint hash, string s)
    {
        foreach (var ch in s)
        {
            hash ^= ch;
            hash *= 16777619u;
        }

        return hash;
    }

    private static uint Hash(uint hash, int value)
    {
        for (var i = 0; i < 4; i++)
        {
            hash ^= (uint)(value >> (i * 8)) & 0xFF;
            hash *= 16777619u;
        }

        return hash;
    }

    private class Entry
    {
        public Rigidbody2D body;
        public GameObject go;
        public bool isRect;
        public int parent;
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
        public Transform tr;
    }

    private class Tracked
    {
        public Sprite baseSprite;
        public Component comp;
    }
}

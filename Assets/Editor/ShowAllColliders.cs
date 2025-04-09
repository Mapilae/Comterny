using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class ShowAllColliders
{
    private const string PREF_KEY = "ShowAllColliders_Enabled"; // 설정 저장 키
    private static bool isEnabled;

    static ShowAllColliders()
    {
        isEnabled = EditorPrefs.GetBool(PREF_KEY, false); // 저장된 값 불러오기
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        if (!isEnabled) return; // 비활성화 상태면 실행 안 함

        Handles.color = Color.green; // 콜라이더 색상
        foreach (Collider2D collider in Object.FindObjectsOfType<Collider2D>())
        {
            if (collider.enabled)
            {
                DrawCollider(collider);
            }
        }
    }

    private static void DrawCollider(Collider2D collider)
    {
        if (collider is BoxCollider2D box)
        {
            Vector2 size = box.size * collider.transform.lossyScale;
            Vector2 position = (Vector2)box.transform.position + box.offset;
            Handles.DrawWireCube(position, size);
        }
        else if (collider is CircleCollider2D circle)
        {
            Vector2 position = (Vector2)circle.transform.position + circle.offset;
            float radius = circle.radius * Mathf.Max(circle.transform.lossyScale.x, circle.transform.lossyScale.y);
            Handles.DrawWireDisc(position, Vector3.forward, radius);
        }
        else if (collider is PolygonCollider2D poly)
        {
            for (int i = 0; i < poly.pathCount; i++)
            {
                Vector2[] points = poly.GetPath(i);
                for (int j = 0; j < points.Length; j++)
                {
                    Vector2 start = (Vector2)poly.transform.position + (Vector2)(poly.transform.localToWorldMatrix * points[j]);
                    Vector2 end = (Vector2)poly.transform.position + (Vector2)(poly.transform.localToWorldMatrix * points[(j + 1) % points.Length]);
                    Handles.DrawLine(start, end);
                }
            }
        }
    }

    // Inspector에서 설정 변경
    [MenuItem("Tools/Show All 2D Colliders")]
    public static void ToggleColliderView()
    {
        isEnabled = !isEnabled;
        EditorPrefs.SetBool(PREF_KEY, isEnabled);
        SceneView.RepaintAll(); // Scene 뷰 갱신
    }
}

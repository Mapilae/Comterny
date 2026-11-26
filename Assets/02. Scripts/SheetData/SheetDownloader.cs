#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

/// <summary>
/// 구글 시트(TSV)를 받아 SheetDataObj 에셋으로 저장한다.
/// - 씬(SheetDataDownload)의 오브젝트 우클릭 메뉴: Download and Save SheetData
/// - 상단 메뉴: Tools/Sheet/시나리오 시트 다운로드 (씬을 열 필요 없음)
/// </summary>
[ExecuteInEditMode]
public class SheetDownloader : MonoBehaviour
{
    public const string DefaultTsvUrl =
        "https://docs.google.com/spreadsheets/d/e/2PACX-1vQL4QXgDuaYrnk5mL4aT8DTinFtM80x6Y0j2zPQkI2Y_sUhcCnJCxANag4Khc2pWA5ZK4mXq0kJKo_f/pub?output=tsv";

    public const string DefaultOutputPath = "Assets/Resources/SheetDataObj.asset";

    private static bool isDownloading;

    [TextArea(3, 10)] public string tsvUrl = DefaultTsvUrl;

    public string outputAssetPath = DefaultOutputPath;

    [ContextMenu("Download and Save SheetData")]
    public void DownloadSheetData()
    {
        Download(tsvUrl, outputAssetPath);
    }

    [MenuItem("Tools/Sheet/시나리오 시트 다운로드")]
    private static void DownloadFromMenu()
    {
        // 열린 씬에 SheetDownloader가 있으면 그 설정(URL/경로)을 따른다
        var inScene = FindFirstObjectByType<SheetDownloader>(FindObjectsInactive.Include);
        if (inScene != null) Download(inScene.tsvUrl, inScene.outputAssetPath);
        else Download(DefaultTsvUrl, DefaultOutputPath);
    }

    [MenuItem("Tools/Sheet/시나리오 시트 다운로드", true)]
    private static bool CanDownloadFromMenu()
    {
        return !isDownloading;
    }

    public static void Download(string url, string outputPath)
    {
        if (isDownloading)
        {
            Debug.LogWarning("시트 다운로드가 이미 진행 중입니다.");
            return;
        }

        isDownloading = true;
        // 1행(헤더)부터 받아서 헤더 이름으로 열을 찾는다 (Voice 열 위치가 자유로움)
        var sheetURL = url + "&range=A1:Z&nocache=" + DateTime.UtcNow.Ticks + Random.Range(0, 99999);
        var www = UnityWebRequest.Get(sheetURL);
        Debug.Log("시트 다운로드 시작...");

        // 에디터 모드에서도 동작하도록 코루틴 대신 완료 콜백을 쓴다
        www.SendWebRequest().completed += _ =>
        {
            try
            {
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("TSV 다운로드 실패: " + www.error);
                    return;
                }

                Save(www.downloadHandler.text, outputPath);
            }
            finally
            {
                www.Dispose();
                isDownloading = false;
            }
        };
    }

    private static void Save(string tsvText, string outputPath)
    {
        var blocks = SheetTsvParser.Parse(tsvText);

        // 기존 에셋이 있으면 내용만 바꿔서 GUID(씬의 참조)를 유지한다
        var sheetData = AssetDatabase.LoadAssetAtPath<SheetDataObj>(outputPath);
        if (sheetData == null)
        {
            sheetData = ScriptableObject.CreateInstance<SheetDataObj>();
            sheetData.block = blocks;
            AssetDatabase.CreateAsset(sheetData, outputPath);
        }
        else
        {
            sheetData.block = blocks;
            EditorUtility.SetDirty(sheetData);
        }

        AssetDatabase.SaveAssets();

        var voiceCount = 0;
        foreach (var b in blocks)
        foreach (var c in b.cells)
            if (!string.IsNullOrEmpty(c.voice))
                voiceCount++;
        Debug.Log($"✅ SheetData 저장 완료: {outputPath} (블록 {blocks.Count}개, 보이스 지정 대사 {voiceCount}개)");
    }
}
#endif

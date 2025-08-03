#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class SheetDownloader : MonoBehaviour
{
    [TextArea(3, 10)] public string tsvUrl =
        "https://docs.google.com/spreadsheets/d/e/2PACX-1vQL4QXgDuaYrnk5mL4aT8DTinFtM80x6Y0j2zPQkI2Y_sUhcCnJCxANag4Khc2pWA5ZK4mXq0kJKo_f/pub?output=tsv";

    public string outputAssetPath = "Assets/Resources/SheetDataObj.asset";

    [ContextMenu("Download and Save SheetData")]
    public void DownloadSheetData()
    {
        StartCoroutine(DownloadAndCreateSheetData());
    }

    private IEnumerator DownloadAndCreateSheetData()
    {
        var sheetURL = tsvUrl + "&range=A2:F&nocache=" + DateTime.UtcNow.Ticks + Random.Range(0, 99999);
        using (var www = UnityWebRequest.Get(sheetURL))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("TSV 다운로드 실패: " + www.error);
                yield break;
            }

            var tsvText = www.downloadHandler.text;

            // 파싱 및 ScriptableObject 생성
            var sheetData = ScriptableObject.CreateInstance<SheetDataObj>();

            ParseTSVToSheetData(tsvText, sheetData);

            // 저장
            AssetDatabase.CreateAsset(sheetData, outputAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("✅ SheetData ScriptableObject 저장 완료: " + outputAssetPath);
        }
    }

    private void ParseTSVToSheetData(string tsvText, SheetDataObj sheetData)
    {
        var rows = tsvText.Split('\n');
        var currentId = "";

        for (var i = 0; i < rows.Length; i++)
        {
            var cols = rows[i].Replace("\r", "").Split('\t');
            if (cols.Length < 5) continue;

            var id = cols[0];
            var startCmd = cols[1];
            var standingNum = cols[2];
            var charName = cols[3];
            var dialogue = cols[4];
            var afterCmd = cols[5];

            if (id.Equals(standingNum) && string.IsNullOrEmpty(dialogue)) continue;
            if (id.Equals("//")) continue;

            if (!string.IsNullOrEmpty(id))
            {
                currentId = id;
                var block = new Block(id);
                sheetData.AddBlock(block);
            }

            if (!string.IsNullOrEmpty(currentId))
            {
                var cell = new Cell(charName, string.IsNullOrEmpty(standingNum) ? "0" : standingNum, dialogue, startCmd,
                    afterCmd);
                sheetData.AddCell(currentId, cell);
            }
        }
    }
}
#endif
using System;
using System.Collections.Generic;

/// <summary>
/// 시나리오 시트(TSV)를 Block 목록으로 바꾼다. SheetDownloader(에디터)와 SheetLoader(런타임)가 같이 쓴다.
/// 첫 행이 헤더(start_cmd / StandingNum / Name / Dialogue / after_cmd / Voice ...)면 헤더 이름으로 열을 찾으므로
/// 시트에 Voice 열을 어디에 넣어도 된다. 헤더가 없으면 예전 순서(A~F) + G열 = Voice로 읽는다.
/// </summary>
public static class SheetTsvParser
{
    private const int DefaultId = 0, DefaultStartCmd = 1, DefaultStanding = 2, DefaultName = 3, DefaultDialogue = 4,
        DefaultAfterCmd = 5, DefaultVoice = 6;

    public static List<Block> Parse(string tsvText)
    {
        var blocks = new List<Block>();
        if (string.IsNullOrEmpty(tsvText)) return blocks;

        var rows = tsvText.Split('\n');
        int colId = DefaultId, colStart = DefaultStartCmd, colStanding = DefaultStanding, colName = DefaultName,
            colDialogue = DefaultDialogue, colAfter = DefaultAfterCmd, colVoice = DefaultVoice;
        var firstRow = 0;

        // 헤더 행 찾기
        if (rows.Length > 0)
        {
            var header = rows[0].Replace("\r", "").Split('\t');
            var startIdx = IndexOf(header, "start_cmd");
            if (startIdx >= 0)
            {
                firstRow = 1;
                colStart = startIdx;
                colStanding = IndexOrDefault(header, "StandingNum", colStanding);
                colName = IndexOrDefault(header, "Name", colName);
                colDialogue = IndexOrDefault(header, "Dialogue", colDialogue);
                colAfter = IndexOrDefault(header, "after_cmd", colAfter);
                colVoice = IndexOf(header, "Voice");
                if (colVoice < 0) colVoice = IndexOf(header, "보이스");
                // 헤더에 Voice 열이 없으면 보이스 없음(-1). G열 '비고'를 보이스로 잘못 읽지 않기 위함
            }
        }

        Block current = null;
        for (var i = firstRow; i < rows.Length; i++)
        {
            var cols = rows[i].Replace("\r", "").Split('\t');
            if (cols.Length < 5) continue;

            var id = Get(cols, colId);
            var startCmd = Get(cols, colStart);
            var standingNum = Get(cols, colStanding);
            var charName = Get(cols, colName);
            var dialogue = Get(cols, colDialogue);
            var afterCmd = Get(cols, colAfter);
            var voice = Get(cols, colVoice).Trim();

            if (id.Equals(standingNum) && string.IsNullOrEmpty(dialogue)) continue;
            if (id.Equals("//")) continue;

            if (!string.IsNullOrEmpty(id))
            {
                current = new Block(id);
                blocks.Add(current);
            }

            current?.AddCell(new Cell(charName, string.IsNullOrEmpty(standingNum) ? "0" : standingNum, dialogue,
                startCmd, afterCmd, voice));
        }

        return blocks;
    }

    private static string Get(string[] cols, int index)
    {
        return index >= 0 && index < cols.Length ? cols[index] : "";
    }

    private static int IndexOf(string[] header, string name)
    {
        for (var i = 0; i < header.Length; i++)
            if (string.Equals(header[i].Trim(), name, StringComparison.OrdinalIgnoreCase))
                return i;
        return -1;
    }

    private static int IndexOrDefault(string[] header, string name, int fallback)
    {
        var i = IndexOf(header, name);
        return i >= 0 ? i : fallback;
    }
}

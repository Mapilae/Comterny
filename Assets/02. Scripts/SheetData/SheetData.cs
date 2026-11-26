using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Block
{
    public string id;

    public List<Cell> cells;

    public Block(string id)
    {
        this.id = id;
        cells = new List<Cell>();
    }

    public void AddCell(Cell cell)
    {
        cells.Add(cell);
    }
}

[Serializable]
public class Cell
{
    public string name;
    public string standingNum;
    [TextArea] public string content;
    public string start_cmd;
    public string after_cmd;

    [Tooltip("보이스 파일명 (캐릭터 폴더 제외, 확장자 생략 가능). 캐릭터는 name으로 찾는다")]
    public string voice;

    public Cell(string name, string standingNum, string content, string start_cmd, string after_cmd,
        string voice = "")
    {
        this.name = name;
        this.standingNum = standingNum;
        this.content = content;
        this.start_cmd = start_cmd;
        this.after_cmd = after_cmd;
        this.voice = voice ?? "";
    }
}

public class SheetData : MonoBehaviour
{
    public static SheetData instance;
    public SheetDataObj sheetDataObj;
    [Header("Scenario data")] private List<Block> block;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        block = sheetDataObj.block;
    }

    public void AddCell(string id, Cell cell)
    {
        block[FindIDIndex(id)].AddCell(cell);
    }

    public int FindIDIndex(string id)
    {
        if (block == null)
        {
            Debug.Log("block is null");
            return -1;
        }

        if (block.Count == 0)
        {
            Debug.Log("block count is 0!");
            return -1;
        }

        for (var i = 0; i < block.Count; i++)
            if (block[i].id.Equals(id))
                return i;

        Debug.Log($"{id} found!");
        return -1;
    }

    public void AddBlock(Block block)
    {
        this.block.Add(block);
    }


    public Block GetBlock(string id)
    {
        return block[FindIDIndex(id)];
    }
}
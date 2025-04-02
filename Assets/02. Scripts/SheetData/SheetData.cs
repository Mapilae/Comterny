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
    public int standingNum;
    [TextArea] public string content;


    public Cell(string name, int standingNum, string content)
    {
        this.name = name;
        this.standingNum = standingNum;
        this.content = content;
    }
}

public class SheetData : MonoBehaviour
{
    public static SheetData instance;

    [Header("�ó�����")] public List<Block> block;

    private void Awake()
    {
        instance = this;
    }

    public void AddCell(string id, Cell cell)
    {
        block[FindIDIndex(id)].AddCell(cell);
    }

    public int FindIDIndex(string id)
    {
        if (block == null)
        {
            Debug.Log("block�� null�Դϴ�");
            return -1;
        }

        if (block.Count == 0)
        {
            Debug.Log("block\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdʽ\ufffd\ufffdϴ\ufffd!");
            return -1;
        }

        for (int i = 0; i < block.Count; i++)
            if (block[i].id.Equals(id))
                return i;

        Debug.Log($"{id}�� �������� �ʽ��ϴ�!");
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
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SheetDataObj", menuName = "Dialogue/SheetDataObj", order = 1)]
public class SheetDataObj : ScriptableObject
{
    public static SheetDataObj instance;
    public List<Block> block = new();

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
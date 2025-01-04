using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    void Start()
    {
        if (BlockManager.ActionBlock == null || BlockManager.Blocks.Count == 0) return;

        Block[] blocks = BlockManager.Blocks.Values.ToArray();
        int size = blocks.Length;

        Block block = blocks[UnityEngine.Random.Range(0, size)];
        BlockManager.ActionBlock.transform.position = block.transform.position;
        BlockManager.Remove(block.Position);
        block.gameObject.SetActive(false);
    }
}

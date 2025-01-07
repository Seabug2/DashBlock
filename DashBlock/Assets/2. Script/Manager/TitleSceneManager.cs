using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    void Start()
    {
        CameraController.SetOriginPosition(Camera.main.transform.position);

        Block[] blocks = BlockManager.Tiles.Values.ToArray();
        int size = blocks.Length;

        Block block = blocks[UnityEngine.Random.Range(0, size)];
        ActionBlock.instance.transform.position = block.transform.position;
        BlockManager.Remove(block.Position);

        BlockManager.limit_x = 26;
        BlockManager.limit_y = 49;
        block.gameObject.SetActive(false);
    }
}

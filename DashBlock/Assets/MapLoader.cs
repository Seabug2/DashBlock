﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

public static class MapLoader
{
    private const sbyte MaxLimit = 127;

    //TODO : 시트의 첫 페이지는 다른 모든 시트의 정보를 저장하고 있음

    public static Dictionary<string, string> mapKeyValuePairs = new()
    {
        // Key : Map Name, Value : Map gid;
        { "Map_Data_Title", "95630366" },
        { "Map_Data_0", "1553283520" },
        { "Map_Data_1", "1268210948" },
        { "Map_Data_2", "573979549" }

    };

    public static string putMapName;
    public static string mapData;

    const string url = "https://docs.google.com/spreadsheets/d/194NAzYpdn938JB_HMUGmefgy66cs3sOhxcl2iUnOAms/export?format=csv";


    public async static UniTask MapSheetRequest(string mapName)
    {
        BlockManager.PlayerBlock.IsMoving = false;

        UnityWebRequest www = UnityWebRequest.Get(url + "&gid=" + $"{mapKeyValuePairs[mapName]}");
        Debug.Log(url + "&gid =" + $"{mapKeyValuePairs[mapName]}");
        await www.SendWebRequest();

        mapData = www.downloadHandler.text;
        MapLoader.LoadMap(mapData);


    }

    public static void LoadMap(string mapData)
    {
        BlockManager.PlayerBlock.IsMoving = false;

        if (string.IsNullOrWhiteSpace(mapData))
        {
            Debug.LogError("Map data is empty.");
            return;
        }

        string[] lines = mapData.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        sbyte limitY = (sbyte)lines.Length;

        if (limitY > MaxLimit)
        {
            Debug.LogError($"Map height exceeds sbyte limits ({MaxLimit}).");
            return;
        }

        string[] firstLineNumbers = lines[0].Split(',');
        sbyte limitX = (sbyte)firstLineNumbers.Length;

        if (limitX > MaxLimit)
        {
            Debug.LogError($"Map width exceeds sbyte limits ({MaxLimit}).");
            return;
        }

        CameraController.SetPosition(limitX, limitY);

        BlockManager.Reset();
        BlockManager.limit_x = (sbyte)(limitX - 1);
        BlockManager.limit_y = (sbyte)(limitY - 1);

        BlockPosition position;
        for (sbyte y = 0; y < limitY; y++)
        {
            string[] numbers = lines[y].Split(',');
            for (sbyte x = 0; x < limitX; x++)
            {
                if (!sbyte.TryParse(numbers[x], out sbyte number))
                {
                    Debug.LogWarning($"Invalid number at ({x}, {y}): '{numbers[x]}'. Skipping.");
                    continue;
                }

                if (number == 0)
                {
                    continue;
                }

                position = new(x, (sbyte)(limitY - y - 1));

                if (number == -1)
                {
                    SetPlayerBlock(position, 99);
                }
                else
                {
                    CreateBlock(number, position);
                }
            }
        }


        BlockManager.PlayerBlock.IsMoving = true;
    }

    private static void SetPlayerBlock(Vector3 position, sbyte hp)
    {
        BlockManager.PlayerBlock.transform.position = position;
        BlockManager.PlayerBlock.HP = hp;
        if (!BlockManager.PlayerBlock.gameObject.activeSelf)
        {
            BlockManager.PlayerBlock.gameObject.SetActive(true);
        }
    }

    private static void CreateBlock(sbyte hp, BlockPosition position)
    {
        Block block = BlockManager.GetItem<Block>();
        block.Init(position, hp);
    }

}

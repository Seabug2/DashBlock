using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MapLoader
{
    private const sbyte MaxLimit = 127;
    private const string TitleSceneName = "TitleScene"; // 타이틀 씬 이름을 적절히 설정하세요

    public static void LoadMap(string mapData)
    {
        if (string.IsNullOrWhiteSpace(mapData))
        {
            Debug.LogError("Map data is empty.");
            LoadTitleScene();
            return;
        }

        string[] lines = mapData.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        sbyte limitY = (sbyte)lines.Length;

        if (limitY > MaxLimit)
        {
            Debug.LogError($"Map height exceeds sbyte limits ({MaxLimit}).");
            LoadTitleScene();
            return;
        }

        string[] firstLineNumbers = lines[0].Split(',');
        sbyte limitX = (sbyte)firstLineNumbers.Length;

        if (limitX > MaxLimit)
        {
            Debug.LogError($"Map width exceeds sbyte limits ({MaxLimit}).");
            LoadTitleScene();
            return;
        }

        SetCamera(limitX, limitY);

        BlockManager.Reset();
        BlockManager.limit_x = (sbyte)(limitX - 1);
        BlockManager.limit_y = (sbyte)(limitY - 1);

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

                Vector3 position = new Vector3(x, limitY - y - 1, 0);

                if (number == -1)
                {
                    SetActionBlockPosition(position);
                }
                else
                {
                    CreateBlock(number, position);
                }
            }
        }
    }

    private static void SetActionBlockPosition(Vector3 position)
    {
        if (ActionBlock.instance != null)
        {
            ActionBlock.instance.transform.position = position;
        }
        else
        {
            Debug.LogError("ActionBlock instance is null.");
        }
    }

    private static void CreateBlock(sbyte hp, Vector3 position)
    {
        Block block = BlockManager.GetItem<Block>();
        if (block != null)
        {
            BlockManager.targetCount++;
            block.transform.position = position;
            block.HP = hp;
        }
        else
        {
            Debug.LogError("Failed to get a Block instance from BlockManager.");
        }
    }

    private static void SetCamera(sbyte x, sbyte y)
    {
        Camera mainCamera = CameraController.Main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera is not assigned in CameraController.");
            return;
        }

        float blockSize = Screen.width / (float)x;

        if (x > y)
        {
            float screenHeight = Screen.height / blockSize;
            mainCamera.orthographicSize = screenHeight * 0.5f;
            Vector3 originPosition = Vector3.Lerp(Vector3.zero, new Vector3(x - 1, y, -10), 0.5f);
            CameraController.SetOriginPosition(originPosition);
        }
        else
        {
            mainCamera.orthographicSize = (y + 3) * 0.5f;
            Vector3 originPosition = Vector3.Lerp(new Vector3(0, -2, -10), new Vector3(x - 1, y + 1, -10), 0.5f);
            CameraController.SetOriginPosition(originPosition);
        }
    }

    private static void LoadTitleScene()
    {
        SceneManager.LoadScene(TitleSceneName);
    }
}

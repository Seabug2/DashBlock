using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : Singleton
{
    public void LoadMap(string fileName)
    {
        string mapData = fileName;

        string[] lines = mapData.Split("\n");
        sbyte limit_y = (sbyte)(lines.Length);

        string[] numbers = lines[0].Split(",");
        sbyte limit_x = (sbyte)numbers.Length;

        SetCamera(limit_x, limit_y);

        if (limit_y > 127 || limit_x > 127)
        {
            Debug.LogError("Map size exceeds sbyte limits (127x127).");

            //TODO : 맵 불러오기 실패하면 타이틀 씬으로 이동하게 만든다.
            return;
        }

        BlockManager.Reset();
        BlockManager.limit_x = (sbyte)(limit_x - 1);
        BlockManager.limit_y = (sbyte)(limit_y - 1);

        for (sbyte y = 0; y < limit_y; y++)
        {
            numbers = lines[y].Split(",");
            for (sbyte x = 0; x < limit_x; x++)
            {
                if (sbyte.TryParse(numbers[x], out sbyte number))
                {
                    if (number == 0)
                    {
                        continue;
                    }

                    Vector3 position = new Vector3(x, limit_y - y - 1, 0);

                    if (number <= -1)
                    {
                        ActionBlock.instance.transform.position = position;
                    }
                    else
                    {
                        Block b = BlockManager.GetItem<Block>();
                        BlockManager.targetCount++;
                        b.transform.position = new Vector3(x, limit_y - y - 1, 0);
                        b.HP = number;
                    }
                }
            }
        }
    }

    void SetCamera(sbyte x, sbyte y)
    {
        if (x > y)
        {
            float bloackSize = Screen.width / x;
            float screenHeight = Screen.height / bloackSize;
            CameraController.Main.orthographicSize = screenHeight * 0.5f;
            CameraController.SetOriginPoistion(Vector3.Lerp(Vector3.zero, new Vector3(x - 1, y, -10), .5f));
        }
        else
        {
            CameraController.Main.orthographicSize = (y + 3) * 0.5f;
            CameraController.SetOriginPoistion(Vector3.Lerp(new Vector3(0, -2, -10), new Vector3(x - 1, y + 1, -10), .5f));
        }
    }
}

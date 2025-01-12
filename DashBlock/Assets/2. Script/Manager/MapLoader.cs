using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

public class SheatData
{
    //"gid" / "맵 이름"
    public string Gid { get; private set; }
    public string MapName { get; private set; }

    public SheatData(string line)
    {
        string[] data = line.Split(',');
        Gid = data[1];
        MapName = data[2];
    }
}

public static class MapLoader
{
    private const sbyte MaxLimit = 127;

    const string url = "https://docs.google.com/spreadsheets/d/194NAzYpdn938JB_HMUGmefgy66cs3sOhxcl2iUnOAms/export?format=csv";

    public static SheatData[] SheatDatas;
    static string titleMapData;

    public async static UniTaskVoid Init()
    {
        BlockManager.BlockData = Resources.Load<BlockPrefabsList>("Block_List");


        string datas = await SheetRequest("0");
        Debug.Log(datas);

        //줄나눔
        string[] lines = datas.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        // 타이틀 정보를 저장
        SheatData sheat = new(lines[1]);
        titleMapData = await SheetRequest(sheat.Gid);
        Debug.Log(titleMapData);




        int mapDataCount = lines.Length - 2; //두 번째 줄까지 제거한 수
        SheatDatas = new SheatData[mapDataCount];
        //맵 데이터를 저장
        for (int i = 2; i < mapDataCount + 2; i++)
        {
            sheat = new(lines[i]);
            SheatDatas[i - 2] = sheat;
            Debug.Log($"{SheatDatas[i - 2].MapName}을 {i - 2}번에 저장");
        }




        LoadMap(titleMapData);
    }


    public async static UniTask<string> SheetRequest(string gid)
    {
        UnityWebRequest www = UnityWebRequest.Get($"{url}&gid={gid}");
        await www.SendWebRequest();

        string mapData = www.downloadHandler.text;
        return mapData;
    }

    public static void LoadMap(string mapData, Action OnCompletedLoadMap = null)
    {
        ActionBlock.ActiveMovingBlocks = 100;




        if (string.IsNullOrWhiteSpace(mapData))
        {
            Debug.LogError("Map data is empty.");
            return;
        }

        string[] lines = mapData.Split('\n');
        int limitY = lines.Length;

        if (limitY > MaxLimit)
        {
            Debug.LogError($"Map height exceeds sbyte limits ({MaxLimit}).");
            return;
        }

        string[] firstLineNumbers = lines[0].Split(',');
        int limitX = firstLineNumbers.Length;

        if (limitX > MaxLimit)
        {
            Debug.LogError($"Map width exceeds sbyte limits ({MaxLimit}).");
            return;
        }

        CameraController.SetPosition(limitX, limitY);

        BlockManager.ResetGame();
        BlockManager.limit_x = (limitX - 1);
        BlockManager.limit_y = (limitY - 1);

        Debug.Log($"Map Size {BlockManager.limit_x } / {BlockManager.limit_y    }");

        string[] datas;
        char blockType;

        for (int y = 0; y < limitY; y++)
        {
            datas = lines[y].Split(',');
            for (int x = 0; x < limitX; x++)
            {
                //저장된 데이터가 없는 경우
                if (string.IsNullOrWhiteSpace(datas[x]) || datas[x].Length < 3)
                {
                    continue;
                }

                blockType = datas[x][0];
                //Debug.Log($"생성하려는 타입 ID = {blockType}");

                string hp = datas[x].Substring(1);

                if (!int.TryParse(hp, out int HP))
                {
                    Debug.LogWarning($"Invalid number at ({x}, {y}): '{datas[x]}'. Skipping.");
                    continue;
                }




                Vector2 position = new(x, (limitY - y - 1));

                if (blockType == '1')
                {
                    BlockManager.PlayerBlock.Init(position, HP);
                }
                else
                {
                    SetBlock(blockType, HP, position);
                }
            }
        }



        ActionBlock.ActiveMovingBlocks = 0;
        OnCompletedLoadMap?.Invoke();
    }

    private static void SetBlock(char blockType, int hp, Vector3 position)
    {
        Block block = BlockManager.GetBlock(blockType - '0');
        //Debug.Log((int)blockType);
        block.Init(position, hp);
    }
}

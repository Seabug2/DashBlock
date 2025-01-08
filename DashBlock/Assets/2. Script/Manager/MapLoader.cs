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

        BlockManager.ResetGame();
        BlockManager.limit_x = (sbyte)(limitX - 1);
        BlockManager.limit_y = (sbyte)(limitY - 1);

        string[] datas;
        string blockType;

        for (sbyte y = 0; y < limitY; y++)
        {
            datas = lines[y].Split(',');
            for (sbyte x = 0; x < limitX; x++)
            {
                //저장된 데이터가 없는 경우
                if (string.IsNullOrEmpty(datas[x]))
                {
                    continue;
                }




                blockType = datas[0];

                if (blockType == "0")
                {
                    continue;
                }

                string hp = datas[x].Substring(1);

                if (!sbyte.TryParse(hp, out sbyte HP))
                {
                    Debug.LogWarning($"Invalid number at ({x}, {y}): '{datas[x]}'. Skipping.");
                    continue;
                }




                Vector3 position = new(x, (sbyte)(limitY - y - 1));

                if (datas[x] == "1")
                {
                    BlockManager.PlayerBlock.Init(position, HP);
                }
                else
                {
                    SetBlock(datas[x], HP, position);
                }
            }
        }



        ActionBlock.ActiveMovingBlocks = 0;
        OnCompletedLoadMap?.Invoke();
    }




    private static void SetBlock(string blockType, sbyte hp, Vector3 position)
    {
        if (int.TryParse(blockType, out int blockTypeID))
        {
            Block block = BlockManager.GetBlock<Block>();
            block.Init(position, hp);
        }
    }
}

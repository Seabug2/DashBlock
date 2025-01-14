using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

public class MapData
{
    //"gid" / "맵 이름"
    public string Gid { get; private set; }
    public string MapName { get; private set; }

    public MapData(string line)
    {
        string[] data = line.Split(',');
        Gid = data[1];
        MapName = data[2];
    }
}

public static class MapLoader
{
    const string url = "https://docs.google.com/spreadsheets/d/194NAzYpdn938JB_HMUGmefgy66cs3sOhxcl2iUnOAms/export?format=csv";

    static MapData[] SheatDatas;
    static MapData currentMap;
    static string titleMapData;

    public async static UniTaskVoid Init()
    {
        //첫 번째 스프레드 시트를 불러오고,
        string datas = await SheetRequest("0");

        //줄을 나눈 후
        string[] lines = datas.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        //맵 시트에 저장된 첫 번째 타이틀 씬의 정보를 불러옴
        MapData sheat = new(lines[1]);
        titleMapData = await SheetRequest(sheat.Gid);



        //두 번째 줄까지 제거한 수 (첫 번째 줄은 분류, 두 번째 줄은 타이틀 씬의 정보)
        int mapDataCount = lines.Length - 2;
        SheatDatas = new MapData[mapDataCount];
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

        string[] firstLineNumbers = lines[0].Split(',');
        int limitX = firstLineNumbers.Length;

        CameraController.SetPosition(limitX, limitY);

        Block.ResetTileMap();
        Block.limit_x = (limitX - 1);
        Block.limit_y = (limitY - 1);

        Debug.Log($"Map Size {Block.limit_x} / {Block.limit_y}");




        string[] line;
        for (int y = 0; y < limitY; y++)
        {
            line = lines[y].Split(',');
            for (int x = 0; x < limitX; x++)
            {
                //저장된 데이터가 없는 경우
                if (string.IsNullOrWhiteSpace(line[x]) || line[x].Length != 3)
                {
                    continue;
                }

                SetTile(line[x], new Vector2(x, limitY - y - 1));
            }
        }

        ActionBlock.ActiveMovingBlocks = 0;
        OnCompletedLoadMap?.Invoke();
    }

    private static void SetTile(string tileData, Vector2 position)
    {
        int blockType = tileData[0] - 'A';
        string hp = tileData.Substring(1);

        if (!int.TryParse(hp, out int HP))
        {
            HP = 1;
        }

        int i = blockType - 'A';
        Block block = Block.GetBlock(i);

        //Block의 B
        if (blockType == 'B')
        {
            Block.BlockCount++;
        }

        block.Init(position, HP);
    }
}

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

    public async static UniTaskVoid Initialize()
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


        LoadMap().Forget();
    }

    public async static UniTask<string> SheetRequest(string gid)
    {
        UnityWebRequest www = UnityWebRequest.Get($"{url}&gid={gid}");
        await www.SendWebRequest();

        string mapData = www.downloadHandler.text;
        return mapData;
    }

    /// <summary>
    /// 화면을 가린 후에, 맵 초기화와 맵 생성
    /// </summary>
    public async static UniTask LoadMap(MapData mapData = null, Action OnCompletedLoadMap = null)
    {
        await UniTask.WaitWhile(() => ActionBlock.IsAnyActionBlockMoving);

        //TODO : 조작을 막고 화면을 가려야함
        if (Locator.TryGet(out BlockController controller))
        {
            controller.SetActive(false);
        }

        //await 화면 가리기

        Block.ResetTileMap();


        string sheet;

        if (mapData == null)
        {
            sheet = titleMapData;
        }
        else
        {
            sheet = await SheetRequest(mapData.Gid);
        }

        string[] lines = sheet.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        int length_Y = lines.Length;

        string[] firstLineNumbers = lines[0].Split(',');
        int length_X = firstLineNumbers.Length;

        CameraController.SetPosition(length_X, length_Y);

        Block.limit_x = (length_X - 1);
        Block.limit_y = (length_Y - 1);

        Debug.Log($"Map Size {Block.limit_x} / {Block.limit_y}");




        string[] line; // = , , , , ,로 구분된 행 데이터
        for (int y = 0; y < length_Y; y++)
        {
            line = lines[y].Split(',');
            for (int x = 0; x < length_X; x++)
            {
                string cell = line[x];

                //저장된 데이터가 없는 경우
                if (string.IsNullOrWhiteSpace(line[x]) || line[x].Length != 3)
                {
                    continue;
                }

                SetTile(cell, new Vector2(x, length_Y - y - 1));
            }
        }

        OnCompletedLoadMap?.Invoke();
    }

    private static void SetTile(string tileData, Vector2 position)
    {
        //"A09"
        //블록의 종류를 확인
        Block block = Block.GetBlock(tileData[0]);

        //생성할 블록의 HP를 확인
        string hp = tileData.Substring(1);
        if (!int.TryParse(hp, out int HP))
        {
            HP = 1;
        }

        //Block의 B
        if (tileData[0] == 'B')
        {
            Block.BlockCount++;
        }

        block.Init(position, HP);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    GameObject block;
    GameObject actionBlock;
    //TextAsset csv;
    string csv;

    //public string loadMapName;
    private void Start()
    {
        //LoadMap(loadMapName);
    }

    /*
     * TODO : 카메라 사이즈 조절
     * 1. 화면은 세로 고정
     * 2. 스크린의 가장 긴 축을 기준으로 모든 블록이 보이도록 카메라 size를 맞추고
     * 3. 블록들의 정중앙에 카메라를 위치 시켜야함
     * 4. 그 작업이 마치면 Fade In 등의 화면전환 효과를 만들어야함
     */

    public void LoadMap(string fileName)
    {
        Debug.Log(fileName);

        block = Resources.Load("Block") as GameObject;
        actionBlock = Resources.Load("ActionBlock") as GameObject;
        //csv = fileName;//Resources.Load("Maps/" + fileName) as TextAsset;

        string mapData = fileName;//csv.ToString();
        string[] lines = mapData.Split("\n");
        sbyte limit_y = (sbyte)lines.Length;

        // 첫 번째 줄로 열의 개수를 계산
        string[] numbers = lines[0].Split(",");
        sbyte limit_x = (sbyte)numbers.Length;

        SetCamera(limit_x, limit_y);

        // 행과 열 크기 검사
        if (limit_y > 127 || limit_x > 127)
        {
            Debug.LogError("Map size exceeds sbyte limits (127x127).");
            return; // 행렬 크기가 127을 초과하면 종료
        }

        Debug.Log($"Map Size: {limit_x}, {limit_y}");

        bool spawnedActionBlock = false;

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
                    else if (number <= -1)
                    {
                        if (spawnedActionBlock) continue;
                        spawnedActionBlock = true;
                        Instantiate(actionBlock, new Vector3(x, limit_y - y, 0), Quaternion.identity);
                    }
                    else
                    {
                        Block b = Instantiate(block, new Vector3(x, limit_y - y, 0), Quaternion.identity).GetComponent<Block>();
                        b.HP = number;
                    }
                }
            }
        }
    }

    void SetCamera(sbyte x, sbyte y)
    {
        if (x > y + 3)
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

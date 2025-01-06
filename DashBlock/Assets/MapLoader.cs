using UnityEngine;

public class MapLoader : MonoBehaviour
{
    TextAsset csv;

    public string loadMapName;
    private void Start()
    {
        LoadMap(loadMapName);
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
        BlockManager.Reset();

        GameObject block = Resources.Load("Block") as GameObject;
        GameObject actionBlock = Resources.Load("ActionBlock") as GameObject;
        csv = Resources.Load("Maps/" + fileName) as TextAsset;

        string mapData = csv.ToString();
        string[] lines = mapData.Split("\n");
        sbyte limit_y = (sbyte)(lines.Length);

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

        BlockManager.limit_x = (sbyte)(limit_x - 1);
        BlockManager.limit_y = (sbyte)(limit_y - 1);
        
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
                        Instantiate(actionBlock, new Vector3(x, limit_y - y - 1, 0), Quaternion.identity);
                    }
                    else
                    {
                        Block b = Instantiate(block, new Vector3(x, limit_y - y - 1, 0), Quaternion.identity).GetComponent<Block>();
                        b.HP = number;
                    }
                }
            }
        }
    }

    GameObject GetBlock(int blockNumber)
    {
        //switch (blockNumber)
        //{
        //    case 0: Players Action Block
        //        break;
        //    case 1: Basic Block
        //        break;
        //    case 2: Solid Block
        //        break;
        //    case 3: Sliding Block
        //        break;
        //    case 4:
        //        break;
        //    case 5:
        //        break;
        //    case 6:
        //        break;
        //    case 7:
        //        break;
        //    case 8:
        //        break;
        //    case 9:
        //        break;
        //}

        return null;
    }


    public virtual void OnFailedLoaded()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
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

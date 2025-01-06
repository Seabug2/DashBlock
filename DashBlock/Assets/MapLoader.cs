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
     * TODO : ī�޶� ������ ����
     * 1. ȭ���� ���� ����
     * 2. ��ũ���� ���� �� ���� �������� ��� ����� ���̵��� ī�޶� size�� ���߰�
     * 3. ��ϵ��� ���߾ӿ� ī�޶� ��ġ ���Ѿ���
     * 4. �� �۾��� ��ġ�� Fade In ���� ȭ����ȯ ȿ���� ��������
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

        // ù ��° �ٷ� ���� ������ ���
        string[] numbers = lines[0].Split(",");
        sbyte limit_x = (sbyte)numbers.Length;

        SetCamera(limit_x, limit_y);

        // ��� �� ũ�� �˻�
        if (limit_y > 127 || limit_x > 127)
        {
            Debug.LogError("Map size exceeds sbyte limits (127x127).");
            return; // ��� ũ�Ⱑ 127�� �ʰ��ϸ� ����
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

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
     * TODO : ī�޶� ������ ����
     * 1. ȭ���� ���� ����
     * 2. ��ũ���� ���� �� ���� �������� ��� ����� ���̵��� ī�޶� size�� ���߰�
     * 3. ��ϵ��� ���߾ӿ� ī�޶� ��ġ ���Ѿ���
     * 4. �� �۾��� ��ġ�� Fade In ���� ȭ����ȯ ȿ���� ��������
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

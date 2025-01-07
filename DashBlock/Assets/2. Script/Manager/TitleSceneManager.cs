using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    void Start()
    {
        TextAsset csv = Resources.Load("Maps/Title") as TextAsset;
        MapLoader.LoadMap(csv.text);
    }
}

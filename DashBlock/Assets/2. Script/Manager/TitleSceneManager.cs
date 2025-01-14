using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    void Start()
    {
        MapLoader.Init().Forget();
    }
}

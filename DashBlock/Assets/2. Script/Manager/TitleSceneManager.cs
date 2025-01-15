using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MapLoader.LoadMap().Forget();
        }
    }
}

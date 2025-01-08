using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class TitleSceneManager : MonoBehaviour
{
    void Start()
    {
        MapLoader.Init().Forget();
    }
}

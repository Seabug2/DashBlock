using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public class GoogleSheetManager : MonoBehaviour
{
    public static Dictionary<string, string> mapKeyValuePairs = new Dictionary<string, string>()
    {
        // Key : Map Name, Value : Map gid;
        { "Map_Data_Title" ,"95630366" },
        { "Map_Data_0", "1553283520" },
        { "Map_Data_1", "1268210948" },
        { "Map_Data_2", "573979549" }

    };

    public string putMapName;
    public string mapData;

    const string url = "https://docs.google.com/spreadsheets/d/194NAzYpdn938JB_HMUGmefgy66cs3sOhxcl2iUnOAms/export?format=csv";
    TextAsset csv;


    IEnumerator MapSheetRequest(string mapName)
    {
       
               
        UnityWebRequest www = UnityWebRequest.Get(url + "&gid=" + $"{mapKeyValuePairs[mapName]}");
        Debug.Log(url + "&gid =" + $"{mapKeyValuePairs[mapName]}");
        yield return www.SendWebRequest();

        mapData = www.downloadHandler.text;
        MapLoader.LoadMap(mapData);
        
        
    }




    private void Start()
    {
        StartCoroutine(MapSheetRequest(putMapName));

    }





}

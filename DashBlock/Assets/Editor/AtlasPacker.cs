using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AtlasPacker : EditorWindow
{
    int blockSize = 512;
    int atlasSizeInBlocks = 16;
    int atlasSize;

    Object[] rawTextures = new Object[512];

    List<Texture2D> sortedTextures = new List<Texture2D>();
    Texture2D atlas;


    [MenuItem("Window/Tools/Atlas Packer")]
    public static void ShowWindow()
    {
        GetWindow<AtlasPacker>("Atlas Packer");
    }

    private void OnGUI()
    {
        atlasSize = blockSize * atlasSizeInBlocks;

        GUILayout.Label("Dash Block Texture Atlas Packer", EditorStyles.boldLabel);

        blockSize = EditorGUILayout.IntField("Block Size", blockSize);
        atlasSizeInBlocks = EditorGUILayout.IntField("Atlas Size (in Blocks)", atlasSizeInBlocks);

        if(GUILayout.Button("Load Textures"))
        {
            Debug.Log("Atlas Packer : Loading Textures..");
            LoadTextures();                
        }

        if(GUILayout.Button("Clear Textures"))
        {
            atlas = new Texture2D(atlasSize, atlasSize);
            Debug.Log("Atlas Packer : Textures Cleared");

        }

        if(GUILayout.Button("Save Atlas"))
        {
            SaveAtlas();
        }

        if(atlas != null)
        {
            GUILayout.Label(atlas);
        }




    }



    private void LoadTextures()
    {
        sortedTextures.Clear();
        rawTextures = Resources.LoadAll("AtlasPacker", typeof(Texture2D));

        foreach(Object tex in rawTextures)
        {
            Texture2D t = (Texture2D)tex;
            if(t.width == blockSize && t.height == blockSize)
            {
                sortedTextures.Add(t);
            }
            else
            {
                Debug.Log("Atlas Packer : " + tex.name + "has incorrect size.");
            }        
        }

        Debug.Log("Atlas Packer : " + sortedTextures.Count + " textures loaded");
        PackAtlas();

    }

    void PackAtlas()
    {
        atlas = new Texture2D(atlasSize, atlasSize);
        Color[] pixels = new Color[atlasSize * atlasSize];

        for (int x = 0; x < atlasSize; x++)
        {
            for (int y = 0; y < atlasSize; y++)
            {
                int currentBlockX = x / blockSize;
                int currentBlockY = y / blockSize;

                int index = currentBlockY * atlasSizeInBlocks + currentBlockX;

                int currentPixelX = x - (currentBlockX * blockSize);
                int currentPixelY = y - (currentBlockY * blockSize);

                if (index < sortedTextures.Count)
                    pixels[(atlasSize - y - 1) * atlasSize + x] = sortedTextures[index].GetPixel(currentPixelX, blockSize - currentPixelY - 1);
                else
                    pixels[(atlasSize - y - 1) * atlasSize + x] = new Color(0f, 0f, 0f, 0f);
            }
        }

        atlas.SetPixels(pixels);
        atlas.Apply();
    }

    void SaveAtlas()
    {
        byte[] bytes = atlas.EncodeToPNG();
        string path = Application.dataPath + "/Sprite/Pack_Atlas.png";
        try
        {
            File.WriteAllBytes(path, bytes);
            Debug.Log("Atlas Packer: Atlas saved to " + path);
        }
        catch
        {
            Debug.LogError("Atlas Packer: Couldn't save atlas to file.");
        }
    }













}

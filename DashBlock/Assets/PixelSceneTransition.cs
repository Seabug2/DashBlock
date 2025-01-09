using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PixelSceneTransition : MonoBehaviour
{
    public int width;
    public int height;

    public float transitionTime = 0;
    private float pixelateSize = 0;
    private float pixelateSizeMax = 512;
    private float pixelateSizeMin = 1;

    public Image image;
    public Material screenMaterial;

    void Start()
    {
        width = Screen.width;
        height = Screen.height;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            SceneTransition();

        }
        
    }

    public void SceneTransition()
    {
        StartCoroutine(CaptureScreen());
    }

    IEnumerator CaptureScreen()
    {
        yield return new WaitForEndOfFrame();

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        tex.ReadPixels(new Rect(0,0, width, height), 0, 0);
        tex.Apply();        
        image.gameObject.SetActive(true);
        image.sprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        screenMaterial = image.material;
        screenMaterial.SetTexture("_MainTex", image.sprite.texture);


    }

    IEnumerator ScreenTransition()
    {
        yield return new WaitForSeconds(transitionTime);
        screenMaterial.EnableKeyword("PIXELATE_ON");
        screenMaterial.SetFloat("_PixelateSize", pixelateSizeMax);

        //while()
        //{
        //
        //}





        


        
        //
    }
}

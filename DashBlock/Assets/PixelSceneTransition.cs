using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelSceneTransition : MonoBehaviour
{
    public int width;
    public int height;

    public float transitionTime = 5f;
    private float pixelateSizeMax = 512;
    private float pixelateSizeMin = 1;

    public Image image;
    
    //public RawImage image;
    public Material screenMaterial;

    void Start()
    {
        width = Screen.width;
        height = Screen.height;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
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
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        image.gameObject.SetActive(true);
        image.sprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        screenMaterial = image.material;
        screenMaterial.SetTexture("_MainTex", image.sprite.texture);

        StartCoroutine(ScreenTransition());

    }

    IEnumerator ScreenTransition()
    {
        Debug.Log("ScreenTransition Start");
        float elapsedTime = 0f;
        screenMaterial.EnableKeyword("PIXELATE_ON");

        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionTime;
            float pixelateSize = Mathf.Lerp(pixelateSizeMax, pixelateSizeMin, t);
            screenMaterial.SetFloat("_PixelateSize", pixelateSize);
            yield return null;
        }

        screenMaterial.SetFloat("_PixelateSize", pixelateSizeMin);
        //screenMaterial.DisableKeyword("PIXELATE_ON");

        Debug.Log("ScreenTransition End");
        // 픽셀화 종료 후 이미지 비활성화
        //image.gameObject.SetActive(false);
    }
}

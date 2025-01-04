using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    [SerializeField]
    private float _shockwaveTime = 0.75f;

    [SerializeField]
    private Transform _actionBlockTransform;

    private Coroutine _coroutine;

    [SerializeField]
    private Material _material;

    private static int _waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");


    //private void Awake()
    //{
    //    _material = GetComponent<Material>();
    //}

    private void OnEnable()
    {
        CallShockWave();
    }

    private void CallShockWave()
    {
        transform.position = _actionBlockTransform.position; 
        _coroutine = StartCoroutine(ShockWaveAction(-0.3f, 1.0f));

    }

    private IEnumerator ShockWaveAction(float startPos, float endPos)
    {
        _material.SetFloat(_waveDistanceFromCenter, startPos);
        
        float lerpedAmount = 0f;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < _shockwaveTime)
        {
            elapsedTime += Time.deltaTime;

            lerpedAmount = Mathf.Lerp(startPos, endPos, (elapsedTime / _shockwaveTime));
            _material.SetFloat(_waveDistanceFromCenter, lerpedAmount);
            yield return null;
        }
        gameObject.SetActive(false);

    }

}

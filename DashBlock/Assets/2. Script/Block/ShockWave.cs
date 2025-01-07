using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO : ΩÃ±€≈Ê¿∏∑Œ ∏∏µÈ∞Ì ΩÕ¥Ÿ
public class ShockWave : Singleton
{
    [SerializeField]
    private float _shockwaveTime = 0.75f;

    private Coroutine _coroutine;

    [SerializeField]
    private Material _material;

    private static int _waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");


    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }

    public void CallShockWave(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
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

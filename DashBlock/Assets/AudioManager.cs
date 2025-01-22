using UnityEngine;

public class AudioManager : Singleton
{
    AudioSource audioSource;

    public AudioClip collisionClip;
    public AudioClip failedMoveClip;
    public AudioClip breakClip;
    public AudioClip restoreClip;
    public AudioClip gameoverClip;

    protected override void Awake()
    {
        base.Awake();
        if (!TryGetComponent(out audioSource))
            audioSource = gameObject.AddComponent<AudioSource>();

        ActionBlock.OnCollision += (_) => { AudioPlay(collisionClip); };
        ActionBlock.OnFailedMoveAction += (_) => { AudioPlay(failedMoveClip); };
        Block.OnDestroyed += (_) => { AudioPlay(breakClip); };
    }

    void AudioPlay(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    /*
    private void OnDestroy()
    {
        //어떻게 해제하지??
        //ActionBlock.OnCollision -= ;
        //Block.OnDestroyed -= ;
    }
    */
}

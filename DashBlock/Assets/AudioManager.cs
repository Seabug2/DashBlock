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
        //��� ��������??
        //ActionBlock.OnCollision -= ;
        //Block.OnDestroyed -= ;
    }
    */
}

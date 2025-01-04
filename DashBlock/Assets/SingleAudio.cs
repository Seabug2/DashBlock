using UnityEngine;

public class SingleAudio : Singleton
{
    AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        audioSource = gameObject.AddComponent<AudioSource>();
        Debug.Log("�ʱ�ȭ �Ϸ�");
    }

    public void PlayOneShot(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }
}

using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager singleton;
    public AudioClip[] starSound;
    public AudioClip starCreation;
    public AudioClip starBreak;
    public AudioClip startSound;
    public AudioClip stringSound;
    public AudioClip linkCutSound;

    void Awake() {
        singleton = this;
    }

    public void Play(AudioClip audioClip, float volume, AudioSource audioSource) {
        audioSource.PlayOneShot(audioClip, volume);
    }

}

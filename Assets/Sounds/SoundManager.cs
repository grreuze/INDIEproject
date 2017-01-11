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

    float lastTimePlayed;
    AudioClip lastAudioClip;
    public void Play(AudioClip audioClip, float volume, AudioSource audioSource) {
        if (audioClip != lastAudioClip || lastTimePlayed > 0.1f) {
            audioSource.PlayOneShot(audioClip, volume);
            lastAudioClip = audioClip;
            lastTimePlayed = Time.time;
        }
    }

}

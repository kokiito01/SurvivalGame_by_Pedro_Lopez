using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }


    public AudioSource toolSwingSound;
    public AudioSource startingZoneBGMusic;
    public AudioSource animalsFondo;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlaySound(AudioSource soundToPlay)
    {
        if (!soundToPlay.isPlaying)
        {
            soundToPlay.Play();
            Debug.Log("Sonido reproducido: " + soundToPlay.clip.name);
        }
        else
        {
            Debug.Log("El sonido ya está en reproducción: " + soundToPlay.clip.name);
        }
    }
}


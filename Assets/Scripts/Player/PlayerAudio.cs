using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    // As for now this will suffice
    // ig should add events for jumping 
    // or just write functions here 
    // and call them from PlayerControl.cs
    
    public static PlayerAudio Instance;
    public AudioSource jumpSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayJumpSound()
    {
        jumpSource.Play();
    }
}

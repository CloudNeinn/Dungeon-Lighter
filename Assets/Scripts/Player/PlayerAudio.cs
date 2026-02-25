using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    // As for now this will suffice
    // ig should add events for jumping 
    // or just write functions here 
    // and call them from PlayerController.cs
    
    public static PlayerAudio Instance;
    [SerializeField] private AudioSource _jumpSource;

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
        _jumpSource.Play();
    }
}

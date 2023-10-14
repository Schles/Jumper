using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    
    public AudioListener audioListener;

    private static SoundManager soundManager;

    public static SoundManager Instance {
        get {
        if (!soundManager) {
            soundManager = FindFirstObjectByType(typeof(SoundManager)) as SoundManager;

            if (!soundManager) {
            Debug.LogError("There needs to be one active SoundManager script on a GameObject in your scene.");
            } else {
            soundManager.Init();

            //  Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(soundManager);
            }
        }
        return soundManager;
        }
    }

    void Init() {
        // if (eventDictionary == null) {
        //     eventDictionary = new Dictionary<string, Action<Dictionary<string, object>>>();
        // }
    }

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip, position);
    }
}

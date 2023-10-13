using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fruit : MonoBehaviour, IDataPersistance
{
    [SerializeField] private string id;
    
    [ContextMenu("Set ID")]
    private void GenrateGuid()
    {
        id = Guid.NewGuid().ToString();
    }


    private Animator animator;

    private AudioSource audioSource;

    private bool hasBeenCollected = false; 
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        var levelName = SceneManager.GetActiveScene().name;
        if (GameProgress.levelProgress[levelName].collectedFruits.ContainsKey(id))
        {
            hasBeenCollected = GameProgress.levelProgress[levelName].collectedFruits[id];
        }

        if (hasBeenCollected)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasBeenCollected && other.gameObject.CompareTag("Player"))
        {
            hasBeenCollected = true;
            animator.SetBool("IsCollected", true);
            EventManager.TriggerEvent("collectedFruit", new Dictionary<string, object> { { "gameObject", gameObject } });
            var levelName = SceneManager.GetActiveScene().name;
            GameProgress.levelProgress[levelName].collectedFruits.Add(id, true);

            audioSource.Play();
            
        }
    }

    public void LoadGame(GameData data)
    {
        data.collectedFruits.TryGetValue(id, out hasBeenCollected);
        if(hasBeenCollected) {
            gameObject.SetActive(false);
        }
    }

    public void SaveGame(ref GameData data)
    {
        if (data.collectedFruits.ContainsKey(id))
        {
            data.collectedFruits.Remove(id);
        }
        
        
        data.collectedFruits.Add(id, hasBeenCollected);
        
    }

}

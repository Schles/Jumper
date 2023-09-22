using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    
    public static event Action<GameObject> OnFruitCollectedAction;
    
    private AudioSource audioSource;

    private bool hasBeenCollected = false; 
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
            OnFruitCollectedAction.Invoke(gameObject);
            audioSource.Play();
            
        }
    }
}

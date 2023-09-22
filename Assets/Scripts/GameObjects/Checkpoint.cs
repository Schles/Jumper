using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    private GameController gameController;
    public bool active = false;
    
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindAnyObjectByType<GameController>();
        GetComponent<Animator>().StopPlayback();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        this.active = true;
        GetComponent<Animator>().StartPlayback();
        GetComponent<AudioSource>().Play();
    }

    public void Deactivate()
    {
        this.active = false;
        GetComponent<Animator>().StopPlayback();
        
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {

        if (gameController.activeCheckpoint == this)
        {
            return;
        }
        
        if (gameController.activeCheckpoint)
        {
            gameController.activeCheckpoint.Deactivate();
        }
        
        gameController.activeCheckpoint = this;
        
        this.Activate();
    }
}

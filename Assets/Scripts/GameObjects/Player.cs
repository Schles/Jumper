using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{



    AudioSource[] sounds;

    public bool isDead = false;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        sounds = GetComponents<AudioSource>();
    }

    void OnEnable()
    {
        EventManager.StartListening("restart", OnRestart);
    }

    void OnDisable()
    {
        EventManager.StopListening("restart", OnRestart);
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("IsDead", this.isDead);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {     
        switch (other.gameObject.tag)
        {
            case "Goal":
                //sounds[2].Play();
                //GetComponent<PlayerMovement>().canDoubleJump = true;
                
                EventManager.TriggerEvent("finish", new Dictionary<string, object>{});
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
         
        switch (collision.gameObject.tag)
        {
            case "Trap":
                this.isDead = true;
                sounds[1].Play();

                break;

        }

    }

    private void OnRestart(Dictionary<string, object> message)
    {
        this.isDead = false;
        transform.position = (Vector3) message["spawnPoint"];
        animator.Rebind();
        animator.Update(0f);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class Player : MonoBehaviour
{



    AudioSource[] sounds;

    public static event Action<string> OnGameOverAction;
    public static event Action<string> OnGameWonAction;


    public bool isDead = false;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        sounds = GetComponents<AudioSource>();
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
                sounds[2].Play();
                GameState.Instance.canDoubleJump = true;
                //OnGameWonAction.Invoke("Gewonnen! Gratuliere :)");
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
                OnGameOverAction.Invoke("dead");
                break;

        }

    }
}

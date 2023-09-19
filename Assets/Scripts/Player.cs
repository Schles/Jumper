using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class Player : MonoBehaviour
{


    public static event Action<string> OnGameOverAction;
    public static event Action<string> OnGameWonAction;
    public static event Action<GameObject> OnFruitCollectedAction;

    public bool isDead = false;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
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
                OnGameWonAction.Invoke("Gewonnen! Gratuliere :)");
                break;

            case "Fruit":
                OnFruitCollectedAction.Invoke(other.gameObject);
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
         
        switch (collision.gameObject.tag)
        {
            case "Trap":
                //Destroy(this.gameObject);
                this.isDead = true;
                //OnGameOverAction.Invoke("Am Gegner gestorben");
                break;

        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        
    }
}

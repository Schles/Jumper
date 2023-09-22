using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public float bestTime = -1f;
    public int bestFruitsCollected = 0;
    public float lastTime = 0f;
    public int lastFruitsCollected = 0;
    public string lastReason;

    public bool canDoubleJump = false;

    public static GameState Instance;


    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

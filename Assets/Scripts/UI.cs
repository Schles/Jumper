using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{

    private GameController gameController;
    private Label labelGameTime;
    private Label labelBestTime;
    private Label labelLastTime;
    private Label labelLastReason;
    private Label labelFruits;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (GameState.Instance == null)
        {
            return;
        }
        */

        string doubleJump = GameState.Instance.canDoubleJump ? " | Double Jump enabled" : "";
        
        labelFruits.text = gameController.fruitsCollected.ToString() + " Fruit(s)" + doubleJump;
        
        labelGameTime.text =  gameTimeToHumanTime(gameController.gameTime);
        labelLastReason.text = GameState.Instance.lastReason;
    }

    private void OnEnable()
    {

        gameController = GameObject.FindFirstObjectByType<GameController>();

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        labelGameTime = root.Q<Label>("gameTime");
        labelLastReason = root.Q<Label>("lastReason");
        labelFruits = root.Q<Label>("fruits");
    }

    private string gameTimeToHumanTime(float gameTime)
    {
        float minutes = Mathf.Floor((gameTime %= 3600) / 60);
        float seconds = Mathf.Floor(gameTime % 60);
        
        
        return this.formatNumber(minutes, 2) + ":" + this.formatNumber(seconds, 2);
    }

    private string formatNumber(float number, int minDigits)
    {
        if (number < Math.Pow(10, minDigits - 1)) 
        {
            string res = "";
            for (int i = 0; i < minDigits - 1; i++)
            {
                res += "0";
            }

            return res + number.ToString();
        } else
        {
            return number.ToString();
        }
    }
}

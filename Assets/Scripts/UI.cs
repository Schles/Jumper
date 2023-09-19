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

        labelGameTime.text = "Time: " + gameController.fruitsCollected + " Fruits - " + this.gameTimeToHumanTime(gameController.gameTime);
        if (GameState.Instance.bestTime > 0f) 
            labelBestTime.text = "Best:" + GameState.Instance.bestFruitsCollected + " Fruits - " + this.gameTimeToHumanTime(GameState.Instance.bestTime);
        if (GameState.Instance.lastTime > 0f)
            labelLastTime.text = "Last:" + GameState.Instance.lastFruitsCollected + " Fruits - " +this.gameTimeToHumanTime(GameState.Instance.lastTime);
        labelLastReason.text = GameState.Instance.lastReason;
    }

    private void OnEnable()
    {

        gameController = GameObject.FindFirstObjectByType<GameController>();

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        labelGameTime = root.Q<Label>("gameTime");
        labelLastTime = root.Q<Label>("lastTime");
        labelBestTime = root.Q<Label>("bestTime");
        labelLastReason = root.Q<Label>("lastReason");
    }

    private string gameTimeToHumanTime(float gameTime)
    {
        float minutes = Mathf.Floor((gameTime %= 3600) / 60);
        float seconds = gameTime % 60;
        
        
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

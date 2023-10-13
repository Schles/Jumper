using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameProgress
{

    public static List<string> levels = new List<string>();

    public static string nextLevel = "Level2";

    public static string curLevel = "Level1";

    public static float gameTime = 0;

    public static SerializableDictionary<string, LevelProgress> levelProgress = new SerializableDictionary<string, LevelProgress>();

    static GameProgress() {
        levels.Add("Level1");
        levels.Add("Level2");
    }
}

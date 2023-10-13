using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{

    public float gameTime;
    public int fruitsCollected;
    public int deathCount;
    public bool canDoubleJump;

    public SerializableDictionary<string, bool> collectedFruits = new SerializableDictionary<string, bool>();

    public SerializableDictionary<string, bool> checkpoints = new SerializableDictionary<string, bool>();

    public string activeCheckpointId;

    public GameData() {
        deathCount = 0;
        gameTime = 0;
        canDoubleJump = false;
        fruitsCollected = 0;
        collectedFruits = new SerializableDictionary<string, bool>();
        checkpoints = new SerializableDictionary<string, bool>();
        activeCheckpointId = "";
    }

}

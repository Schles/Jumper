using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgress
{
    public float? bestTime;

    public SerializableDictionary<string, bool> collectedFruits = new SerializableDictionary<string, bool>();
}

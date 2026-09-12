using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic; 

public class SaveGame
{
    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;

    public float playerHealth;
    public float playerStamina;
    public float playerMana;

    public List<ItemSaveData> inventory = new List<ItemSaveData>();
    public string sceneName; 

    public class ItemSaveData
    {
        public string itemID;
        public int quantity; 
    }
}
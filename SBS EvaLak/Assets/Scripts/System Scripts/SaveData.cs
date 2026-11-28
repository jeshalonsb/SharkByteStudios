using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SaveData : MonoBehaviour
{
    public InventoryData _InventoryData1 = new InventoryData();
    public InventoryData _InventoryData2 = new InventoryData();

    //[SerializeField] private InventoryData _InventoryData = new InventoryData();
    [SerializeField] private Inventory inventory;

    [SerializeField] private List<string> SaveSceneList = new List<string>() { "HangarLevel", "DeckBuildScene", "ShopScene", "MechUpgrade", "BattleSceneTest", "WinScene", "LoseScene" };
    [SerializeField] private string sceneName;

    public void OnLevelWasLoaded() //at start of new scene loading
    {
        sceneName = SceneManager.GetActiveScene().name;
        if (SaveSceneList.Contains(sceneName)) { SaveIntoJson(); }
    }

    public void SaveIntoJson()
    {
        if (inventory == null) { inventory = GameObject.Find("DDOL").GetComponent<Inventory>(); }
        if (inventory.saveSlot == 1)
        { 
            UpdateData(_InventoryData1);
            string saveslot = JsonUtility.ToJson(_InventoryData1);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/InventoryData1.json", saveslot);
            Debug.Log("Saved data to Save Slot 1.");
        }
        if (inventory.saveSlot == 2)
        {
            UpdateData(_InventoryData2);
            string saveslot = JsonUtility.ToJson(_InventoryData2);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/InventoryData2.json", saveslot);
            Debug.Log("Saved data to Save Slot 2.");
        }
    }
    
    public void UpdateData(InventoryData _InventoryData)
    {
        if (inventory == null) { inventory = GameObject.Find("DDOL").GetComponent<Inventory>(); }

        _InventoryData.actionCards = inventory.deckList;
        _InventoryData.unlockedActionCards = inventory.unlockedDeckList;

        _InventoryData.componentCards = inventory.mechDeckList;
        _InventoryData.unlockedComponentCards = inventory.mechDeckList;

        _InventoryData.scrap = inventory.totalScrap;

        _InventoryData.headCardSlot = inventory.HeadCardSlot;
        _InventoryData.armsCardSlot = inventory.ArmsCardSlot;
        _InventoryData.legsCardSlot = inventory.LegsCardSlot;

        _InventoryData.headCardSlotValue = inventory.HeadCardSlotValue;
        _InventoryData.armsCardSlotValue = inventory.ArmsCardSlotValue;
        _InventoryData.legsCardSlotValue = inventory.LegsCardSlotValue;

        _InventoryData.endless = inventory.endless;
        _InventoryData.battleNum = inventory.battleNum;
        _InventoryData.saveslot = inventory.saveSlot;
    }
}


[System.Serializable]
public class InventoryData
{
    [Header("Owned Cards")]
    public string DeckList = "fulldeckList";
    public List<string> actionCards;
    public List<string> unlockedActionCards;

    public string MechList = "mechDeckList";
    public List<string> componentCards;
    public List<string> unlockedComponentCards;

    [Header("Scrap")]
    public int scrap;

    [Header("Active Effects")]
    public string headCardSlot;
    public string armsCardSlot;
    public string legsCardSlot;

    public int headCardSlotValue;
    public int armsCardSlotValue;
    public int legsCardSlotValue;

    [Header("Game Mode Data")]
    public bool endless; //save your gamemode
    public int battleNum; //save how many battles youve done if endless
    public int saveslot;
}

public class JsonLoader : MonoBehaviour
{
    public string filePath;
    public int saveslot;
    public Inventory inventory;

    public void LoadData()
    {
        if(saveslot == 1) { Path.Combine(Application.persistentDataPath, "InventoryData1.json"); }
        if(saveslot == 2) { Path.Combine(Application.persistentDataPath, "InventoryData2.json"); }
        
        if(File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            InventoryData data = JsonUtility.FromJson<InventoryData>(json);
            Debug.Log("Loaded Save Slot.");

            SendDataToInventory(data);
        }
        else { Debug.LogWarning("Save File not found at: " + filePath); }
    }

    public void SendDataToInventory(InventoryData data)
    {
        //save data
        if(saveslot == 1)
        {
            SaveData saveData = GetComponent<SaveData>();
            saveData._InventoryData1 = data;
        }
        if (saveslot == 2)
        {
            SaveData saveData = GetComponent<SaveData>();
            saveData._InventoryData2 = data;
        }

        //send to inventory
        UpdateData(data);
    }

    public void UpdateData(InventoryData data)
    {
        if (inventory == null) { inventory = GameObject.Find("DDOL").GetComponent<Inventory>(); }

        inventory.deckList = data.actionCards;
        inventory.unlockedDeckList = data.unlockedActionCards;

        inventory.mechDeckList = data.componentCards;
        inventory.mechDeckList = data.unlockedComponentCards;

        inventory.totalScrap = data.scrap;

        inventory.HeadCardSlot = data.headCardSlot;
        inventory.ArmsCardSlot = data.armsCardSlot;
            inventory.LegsCardSlot = data.legsCardSlot;

        inventory.HeadCardSlotValue = data.headCardSlotValue;
        inventory.ArmsCardSlotValue = data.armsCardSlotValue;
        inventory.LegsCardSlotValue = data.legsCardSlotValue;

        inventory.endless = data.endless;
        inventory.battleNum = data.battleNum;
        inventory.saveSlot = data.saveslot;
    }
}
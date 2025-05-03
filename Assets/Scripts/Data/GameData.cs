using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public bool hasDoubleJump;
    public bool hasHighJump;
    public bool hasDash;
    public bool hasSpecialAttack;
    public bool hasBarrier;
    public bool hasDeflectProjectile;
    public bool hasWallJump;
    public bool hasGlider;
    public bool hasTeleport;
    public bool hasHook;
    public int constDJI;
    public Vector3 playerPosition;
    public int[] saveSceneIDs;
    public int totalCurrency;
    public string currentCheckpointID;
    public SerializableDictionary<string, bool> doorCondition;
    public List<string> inventoryItemsID;
    public List<string> equippedItemsActiveID;
    public List<string> equippedItemsPassiveID;
    public SerializableDictionary<string, List<string>> storeItems;
    public List<string> mainWorldVisitedScenes;
    public List<string> otherWorldVisitedScenes;

    // values defined in the construcor will be the initial values for the game
    public GameData()
    {
        this.playerPosition = new Vector3(-56f, 39.1f, 1.8f);
        doorCondition = new SerializableDictionary<string, bool>();
        hasHighJump = false;
        hasDash = false;
        hasWallJump = false;
        hasSpecialAttack = false;
        hasBarrier = false;
        hasDeflectProjectile = false;
        hasDoubleJump = false;
        hasGlider = false;
        hasTeleport = false;
        hasHook = false;
        constDJI = 0;
        saveSceneIDs = new int[] {2, 3};
        totalCurrency = 0;
        inventoryItemsID = new List<string>();
        equippedItemsActiveID = new List<string> {null};
        equippedItemsPassiveID = new List<string> {null, null, null};
        storeItems = new SerializableDictionary<string, List<string>>()
        {
            { "93b04e2e-15ee-4f88-86f5-72de0bc12346", new List<string> { "ef8dfcdc-ba94-4d30-bdb8-438c872c9426", "a4ce64bd-4d44-4abf-8a0a-6966100edb2d", "0edde7ac-b749-4c95-8fbc-4f4fc1cccde0"} }, // flying island --- tote of speed --- barrier --- fall attack
            { "12a30e5c-e41f-4815-a0d0-928cf4720192", new List<string> { "28b7a68e-af16-45f4-af51-fc58de915088", "f4340b71-6b68-41c7-92b4-7c123fed3768"} }, // main world third scene --- magic missile --- totem of dashing
            { "3798d47a-3478-4061-b908-7f8fe656aa72", new List<string> { "502abe89-472e-46ae-add4-ac95450d0218", "bb6c4685-d0b3-4e28-b4bb-eff4c38b42f3"} }  // other world spawn scene --- totem of jumping -- crown of stars
        };
        mainWorldVisitedScenes = new List<string>();
        otherWorldVisitedScenes = new List<string>();
    }
}

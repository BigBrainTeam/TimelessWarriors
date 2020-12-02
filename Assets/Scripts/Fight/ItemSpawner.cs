using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour {

    List<GameObject> spawnableItems;
    public List<GameObject> currentItems;
    [SerializeField] int maxItems = 0, maxX = 0, minX = 0, y = 0;
    int possibleItems;
    System.Random rnd;

	// Use this for initialization
	void Start () {
        loadItems();
        initValues();

        if(spawnableItems.Count>0) InvokeRepeating("spawnCheck", 10f, 10f);
	}

    private void initValues()
    {
        possibleItems = spawnableItems.Count;
        rnd = new System.Random();
    }

    private void loadItems()
    {
        spawnableItems = new List<GameObject>();
        currentItems = new List<GameObject>();

        foreach(string item in Settings.Instance.fightSettings.selectedItems)
        {
            GameObject go = Resources.Load<GameObject>("Items/" + item);
            spawnableItems.Add(go);
        }
    }

    private void spawnCheck()
    {
        int rndmVal = rnd.Next(100);
        if (currentItems.Count < maxItems && rndmVal >= 50) spawnItem();
    }

    public int getRandomX()
    {
        return rnd.Next(minX, maxX);
    }

    private void spawnItem()
    {
        int randomValue = rnd.Next(possibleItems);
        int randomPos = getRandomX();
        Vector2 position = new Vector2(randomPos, y);
        GameObject go = Instantiate(spawnableItems[randomValue], position, Quaternion.identity);
        currentItems.Add(go);
    }
}

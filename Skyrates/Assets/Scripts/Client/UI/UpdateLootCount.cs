using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Data;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using UnityEngine;
using UnityEngine.UI;

public class UpdateLootCount : MonoBehaviour
{

    public LocalData data;

    public Text LootText;

    public GameObject LootPlusPrefab;

    public Transform LootPlusSpawn;

    public float LootPlusVelocity;

    public float LootPlusTime;

    private readonly Queue<Coroutine> LootPlusQueue = new Queue<Coroutine>();

    private void Update()
    {
		//TODO: Add new UI.
        //this.LootText.text = string.Format("{0} / {1}", this.data.LootCount, this.data.LootGoal);
    }

    private void OnEnable()
    {
        GameManager.Events.LootCollected += this.OnLootCollected;
    }

    private void OnDisable()
    {
        GameManager.Events.LootCollected -= this.OnLootCollected;
    }

    // only fired when the loot is collected locally
    void OnLootCollected(GameEvent evt)
    {
        //Debug.Log("Loot collected");
        this.AddLootPlus();
    }

    void AddLootPlus()
    {
        this.LootPlusQueue.Enqueue(StartCoroutine(this.AddLootPlusFloater()));
    }

    IEnumerator AddLootPlusFloater()
    {
        GameObject generated = Instantiate(this.LootPlusPrefab, this.LootPlusSpawn);
        
        float timeRemaining = this.LootPlusTime;
        float timePrevious = Time.time;
        while (timeRemaining > 0)
        {
            // wait for next frame
            yield return null;

            float timeElapsed = Time.time - timePrevious;
            timePrevious = Time.time;
            timeRemaining -= timeElapsed;

            generated.transform.position += Vector3.up * this.LootPlusVelocity * timeElapsed;
        }

        Destroy(generated);

        this.LootPlusQueue.Dequeue();
    }

}
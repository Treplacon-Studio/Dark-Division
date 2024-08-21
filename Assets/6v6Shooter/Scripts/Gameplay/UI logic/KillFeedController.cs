using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject killFeedEntryPrefab;
    [SerializeField] private Transform killFeedContainer;
    [SerializeField] private float killFeedDuration = 5f;

    private Queue<GameObject> killFeedQueue = new Queue<GameObject>();

    // Call this function when a player is killed
    public void AddKillFeedEntry(string killerName, string victimName, Sprite weaponIcon)
    {
        // Instantiate a new kill feed entry
        GameObject newEntry = Instantiate(killFeedEntryPrefab, killFeedContainer);
        
        // Get UI elements in the entry (Text and Image)
        Text[] texts = newEntry.GetComponentsInChildren<Text>();
        texts[0].text = killerName;  // Killer's name
        texts[1].text = victimName;  // Victim's name
        
        Image weaponImage = newEntry.GetComponentInChildren<Image>();
        weaponImage.sprite = weaponIcon;

        // Add to the queue and start coroutine to remove it after a delay
        killFeedQueue.Enqueue(newEntry);
        StartCoroutine(RemoveKillFeedEntryAfterDelay(newEntry, killFeedDuration));
    }

    // Coroutine to remove the kill feed entry after a delay
    private IEnumerator RemoveKillFeedEntryAfterDelay(GameObject entry, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Remove the entry from the queue and destroy the object
        if (killFeedQueue.Count > 0)
        {
            GameObject expiredEntry = killFeedQueue.Dequeue();
            Destroy(expiredEntry);
        }
    }
}

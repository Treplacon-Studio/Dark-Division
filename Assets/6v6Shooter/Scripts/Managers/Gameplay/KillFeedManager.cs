using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class KillFeedManager : MonoBehaviourPunCallbacks
{
    public static KillFeedManager Instance { get; private set; }

    [Header("KillFeed")]
    public GameObject killFeedElementPrefab;
    public Sprite M4A1Icon;
    public Sprite scarIcon;
    public Sprite tac45Icon;
    public Sprite vectorIcon;
    public Sprite velIcon;
    public Sprite stoegerIcon;
    public Sprite gauge320Icon;
    public Sprite fnFiveIcon;
    public Sprite dsr50Icon;
    public Sprite balistaIcon;

    private Dictionary<string, Sprite> weaponIcons;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensures only one instance exists
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: preserves across scenes
        }

        InitializeWeaponIcons();
    }

    private void InitializeWeaponIcons()
    {
        weaponIcons = new Dictionary<string, Sprite>
        {
            { "M4A1Sentinel254", M4A1Icon },
            { "ScarEnforcer557", scarIcon },
            { "Tac45", tac45Icon },
            { "VectorGhost500", vectorIcon },
            { "VelIronclad308", velIcon },
            { "Stoeger22", stoegerIcon },
            { "Gauge320", gauge320Icon },
            { "FnFive8", fnFiveIcon },
            { "Dsr50", dsr50Icon },
            { "BalistaVortex", balistaIcon },
        };
    }

    // Broadcast the kill feed info to all players
    [PunRPC]
    public void ShareKillFeed(string victimName, string killerName, string weaponName)
    {
        Debug.Log($"ShareKillFeed called with victim: {victimName}, killer: {killerName}, weapon: {weaponName}");

        object[] data = new object[] { victimName, killerName, weaponName };

        PhotonNetwork.RaiseEvent(
            0, 
            data, 
            new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.All }, 
            new ExitGames.Client.Photon.SendOptions { Reliability = true }
        );
    }



    // Local method to be called by players to update their own UI
    public void UpdateKillFeedLocally(string victimName, string killerName, string weaponName, Transform killFeedTransform)
    {
        Debug.Log($"UpdateKillFeedLocally called with victim: {victimName}, killer: {killerName}, weapon: {weaponName}");

        GameObject killFeedElement = Instantiate(killFeedElementPrefab, killFeedTransform);

        TextMeshProUGUI victimNameText = killFeedElement.transform.Find("VictimNameText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI killerNameText = killFeedElement.transform.Find("KillerNameText").GetComponent<TextMeshProUGUI>();
        Transform gunIconTransform = killFeedElement.transform.Find("GunIcon");

        if (victimNameText != null) victimNameText.text = victimName;
        if (killerNameText != null) killerNameText.text = killerName;

        if (gunIconTransform != null)
        {
            Image gunIconImage = gunIconTransform.GetComponent<Image>();
            if (gunIconImage != null && weaponIcons.TryGetValue(weaponName, out Sprite weaponIcon))
            {
                gunIconImage.sprite = weaponIcon;
                gunIconImage.enabled = true;
            }
        }

        killFeedElement.SetActive(true);
        StartCoroutine(RemoveFeed(killFeedElement));
    }




    private IEnumerator RemoveFeed(GameObject killFeedElement)
    {
        CanvasGroup canvasGroup = killFeedElement.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("No CanvasGroup component found.");
            yield break;
        }

        yield return new WaitForSeconds(6f);

        float fadeDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        killFeedElement.SetActive(false);

        Destroy(killFeedElement);
    }
}

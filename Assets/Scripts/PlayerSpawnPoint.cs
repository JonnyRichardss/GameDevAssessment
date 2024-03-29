using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class PlayerSpawnPoint : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject bulletPrefab;
    public GameObject hitscanPrefab;
    public Slider healthBar;
    public Slider chargeBar;
    public Camera UICamera;
    public Image healthIcon;
    public Image chargeIcon;
    public Image damageIcon;

    public GameObject allSpawns;
    private GameObject player;
    private PlayerController newScript;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
    }
    public void SpawnPlayer()
    {
        player = Instantiate(playerPrefab, gameObject.transform);
        newScript = player.GetComponent<PlayerController>();
        newScript.healthBar = healthBar;
        newScript.chargeBar = chargeBar;
        newScript.bulletPrefab = bulletPrefab;
        newScript.hitscanPrefab = hitscanPrefab;
        newScript.healthIcon = healthIcon;
        newScript.chargeIcon = chargeIcon;
        newScript.damageIcon = damageIcon;
        var camData = player.GetComponentInChildren<Camera>().GetUniversalAdditionalCameraData();
        camData.cameraStack.Add(UICamera);
        if (allSpawns != null)
        {
            allSpawns.SendMessage("OnNewPlayer", player);
        }
    }
}

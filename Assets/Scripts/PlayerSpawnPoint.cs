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
    
    private GameObject player;
    private PlayerController newScript;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer(100f,1f);
    }
    public void SpawnPlayer(float health,float charge)
    {
        player = Instantiate(playerPrefab, gameObject.transform);
        newScript = player.GetComponent<PlayerController>();
        newScript.healthBar = healthBar;
        newScript.chargeBar = chargeBar;
        newScript.health = health;
        newScript.weaponCharge = charge;
        newScript.bulletPrefab = bulletPrefab;
        newScript.hitscanPrefab = hitscanPrefab;
        var camData = player.GetComponentInChildren<Camera>().GetUniversalAdditionalCameraData();
        camData.cameraStack.Add(UICamera);
    }
}

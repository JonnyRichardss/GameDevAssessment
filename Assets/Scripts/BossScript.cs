using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;
using UnityEngine.SceneManagement;
public class BossScript : MonoBehaviour
{
    public GameObject player;
    public GameObject bulletPrefab;
    public Slider bossBar;
    public TextMeshProUGUI text;
    public TextMeshProUGUI winText;
    private bool bezerk=false;
    private float bezerkMult = 1f;
    private float maxHealth = 500f;
    private float health;
    private float attackTimer;
    private NavMeshAgent nav;

    // Start is called before the first frame update
    void Start()
    {
        winText.text = "";
        nav = GetComponent<NavMeshAgent>();
        health = maxHealth;
        Physics.IgnoreLayerCollision(6, 9);
        Physics.IgnoreLayerCollision(6, 7);
        Image[] images = bossBar.GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            if (image.name == "Bar")
            {
                image.color = Color.magenta;
            }

        }
    }
    // Update is called once per frame
    void Update()
    {
        VariableHolder.bossTimer += Time.deltaTime;
        nav.SetDestination(player.transform.position);
        transform.LookAt(player.transform);
        transform.GetChild(0).LookAt(player.transform);
        transform.GetChild(1).LookAt(player.transform);
        if (attackTimer <= 0)
        {
            PickAttack();
        }
        else
        {
            attackTimer -= Time.deltaTime*bezerkMult;
        }
        if (!bezerk) 
        { 
            BezerkCheck();
            text.text = string.Format("Score: {0}\nBoss Time: {1}", VariableHolder.playerScore, VariableHolder.bossTimer);
        }
        else
        {
            text.text = string.Format("Score: {0}\nBoss Time: {1}\n BOSS IS GOING BEZERK!!", VariableHolder.playerScore, VariableHolder.bossTimer);
        }
        bossBar.value = health;
        
    }
    void BezerkCheck()
    {
        if (health/maxHealth <= .25)
        {
            bezerk = true;
            bezerkMult = 2;
            nav.speed *= 2;
        }
    }
    void PickAttack()
    {
        switch (Mathf.RoundToInt(Random.value))
        {
            case 0:
                FireBullets();
                break;
            case 1:
                StartCoroutine(Punch(Mathf.RoundToInt(Random.value)));
                break;
        }
        attackTimer = Random.Range(.7f, 2f);
    }
    void FireBullets()
    {

        for(float angle = -90f*(1f/4f); angle <= 90f * (1f / 4f) ;angle += 90f / 16f)
        {
            GameObject newBullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            newBullet.transform.Rotate(Vector3.up, angle);
            EnemyBulletScript script = newBullet.GetComponent<EnemyBulletScript>();
            newBullet.name = "EnemyShot";
            script.impulse = .15f*bezerkMult;
            script.lifetime = 4f;
            script.damage = 5f*bezerkMult;
            script.bulletParent = gameObject;
        }
    }
    IEnumerator Punch(int armIndex)
    {
        for (int i = 0; i < 10; i++)
        {
            transform.GetChild(armIndex).localPosition += new Vector3(0f, 0f, .15f);
            yield return new WaitForSeconds(.01f);
        }
        for (int i = 0; i < 10; i++)
        {
            transform.GetChild(armIndex).localPosition += new Vector3(0f, 0f, -.15f);
            yield return new WaitForSeconds(.01f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.SendMessage("OnDamageTaken", 10f*bezerkMult);
        }
        if (collision.collider.CompareTag("Bullet"))
        {
            health -= 2f;

        }
    }
    private void RemoveHealth(float damage)
    {
        health -= damage;
        VariableHolder.playerScore += damage;
        if (health <= 0)
        {
            bezerkMult = 0;
            winText.text = "You Win!!!";
            StartCoroutine(TitleAfterFive());
            VariableHolder.bossBeaten = true;
            VariableHolder.bossTimer -= 5;
        }
    }
    IEnumerator TitleAfterFive()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("TitleScreen");
    }
    public void OnScannedHit(float damage)
    {
        RemoveHealth(damage);
    }
}

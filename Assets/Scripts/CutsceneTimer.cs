using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CutsceneTimer : MonoBehaviour
{
    float timer = 5;
    public AudioSource source;
    // Update is called once per frame
    private void Start()
    {
        source.Play();
    }
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            SceneManager.LoadScene("BossLevel");
        }
    }
}

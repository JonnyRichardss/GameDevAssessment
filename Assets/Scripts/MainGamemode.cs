using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainGamemode : MonoBehaviour
{
    public TextMeshProUGUI UItext;
    private float modeTimer;
    // Start is called before the first frame update
    void Start()
    {
        VariableHolder.ResetVars();
        modeTimer = 60f;
    }

    // Update is called once per frame
    void Update()
    {
        modeTimer -= Time.deltaTime;
        UItext.text = string.Format("Score: {0}\nTime Left: {1}", VariableHolder.playerScore, modeTimer);
        if (modeTimer <= 0)
        {
            SceneManager.LoadScene("BossCutscene");
        }
    }
}

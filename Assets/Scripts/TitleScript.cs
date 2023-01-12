using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class TitleScript : MonoBehaviour
{
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        string bossTime;
        if (VariableHolder.bossBeaten)
        {
            bossTime = VariableHolder.bossTimer.ToString();
        }
        else
        {
            bossTime = "Not Beaten!";
        }
        text.text = string.Format("Last score: {0}\nLast Boss Time {1}",VariableHolder.playerScore,bossTime);
    }
    public void BeginGame()
    {
        SceneManager.LoadScene("MainLevel");
    }
    public void QuitGame()
    {
        Debug.Log("QUITBUTTON");
        Application.Quit();
    }

}

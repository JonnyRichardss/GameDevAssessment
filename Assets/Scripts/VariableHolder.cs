using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VariableHolder
{
    public static float playerHealth;
    public static float playerScore;
    public static float playerCharge;
    public static bool godMode;
    public static bool shotgun;
    public static void ResetVars()
    {
        playerHealth = 100f;
        playerScore = 0f;
        playerCharge = 1f;
        godMode = false;
        shotgun = false;
    }
}

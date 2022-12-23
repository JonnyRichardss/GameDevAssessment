using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationScript : MonoBehaviour
{
    public enum NavType
    {
        Collision,
        AntiCollision,
        Radius
    }
    public bool moving = false;
    public float moveTimer = 0;
    public void Navigate(GameObject target, NavType navType, float timerLength)
    {
        if (moving)
        {
            DecrementTimer();
            return;
        }
        moveTimer = 1;
        moving = true;
    }
    public void DecrementTimer()
    {
        moveTimer -= Time.deltaTime;
        if (moveTimer < 0)
        {
            moving = false;
        }
    }
}

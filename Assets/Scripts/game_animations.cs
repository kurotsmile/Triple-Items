using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class game_animations : MonoBehaviour
{
    public void stop_animation()
    {
        GameObject.Find("Games").GetComponent<Games>().anim.Play("Game_Main");
    }
}
 
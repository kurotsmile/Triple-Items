using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Games : MonoBehaviour
{
    public Carrot.Carrot carrot;
    public Box_Manager boxs;

    [Header("Sounds")]
    public AudioSource soundBk;

    void Start()
    {
        this.carrot.Load_Carrot(this.check_exit_app);
        this.carrot.game.load_bk_music(this.soundBk);
        this.boxs.on_load();
    }

    private void check_exit_app()
    {
     
    }

    void Update()
    {
        
    }

    public void btn_setting()
    {
        this.carrot.Create_Setting();
    }
}

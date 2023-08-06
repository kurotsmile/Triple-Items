using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Games : MonoBehaviour
{
    public Carrot.Carrot carrot;
    public Box_Manager boxs;
    public GameObject effect_explosie;

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

    public void btn_setting()
    {
        this.carrot.Create_Setting();
    }

    public void create_effect(Vector3 pos)
    {
        GameObject obj_effect = Instantiate(this.effect_explosie);
        obj_effect.transform.SetParent(this.transform.root);
        obj_effect.transform.position = pos;
        obj_effect.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}

using UnityEngine;

public class Games : MonoBehaviour
{
    public Carrot.Carrot carrot;
    public Box_Manager boxs;
    public GameObject[] effect_prefab;

    [Header("Sounds")]
    public AudioSource soundBk;
    public AudioSource[] sound;

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

    public void create_effect(Vector3 pos,int index=0)
    {
        GameObject obj_effect = Instantiate(this.effect_prefab[index]);
        obj_effect.transform.SetParent(this.transform.root);
        obj_effect.transform.position = pos;
        obj_effect.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void play_sound(int index_sound)
    {
        if(this.carrot.get_status_sound()) this.sound[index_sound].Play();
    }

    public void btn_top_player()
    {
        this.play_sound(0);
        this.carrot.game.Show_List_Top_player();
    }

    public void btn_user_login()
    {
        this.carrot.user.show_login();
    }

    public void update_score_to_server(int score)
    {
        if (Random.Range(0, 3) == 1)
        {
            this.carrot.game.update_scores_player(score);
        }
    }
}

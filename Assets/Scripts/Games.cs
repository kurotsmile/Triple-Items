using UnityEngine;

public class Games : MonoBehaviour
{
    public Carrot.Carrot carrot;
    public Box_Manager boxs;
    public History history;
    public GameObject[] effect_prefab;
    public IronSourceAds ads;

    [Header("Sounds")]
    public AudioSource soundBk;
    public AudioSource[] sound;

    [Header("Animations")]
    public Animator anim;

    [Header("Asset Icon")]
    public Sprite spIconHistory;
    public Sprite spIconHistoryPlay;
    public Sprite spIconClear;

    void Start()
    {
        this.carrot.Load_Carrot(this.check_exit_app);
        this.carrot.game.load_bk_music(this.soundBk);
        this.boxs.on_load();
        this.history.OnLoad();
    }

    private void check_exit_app()
    {
        if (boxs.PanelGameOver.activeInHierarchy)
        {
            boxs.BtnPlayAgain();
            carrot.set_no_check_exit_app();
        }
        else if (boxs.PanelGamePause.activeInHierarchy)
        {
            boxs.BtnContinue();
            carrot.set_no_check_exit_app();
        }
    }

    public void btn_setting()
    {
        this.ads.show_ads_Interstitial();
        this.carrot.Create_Setting();
    }

    public void CreateEffect(Vector3 pos, int index = 0, float scale = 1f)
    {
        GameObject obj_effect = Instantiate(this.effect_prefab[index]);
        obj_effect.transform.SetParent(this.transform.root);
        obj_effect.transform.position = pos;
        obj_effect.transform.localScale = new Vector3(scale, scale, scale);
        Destroy(obj_effect, 1f);
    }

    public void play_sound(int index_sound)
    {
        if (this.carrot.get_status_sound()) this.sound[index_sound].Play();
    }

    public void BtnHistory()
    {
        this.ads.show_ads_Interstitial();
        this.carrot.play_sound_click();
        this.history.ShowList();
    }
}

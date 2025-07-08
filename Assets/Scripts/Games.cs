using Carrot;
using UnityEngine;
using UnityEngine.UI;

public class Games : MonoBehaviour
{
    [Header("Object Main")]
    public Carrot.Carrot carrot;
    public Box_Manager boxs;
    public History history;
    public GameObject[] effect_prefab;
    public IronSourceAds ads;
    public Carrot_DeviceOrientationChange r;

    [Header("Object Ui")]
    public GameObject panelHome;
    public GameObject panelPlay;

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
        boxs.OnLoad();
        this.history.OnLoad();
        this.panelHome.SetActive(true);
        this.panelPlay.SetActive(false);

        this.ads.On_Load();
        this.carrot.act_buy_ads_success = this.ads.RemoveAds;
        this.carrot.game.act_click_watch_ads_in_music_bk = this.ads.ShowRewardedVideo;
        this.ads.onRewardedSuccess = this.carrot.game.OnRewardedSuccess;

        carrot.delay_function(2f, this.CheckRotateScene);
    }

    private void check_exit_app()
    {
        if (panelPlay.activeInHierarchy)
        {
            BtnOnBackHome();
            carrot.set_no_check_exit_app();
        }
        else if (boxs.PanelGameOver.activeInHierarchy)
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

    public void CheckRotateScene()
    {
        carrot.delay_function(1.2f, () =>
        {
            if (this.r.Get_status_portrait())
                boxs.area_body_all_item.GetComponent<GridLayoutGroup>().constraintCount = 5;
            else
                boxs.area_body_all_item.GetComponent<GridLayoutGroup>().constraintCount = 6;
        });
    }

    public void btn_setting()
    {
        this.ads.show_ads_Interstitial();
        Carrot_Box boxSetting = this.carrot.Create_Setting();
        Carrot_Box_Item itemHistory = boxSetting.create_item_of_top();
        itemHistory.set_icon(spIconHistory);
        itemHistory.set_title("Play History");
        itemHistory.set_tip("Your play history and scores");
        itemHistory.set_act(BtnHistory);
    }

    public void CreateEffect(Vector3 pos, int index = 0, float scale = 1f, float timerDestroy = 1f)
    {
        GameObject obj_effect = Instantiate(this.effect_prefab[index]);
        obj_effect.transform.SetParent(this.transform.root);
        obj_effect.transform.position = pos;
        obj_effect.transform.localScale = new Vector3(scale, scale, scale);
        Destroy(obj_effect, timerDestroy);
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

    public void BtnSelModePlay(int indexMode)
    {
        this.ads.show_ads_Interstitial();
        this.carrot.play_sound_click();
        this.panelHome.SetActive(false);
        this.panelPlay.SetActive(true);
        if (indexMode == 0)
        {
            boxs.TextNameMode.text = "Easy mode";
            boxs.StartGame(30, 2);
        }

        if (indexMode == 1)
        {
            boxs.TextNameMode.text = "Normal Mode";
            boxs.StartGame(60, 3);
        }

        if (indexMode == 2)
        {
            boxs.TextNameMode.text = "Super Hard Mode";
            boxs.StartGame(68, 4);
        }
    }

    public void BtnOnBackHome()
    {
        this.ads.show_ads_Interstitial();
        this.panelHome.SetActive(true);
        this.panelPlay.SetActive(false);
        this.carrot.play_sound_click();
        boxs.IsPlay = false;
    }

    public void BtnShare()
    {
        this.carrot.show_share();
    }

    public void BtnOtherGame()
    {
        carrot.show_list_carrot_app();
    }

    public void BtnRate()
    {
        carrot.show_rate();
    }
}

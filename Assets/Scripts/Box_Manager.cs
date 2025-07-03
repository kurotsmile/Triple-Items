using System.Collections;
using System.Collections.Generic;
using Carrot;
using UnityEngine;
using UnityEngine.UI;

public class Box_Manager : MonoBehaviour
{
    public Games game;
    public GameObject panel_loading;
    public GameObject PanelGameOver;
    public GameObject PanelGamePause;

    [Header("Icons")]
    public Sprite sp_icon_all_style;
    public Sprite sp_icon_buy;

    [Header("Ui")]
    public Slider sliderTimer;
    public Image imgBkStatus;

    [Header("Obj Main")]
    public Transform area_body_all_item;
    public Transform area_body_all_scroll;
    public GameObject prefab_box_items;
    public Sprite[] sp_item_box;
    public Color32[] color_bk_box;
    public Text txt_scores;
    public GameObject objBtnClear;
    private int scores = 0;

    [Header("Tray check")]
    public Transform[] tr_check;
    private List<box_items> list_items_tray;
    private List<Sprite> list_sp_icon;

    [Header("Msg Scores")]
    public Text txt_msg_scores;
    private int max_box_item = 82;
    private GameObject obj_Item_Cur = null;
    private float TimerSpeed = 1.2f;
    public bool IsPlay = true;

    public void on_load()
    {
        this.PanelGameOver.SetActive(false);
        this.PanelGamePause.SetActive(false);
        this.CreateTable();
    }

    private void CreateTable()
    {
        this.IsPlay = true;
        this.sliderTimer.value = 1;
        this.objBtnClear.SetActive(false);
        this.list_sp_icon = new List<Sprite>();

        for (int i = 0; i < this.sp_item_box.Length; i++) this.list_sp_icon.Add(this.sp_item_box[i]);

        this.GetComponent<Games>().carrot.clear_contain(this.area_body_all_item);
        this.list_items_tray = new List<box_items>();

        for (int i = 0; i < max_box_item; i++)
        {
            int index_rand = Random.Range(0, this.list_sp_icon.Count);
            GameObject item_obj = Instantiate(this.prefab_box_items);
            item_obj.transform.SetParent(this.area_body_all_item);
            item_obj.transform.localPosition = new Vector3(0, 0, 0);
            item_obj.transform.localScale = new Vector3(1f, 1f, 1f);
            item_obj.GetComponent<box_items>().set_type(box_status_type.in_body);
            item_obj.GetComponent<box_items>().set_data(this.list_sp_icon[index_rand], index_rand);
            item_obj.GetComponent<box_items>().set_color_bk(Color.white);
        }
        this.scores = 0;
        this.panel_loading.SetActive(false);
        this.check_scores();
    }

    public Transform get_tr_tray_none()
    {
        for (int i = 0; i < this.tr_check.Length; i++)
        {
            if (this.tr_check[i].childCount == 0) return this.tr_check[i].transform;
        }
        return null;
    }

    public void add_box_to_tray(box_items box_item)
    {
        this.objBtnClear.SetActive(true);
        this.panel_loading.SetActive(false);
        this.list_items_tray.Add(box_item);
        if (this.list_items_tray.Count >= 3)
        {
            this.objBtnClear.SetActive(false);
            bool is_true = true;
            int type_box = this.list_items_tray[0].get_type_box();
            int type_color_box = this.list_items_tray[0].get_type_color();

            for (int i = 0; i < this.list_items_tray.Count; i++)
            {
                if (type_box != this.list_items_tray[i].get_type_box() || type_color_box != this.list_items_tray[i].get_type_color()) is_true = false;
            }

            if (is_true)
            {
                Debug.Log("Is True");
                for (int i = 0; i < this.list_items_tray.Count; i++)
                {
                    this.game.CreateEffect(this.list_items_tray[i].transform.position, 0, 0.3f);
                    Destroy(this.list_items_tray[i].gameObject);

                }
                this.sliderTimer.value += 0.2f;
                this.game.play_sound(0);

                int scores_add = (type_color_box + 1);
                this.add_scores(scores_add);
                this.create_box_item_in_tray(type_color_box);
                this.txt_msg_scores.text = "x" + scores_add;
                this.game.anim.Play("game_add_scores");
            }
            else
            {
                for (int i = 0; i < this.list_items_tray.Count; i++)
                {
                    this.list_items_tray[i].give_up(this.area_body_all_item);
                }
                Debug.Log("Is false");
                this.sliderTimer.value -= 0.2f;
                this.game.play_sound(1);
            }
            this.list_items_tray = new List<box_items>();
            this.panel_loading.SetActive(true);
        }
    }

    private void add_scores(int scores_add)
    {
        this.scores += scores_add;
        this.check_scores();
    }

    private void check_scores()
    {
        this.txt_scores.text = "Scores:" + this.scores;
    }

    private void create_box_item_in_tray(int type_color_box)
    {
        int index_rand = Random.Range(0, this.list_sp_icon.Count);
        GameObject item_obj = Instantiate(this.prefab_box_items);
        item_obj.transform.SetParent(this.tr_check[1]);
        item_obj.transform.localPosition = new Vector3(0, 0, 0);
        item_obj.transform.localScale = new Vector3(1f, 1f, 1f);
        item_obj.GetComponent<box_items>().set_type(box_status_type.in_tray);
        item_obj.GetComponent<box_items>().set_data(this.list_sp_icon[index_rand], index_rand);

        int index_color_next = type_color_box + 1;
        Color32 color_next = this.color_bk_box[index_color_next];
        item_obj.GetComponent<box_items>().set_color_bk(color_next);
        item_obj.GetComponent<box_items>().level_Up();
        item_obj.GetComponent<box_items>().on_move();
        this.game.ads.show_ads_Interstitial();
    }

    public void create_box_item_missing_for_body()
    {
        int count_item_box_live = this.area_body_all_item.childCount;
        int count_item_box_missing = this.max_box_item - count_item_box_live;
        if (count_item_box_missing > 0)
        {
            this.create_box_item_in_body();
            this.create_box_item_in_body();
        }
        this.panel_loading.SetActive(false);
    }

    private void create_box_item_in_body()
    {
        int index_rand = Random.Range(0, this.list_sp_icon.Count);
        GameObject item_obj = Instantiate(this.prefab_box_items);
        item_obj.transform.SetParent(this.area_body_all_item);
        item_obj.transform.localPosition = new Vector3(0, 0, 0);
        item_obj.transform.localScale = new Vector3(1f, 1f, 1f);
        item_obj.GetComponent<box_items>().set_type(box_status_type.in_body);
        item_obj.GetComponent<box_items>().set_data(this.list_sp_icon[index_rand], index_rand);
        item_obj.GetComponent<box_items>().set_color_bk(Color.white);
        this.obj_Item_Cur = item_obj;
        this.game.carrot.delay_function(0.2f, this.create_effect_creater_box_item);
    }

    private void create_effect_creater_box_item()
    {
        this.game.CreateEffect(this.obj_Item_Cur.transform.position, 1);
    }

    public void return_box_for_body(Transform tr_child)
    {
        tr_child.SetParent(this.area_body_all_item);
        tr_child.SetSiblingIndex(this.max_box_item / 2);
    }

    void Update()
    {
        if (this.IsPlay)
        {
            this.TimerSpeed += 0.1f;
            if (this.TimerSpeed > 3f)
            {
                this.sliderTimer.value -= 0.01f;
                this.TimerSpeed = 0f;
                if (sliderTimer.value > 0.5f)
                    imgBkStatus.color = Color.black;
                else
                    imgBkStatus.color = Color.red;
            }

            if (this.sliderTimer.value <= 0)
            {
                this.game.play_sound(2);
                this.game.carrot.play_vibrate();
                this.sliderTimer.value = 0;
                this.IsPlay = false;
                this.PanelGameOver.SetActive(true);
                if (this.scores > 0)
                {
                    IDictionary dataScore = Json.Deserialize("{}") as IDictionary;
                    dataScore["date"] = System.DateTime.Now.ToString();
                    dataScore["value"] = this.scores.ToString();
                    this.game.history.Add(dataScore);
                }
                this.game.ads.show_ads_Interstitial();
            }
        }
    }

    public void BtnPlayAgain()
    {
        this.game.ads.show_ads_Interstitial();
        this.game.carrot.play_sound_click();
        this.PanelGameOver.SetActive(false);
        this.CreateTable();
    }

    public void BtnContinue()
    {
        this.game.carrot.play_sound_click();
        this.IsPlay = true;
        this.PanelGamePause.SetActive(false);
        this.game.ads.show_ads_Interstitial();
    }

    public void BtnPause()
    {
        this.game.carrot.play_sound_click();
        this.IsPlay = false;
        this.PanelGamePause.SetActive(true);
    }

    public void BtnClear()
    {
        this.game.carrot.play_sound_click();
        for (int i = 0; i < this.list_items_tray.Count; i++)
        {
            Destroy(this.list_items_tray[i].gameObject);
        }
        this.list_items_tray = new List<box_items>();
        this.objBtnClear.SetActive(false);
    }
}

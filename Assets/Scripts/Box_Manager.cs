using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Carrot;
using UnityEngine;
using UnityEngine.UI;

public class Box_Manager : MonoBehaviour
{
    public Games game;
    public GameObject panel_loading;
    public GameObject PanelGameOver;
    public GameObject PanelGamePause;
    public GameObject BoxItemEffectPrefab;
    public int MaxBoxItem = 60;

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
    private List<box_items_effect> list_items_tray = new();
    private List<box_items> listBox = new();
    private List<GameObject> listFindHelp = new();
    private List<Sprite> list_sp_icon;

    [Header("Msg Scores")]
    public Text txt_msg_scores;
    private float TimerSpeed = 1.2f;
    private float TimerHelp = 0f;
    public bool IsPlay = true;
    private bool IsCheck = false;

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

        this.ClearTray();
        this.GetComponent<Games>().carrot.clear_contain(this.area_body_all_item);

        for (int i = 0; i < MaxBoxItem; i++)
        {
            int index_rand = UnityEngine.Random.Range(0, this.list_sp_icon.Count);
            GameObject item_obj = Instantiate(this.prefab_box_items);
            item_obj.transform.SetParent(this.area_body_all_item);
            item_obj.transform.localPosition = new Vector3(0, 0, 0);
            item_obj.transform.localScale = new Vector3(1f, 1f, 1f);
            box_items boxItem = item_obj.GetComponent<box_items>();
            var b = boxItem;
            boxItem.set_data(this.list_sp_icon[index_rand], index_rand);
            boxItem.index = i;
            boxItem.set_color_bk(Color.white);
            boxItem.SetActClick(() =>
            {
                this.ClearListHelp();
                game.CreateEffect(boxItem.transform.position, 2, 0.2f, 0.5f);
                game.carrot.play_sound_click();
                int indexTrTaget = this.Get_index_tr_tray_null();
                if (indexTrTaget != -1)
                {
                    b.CloseBox();
                    GameObject objBoxEffect = Instantiate(this.BoxItemEffectPrefab);
                    objBoxEffect.transform.SetParent(this.area_body_all_item);
                    objBoxEffect.transform.localPosition = boxItem.transform.localPosition;
                    objBoxEffect.transform.localScale = new Vector3(1f, 1f, 1f);
                    box_items_effect boxEffect = objBoxEffect.GetComponent<box_items_effect>();

                    boxEffect.indexTray = indexTrTaget;
                    boxEffect.OnLoad(this.tr_check[indexTrTaget].transform, b, boxEffect_status_type.in_body);
                    boxEffect.img_border.color = color_bk_box[b.type_color];
                    this.SetBoxToTray(boxEffect, indexTrTaget);
                }
            });
            listBox.Add(boxItem);
        }
        this.scores = 0;
        this.panel_loading.SetActive(false);
        this.check_scores();
    }

    private List<box_items> FindFirstTriple(List<box_items> boxes)
    {
        return boxes
            .Where(b => b.status == BoxStatusType.open)
            .GroupBy(b => new { b.type, b.type_color })
            .FirstOrDefault(g => g.Count() >= 3)?
            .Take(3)
            .ToList();
    }

    private void ClearListHelp()
    {
        if (listFindHelp.Count > 0)
        {
            for (int i = 0; i < listFindHelp.Count; i++)
            {
                Destroy(listFindHelp[i].gameObject);
            }
            listFindHelp.Clear();
        }
    }

    private int Get_index_tr_tray_null()
    {
        for (int i = 0; i < this.list_items_tray.Count; i++)
        {
            if (list_items_tray[i] == null) return i;
        }
        return -1;
    }

    private int CountTrayNotNull()
    {
        int count = 0;
        for (int i = 0; i < this.tr_check.Length; i++)
        {
            if (list_items_tray[i] != null) count++;
        }
        return count;
    }

    public void SetBoxToTray(box_items_effect box_item, int indexTray)
    {
        this.objBtnClear.SetActive(true);
        this.panel_loading.SetActive(false);
        this.list_items_tray[indexTray] = box_item;
    }

    public void CheckTray()
    {
        if (this.CountTrayNotNull() >= 3 && this.IsCheck == false)
        {
            this.objBtnClear.SetActive(false);
            this.IsCheck = true;
            game.carrot.delay_function(UnityEngine.Random.Range(1f, 1.5f), CheckAllBoxInTray);
        }
    }

    private void CheckAllBoxInTray()
    {
        bool is_true = true;
        int type_box = this.list_items_tray[0].boxItem.type;
        int type_color_box = this.list_items_tray[0].boxItem.type_color;

        for (int i = 0; i < this.list_items_tray.Count; i++)
        {
            if (type_box != this.list_items_tray[i].boxItem.type || type_color_box != this.list_items_tray[i].boxItem.type_color) is_true = false;
        }

        if (is_true)
        {
            TimerHelp = 0f;
            List<box_items> listBoxGet = new();
            Debug.Log("Is True");
            for (int i = 0; i < this.list_items_tray.Count; i++)
            {
                listBoxGet.Add(this.list_items_tray[i].boxItem);
                this.game.CreateEffect(this.list_items_tray[i].transform.position, UnityEngine.Random.Range(0, 1), 0.3f);
                Destroy(this.list_items_tray[i].gameObject);
            }

            this.sliderTimer.value += 0.2f;
            this.game.play_sound(0);

            int scores_add = (type_color_box + 1);
            this.add_scores(scores_add);

            int indexSelLevelUp = UnityEngine.Random.Range(0, listBoxGet.Count);
            for (int i = 0; i < listBoxGet.Count; i++)
            {
                int index_rand = UnityEngine.Random.Range(0, this.list_sp_icon.Count);
                listBoxGet[i].set_data(this.list_sp_icon[index_rand], index_rand);
                listBoxGet[i].ReOpen();
                if (indexSelLevelUp != i) listBoxGet[i].type_color = 0;
            }

            GameObject objBoxEffect = Instantiate(this.BoxItemEffectPrefab);
            objBoxEffect.transform.SetParent(this.list_items_tray[1].transform);
            objBoxEffect.transform.localPosition = Vector3.zero;
            objBoxEffect.transform.localScale = new Vector3(1f, 1f, 1f);
            box_items_effect boxEffect = objBoxEffect.GetComponent<box_items_effect>();
            int boxLevel = listBoxGet[indexSelLevelUp].level_Up();
            box_items b = listBoxGet[indexSelLevelUp];
            b.set_color_bk(this.color_bk_box[boxLevel]);
            boxEffect.OnLoad(b.gameObject.transform, b, boxEffect_status_type.create);
            boxEffect.img_border.color = color_bk_box[b.type_color];
            b.CloseBox();

            this.txt_msg_scores.text = "x" + scores_add;
            this.game.anim.Play("game_add_scores");
        }
        else
        {
            for (int i = 0; i < this.list_items_tray.Count; i++)
            {
                this.list_items_tray[i].give_up();
            }
            Debug.Log("Is false");
            this.sliderTimer.value -= 0.2f;
            this.game.play_sound(1);
        }
        this.list_items_tray = new List<box_items_effect>(3);
        this.list_items_tray.Add(null);
        this.list_items_tray.Add(null);
        this.list_items_tray.Add(null);
        this.IsCheck = false;
    }

    public void RemoveBoxToTray(int index)
    {
        this.list_items_tray.RemoveAt(index);
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

    void Update()
    {
        if (this.IsPlay && !this.IsCheck)
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

            if (listFindHelp.Count==0)
            {
                this.TimerHelp += 0.1f;
                if (this.TimerHelp > 30f)
                {
                    Debug.Log("Tim help");
                    List<box_items> listbox = this.FindFirstTriple(this.listBox);
                    if (listbox.Count >= 0)
                    {
                        for (int i = 0; i < listbox.Count; i++)
                        {
                            if (listbox[i] != null)
                            {
                                GameObject obj_effect = Instantiate(game.effect_prefab[4]);
                                obj_effect.transform.SetParent(listbox[i].gameObject.transform);
                                obj_effect.transform.localPosition = Vector3.zero;
                                obj_effect.transform.localScale = new Vector3(1f, 1f, 1f);
                                listFindHelp.Add(obj_effect);
                            }
                        }
                    }
                    this.TimerHelp = 0;
                }
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
        this.ClearTray();
    }

    private void ClearTray()
    {
        for (int i = 0; i < this.list_items_tray.Count; i++)
        {
            if (this.list_items_tray[i] != null)
            {
                this.list_items_tray[i].boxItem.ReOpen();
                Destroy(this.list_items_tray[i].gameObject);
            }
        }
        list_items_tray = new();
        list_items_tray.Add(null);
        list_items_tray.Add(null);
        list_items_tray.Add(null);
        objBtnClear.SetActive(false);
    }
}

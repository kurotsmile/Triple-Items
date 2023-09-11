using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Box_Manager : MonoBehaviour
{
    public Games game;
    public GameObject panel_loading;

    [Header("Icons")]
    public Sprite sp_icon_all_style;
    public Sprite sp_icon_buy;

    [Header("Obj Main")]
    public Transform area_body_all_item;
    public Transform area_body_all_scroll;
    public GameObject prefab_box_items;
    public Sprite[] sp_item_box;
    public Color32[] color_bk_box;
    public Text txt_scores;
    private int scores=0;

    [Header("Tray check")]
    public Transform[] tr_check;
    private List<box_items> list_items_tray;
    private List<Sprite> list_sp_icon;

    [Header("Msg Scores")]
    public Text txt_msg_scores;

    private Carrot.Carrot_Box box_list_style_icon = null;
    private int length_icon_in_categegory = 0;
    private int count_download_icon = 0;

    private int max_box_item = 82;
    private GameObject obj_Item_Cur = null;
    private string s_key_category_shop_temp = "";

    public void on_load()
    {
        this.list_sp_icon = new List<Sprite>();

        for (int i = 0; i < this.sp_item_box.Length; i++) this.list_sp_icon.Add(this.sp_item_box[i]);

        this.GetComponent<Games>().carrot.clear_contain(this.area_body_all_item);
        this.list_items_tray = new List<box_items>();

        for(int i = 0; i < max_box_item; i++) 
        {
            int index_rand = Random.Range(0, this.list_sp_icon.Count);
            GameObject item_obj = Instantiate(this.prefab_box_items);
            item_obj.transform.SetParent(this.area_body_all_item);
            item_obj.transform.localPosition = new Vector3(0, 0, 0);
            item_obj.transform.localScale = new Vector3(1f, 1f, 1f);
            item_obj.GetComponent<box_items>().set_type(box_status_type.in_body);
            item_obj.GetComponent<box_items>().set_data(this.list_sp_icon[index_rand],index_rand);
            item_obj.GetComponent<box_items>().set_color_bk(Color.white);
        }

        this.panel_loading.SetActive(false);
        this.check_scores();
    }


    #region List Category Icon
    public void change_style_box()
    {
        this.game.carrot.show_loading();
        Query iconQuery = this.game.carrot.db.Collection("icon_category");
        iconQuery.Limit(20);
        iconQuery.GetSnapshotAsync().ContinueWithOnMainThread(Task =>{
            QuerySnapshot IconQuerySnapshot = Task.Result;
            if (Task.IsCompleted)
            {
                this.game.carrot.hide_loading();
                this.box_list_style_icon = this.game.carrot.Create_Box();
                this.box_list_style_icon.set_icon(this.sp_icon_all_style);
                this.box_list_style_icon.set_title("Bundle of object styles");

                foreach (DocumentSnapshot documentSnapshot in IconQuerySnapshot.Documents)
                {
                    IDictionary icon_data=documentSnapshot.ToDictionary();
                    Carrot.Carrot_Box_Item item_cat= this.box_list_style_icon.create_item("item_icon");
                    item_cat.set_icon(this.sp_icon_all_style);
                    string s_key;
                    var key_cat = "";
                    if (icon_data["key"] != null)
                    {
                        s_key = icon_data["key"].ToString();
                        key_cat = s_key;
                        item_cat.set_title(icon_data["key"].ToString());
                        item_cat.set_tip(icon_data["key"].ToString());
                        
                        if (PlayerPrefs.GetString("data_cat_" + key_cat) != "")
                        {
                            Carrot.Carrot_Box_Btn_Item btn_data=item_cat.create_item();
                            btn_data.set_icon(this.game.carrot.icon_carrot_database);
                            btn_data.set_color(this.game.carrot.color_highlight);
                        }
                    }

                    if (icon_data["buy"] != null)
                    {
                        if (icon_data["buy"].ToString() == "buy")
                        {
                            Carrot.Carrot_Box_Btn_Item btn_buy = item_cat.create_item();
                            btn_buy.set_icon(this.sp_icon_buy);
                            btn_buy.set_color(this.game.carrot.color_highlight);
                            item_cat.set_act(() => this.buy_category_icon(key_cat));
                        }
                        else
                        {
                            item_cat.set_act(() => this.download_icon_by_category(key_cat));
                        }
                    }
                    else
                    {
                        item_cat.set_act(() => this.download_icon_by_category(key_cat));
                    }
                };

                this.box_list_style_icon.update_color_table_row();
            }
        });
    }

    private void buy_category_icon(string s_key)
    {
        this.s_key_category_shop_temp = s_key;
        this.game.carrot.shop.buy_product(2);
    }

    public void pay_success_category_icon(string s_id)
    {
        if (s_id == this.game.carrot.shop.get_id_by_index(2))
        {
            this.download_icon_by_category(this.s_key_category_shop_temp);
            this.s_key_category_shop_temp = "";
        }
    }

    private void download_icon_by_category(string s_key_category)
    {
        this.game.carrot.show_loading();
        Query iconQuery = this.game.carrot.db.Collection("icon");
        iconQuery = iconQuery.WhereEqualTo("category", s_key_category);
        iconQuery.Limit(20);
        iconQuery.GetSnapshotAsync().ContinueWithOnMainThread(Task => {
            QuerySnapshot IconQuerySnapshot = Task.Result;
            if (Task.IsCompleted)
            {
                IList list_cat_data = (IList)IconQuerySnapshot.Documents;
                this.length_icon_in_categegory = list_cat_data.Count;
                PlayerPrefs.SetString("data_cat_" + s_key_category, Carrot.Json.Serialize(list_cat_data));
                this.list_sp_icon = new List<Sprite>();
                this.game.carrot.hide_loading();
                if (this.box_list_style_icon != null) this.box_list_style_icon.close();
                foreach (DocumentSnapshot documentSnapshot in IconQuerySnapshot.Documents)
                {
                    IDictionary data_icon = (IDictionary)documentSnapshot.ToDictionary();
                    data_icon["id"] = documentSnapshot.Id;
                    string id_icon = documentSnapshot.Id;

                    Sprite sp_icon = this.game.carrot.get_tool().get_sprite_to_playerPrefs(id_icon);
                    if (sp_icon != null)
                    {
                        this.list_sp_icon.Add(sp_icon);
                        this.count_download_icon++;
                        this.check_done_download_list_icon();
                    }
                    else
                    {
                        string url_icon = data_icon["icon"].ToString();
                        if (url_icon != "") this.game.carrot.get_img_and_save_playerPrefs(url_icon, null, id_icon, download_success_icon_item);
                    }
                };
            }

            if (Task.IsFaulted)
            {
                this.game.carrot.hide_loading();
            }
        });
    }

    private void download_success_icon_item(Texture2D texture)
    {
        Sprite sp_icon=this.game.carrot.get_tool().Texture2DtoSprite(texture);
        this.list_sp_icon.Add(sp_icon);
        this.count_download_icon++;
        this.check_done_download_list_icon();
    }

    private void check_done_download_list_icon()
    {
        if (this.count_download_icon >= this.length_icon_in_categegory)
        {
            this.change_icon_for_list_box();
        } 
    }
    #endregion

    public Transform get_tr_tray_none() 
    {
        for(int i = 0; i < this.tr_check.Length; i++)
        {
            if (this.tr_check[i].childCount==0) return this.tr_check[i].transform;
        }
        return null;
    }

    public void add_box_to_tray(box_items box_item)
    {
        this.panel_loading.SetActive(false);
        this.list_items_tray.Add(box_item);
        if (this.list_items_tray.Count>=3)
        {
            bool is_true = true;
            int type_box = this.list_items_tray[0].get_type_box();
            int type_color_box = this.list_items_tray[0].get_type_color();  

            for(int i = 0; i < this.list_items_tray.Count; i++)
            {
                if (type_box != this.list_items_tray[i].get_type_box()||type_color_box != this.list_items_tray[i].get_type_color()) is_true = false;
            }

            if (is_true)
            {
                Debug.Log("Is True");
                for (int i = 0; i < this.list_items_tray.Count; i++)
                {
                    this.game.create_effect(this.list_items_tray[i].transform.position);
                    Destroy(this.list_items_tray[i].gameObject); 
                }
                this.game.play_sound(0);

                int scores_add= (type_color_box + 1);
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
                this.game.play_sound(1);
            };
            this.list_items_tray = new List<box_items>();
            this.panel_loading.SetActive(true);
        }
    }

    private void add_scores(int scores_add)
    {
        this.scores+=scores_add;
        this.check_scores();
        this.game.update_score_to_server(this.scores);
    }

    private void check_scores()
    {
        this.txt_scores.text = "Scores:" + this.scores;
    }

    private void change_icon_for_list_box()
    {
        foreach(Transform child in this.area_body_all_item)
        {
            int rand_index = Random.Range(0, this.list_sp_icon.Count);
            child.GetComponent<box_items>().set_data(this.list_sp_icon[rand_index],rand_index);
        }
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
        Color32 color_next= this.color_bk_box[index_color_next];
        item_obj.GetComponent<box_items>().set_color_bk(color_next);
        item_obj.GetComponent<box_items>().level_Up();
        item_obj.GetComponent<box_items>().on_move();
        this.game.carrot.ads.show_ads_Interstitial();
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
        this.game.create_effect(this.obj_Item_Cur.transform.position, 1);
    }

    public void return_box_for_body(Transform tr_child)
    {
        tr_child.SetParent(this.area_body_all_item);
        tr_child.SetSiblingIndex(this.max_box_item / 2);
    }
}

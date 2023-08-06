using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Box_Manager : MonoBehaviour
{
    public Games game;

    [Header("Icons")]
    public Sprite sp_icon_all_style;

    [Header("Obj Main")]
    public Transform area_body;
    public GameObject prefab_box_items;
    public Sprite[] sp_item_box;
    public Text txt_scores;
    private int scores=0;

    [Header("Tray check")]
    private int index_tray_check = 0;
    public Transform[] tr_check;
    private List<box_items> list_items_tray;
    private List<Sprite> list_sp_icon;

    private Carrot.Carrot_Box box_list_style_icon = null;
    private int length_icon_in_categegory = 0;
    private int count_download_icon = 0;

    public void on_load()
    {
        this.GetComponent<Games>().carrot.clear_contain(this.area_body);
        this.list_items_tray = new List<box_items>();

        for(int i = 0; i < 36; i++)
        {
            int index_rand = Random.Range(0, this.sp_item_box.Length);
            GameObject item_obj = Instantiate(this.prefab_box_items);
            item_obj.transform.SetParent(this.area_body);
            item_obj.transform.localPosition = new Vector3(0, 0, 0);
            item_obj.transform.localScale = new Vector3(1f, 1f, 1f);
            item_obj.GetComponent<box_items>().set_data(this.sp_item_box[index_rand],index_rand);
        }

        this.check_scores();
    }

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
                    if (icon_data["key"] != null)
                    {
                        var key_cat = icon_data["key"].ToString();
                        item_cat.set_title(icon_data["key"].ToString());
                        item_cat.set_tip(icon_data["key"].ToString());
                        item_cat.set_act(()=>this.download_icon_by_category(key_cat));
                        if (PlayerPrefs.GetString("data_cat_" + key_cat) != "")
                        {
                            Carrot.Carrot_Box_Btn_Item btn_data=item_cat.create_item();
                            btn_data.set_icon(this.game.carrot.icon_carrot_database);
                            btn_data.set_color(this.game.carrot.color_highlight);
                        }
                    }
                };
            }
        });
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

    public Transform get_tr_tray_cur()
    {
        return this.tr_check[this.index_tray_check].transform;
    }

    public void add_box_to_tray(box_items box_item)
    {
        this.list_items_tray.Add(box_item);
        this.index_tray_check++;
        if (this.index_tray_check >= 3)
        {
            bool is_true = true;
            int type_box = this.list_items_tray[0].get_type_box();

            for(int i = 0; i < this.list_items_tray.Count; i++)
            {
                if (type_box != this.list_items_tray[i].get_type_box()) is_true = false;
            }

            if (is_true)
            {
                Debug.Log("Is True");
                for (int i = 0; i < this.list_items_tray.Count; i++)
                {
                    this.game.create_effect_explosie(this.list_items_tray[i].transform.position);
                    Destroy(this.list_items_tray[i].gameObject);
                }
                this.game.play_sound(0);
                this.add_scores();
            }
            else
            {
                for (int i = 0; i < this.list_items_tray.Count; i++)
                {
                    this.list_items_tray[i].give_up(this.area_body);
                }
                Debug.Log("Is false");
                this.game.play_sound(1);
            };
            this.list_items_tray = new List<box_items>();
            this.index_tray_check = 0;
        }
    }

    private void add_scores()
    {
        this.scores++;
        this.check_scores();
    }

    private void check_scores()
    {
        this.txt_scores.text = "Scores:" + this.scores;
    }

    private void change_icon_for_list_box()
    {
        foreach(Transform child in this.area_body)
        {
            int rand_index = Random.Range(0, this.list_sp_icon.Count);
            child.GetComponent<box_items>().set_data(this.list_sp_icon[rand_index],rand_index);
        }
    }

}

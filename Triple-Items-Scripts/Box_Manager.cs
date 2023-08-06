using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_Manager : MonoBehaviour
{
    public Carrot.Carrot carrot;

    [Header("Icons")]
    public Sprite sp_icon_all_style;

    [Header("Obj Main")]
    public Transform area_body;
    public GameObject prefab_box_items;
    public Sprite[] sp_item_box;

    [Header("Tray check")]
    private int index_tray_check = 0;
    public Transform[] tr_check;
    private List<box_items> list_items_tray;

    public void on_load()
    {
        this.GetComponent<Games>().carrot.clear_contain(this.area_body);
        this.list_items_tray = new List<box_items>();

        for(int i = 0; i < 30; i++)
        {
            int index_rand = Random.Range(0, this.sp_item_box.Length);
            GameObject item_obj = Instantiate(this.prefab_box_items);
            item_obj.transform.SetParent(this.area_body);
            item_obj.transform.localPosition = new Vector3(0, 0, 0);
            item_obj.transform.localScale = new Vector3(1f, 1f, 1f);
            item_obj.GetComponent<box_items>().set_data(this.sp_item_box[index_rand],index_rand);
        }
    }

    public void change_style_box()
    {
        this.carrot.show_loading();
        Query iconQuery = this.carrot.db.Collection("icon_category");
        iconQuery.Limit(20);
        iconQuery.GetSnapshotAsync().ContinueWithOnMainThread(Task =>{
            QuerySnapshot IconQuerySnapshot = Task.Result;
            if (Task.IsCompleted)
            {
                carrot.hide_loading();
                Carrot.Carrot_Box box_list_style = this.carrot.Create_Box();
                box_list_style.set_icon(this.sp_icon_all_style);
                box_list_style.set_title("Bundle of object styles");

                foreach (DocumentSnapshot documentSnapshot in IconQuerySnapshot.Documents)
                {
                    IDictionary icon_data=documentSnapshot.ToDictionary();
                    Carrot.Carrot_Box_Item item_cat=box_list_style.create_item("item_icon");
                    item_cat.set_icon(this.sp_icon_all_style);
                    if (icon_data["key"] != null)
                    {
                        item_cat.set_title(icon_data["key"].ToString());
                        item_cat.set_tip(icon_data["key"].ToString());
                    }
                };
            }
        });
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
                Debug.Log("Is True");
            else
                Debug.Log("Is False");

            this.act_give_up_all_item();

            this.list_items_tray = new List<box_items>();
            this.index_tray_check = 0;
        }
    }

    private void act_give_up_all_item()
    {
        for (int i = 0; i < this.list_items_tray.Count; i++)
        {
            this.list_items_tray[i].give_up(this.area_body);
        }
    }
}

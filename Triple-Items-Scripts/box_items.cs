using Carrot;
using UnityEngine;
using UnityEngine.UI;

public enum box_status_type
{
    in_tray,in_body
}

public class box_items : MonoBehaviour
{
    public Image img_icon;
    public Image img_border;

    private bool is_move = false;
    private float speed_move = 5.2f;
    private Transform tr_target;
    private int type = -1;
    private int type_color = 0;
    private bool is_done_check = false;
    private box_status_type status_type;

    public void click()
    {
        GameObject.Find("Games").GetComponent<Games>().carrot.play_sound_click();
        this.transform.SetParent(this.transform.root);
        this.tr_target = GameObject.Find("Games").GetComponent<Games>().boxs.get_tr_tray_cur();
        this.is_move = true;
    }

    public void set_data(Sprite sp_icon,int type) 
    {
        this.img_icon.sprite = sp_icon;
        this.type = type;
    }

    public void on_move()
    {
        this.is_move = true;
    }

    public void set_type(box_status_type box_status_type)
    {
        this.status_type = box_status_type;
        if (this.status_type == box_status_type.in_body)
            this.tr_target = GameObject.Find("Games").GetComponent<Games>().boxs.get_tr_tray_cur();
        else
            this.tr_target = GameObject.Find("Games").GetComponent<Games>().boxs.area_body;
    }

    private void Update()
    {
        if (this.is_move)
        {
            transform.position = Vector3.MoveTowards(this.transform.position, tr_target.position, this.speed_move * Time.deltaTime);

            if (transform.position == tr_target.position)
            {
                this.is_move = false;
                if (this.status_type == box_status_type.in_body)
                {
                    if (this.is_done_check)
                    {
                        this.transform.SetParent(GameObject.Find("Games").GetComponent<Games>().boxs.area_body);
                        this.is_done_check = false;
                    }
                    else
                    {
                        this.is_done_check = true;
                        this.transform.SetParent(GameObject.Find("Games").GetComponent<Games>().boxs.get_tr_tray_cur());
                        GameObject.Find("Games").GetComponent<Games>().boxs.add_box_to_tray(this);
                    }
                }
                else
                {
                    this.transform.SetParent(GameObject.Find("Games").GetComponent<Games>().boxs.area_body);
                    this.is_done_check = false;
                    this.status_type = box_status_type.in_body;
                    GameObject.Find("Games").GetComponent<Games>().boxs.create_box_item_missing_for_body();
                }

            }
        }
    }

    public void give_up(Transform tr_return)
    {
        this.tr_target = tr_return;
        this.is_move = true;
    }

    public int get_type_box()
    {
        return this.type;
    }

    public void set_color_bk(Color32 color_set)
    {
        this.img_border.color = color_set;
    }

    public int get_type_color()
    {
        return this.type_color;
    }

    public void level_Up()
    {
        this.type_color++;
    }
}

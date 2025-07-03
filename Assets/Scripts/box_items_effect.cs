using UnityEngine;
using UnityEngine.UI;


public enum boxEffect_status_type
{
    in_tray, in_body,give_up,create
}

public class box_items_effect : MonoBehaviour
{
    public Image img_icon;
    public Image img_border;
    public box_items boxItem;
    public int indexTray = -1;
    public boxEffect_status_type status_type = boxEffect_status_type.in_body;
    private bool is_move = false;
    private float speed_move = 5.2f;
    private Transform tr_target;
    
    public void OnLoad(Transform trTray, box_items BoxItem, boxEffect_status_type status)
    {
        this.tr_target = trTray;
        this.transform.SetParent(this.transform.root);
        this.is_move = true;
        this.img_icon.sprite = BoxItem.img_icon.sprite;
        this.img_border.color = BoxItem.img_border.color;
        this.boxItem = BoxItem;
        this.status_type = status;
    }

    private void Update()
    {
        if (this.is_move)
        {
            if (tr_target == null) return;
            transform.position = Vector3.MoveTowards(this.transform.position, tr_target.position, this.speed_move * Time.deltaTime);

            if (transform.position == tr_target.position)
            {
                this.is_move = false;
                if (this.status_type == boxEffect_status_type.in_body)
                {
                    this.transform.SetParent(this.tr_target);
                    this.status_type = boxEffect_status_type.in_tray;
                    GameObject.Find("Games").GetComponent<Games>().boxs.CheckTray();
                }
                else if (this.status_type == boxEffect_status_type.in_tray)
                {
                    this.boxItem.ReOpen();
                    Destroy(this.gameObject);
                }
                else
                {
                    this.boxItem.ReOpen();
                    Destroy(this.gameObject);
                }
            }
        }
    }

    public void give_up()
    {
        this.tr_target = this.boxItem.transform;
        this.is_move = true;
        this.status_type = boxEffect_status_type.give_up;
    }

    public void BtnClick()
    {
        if (this.status_type == boxEffect_status_type.in_tray)
        {
            this.give_up();
            GameObject.Find("Games").GetComponent<Games>().boxs.RemoveBoxToTray(this.indexTray);
        }
    }

}

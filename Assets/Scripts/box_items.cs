using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum BoxStatusType{open,close,none}
public class box_items : MonoBehaviour
{
    public Image img_icon;
    public Image img_border;
    public Image img_none;
    public BoxStatusType status = BoxStatusType.open;
    public int index;
    public int type = 0;
    public int type_color = 0;
    private UnityAction act;

    public void SetActClick(UnityAction act)
    {
        this.act = act;
    }

    public void click()
    {
        if (this.status == BoxStatusType.open) this.act?.Invoke();
    }

    public void set_data(Sprite sp_icon, int type)
    {
        this.img_none.gameObject.SetActive(false);
        this.img_icon.gameObject.SetActive(true);
        this.img_icon.sprite = sp_icon;
        this.type = type;
    }
    
    public void set_color_bk(Color32 color_set)
    {
        this.img_border.color = color_set;
    }

    public int level_Up()
    {
        return this.type_color++;
    }

    public void ReOpen()
    {
        this.status = BoxStatusType.open;
        this.img_icon.gameObject.SetActive(true);
        this.img_none.gameObject.SetActive(false);
        this.img_border.color = GameObject.Find("Games").GetComponent<Games>().boxs.color_bk_box[this.type_color];
    }

    public void CloseBox()
    {
        this.status = BoxStatusType.close;
        this.img_icon.gameObject.SetActive(false);
        this.img_none.gameObject.SetActive(true);
        this.img_border.color = Color.white;
    }
}

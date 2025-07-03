using System.Collections;
using System.Collections.Generic;
using Carrot;
using UnityEngine;
using UnityEngine.UI;

public class History : MonoBehaviour
{
    public Games game;
    public Color32 colorTopPlayer;
    private int Length = 0;
    Carrot_Box box = null;
    public void OnLoad()
    {
        this.Length = PlayerPrefs.GetInt("h_lenght", 0);
    }

    public void Add(IDictionary data)
    {
        PlayerPrefs.SetString("h_" + this.Length, Json.Serialize(data));
        this.Length++;
        PlayerPrefs.SetInt("h_lenght", this.Length);
    }

    private void Clear()
    {
        this.Length = 0;
        PlayerPrefs.SetInt("h_lenght", this.Length);
        if (box != null) box.close();
    }

    public void ShowList()
    {
        List<IDictionary> listHistory = new();
        IDictionary dataTopPlayer = null;
        int socerMax = 0;
        for (int i = 0; i < this.Length; i++)
        {
            string sHistoryData = PlayerPrefs.GetString("h_" + i);
            if (sHistoryData != "")
            {
                IDictionary dataS = Json.Deserialize(sHistoryData) as IDictionary;
                if (int.Parse(dataS["value"].ToString()) > socerMax)
                {
                    socerMax = int.Parse(dataS["value"].ToString());
                    dataTopPlayer = dataS;
                }

                listHistory.Add(dataS);
            }
        }

        if (listHistory.Count > 0)
        {
            game.boxs.IsPlay = false;
            this.box = game.carrot.Create_Box();
            box.set_icon(game.spIconHistory);
            box.set_title("History");

            box.create_btn_menu_header(this.game.spIconClear).set_act(() =>
            {
                game.carrot.play_sound_click();
                game.carrot.play_vibrate();
                this.Clear();
            });

            if (dataTopPlayer != null)
            {
                Carrot_Box_Item ItemHistoryTop = box.create_item();
                ItemHistoryTop.set_icon(game.carrot.game.icon_top_player);
                ItemHistoryTop.set_title("Highest Score : " + dataTopPlayer["value"].ToString());
                ItemHistoryTop.set_tip(dataTopPlayer["date"].ToString());
                ItemHistoryTop.GetComponent<Image>().color = this.colorTopPlayer;
            }

            for (int i = listHistory.Count - 1; i >= 0; i--)
            {
                Carrot_Box_Item ItemHistory = box.create_item();
                ItemHistory.set_icon(game.spIconHistoryPlay);
                ItemHistory.set_title("Score : " + listHistory[i]["value"].ToString());
                ItemHistory.set_tip(listHistory[i]["date"].ToString());
            }

            box.set_act_before_closing(() =>
            {
                game.boxs.IsPlay = true;
            });
        }
        else
        {
            game.carrot.Show_msg("Game History", "None List");
        }

    }
}

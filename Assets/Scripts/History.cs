using System.Collections;
using Carrot;
using UnityEngine;

public class History : MonoBehaviour
{
    public Games game;
    private int Length = 0;
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

    public void ShowList()
    {
        Carrot_Box box = game.carrot.Create_Box();
        box.set_icon(game.spIconHistory);
        box.set_title("History");
    }
}

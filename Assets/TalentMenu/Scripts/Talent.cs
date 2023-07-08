using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Talent : MonoBehaviour, IComparable<Talent> {


    string description;                                                     //what the talent does
    int id;                                                                 //unique ID number for the talent
    int cost;                                                               //how many coins to purchase
    int effectId;                                                           //the int effect ID to tell the player
    private TextMeshProUGUI nameText, descriptionText, costText;            //reference to text children to write information
    private SpriteRenderer sr;                                              //sprite renderer object to change the sprite

    [SerializeField] GameObject purchasedMarkPreFab;                        //prefab to write a check mark over the object for purchased talents
    TextMeshProUGUI playerMoney;                                            //text reference to update player money


    private bool HasBeenPurchased { get; set; }

    public int CompareTo(Talent other) {
        return id.CompareTo(other.id);
    }

    //called when the object is clicked on, try to purchase the talent if it hasnt been purchased already
    private void OnMouseDown() {
        if (!HasBeenPurchased) {
            int money = int.Parse(playerMoney.text);
            if (money >= cost)
                Purchase(money);
        }

    }

    //set all variables including name,description,and cost text and init the sprite renderer
    public void Init(string name, string desc, int cost, int id, int effectId, bool hasBeenPurchased) {
        this.id = id;
        description = desc;
        this.cost = cost;
        this.effectId = effectId;

        playerMoney = GameObject.Find("CoinText").GetComponent<TextMeshProUGUI>();

        nameText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        descriptionText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        costText = transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        sr = GetComponent<SpriteRenderer>();

        nameText.SetText(name);
        descriptionText.SetText(description);
        costText.SetText(cost + "");
    }
    //set the displayed sprite to the passed in sprite
    public void SetSprite(Sprite sprite) {
        sr.sprite = sprite;
    }

    //purchase the talent, creates the check mark to show on the ui the talent has been purchased
    //removes money from the playermoney text on the store object (not working)
    //emoves money from the player and passes over the effectID and ID to apply the talent upgrade to the player
    public void Purchase(int playerCurrentMoney) {
        HasBeenPurchased = true;
        Instantiate(purchasedMarkPreFab, transform.position, Quaternion.identity).transform.parent = transform;
        playerMoney.text = playerCurrentMoney - cost + "";
        PlayerBehaviour playerBehaviour = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        playerBehaviour.RemoveCoins(cost);
        playerBehaviour.ApplyTalent(effectId, id);
    }
    
    public static bool operator <(Talent a, Talent b) {
        return a.CompareTo(b) < 0;
    }
    public static bool operator >(Talent a, Talent b) {
        return a.CompareTo(b) < 0;
    }
    

}

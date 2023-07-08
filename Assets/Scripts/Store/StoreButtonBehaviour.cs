using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StoreButtonBehaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI costTest;
    private int cost;
    private float size;
    private bool isForSale;
    GameObject weapon;
    // Start is called before the first frame update
    void Start()
    {
        isForSale = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Purchase() {

        if (isForSale) {
            GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>().EquipWeapon(weapon);    

        }
            
    }


    public void SetCost(int cost, GameObject weapon) {
        this.cost = cost;
        this.weapon = weapon;
        costTest.SetText(cost.ToString());
    }

}

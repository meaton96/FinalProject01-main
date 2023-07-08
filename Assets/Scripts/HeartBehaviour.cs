using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBehaviour : Item
{
    // Start is called before the first frame update
    public double HealAmount { get; set; }
    void Start()
    {
        preFab = GameObject.FindWithTag("GameControl").gameObject.GetComponent<GameController>().HeartItemPreFab;
        HealAmount = 1;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}

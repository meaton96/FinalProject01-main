using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehaviour : Item
{
    // Start is called before the first frame update
    public int Value { get; set; }
 
    
    void Start()
    {
        preFab = GameObject.FindWithTag("GameControl").gameObject.GetComponent<GameController>().CoinPreFab;
        Value = 1;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

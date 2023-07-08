    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SlimeBehaviour : EnemyBehaviour
{
    // Start is called before the first frame update
    protected override void Start() {
        //set movement speed attack range and aggro range then call the base class start method
        movementSpeed = .5f;
        attackRange = .2f;
        aggroRange = 1f;
        base.Start();
    }





    
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MagicianBehaviour : EnemyBehaviour
{
    [SerializeField] float projectileSpeed;                         //how fast the enemy's projectile travels
    private Animator magicianAnimator;                              //pointer to animation controller
    private const float AttackDelayTime = 1f;                       //1 second delay between attacks
    [SerializeField] private float AttackDelayCounter = 0;          //timer to track attack delay
    private int projectileDamage;                                   //how much damage the magician does with its attacks

    private const string DEATH_TRIGGER = "Death";
    private const string ATTACK_TRIGGER = "attack";
    private const string THROW_TRIGGER = "throw";
    private const string HURT_TRIGGER = "Hurt";
    private const string SPEED_TRIGGER = "Speed";


    protected override void Start() {
        movementSpeed = .6f;                                        //base movement speed is slowed a bit
        attackRange = 1.5f;                                         //how far the magician gets from the player before attacking
        aggroRange = 2f;                                            //unused
        projectileDamage = 2;                                       //damage is 1 full heart
        magicianAnimator = GetComponent<Animator>();                //get the animator object
        base.Start();
    }
    //attacks the player every 1AttackDelayTime seconds, plays the throwing animation which
    //calls the Shoot method part way through
    public override void AttackPlayer() {
        if (GetVectorToPlayer().magnitude > attackRange)
            state = State.Aggroed;
        if (AttackDelayCounter >= 0) {
            AttackDelayCounter -= Time.deltaTime;
        }
        else {
            magicianAnimator.SetTrigger(THROW_TRIGGER);
            AttackDelayCounter = AttackDelayTime;
        }
        
    }
    //fire projectile at the player
    public void Shoot() {
        //Instantiate the ball
        GameObject ball = Instantiate(GameObject.FindWithTag("GameControl")
            .GetComponent<GameController>().MagicianBallPreFab, transform.position, Quaternion.identity);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), ball.GetComponent<Collider2D>());     //ignore collisions with the enemy
        Projectile projScript = ball.GetComponent<Projectile>();                                    
        projScript.SetDirection(GetVectorToPlayer().normalized);
        projScript.SetSpeed(projectileSpeed);                //shoot towards the player and set speed
        
        projScript.Damage = projectileDamage;               //set damage
    }

}

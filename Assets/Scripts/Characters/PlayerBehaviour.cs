using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// remove this namespace bracketing


public class PlayerBehaviour : MonoBehaviour {
    [SerializeField] private bool facingRight;                          //if the player is facing right or left
    [SerializeField] private float rollSpeed;                           //the speed the player moves when rolling
    private float rollSpeedCounter;                                     //float to count the duration of the player roll
    [SerializeField] private float rollSpeedReduction;                  //how fast the roll speed decreases over time
    private Vector3 rollDir;                                            //vector diretion for the player roll
    private List<Item> backpack;                                        //list of items in the player's backpack

    public enum Damage { Half, Full }                                   //enum to represent a full heart damage or half heart damage event
    //string constants for player animations
    private const string END_GAME_SCENE = "End Game";
    private const string ANIM_ATTACK_TAG = "Attack";
    private const string ANIM_SHOOT_TAG = "RangedAttack";
    private const string ANIM_ROLL_TAG = "Roll";
    private const string ANIM_DEATH_TAG = "Death";
    private const string ANIM_CELEBRATE_TAG = "Celebrate";
    private const string ANIM_SPEED_TAG = "Speed";
    private const string BORDER_ROCK_TAG = "BorderRocks";

    public const int HEALTH_CEILING = 30;

    //enum representing if the player is rolling or not
    private enum State { Normal, Roll }
    private State state;

    public int health_current = 10;
    public int health_max = 10;
    public InterfaceScript interfaceScript;         //reference to the User Interface to update it
    public float speed;                             //player movement speed
    public float playerKnockbackOnEnemyCollision;   //player knockback amount when colliding with an enemy (unused)
    public int numArrowsFired;                      //number of arrows fired when shooting bow (unused)
    public int melee_damage;                        //amount of damage that the player does when using melee weapon (unused)
    public int ranged_damage;                       //amount of damage the player does when firing a ranged weapon (unused)
    public GameObject arrowGameObject;              //reference to arrow prefab
    private const float ROLL_DISTANCE = 1.5f;       //the base distance the player can roll
    private int numCoins { get; set; }              //numnber of coins the player has
    Rigidbody2D rb;
    Animator animator;
    private bool godMode = false;                   //cheat mode, invlunerable to damage
    public List<int> purchasedTalents = new();      //contains a list of all purchased talents by keeping track of their Ids

    public static PlayerBehaviour Instance;         //this

    //set rigid body and animator, init backpack and set state to not rolling
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        backpack = new();
        animator = GetComponent<Animator>();
        state = State.Normal;
        //numCoins = 5;

    }
    //preserve the player when swapping to a new scene
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() {
        //if in normal state then handle movement and animation
        //also check for player death
        switch (state) {
            case State.Normal:
                HandleMovement();
                Animate();
                if (health_current <= 0)
                    animator.SetTrigger(ANIM_DEATH_TAG);
                break;
            case State.Roll:        //if rolling just handle the roll
                HandleRoll();
                break;
        }

    }
    //handle player movement by accepting keyboard input
    public void HandleMovement() {
        float moveX = 0f;
        float moveY = 0f;
        if (Input.GetKey(KeyCode.W)) {
            moveY = 1f;
        }
        if (Input.GetKey(KeyCode.S)) {
            moveY = -1f;
        }
        if (Input.GetKey(KeyCode.A)) {
            moveX = -1f;
        }
        if (Input.GetKey(KeyCode.D)) {
            moveX = 1f;
        }
        if (Input.GetKeyDown(KeyCode.G))
            godMode = !godMode;
        if (Input.GetKeyDown(KeyCode.Escape)) {                                             //pause the game
            GameObject.FindWithTag("GameControl").GetComponent<GameController>().Pause();
        }

        //set movement vector to the speed * the direction
        Vector2 movement = speed * Time.deltaTime * new Vector2(moveX, moveY);

        //flip the sprite when changing directions
        if (moveX < 0 && facingRight)
            FlipSprite();
        if (moveX > 0 && !facingRight)
            FlipSprite();

        //set position and velocity
        rb.position += movement;
        rb.velocity = movement;
        //tell the animator to play walking animation 
        if (movement.magnitude > 0)
            animator.SetFloat(ANIM_SPEED_TAG, 1);
        else
            animator.SetFloat(ANIM_SPEED_TAG, 0);
    }

    public int NumCoins() {  return numCoins; } 

    //start the player roll
    //set the state and set rollDir vector 
    private void StartRoll() {

        Vector2 rollDirTemp;
        if (rb.velocity.magnitude == 0) {     //player isnt moving so roll the direction theyre facing
            rollDirTemp = Vector2.right;
        }
        else {
            if (facingRight)
                rollDirTemp = rb.velocity.normalized;
            else
                rollDirTemp = rb.velocity.normalized * -1;
        }
        if (rollDirTemp == null)
            return;
        if (CanRoll(rollDirTemp)) {
            state = State.Roll;
            rollDir = rollDirTemp;
            rollSpeedCounter = rollSpeed;   //reset the rollspeed counter to the roll speed
        }


    }
    //removes the coins from the player coins
    public void RemoveCoins(int numCoinsSpent) {
        numCoins -= numCoinsSpent;
    }
    //equips the passeed in weapon to the player (unused)
    public void EquipWeapon(GameObject weapon) {
        numCoins -= weapon.GetComponent<Weapon>().cost;
    }
    //check if the player can roll or not (unused)
    private bool CanRoll(Vector2 rollDirection) {
        if (!facingRight)
            rollDirection = rollDirection * -1;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rollDirection, ROLL_DISTANCE);

        return hit.collider == null || !hit.collider.gameObject.CompareTag(BORDER_ROCK_TAG);
    }
    //update player position and adjust the roll counter over time to tell when its time to stop the roll animation
    private void HandleRoll() {
        if (facingRight)
            transform.position += rollSpeed * Time.deltaTime * rollDir;
        else
            transform.position -= rollSpeed * Time.deltaTime * rollDir;
        rollSpeedCounter -= rollSpeedCounter * Time.deltaTime * rollSpeedReduction;

        //roll is over reset state to normal ending the roll
        if (rollSpeedCounter <= 1)
            state = State.Normal;
    }
    //set animation tags based on player input, rolling or attacking
    void Animate() {
        if (Input.GetMouseButtonDown(0)) {
            animator.SetTrigger(ANIM_ATTACK_TAG);
        }
        else if (Input.GetKeyDown(KeyCode.Space)) {
            animator.SetTrigger(ANIM_ROLL_TAG);
            StartRoll();
        }

    }
    
    //flips the sprite 180 degrees to face the other way
    void FlipSprite() {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    //kill the player, destroy the game object and load the end game scene for now
    public void PlayerDeath() {
        Destroy(gameObject);
        SceneManager.LoadScene(END_GAME_SCENE, LoadSceneMode.Single);
    }
    //damage the player health by an integer amount
    public void DamagePlayerHealth(int damageDone) {
        if (godMode) {  //player immunity
            return;
        }
        health_current -= damageDone;       
        
        if (health_current <= 0)
            PlayerDeath();
    }

    //handle the player colliding into things
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Item")) {                            //player collided with an item
            if (collision.gameObject.GetComponent<HeartBehaviour>() != null) {
                health_current++;
                //tell the heart object that it was picked up
                collision.gameObject.GetComponent<HeartBehaviour>().PickUp();
                //make sure that the health can't go above maximum
                if (health_current > health_max)
                    health_current = health_max;

            }
            else if (collision.gameObject.GetComponent<CoinBehaviour>() != null) {
                //the item was a coin so add to the coin tracker and update the interface
                numCoins += ((CoinBehaviour)collision.gameObject.GetComponent<CoinBehaviour>().PickUp()).Value;
                interfaceScript.UpdateCoinText(numCoins);
            }
            else
                //item was not coin or heart so pick it up and add it to the backpack (NYI)
                backpack.Add(collision.gameObject.GetComponent<Item>().PickUp());   //might be null??

        }
    }

    //apply a talent by effectID, store talent ID in the list so it cant be purchased again
    public void ApplyTalent(int effectId, int talentId) {
        switch(effectId) {
            case 000001:                                    //effect 1: +1 to maxmimum hearts
                health_max += 2;
                health_current = health_max;
                purchasedTalents.Add(talentId);
                break;
            case 000002:
                break;
        }
    }
}

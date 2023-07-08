using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public abstract class EnemyBehaviour : MonoBehaviour {
    private Rigidbody2D rb;
    private Animator animator;
    private GameObject player;
    private Stack<Item> drops;                                  //stack for item drops when enemy dies
    private int numItemsDropped;                                //number of items the enemy drops on death
    private float wanderDirectionChangeCounter;                 //counter to time duration of wander direction
    private const int DAMAGE_ON_COLLISION = 1;                  //amount of damage inflected on the player when running into them baseline
    protected float movementSpeed;                              //the base movement speed of the enemy
    protected float attackRange;                                //the range the enemy must get to before attacking the player
    protected float aggroRange;                                 //the range the enmy must get to before starting to move toward the player
    [SerializeField] protected float Health;                    

    private const float WANDER_SPEED_MULTI = .5f;               //wandering speed is 1/2 the normal walking speed
    private const float ITEM_SPAWN_RANGE = .2f;                 //distance for spawning items away from dying enemy
    private const int MAX_NUM_ITEMS_DROPPED = 6;                //max range for amount of items dropped
    private const int MAX_ENEMY_HEALTH = 4;                     //maximum value for player health to be randomized to exclusive

    private const float WANDER_TIME = 10f;                      //duration the enemy will wander for before swapping to searching
    private float wanderTimer = 0;                              
    public enum State {         //state for storing the current enemy actions
        Attacking,              //actively attempting to attack the player
        Aggroed,                //unused, replaced by A*
        Dormant,                //do nothing
        Wandering,              //move around in a random direction and change very X seconds
        Searching               //uses A* pathfinding to move towards the player 
    }
    protected State state;


    // Start is called before the first frame update
    protected virtual void Start() {
        Init();

    }
    public void Init() {
        Player = GameObject.FindWithTag("Player");
        GetComponent<AIDestinationSetter>().target = Player.transform;
        Health = Random.Range(1, MAX_ENEMY_HEALTH);
        numItemsDropped = Random.Range(1, MAX_NUM_ITEMS_DROPPED); //random number of drops 1-5 to be replaced by different number per enemy 
        wanderDirectionChangeCounter = 0;
        Rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        state = State.Wandering;                        //set default state
        GetComponent<AIPath>().canMove = false;
        MakeDrops();
     //   SetCollisionIgnores();
    }

    //create and populate the stack of drops
    //randomly assigns a coin or heart currently
    protected void MakeDrops() {
        drops = new Stack<Item>();
        //placeholder 
        for (int x = 0; x < numItemsDropped; x++) {
            if (Random.Range(0, 1f) > .5f) {
                drops.Push(gameObject.AddComponent<CoinBehaviour>());
            }
            else
                drops.Push(gameObject.AddComponent<HeartBehaviour>());
        }

    }

    protected Rigidbody2D Rb { get { return rb; } set { rb = value; } }
    protected Animator GetAnimator { get { return animator; } set { animator = value; } }
    protected GameObject Player { get { return player; } set { player = value; } }

    // Update is called once per frame
    protected virtual void Update() {
        if (rb != null)
            
        switch (state) {
            case State.Wandering:
                Wander();
                break;
            case State.Dormant:
                break;
            case State.Attacking:
                AttackPlayer();
                break;
            case State.Aggroed:
              //  MoveToPlayer();
                break;
            case State.Searching:
                Search();
                break;
        }
       //S Debug.Log(state.ToString()); 
    }
    private void MoveToPlayer() {
        //just sets the velocity maybe add an attack animation later
        SetVelocity(movementSpeed, GetVectorToPlayer());
        //play attack animation or wander animation if the player moved closer or father away
        if (GetVectorToPlayer().magnitude <= attackRange)
            state = State.Attacking;
        if (GetVectorToPlayer().magnitude >= aggroRange) {
            wanderDirectionChangeCounter = 0;
            state = State.Wandering;
        }

    }
    //change direction randomly every X seconds, swap to aggroed if in range
    private void Wander() {
        //check if the enemy has been wandering too long and begin searching for the player
        if (wanderTimer >= WANDER_TIME) {
            state = State.Searching;
            return;
        }
        else
            wanderTimer += Time.deltaTime;

        //change the wander direction every few seconds to a random direction
        if (wanderDirectionChangeCounter <= 1) {
            float angle = Random.Range(0, 2 * Mathf.PI);
            SetVelocity(movementSpeed * WANDER_SPEED_MULTI,
                new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));
            wanderDirectionChangeCounter = WANDER_TIME;
        }
        else
            wanderDirectionChangeCounter -= Time.deltaTime;
        
        //set state to aggro if enemy gets close enough to player
   //     if (GetVectorToPlayer().magnitude <= aggroRange)
       //     state = State.Aggroed;


    }
    /// <summary>
    /// uses A* pathfinding to move towards the player
    /// stopps using pathfinding once in range to begin attacking the player
    /// </summary>
    private void Search() {
        GetComponent<AIPath>().canMove = true;
        if (GetVectorToPlayer().magnitude <= attackRange) {
            GetComponent<AIPath>().canMove = false;
            state = State.Attacking;
        }

    }
    //implemented per enemy type
    public virtual void AttackPlayer() {}
    //sets velocity to move towards player at the passed in value of movement speed
    protected void SetVelocity(float movementSpeed, Vector2 direction) {
        if (rb != null) {
            rb.velocity = direction.normalized * movementSpeed;
            animator.SetFloat("Speed", Mathf.Abs(rb.velocity.magnitude));
        }
    }
    //returns a Vector2 that represents the vector from this enemy to the player
    protected Vector2 GetVectorToPlayer() {
        if (player != null && rb != null)
            return player.GetComponent<Rigidbody2D>().position - rb.position;
        else return Vector2.zero;
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            //collided with player
            player.GetComponent<PlayerBehaviour>().DamagePlayerHealth(DAMAGE_ON_COLLISION);
        }
        else if(collision.gameObject.CompareTag("Item")) {
            drops.Push(collision.gameObject.GetComponent<Item>().PickUp());
        }

    }
    //damage the enemy, dies if its health goes below 0
    public void Damage(float damage) {
        Health -= damage;
        if (Health <= 0)
            Die();

    }
    //destroys the game object killing the enemy
    //pops the stack of drops and spawns them 
    private void Die() {
        while (drops.TryPop(out Item i)) {
            float theta = Random.Range(0, 2 * Mathf.PI);
            Vector2 dropDir = new(transform.position.x + ITEM_SPAWN_RANGE * Mathf.Cos(theta),
                transform.position.y + ITEM_SPAWN_RANGE * Mathf.Sin(theta));
            i.Drop(transform.position, dropDir);
        }
        Destroy(gameObject);
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision) {
        Damage(collision.GetComponent<WeaponHitBoxScript>().damageValue);
    }

}

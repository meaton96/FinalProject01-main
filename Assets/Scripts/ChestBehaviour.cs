using Pathfinding.Ionic.Zip;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestBehaviour : MonoBehaviour {
    private Queue<Item> drops;
    [SerializeField] private Sprite openSprite, closeSprite;
    private bool hasBeenOpened = false;
    public enum State {
        closed, open
    }
    private State state;
    // Start is called before the first frame update

    private void OnTriggerEnter2D(Collider2D collision) {
        if (state == State.closed)
            Open();
        else
            Close();
    }
    //loads the closed image and set state
    private void Close() {
        state = State.closed;
        GetComponent<SpriteRenderer>().sprite = closeSprite;
    }
    //create chest by calling method to create chest drops
    public void Init(int numItems) {
        state = State.closed;
        drops = MakeDrops(numItems);
    }
    
    public bool CanSpawnEnemy() {
        return !hasBeenOpened;
    }
    //create and populate the stack of drops
    //randomly assigns a coin or heart currently
    protected Queue<Item> MakeDrops(int numItemsDropped) {
        Queue<Item> drops = new();
        //placeholder 
        for (int x = 0; x < numItemsDropped; x++) {
            if (Random.Range(0, 1f) > .5f) {
                drops.Enqueue(gameObject.AddComponent<CoinBehaviour>());
            }
            else
                drops.Enqueue(gameObject.AddComponent<HeartBehaviour>());
        }
        return drops;
    }
    public void Open() {
        hasBeenOpened = true;                                               //set flag
        state = State.open;
        GetComponent<SpriteRenderer>().sprite = openSprite;                 //set to open image
        while (drops.TryDequeue(out Item i)) {                              //take all of the items out of the chest and drop them
            float theta = Random.Range(0, 2 * Mathf.PI);                    //random circle around chest
            Vector2 dropDir = new(transform.position.x + 2 * Mathf.Cos(theta),  
                transform.position.y + 2 * Mathf.Sin(theta));
            i.Drop(transform.position, dropDir);                            
        }
    }
}

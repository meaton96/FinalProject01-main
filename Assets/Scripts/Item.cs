using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] protected GameObject preFab;       //item model
    [SerializeField] private float weight;

    //Instantiate the item at the given location to "drop" from an enemy or chest 
    public virtual Item Drop(Vector2 dropLocation, Vector2 dropDirection) {
        GameObject item = Instantiate(preFab, dropLocation, Quaternion.identity);
        item.GetComponent<Rigidbody2D>().AddForce(dropDirection - dropLocation);    //need to set collision to ignore chests
        gameObject.SetActive(true);
        return this;
    }
    //Player picks up the item so destroy it and return
    //just sets as deactive to fix issue with spawning new waves of enemies
    //but this is probably a memory leak and should be changed
    public virtual Item PickUp() {
        gameObject.SetActive(false);
        return this;
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int damage;
    // Start is called before the first frame update
    void Start()
    {
        SetCollisionIgnores();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > 20 || transform.position.x < -20 || transform.position.y > 20 || transform.position.y < -20)
            Destroy(gameObject);
            
    }
    private void FixedUpdate() {
        transform.Translate(direction.normalized * (speed * Time.deltaTime));   //move towards the player
    }
    //projecitle hit something
    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("Enemy"))
            Destroy(gameObject);
        if (collision.gameObject.CompareTag("Player")) {
            collision.gameObject.GetComponent<PlayerBehaviour>().DamagePlayerHealth(damage);
        }
    }
    public void SetDirection(Vector2 dir) { direction = dir; }
    public void SetSpeed(float speed) { this.speed = speed; }
    //tell the projectile to ignore all enemies and items
    //should replace with collision layer ignores
    public void SetCollisionIgnores() {
        GameObject[] itemObject = GameObject.FindGameObjectsWithTag("Item");
        GameObject[] enemyObject = GameObject.FindGameObjectsWithTag("Enemy");
        if (itemObject != null) {
            for (int x = 0; x < itemObject.Length; x++) {
                Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(),
                itemObject[x].GetComponentInChildren<Collider2D>());
            }
        }
        if (enemyObject != null) {
            for (int x = 0; x < enemyObject.Length; x++) {
                Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(),
                enemyObject[x].GetComponentInChildren<Collider2D>());
            }
        }

    }
    public int Damage { set { damage = value; } } 
}

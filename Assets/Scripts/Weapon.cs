using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    private string description;
    private int damage;
    private Type type;
    public int cost;
    [SerializeField] protected GameObject preFab;
    public enum Type {
        Sword,
        Dagger,
        Shield,
        Staff,
        Scythe,
        Spear,
        Axe,
        Hammer
    }
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
    public void Init(string name, int damage, Type type, int cost, GameObject prefab) {
        this.type = type;
        this.name = name;
        this.cost = cost;
        this.preFab = prefab;
        this.damage = damage;
    }
}

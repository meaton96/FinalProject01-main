using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHeartBehaviour : MonoBehaviour {
    //0 - missing
    //1 - empty
    //2 - half filled
    //3 - full
    [SerializeField] private Sprite[] heartSprites = new Sprite[4];
    private SpriteRenderer sr;

    public const int MISSING_INDEX = 0;
    public const int EMPTY_INDEX = 1;
    public const int HALF_INDEX = 2;
    public const int FULL_INDEX = 3;

    // Start is called before the first frame update
    void Start() {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {

    }
    public void SetActiveSprite(int sprite) { 
        if (sprite < 0 || sprite >= heartSprites.Length) {
            return;
        }
        Debug.Log("updating sprite to index " + sprite);
        sr.sprite = heartSprites[sprite];
    }
}

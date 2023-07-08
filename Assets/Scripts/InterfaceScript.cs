using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InterfaceScript : MonoBehaviour {

    public PlayerBehaviour player;                                              //player pointer
    [SerializeField] GameObject UIHeartPreFab;                                  //prefab for the ui heart object
    public float heartDistance;                                                 //distance between heart objects on UI
    public GameObject[] hearts;                                                 //array of currently drawn heart objects
    private int maxPlayerHealth;                                                //the maximum number of heart and current heart index
    public Vector2 heartStartLoc;                                               //place on the screen to begin drawing the hearts
    [SerializeField] private TextMeshProUGUI coinText;                          //coin text object to display number of coins
    [SerializeField] private TextMeshProUGUI enemyRemainingText;                //text to display number of enemies left
    [SerializeField] private TextMeshProUGUI spawnersReaminingText;             //text to display unopened chests in the level

    private int currentPlayerHealth;                                            //the UI's player health tracker to know when to update hearts

    private UIHeartBehaviour[] uiHearts;                                        //array to store heart prefabs

    // Start is called before the first frame update
    void Start() {
        InitHearts();
        currentPlayerHealth = player.health_current;
    }

    /// <summary>
    /// Instantiates heart prefabs to keep track of player hp
    /// </summary>
    void InitHearts() {
        uiHearts = new UIHeartBehaviour[PlayerBehaviour.HEALTH_CEILING / 2];            //set length to half player life maximum ever
        for (int x = 0; x < uiHearts.Length; x++) {
            uiHearts[x] = Instantiate(UIHeartPreFab, new Vector2(               
                heartStartLoc.x + x * heartDistance, heartStartLoc.y), 
                Quaternion.identity).GetComponent<UIHeartBehaviour>();         //creates heart instances and stores its script in the array
            uiHearts[x].gameObject.transform.parent = transform;               //set the parent to the ui object/camera
        }
    }
    /// <summary>
    /// updates the text displaying the amount of enemies left on the level
    /// </summary>
    /// <param name="numEnemies">number of enemies to display</param>
    public void UpdateEnemiesRemaining(int numEnemies) {
        enemyRemainingText.SetText("Remaining " + numEnemies);
    }
    /// <summary>
    /// update the players health when either the max health or current health dont match the values 
    /// in the player class vs this class
    /// </summary>
    private void Update() {
        if (currentPlayerHealth != player.health_current || maxPlayerHealth != player.health_max) {
            currentPlayerHealth = player.health_current;
            maxPlayerHealth = player.health_max;
            UpdateHearts();
        }
    }
    /// <summary>
    /// updates the heart array by changing their sprites between missing-full depending on player hp
    /// </summary>
    void UpdateHearts() {

        for (int x = 0; x < player.health_max / 2; x++) {                   
            if (x < currentPlayerHealth / 2) {                              
                uiHearts[x].SetActiveSprite(UIHeartBehaviour.FULL_INDEX);   //full heart for the total of the current player hp
            }
            else
                uiHearts[x].SetActiveSprite(UIHeartBehaviour.EMPTY_INDEX);  //otherwise put an empty heart 
        }
        if (currentPlayerHealth % 2 != 0) {
            uiHearts[currentPlayerHealth / 2].SetActiveSprite(UIHeartBehaviour.HALF_INDEX); //if the player has odd health value then need to make a half heart
        }
    }
    
    //update coin text by pulling amount of coins from player
    public void UpdateCoinText(int numCoins) {
        coinText.text = numCoins.ToString();
    }

    public void UpdateSpawnerText(int numSpawners) {
        spawnersReaminingText.text = "Spawners: " + numSpawners;
    }


}

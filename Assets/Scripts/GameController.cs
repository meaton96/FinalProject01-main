using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class GameController : MonoBehaviour {
    public GameObject CoinPreFab;                                               //coin object for drawing on UI
    public GameObject HeartItemPreFab;                                          //heart object for drawing on UI
    public GameObject MagicianBallPreFab;                                       //magician projectile 
    [SerializeField] GameObject[] enemyPreFabList = new GameObject[2];          //list of all enemy prefabs in order to randomly choose which type of enemy to spawn
    [SerializeField] GameObject[] grassObjects = new GameObject[5];             //grass objects for drawing background
    [SerializeField] GameObject[] largeRockObjects = new GameObject[2];         //large rocks for creating the border 
    [SerializeField] Vector2 backgroundStartingLocation;                        //vector location for where to start drawing the background
    [SerializeField] int backgroundCol, backgroundRow;                          //number of columns and rows to draw for the background
    private const float GRASS_SIZE = .76f;                                      //the size in pixels of each grass square
    [SerializeField] GameObject playerObject;                                   //the player
    private const float LARGE_ROCK_SIZE = 1.47f;                                //the size in pixels of the large rocks
    private Vector2 borderStartLocation;                                        //vector to start drawing the border rocks
    private int numBorderRocksX, numBorderRocksY;                               //number of border rocks for each direction
    private List<GameObject> currentEnemies;                                    //array of enemies currently spawned in the level
    [SerializeField] private GameObject[] chests = new GameObject[7];           //array holding all the chests on the current level
    [SerializeField] private float enemySpawnDistance = 1.5f;                   //distance away from chest to spawn enemies
    private float spawnTimer = 0;                                               //timer to track enemy spawns
    [SerializeField] private const float SPAWN_TIME = 10;                       //seconds of time between enemy spawns
    [SerializeField] private int CHEST_NUM_ITEMS_DROPPED = 5;                   //number of items dropped by the chests when theyre opened
    [SerializeField] private GameObject talentController, talentBackground;     //pointers to control talent menu
    public static GameController Instance;                                      //this
    private bool isPaused;                                                      //if the game is paused or not

    //ignore collisions, set the game to not paused, init enemy list, init chests and spawn the first of enemies
    void Start() {
        Physics2D.IgnoreLayerCollision(6, 7);       //ignore all collisions between enemies and dropped items
        isPaused = false;
        currentEnemies = new();
        InitChests();
        SpawnEnemies(); 
    }
    //set the game object to not be destroyed on switching scenes
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
        if (currentEnemies == null || (AllAreNull(currentEnemies) && NoSpawnersLeft())) {
            Debug.Log("Round over");
            //end level, load new enemies, new level or something
            //currently game just does nothing after all chests are opened
        }
        else {
            //spawn an enemy from each spawner every SPAWN_TIME seconds
            if (playerObject != null) {
                if (!isPaused) {
                    if (spawnTimer >= SPAWN_TIME) {
                        spawnTimer = 0;
                        SpawnEnemies();
                    }
                    else
                        spawnTimer += Time.deltaTime;

                    playerObject.GetComponent<PlayerBehaviour>().interfaceScript.
                        UpdateEnemiesRemaining(NumEnemiesLeft(currentEnemies));     //update the number of enemies if the player isnt dead
                    playerObject.GetComponent<PlayerBehaviour>().interfaceScript
                        .UpdateSpawnerText(GetNumSpawnersActive());                 //update the text for number of spawners active

                }
            }
        }
    }

    /// <summary>
    /// checks all the chests to see if they have been opened 
    /// </summary>
    /// <returns>true if all chests have been opened</returns>
    private bool NoSpawnersLeft() {
        for (int x = 0; x < chests.Length; x++) {
            if (chests[x].GetComponent<ChestBehaviour>().CanSpawnEnemy())
                return false;
        }
        return true;
    }
    /// <summary>
    /// initialize all the chests by telling them how many items to drop
    /// </summary>
    private void InitChests() {
        for (int x = 0; x < chests.Length; x++) {
            chests[x].GetComponent<ChestBehaviour>().Init(CHEST_NUM_ITEMS_DROPPED);
        }

    }
    /// <summary>
    /// Returns the number of spawners that can currently spawn enemies to update UI text
    /// </summary>
    /// <returns>the number of chests that can still spawn enemies</returns>
    public int GetNumSpawnersActive() {
        int count = 0;
        for (int x = 0; x < chests.Length; x++) {
            if (chests[x].GetComponent<ChestBehaviour>().CanSpawnEnemy()) {
                count++;
            }
        }
        return count;
    }
    /// <summary>
    /// spawns a wave of enemies around each of the active spawners
    /// </summary>
    private void SpawnEnemies() {
        for (int x = 0; x < chests.Length; x++) {
            if (chests[x].GetComponent<ChestBehaviour>().CanSpawnEnemy()) {
                float theta = Random.Range(0, 2 * Mathf.PI);                    //random angle around the spawner
                int index = Random.Range(0, enemyPreFabList.Length);            //random enemy chosen from the enemy prefab list
                Vector2 spawnLocation = new(
                    Mathf.Cos(theta) * enemySpawnDistance + chests[x].transform.position.x,
                    Mathf.Sin(theta) * enemySpawnDistance + chests[x].transform.position.y);        //spawn randomly around the chest
                currentEnemies.Add(Instantiate(enemyPreFabList[index], spawnLocation, Quaternion.identity)); //create and add to the enemy list
            }
        }

    }
    //returns true if each of the objects in the array are null
    private bool AllAreNull<T>(List<T> enemies) {
        for (int x = 0; x < enemies.Count; x++) {
            if (enemies[x] != null) {
                return false;
            }
        }
        return true;
    }
    //returns the amount of enemies left in the enemy array
    private int NumEnemiesLeft(List<GameObject> enemies) {
        int numEnemies = 0;
        for (int x = 0; x < enemies.Count; x++) {
            if (enemies[x] != null)
                numEnemies++;
        }
        return numEnemies;
    }
    public List<GameObject> GetEnemies() { return currentEnemies; }

    /// <summary>
    /// Spawns a number of entities on the level, currently unused 
    /// </summary>
    /// <param name="entityPrefabArray">the list of prefabs to chose from to spawn an entity</param>
    /// <param name="numberToSpawn">the number of entities to spawn</param>
    /// <returns>A list of all spawned game object entities</returns>
    private List<GameObject> SpawnEntities(GameObject[] entityPrefabArray, int numberToSpawn) {

        float leftConstraint, topConstraint, rightConstraint, bottomConstraint;  //constraints for where to spawn the entities 
        
        leftConstraint = -11.5f;
        rightConstraint = 11.5f;
        topConstraint = 6.5f;
        bottomConstraint = -6.5f;

        List<GameObject> entitiesSpawnedList = new();   //create list to store spawned entities 
        for (int i = 0; i < numberToSpawn; i++) {
            int index = Random.Range(0, entityPrefabArray.Length);
            Vector2 location;

            do {
                //create a new random location
                location = new Vector2(Random.Range(leftConstraint, rightConstraint),
                                    Random.Range(bottomConstraint, topConstraint));
                //Instantiate an enemy and store it in the enemies array
                entitiesSpawnedList.Add(Instantiate(entityPrefabArray[index], location, Quaternion.identity));

            } while (SpawnLocationNotEmpty(location)); //keep testing locations until there is an empty area
        }
        return entitiesSpawnedList;
    }
    /// <summary>
    /// Test to see if the spawn location is empty or not
    /// </summary>
    /// <param name="location">the vector2 location to test</param>
    /// <returns>true if the area around the location vector is empty false otherwise</returns>
    private bool SpawnLocationNotEmpty(Vector2 location) {
        return Physics2D.Raycast(location, new Vector2(.1f, 0), 0.1f);
    }
    /// <summary>
    /// handle pausing and unpausing the game loading the talent menu for now
    /// </summary>
    public void Pause() {
        if (Time.timeScale == 1) {
            Time.timeScale = 0;                         //pauses time and activates talent menu
            talentBackground.SetActive(true);
            talentController.SetActive(true);
            isPaused = true;
            Camera.main.orthographicSize = 5;           //zoom camera out because the sizes were messed up
            talentController.GetComponent<TalentTreeBehaviour>().
                SetCoinText(playerObject.GetComponent<PlayerBehaviour>().NumCoins());       //set coin text on talent background
        }
        else {
            isPaused = false;
            Time.timeScale = 1;                         //unpause, deactivate talent menu, zoom camera back in
            talentBackground.SetActive(false);
            talentController.SetActive(false);
            Camera.main.orthographicSize = 2;
        }

    }


    //creates a grass background by tiling the randomly chosen grass squares, unused
    private void CreateBackground() {
        for (int x = 0; x < backgroundCol; x++) {
            for (int y = 0; y < backgroundRow; y++) {
                Instantiate(grassObjects[Random.Range(0, grassObjects.Length)],     
                    new Vector2(backgroundStartingLocation.x + (x * GRASS_SIZE),
                    backgroundStartingLocation.y - (y * GRASS_SIZE)),
                    Quaternion.identity);
            }
        }
    }
    //creates a square border of large rocks so the player cannot leave the game world, unused
    private void CreateOuterBarrier() {
        //2.9 and 1.6 + the width of the rock prevents the camera from viewing the edge of the grass squares 
        borderStartLocation = new Vector2(backgroundStartingLocation.x + 2.9f, backgroundStartingLocation.y - 1.6f);
        numBorderRocksX = (int)(backgroundCol * GRASS_SIZE / LARGE_ROCK_SIZE) - 4;  //4 is about the horizontal width of the camera from the center
        numBorderRocksY = (int)(backgroundRow * GRASS_SIZE / LARGE_ROCK_SIZE) - 2;  //2 is about the vertical height of the camera
        for (int x = 0; x < numBorderRocksX; x++) {
            for (int y = 0; y < numBorderRocksY; y++) {
                if (x == 0 || x == numBorderRocksX - 1 || y == 0 || y == numBorderRocksY - 1) {
                    Instantiate(largeRockObjects[Random.Range(0, 1)],   //choose one of the 2 rock options
                        new Vector2(borderStartLocation.x + x * LARGE_ROCK_SIZE, borderStartLocation.y - y * LARGE_ROCK_SIZE),
                        Quaternion.identity);
                }
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TalentTreeBehaviour : MonoBehaviour {
    [SerializeField] private Sprite[] talentButtonSprites = new Sprite[16];             //images to display for talents currently just random placeholder image
    private const int HEART_SPRITE_INDEX = 15;                                          //where the heart icon is located
    [SerializeField] private GameObject talentObjectPreFab;                             //talent object to create talents with
    private const string TALENT_FILE_PATH = "Assets\\TalentMenu\\talents.json";         //location of the talents.json file to hold talent info
    public static Vector2 ROOT_NODE_LOCATION = new(0f, -4f);                            //where the 1st talent will be displayed
    [SerializeField] private TextMeshProUGUI coinText;                                  //text to display the cost of the talent

    [SerializeField] private GameObject leftBracket, rightBracket;                      //prefabs to create the brackets displaying links 
                                                                                        //between talents
    private Vector3 screenPoint;                                                        //
    private Vector3 offset;                                                             //used to be able to click and drag to move talent tree

    //constants to correctly place brackets offset from their parent talent
    public const float BRACKET_OFFSET_Y = 1.2f;                                         
    public const float BRACKET_OFFSET_X = 1.6f;

    public const float OFFSET_Y = 2.4f;
    public const float OFFSET_X = 3;

    //public const float START_X = 0;
    //public const float START_Y = -4;

    BinaryTree talentTree;
    // Start is called before the first frame update
    void Start() {
        ParseAllTalents();
        talentTree.ActivateAllNodes(talentTree.Root);                                                           
        talentTree.SetNodeTransforms(talentTree.Root, OFFSET_X, OFFSET_Y, BRACKET_OFFSET_X,                     
            BRACKET_OFFSET_Y, leftBracket, rightBracket, gameObject);
                                                                                             
        coinText.text = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>().NumCoins() + "";       
    }

    //records click location and offset of the mouse to the center of the screen
    void OnMouseDown() {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

    }

    //allows user to drag the talent tree around
    void OnMouseDrag() {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = new Vector2(curPosition.x, curPosition.y);

    }
    /// <summary>
    /// Parses the talents.json file and uses the ParseJsonTalent to create talents 
    /// </summary>
    public void ParseAllTalents() {
        string json = "";
        using (StreamReader sr = File.OpenText(TALENT_FILE_PATH)) {
            while (!sr.EndOfStream) {
                json += sr.ReadLine();                      //read the entire file
            }

            json = json[1..];                               //remove the first character in the json string
            json.Trim('}');                                 //remove the final } before splitting the file
            string[] talents = json.Split("}");             //split into individual talent json text
            for (int x = 0; x < talents.Length; x++) {
                ParseJsonTalent(talents[x]);                //parse each talent one at a time
            }
        }
    }
    /// <summary>
    /// sets the coin text to the passed in int
    /// </summary>
    /// <param name="money">the amount of money the player has </param>
    public void SetCoinText(int money) {
        coinText.text = money + "";
    }

    /// <summary>
    /// Takes a section of json data and creates a Talent instance out of it and pushes it into the tree
    /// This is super messy and should be re written most likely but it works 
    /// </summary>
    /// <param name="json">The section of Json data representing a single talent</param>
    void ParseJsonTalent(string json) {
        if (json.IndexOf('{') == -1)
            return;

        int cost, id, effectId;
        string name, description;

        name = json.Substring(2, json.IndexOf('{') - 5);        //remove anything before the opening bracket character
        name = name.Replace('\"', '\u2009');                    //remove the quotations from the name


        json = json.Remove(0, name.Length + 5);                 //remove 5 spaces after the talent name

        string[] jsonLines = json.Split(",");                   //split the json into its parts by splitting on the comma
        string[] variables = new string[jsonLines.Length];      //create array to hold the edited parts of the json text

        for (int x = 0; x < jsonLines.Length; x++) {
            variables[x] = jsonLines[x][jsonLines[x].IndexOf(':')..].Trim('\"');    //remove everything before the colon and trim off the quotation mark at the end
            variables[x] = variables[x][3..];                                       //remove the space and quotation mark at the start
            variables[x] = variables[x].Replace('\"', '\u2009');                    //replace any left over quotation marks with the empty character
        }

        description = variables[0];         //first thing is description
        id = int.Parse(variables[1]);       //then parse the id, cost, and effect (effect is always a 6 digit integer for this to work)
        cost = int.Parse(variables[2]);
        effectId = int.Parse(variables[3][..6]);


        //create a new game object and instantiate it, initialize all of its fields
        GameObject temp = Instantiate(talentObjectPreFab, ROOT_NODE_LOCATION, Quaternion.identity);
        temp.GetComponent<Talent>().Init(name, description, cost, id, effectId, TalentHasBeenPurchased(id));
        temp.SetActive(false);                                              //deactivate it and attach it to the current transform as a parent
        temp.transform.parent = transform;

        //temporaryish code
        //if it is a heart talent then set the icon to the heart otherwise randomize it from the placeholder images
        if (name.Contains("Heart")) {
            temp.GetComponent<Talent>().SetSprite(talentButtonSprites[HEART_SPRITE_INDEX]);
        }
        else
            temp.GetComponent<Talent>().SetSprite(talentButtonSprites[Random.Range(0, HEART_SPRITE_INDEX - 1)]);

        //push it onto the tree
        if (talentTree == null) {
            talentTree = new BinaryTree(temp);
        }
        else {
            talentTree.Add(temp, talentTree.Root);
        }
    }
    /// <summary>
    /// checks if the talent being created has already been purchased
    /// </summary>
    /// <param name="id">the id to check against the list of purchased talents</param>
    /// <returns>true if the id has been purchased and was in the list false otherwise</returns>
    private bool TalentHasBeenPurchased(int id) {
        PlayerBehaviour pb = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        foreach(int talentId in pb.purchasedTalents) {
            if (talentId == id)
                return true;
        }
        return false;
    }




}

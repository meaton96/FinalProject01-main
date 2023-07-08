using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class StoreBehaviour : MonoBehaviour
{
    [SerializeField]public GameObject[] weaponPrefabs = new GameObject[40];
    List<GameObject> weaponsForSale;
    [SerializeField] int numWeaponsForSale;
    private const string ICON_TAG = "StoreWeaponButton";
    private const string EXIT_TAG = "StoreExitButotn";
    public int roundNumber;
    [SerializeField] Vector2 itemStartLocation;
    [SerializeField] float itemDistance;
    [SerializeField] float costYOffset;
    [SerializeField] GameObject[] weaponButtons;
    private float button_padding;
    private float BUTTON_WIDTH;
    
    // Start is called before the first frame update
    void Start()
    {
        weaponButtons = new GameObject[3];
        button_padding = .37f;
        BUTTON_WIDTH = 2.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateWeaponOffers() {
        weaponsForSale = new();
        for (int x = 0; x < numWeaponsForSale; x++) {
            weaponsForSale.Add(CreateWaponToSell(x));
        }
        foreach (GameObject weapon in weaponsForSale) {
            //create weapon game object and then create store button
        }
    }
    private void HandleMouse() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                if (hit.collider.gameObject.CompareTag(ICON_TAG)) {
                    float mousePosX = Input.mousePosition.x;

                    

                }

            }
        }
    }
    private int ButtonIndex(float mousePosX) {
        for (int x = 0; x < weaponButtons.Length; x++) {
            if (mousePosX >= itemStartLocation.x + x * (button_padding + BUTTON_WIDTH) &&
                mousePosX <= itemStartLocation.x + (x + 1) * (button_padding + BUTTON_WIDTH)) {
                return x;
            }
        }
        return -1;
    }

    private GameObject CreateWaponToSell(int num) {
        int index = Random.Range(0, weaponPrefabs.Length);
        GameObject weaponPreFab = weaponPrefabs[index];
        string weaponName = weaponPreFab.name;
        int damage = Random.Range(roundNumber, 2 * roundNumber);
        Weapon.Type type = Weapon.Type.Spear;

        if (weaponName.StartsWith("Axe"))
            type = Weapon.Type.Axe;
        else if (weaponName.StartsWith("Sword"))
            type = Weapon.Type.Sword;
        else if (weaponName.StartsWith("Spear"))
            type = Weapon.Type.Spear;
        else if (weaponName.StartsWith("Scythe"))
            type = Weapon.Type.Scythe;
        else if (weaponName.StartsWith("Hammer"))
            type = Weapon.Type.Hammer;
        else if (weaponName.StartsWith("Dagger"))
            type = Weapon.Type.Dagger;
        else if (weaponName.StartsWith("Staff"))
            type = Weapon.Type.Staff;
        else if (weaponName.StartsWith("Shield"))
            type = Weapon.Type.Shield;

        weaponName = type.ToString() + " " + roundNumber;

        return Instantiate(weaponPreFab, new Vector2(itemStartLocation.x + num * itemDistance, itemStartLocation.y), 
            Quaternion.identity);

    }
}

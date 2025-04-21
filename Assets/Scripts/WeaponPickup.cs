using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public int weaponType; // 1 = Shotgun, 2 = Rifle
    public int ammoAmount = 5;

    private bool playerInRange = false;
    private GameObject player;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
        }
    }

    private void TryPickup()
    {
        if (player == null) return;

        var ammoManager = player.GetComponent<WeaponAmmoManager>();
        var weaponManager = player.GetComponent<PlayerWeaponManager>();

        if (ammoManager == null || weaponManager == null) return;

        // Сброс старого оружия, если есть
        if (weaponManager.currentWeapon == "Shotgun")
            ammoManager.DropCurrentWeapon("Shotgun", player.transform.position);
        else if (weaponManager.currentWeapon == "Rifle")
            ammoManager.DropCurrentWeapon("Rifle", player.transform.position);

        // Установка новых патронов и смена оружия
        if (weaponType == 1)
        {
            ammoManager.SetAmmo("Shotgun", ammoAmount);
            weaponManager.EquipShotgun();
        }
        else if (weaponType == 2)
        {
            ammoManager.SetAmmo("Rifle", ammoAmount);
            weaponManager.EquipRifle();
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }
}

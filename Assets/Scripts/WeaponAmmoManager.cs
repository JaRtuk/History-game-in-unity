using UnityEngine;

public class WeaponAmmoManager : MonoBehaviour
{
    public int ShotgunMaxAmmo = 10;
    public int RifleMaxAmmo = 30;

    public float ShotgunReloadTime = 1f;
    public float RifleFireRate = 6f;

    public GameObject DroppedShotgunPrefab;
    public GameObject DroppedRiflePrefab;

    public int CurrentShotgunAmmo { get; set; }
    public int CurrentRifleAmmo { get; set; }

    public bool HasShotgun => CurrentShotgunAmmo > 0;
    public bool HasRifle => CurrentRifleAmmo > 0;

    public void SetAmmo(string weaponType, int amount)
    {
        if (weaponType == "Shotgun")
            CurrentShotgunAmmo = Mathf.Clamp(amount, 0, ShotgunMaxAmmo);
        else if (weaponType == "Rifle")
            CurrentRifleAmmo = Mathf.Clamp(amount, 0, RifleMaxAmmo);
    }

    public int GetAmmo(string weaponType)
    {
        if (weaponType == "Shotgun") return CurrentShotgunAmmo;
        if (weaponType == "Rifle") return CurrentRifleAmmo;
        return 0;
    }

    public void UseAmmo(string weaponType, int amount)
    {
        if (weaponType == "Shotgun")
            CurrentShotgunAmmo = Mathf.Max(CurrentShotgunAmmo - amount, 0);
        else if (weaponType == "Rifle")
            CurrentRifleAmmo = Mathf.Max(CurrentRifleAmmo - amount, 0);
    }

    public void DropCurrentWeapon(string weaponType, Vector3 position)
    {
        if (weaponType == "Shotgun" && CurrentShotgunAmmo > 0 && DroppedShotgunPrefab != null)
        {
            GameObject dropped = Instantiate(DroppedShotgunPrefab, position, Quaternion.identity);
            WeaponPickup pickup = dropped.GetComponent<WeaponPickup>();
            if (pickup != null)
            {
                pickup.weaponType = 1;
                pickup.ammoAmount = CurrentShotgunAmmo;
            }
            CurrentShotgunAmmo = 0;
        }
        else if (weaponType == "Rifle" && CurrentRifleAmmo > 0 && DroppedRiflePrefab != null)
        {
            GameObject dropped = Instantiate(DroppedRiflePrefab, position, Quaternion.identity);
            WeaponPickup pickup = dropped.GetComponent<WeaponPickup>();
            if (pickup != null)
            {
                pickup.weaponType = 2;
                pickup.ammoAmount = CurrentRifleAmmo;
            }
            CurrentRifleAmmo = 0;
        }
    }
}

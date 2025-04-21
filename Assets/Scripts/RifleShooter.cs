using UnityEngine;

public class RifleShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject muzzleFlash;

    public float fireRate = 0.15f;
    private float nextFireTime;

    private WeaponAmmoManager ammoManager;
    private PlayerWeaponManager weaponManager;

    void Start()
    {
        ammoManager = GetComponent<WeaponAmmoManager>();
        weaponManager = GetComponent<PlayerWeaponManager>();
        muzzleFlash.SetActive(false);
    }

    void Update()
    {
        if (weaponManager == null || ammoManager == null) return;
        if (weaponManager.currentWeapon != "Rifle") return;

        bool isAiming = Input.GetMouseButton(1);
        bool shooting = Input.GetMouseButton(0);

        if (isAiming && shooting && Time.time >= nextFireTime)
        {
            if (ammoManager.CurrentRifleAmmo > 0)
            {
                FireBullet();
                nextFireTime = Time.time + fireRate;
            }
        }

        // Автоматическое сбрасывание автомата, если патроны закончились
        if (ammoManager.CurrentRifleAmmo <= 0 && weaponManager.currentWeapon == "Rifle")
        {
            ammoManager.DropCurrentWeapon("Rifle", transform.position);
            weaponManager.EquipWeapon("None");
        }
    }

    void FireBullet()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<RifleBullet>().SetDirection(dir);

        ammoManager.CurrentRifleAmmo--;
        StartCoroutine(Flash());
    }

    System.Collections.IEnumerator Flash()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
    }
}



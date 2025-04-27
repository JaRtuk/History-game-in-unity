using UnityEngine;

public class ShotgunShooter : MonoBehaviour
{
    public GameObject pelletPrefab;
    public Transform firePoint;
    public GameObject muzzleFlash;
    public int pelletCount = 6;
    public float spreadAngle = 15f;
    public float reloadTime = 1.6f;

    private float lastFireTime;
    private int shotsFired = 0;
    private bool isReloading = false;

    private WeaponAmmoManager ammoManager;
    private PlayerWeaponManager weaponManager;

    private void Start()
    {
        ammoManager = GetComponent<WeaponAmmoManager>();
        weaponManager = GetComponent<PlayerWeaponManager>();
        muzzleFlash.SetActive(false);
    }

    private void Update()
    {
        if (weaponManager.currentWeapon != "Shotgun") return;
        if (isReloading) return;

        bool isAiming = Input.GetMouseButton(1);
        bool shootPressed = Input.GetMouseButtonDown(0);

        if (isAiming && shootPressed && ammoManager.CurrentShotgunAmmo > 0)
        {
            Shoot();
            shotsFired++;

            if (shotsFired >= 2)
            {
                isReloading = true;
                Invoke(nameof(FinishReload), reloadTime);
            }
        }

        // Автоматический сброс дробовика, если патроны кончились
        if (ammoManager.CurrentShotgunAmmo <= 0 && weaponManager.currentWeapon == "Shotgun")
        {
            ammoManager.DropCurrentWeapon("Shotgun", transform.position);
            weaponManager.EquipWeapon("None");
        }
    }

    private void FinishReload()
    {
        shotsFired = 0;
        isReloading = false;
    }

    private void Shoot()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 baseDir = (mousePos - firePoint.position).normalized;

        for (int i = 0; i < pelletCount; i++)
        {
            float angleOffset = Random.Range(-spreadAngle, spreadAngle);
            Vector2 direction = Quaternion.Euler(0, 0, angleOffset) * baseDir;

            GameObject pellet = Instantiate(pelletPrefab, firePoint.position, Quaternion.identity);
            pellet.GetComponent<ShotgunBullet>().SetDirection(direction);
        }

        ammoManager.CurrentShotgunAmmo--;
        StartCoroutine(Flash());
    }

    System.Collections.IEnumerator Flash()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
    }
}


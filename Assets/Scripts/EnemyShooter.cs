// using UnityEngine;

// public class EnemyShooter : MonoBehaviour
// {
//     public enum WeaponType { Rifle, Shotgun }
//     public WeaponType weaponType = WeaponType.Rifle;

//     [Header("Общие настройки")]
//     public Transform firePoint;
//     public float fireRate = 1f;

//     [Header("Для автомата")]
//     public GameObject rifleBulletPrefab;
//     public float rifleBulletSpeed = 10f;

//     [Header("Для дробовика")]
//     public GameObject shotgunPelletPrefab;
//     public int pelletCount = 6;
//     public float spreadAngle = 15f;
//     public float shotgunReloadTime = 1.6f;

//     private int shotsFired = 0;
//     private bool isReloading = false;

//     public void ShootAt(Vector2 targetPosition)
//     {
//         if (weaponType == WeaponType.Rifle)
//         {
//             FireRifle(targetPosition);
//         }
//         else if (weaponType == WeaponType.Shotgun)
//         {
//             if (!isReloading && shotsFired < 2)
//             {
//                 FireShotgun(targetPosition);
//                 shotsFired++;

//                 if (shotsFired >= 2)
//                 {
//                     isReloading = true;
//                     Invoke(nameof(FinishReload), shotgunReloadTime);
//                 }
//             }
//         }
//     }

//     void FireRifle(Vector2 targetPosition)
//     {
//         Vector2 direction = (targetPosition - (Vector2)firePoint.position).normalized;

//         GameObject bullet = Instantiate(rifleBulletPrefab, firePoint.position, Quaternion.identity);
//         bullet.transform.right = direction;

//         Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
//         if (rb != null)
//             rb.velocity = direction * rifleBulletSpeed;
//     }

//     void FireShotgun(Vector2 targetPosition)
//     {
//         Vector2 baseDir = (targetPosition - (Vector2)firePoint.position).normalized;

//         for (int i = 0; i < pelletCount; i++)
//         {
//             float angleOffset = Random.Range(-spreadAngle, spreadAngle);
//             Vector2 direction = Quaternion.Euler(0, 0, angleOffset) * baseDir;

//             GameObject pellet = Instantiate(shotgunPelletPrefab, firePoint.position, Quaternion.identity);
//             ShotgunBullet pelletScript = pellet.GetComponent<ShotgunBullet>();
//             if (pelletScript != null)
//                 pelletScript.SetDirection(direction);
//         }
//     }

//     void FinishReload()
//     {
//         isReloading = false;
//         shotsFired = 0;
//     }

//     public float GetEffectiveFireRate()
//     {
//         return weaponType == WeaponType.Rifle ? fireRate : 0.3f; // дробовик делает 2 выстрела подряд быстро
//     }
// }


using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public enum WeaponType { Rifle, Shotgun }
    public WeaponType weaponType = WeaponType.Rifle;

    [Header("Общие настройки")]
    public Transform firePoint;
    public float fireRate = 1f;

    [Header("Для автомата")]
    public GameObject rifleBulletPrefab;
    public float rifleBulletSpeed = 10f;

    [Header("Для дробовика")]
    public GameObject shotgunPelletPrefab;
    public int pelletCount = 6;
    public float spreadAngle = 15f;
    public float shotgunReloadTime = 1.6f;

    private int shotsFired = 0;
    private bool isReloading = false;

    public void ShootAt(Vector2 targetPosition)
    {
        if (weaponType == WeaponType.Rifle)
        {
            FireRifle(targetPosition);
        }
        else if (weaponType == WeaponType.Shotgun)
        {
            if (!isReloading && shotsFired < 2)
            {
                FireShotgun(targetPosition);
                shotsFired++;

                if (shotsFired >= 2)
                {
                    isReloading = true;
                    Invoke(nameof(FinishReload), shotgunReloadTime);
                }
            }
        }
    }

    void FireRifle(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)firePoint.position).normalized;

        GameObject bullet = Instantiate(rifleBulletPrefab, firePoint.position, Quaternion.identity);
        bullet.transform.right = direction;

        RifleBullet bulletScript = bullet.GetComponent<RifleBullet>();
        if (bulletScript != null)
        {
            bulletScript.owner = gameObject;
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = direction * rifleBulletSpeed;
    }

    void FireShotgun(Vector2 targetPosition)
    {
        Vector2 baseDir = (targetPosition - (Vector2)firePoint.position).normalized;

        for (int i = 0; i < pelletCount; i++)
        {
            float angleOffset = Random.Range(-spreadAngle, spreadAngle);
            Vector2 direction = Quaternion.Euler(0, 0, angleOffset) * baseDir;

            GameObject pellet = Instantiate(shotgunPelletPrefab, firePoint.position, Quaternion.identity);

            ShotgunBullet pelletScript = pellet.GetComponent<ShotgunBullet>();
            if (pelletScript != null)
            {
                pelletScript.owner = gameObject;
                pelletScript.SetDirection(direction);
            }
        }
    }

    void FinishReload()
    {
        isReloading = false;
        shotsFired = 0;
    }

    public float GetEffectiveFireRate()
    {
        return weaponType == WeaponType.Rifle ? fireRate : 0.3f; // дробовик делает 2 выстрела подряд быстро
    }
}

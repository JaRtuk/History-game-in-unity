using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [SerializeField] public string currentWeapon = "None";
    public bool hasShotgun = false;
    public bool hasRifle = false;

    private Animator animator;
    private PlayerMovement movement;

    void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();

        // Устанавливаем стартовое состояние (например, без оружия)
        EquipWeapon("None");
    }

    public void EquipWeapon(string weaponType)
    {
        currentWeapon = weaponType;

        switch (weaponType)
        {
            case "Shotgun":
                hasShotgun = true;
                hasRifle = false;
                animator.SetInteger("weaponType", 1);
                break;

            case "Rifle":
                hasShotgun = false;
                hasRifle = true;
                animator.SetInteger("weaponType", 2);
                break;

            default:
                hasShotgun = false;
                hasRifle = false;
                animator.SetInteger("weaponType", 0);
                break;
        }

        // Форсируем обновление анимации, чтобы моментально переключить визуал
        if (movement != null)
        {
            movement.ForceAnimationUpdate();
        }
    }

    public void EquipShotgun() => EquipWeapon("Shotgun");
    public void EquipRifle() => EquipWeapon("Rifle");
}



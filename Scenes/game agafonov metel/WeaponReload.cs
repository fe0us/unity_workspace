using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class WeaponReload : MonoBehaviour
{
    [Header("Сокет магазина")]
    [Tooltip("XR Socket Interactor, куда вставляется магазин")]
    public XRSocketInteractor magazineSocket;

    [Header("Магазин")]
    [Tooltip("Объект магазина, который лежит в сцене")]
    public GameObject magazineObject;

    [Header("Патроны")]
    public int maxAmmo = 30;
    public int currentAmmo = 30;

    [Header("Ссылка на оружие")]
    public VRGun gun;

    private bool _magazineInserted = false;

    private void Awake()
    {
        if (gun == null) gun = GetComponent<VRGun>();

        // Подписываемся на события сокета
        magazineSocket.selectEntered.AddListener(OnMagazineInserted);
        magazineSocket.selectExited.AddListener(OnMagazineRemoved);
    }

    private void OnDestroy()
    {
        if (magazineSocket != null)
        {
            magazineSocket.selectEntered.RemoveListener(OnMagazineInserted);
            magazineSocket.selectExited.RemoveListener(OnMagazineRemoved);
        }
    }

    private void OnMagazineInserted(SelectEnterEventArgs args)
    {
        _magazineInserted = true;

        // Пополняем патроны
        currentAmmo = maxAmmo;
        Debug.Log("[Reload] Магазин вставлен. Патроны: " + currentAmmo);

        // Разрешаем стрельбу
        if (gun != null) gun.enabled = true;
    }

    private void OnMagazineRemoved(SelectExitEventArgs args)
    {
        _magazineInserted = false;

        // Обнуляем патроны в оружии
        currentAmmo = 0;
        Debug.Log("[Reload] Магазин извлечён. Патроны: " + currentAmmo);

        // Запрещаем стрельбу
        if (gun != null) gun.enabled = false;
    }

    // Вызывается из VRGun при выстреле
    public bool TryConsumeAmmo()
    {
        if (!_magazineInserted) return false;
        if (currentAmmo <= 0) return false;

        currentAmmo--;
        return true;
    }

    public bool HasMagazine => _magazineInserted;
}
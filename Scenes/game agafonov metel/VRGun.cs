using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class VRGun : MonoBehaviour
{
    [Header("Настройки пули")]
    public GameObject bulletPrefab;
    public Transform muzzlePoint;

    [Header("Настройки стрельбы")]
    public float bulletSpeed = 25f;
    public float fireRate = 8f;

    private Transform controller;
    Quaternion startRotation;

    private bool triggerHeld;
    private float nextShotTime;


    public void Select(SelectEnterEventArgs args)
    {
        Debug.Log(args.interactorObject);
        controller = args.interactorObject.transform;
        startRotation = controller.rotation;
    }

    public void Deselect()
    {
        controller = null;
    }


    public void StartShoot()
    {
        triggerHeld = true;
        startRotation = controller.localRotation;
    }

    public void EndShoot()
    {
        //controller.rotation = startRotation;
        triggerHeld = false;
    }

    private void Update()
    {

        if (triggerHeld && Time.time >= nextShotTime)
        {
            Shoot();
        }

        if (!controller) return;    

        if (!triggerHeld && !controller.localRotation.Equals(Quaternion.identity))
        {
            controller.localRotation = Quaternion.Lerp(controller.localRotation, Quaternion.identity, Time.deltaTime);
        }
    }

    public void Shoot()
    {
        // Проверяем патроны через WeaponReload
        WeaponReload reload = GetComponent<WeaponReload>();
        if (reload != null && !reload.TryConsumeAmmo())
        {
            return; // Нет патронов — не стреляем
        }
        if (bulletPrefab == null || muzzlePoint == null)
        {
            Debug.LogWarning("Не назначена пуля или MuzzlePoint!");
            return;
        }

        nextShotTime = Time.time + 1f / Mathf.Max(0.1f, fireRate);

        GameObject bullet = Instantiate(
        bulletPrefab,
        muzzlePoint.position,
        muzzlePoint.rotation
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("На пуле отсутствует Rigidbody!");
            Destroy(bullet);
            return;
        }

        // Игнорируем столкновения пули с автоматом
        Collider[] gunColliders = GetComponentsInChildren<Collider>();
        Collider[] bulletColliders = bullet.GetComponentsInChildren<Collider>();

        foreach (Collider bc in bulletColliders)
        {
            foreach (Collider gc in gunColliders)
            {
                Physics.IgnoreCollision(bc, gc);
            }
        }

        // Запускаем пулю прямо вдоль ствола
        rb.velocity = muzzlePoint.forward * bulletSpeed;

        controller.Rotate(Vector3.right * -1.5f);

        // Удаляем пулю через 5 секунд
        Destroy(bullet, 5f);
    }
}
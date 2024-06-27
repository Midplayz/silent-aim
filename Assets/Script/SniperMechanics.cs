using UnityEngine;
using System.Collections;

public class SniperMechanics : MonoBehaviour
{
    public int magazineSize = 6;
    public int totalAmmo = 30;
    public float recoilAmount = 1f;
    public float cameraRecoilAmount = 0.2f;
    public float recoilDuration = 0.1f;
    public float returnSpeed = 0.5f;
    public float fireRate = 1f; 
    public Transform gunTransform;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public Camera playerCamera;
    public PlayerRotation playerRotationScript;

    private int currentAmmoInMagazine;
    private bool isReloading = false;
    private bool canShoot = true;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    void Start()
    {
        currentAmmoInMagazine = magazineSize;
        originalCameraPosition = playerCamera.transform.localPosition;
        originalCameraRotation = playerCamera.transform.localRotation;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canShoot && !isReloading)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    void Shoot()
    {
        if (currentAmmoInMagazine > 0)
        {
            currentAmmoInMagazine--;

            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            bullet.GetComponent<Rigidbody>().velocity = playerCamera.transform.forward * 50f;

            StartCoroutine(ApplyRecoil());

            StartCoroutine(FireRateDelay());

            totalAmmo--;

            // sfx
            // muzzleflash
        }
        else
        {
            Debug.Log("Out of ammo, reload!");
        }
    }

    IEnumerator FireRateDelay()
    {
        canShoot = false;
        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }

    void Reload()
    {
        if (totalAmmo > 0 && currentAmmoInMagazine < magazineSize)
        {
            int ammoToReload = Mathf.Min(magazineSize - currentAmmoInMagazine, totalAmmo);
            currentAmmoInMagazine += ammoToReload;
            totalAmmo -= ammoToReload;

            // sfx
            // anim

            isReloading = false;
        }
    }

    IEnumerator ApplyRecoil()
    {
        // Gun Recoil
        Quaternion originalGunRotation = gunTransform.localRotation;
        Quaternion targetGunRotation = originalGunRotation * Quaternion.Euler(0, 0, -recoilAmount);

        // Camera Recoil effect
        Vector3 originalCameraPosition = playerCamera.transform.localPosition;
        Quaternion originalCameraRotation = playerCamera.transform.localRotation;
        Quaternion targetCameraRotation = originalCameraRotation * Quaternion.Euler(-cameraRecoilAmount, 0, 0);

        float elapsedTime = 0f;

        while (elapsedTime < recoilDuration)
        {
            gunTransform.localRotation = Quaternion.Slerp(originalGunRotation, targetGunRotation, elapsedTime / recoilDuration);
            if (playerRotationScript.isScoping)
            {
                playerCamera.transform.localPosition = Vector3.Lerp(originalCameraPosition, originalCameraPosition, elapsedTime / recoilDuration);
                playerCamera.transform.localRotation = Quaternion.Slerp(originalCameraRotation, targetCameraRotation, elapsedTime / recoilDuration);
            }
            else
            {
                playerCamera.transform.localRotation = Quaternion.Slerp(originalCameraRotation, targetCameraRotation, elapsedTime / recoilDuration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < recoilDuration * returnSpeed)
        {
            gunTransform.localRotation = Quaternion.Slerp(targetGunRotation, originalGunRotation, elapsedTime / (recoilDuration * returnSpeed));
            if (playerRotationScript.isScoping)
            {
                playerCamera.transform.localPosition = Vector3.Lerp(originalCameraPosition, originalCameraPosition, elapsedTime / (recoilDuration * returnSpeed));
                playerCamera.transform.localRotation = Quaternion.Slerp(targetCameraRotation, originalCameraRotation, elapsedTime / (recoilDuration * returnSpeed));
            }
            else
            {
                playerCamera.transform.localRotation = Quaternion.Slerp(targetCameraRotation, originalCameraRotation, elapsedTime / (recoilDuration * returnSpeed));
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gunTransform.localRotation = originalGunRotation;
        if (playerRotationScript.isScoping)
        {
            playerCamera.transform.localPosition = originalCameraPosition;
            playerCamera.transform.localRotation = originalCameraRotation;
        }
    }
}

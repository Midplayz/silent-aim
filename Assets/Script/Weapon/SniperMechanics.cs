using UnityEngine;
using System.Collections;
using UnityEditor;

public class SniperMechanics : MonoBehaviour
{
    [field: Header("Sniper Mechanics")]
    [field: SerializeField] private int magazineSize = 6;
    [field: SerializeField] private int totalAmmo = 30;
    [field: SerializeField] private float recoilAmount = 1f;
    [field: SerializeField] private float cameraRecoilAmount = 0.2f;
    [field: SerializeField] private float recoilDuration = 0.1f;
    [field: SerializeField] private float returnSpeed = 0.5f;
    [field: SerializeField] private float bulletSpeed = 100f;
    [field: SerializeField] private float fireRate = 1f;
    [field: SerializeField] private float reloadDuration = 2f; 

    [field: Header("Game Objects")]
    [field: SerializeField] private Transform gunTransform;
    [field: SerializeField] private GameObject bulletPrefab;
    [field: SerializeField] private Transform bulletSpawnPoint;
    [field: SerializeField] private Camera playerCamera;

    [field: Header("Scoping Values")]
    [field: SerializeField] private float normalFOV = 60f;
    [field: SerializeField] private float scopedFOV = 20f;
    [field: SerializeField] private float zoomSpeed = 10f;
    [field: SerializeField] private GameObject sniper;
    [field: SerializeField] private Vector3 scopedPosition = new Vector3(0, -0.2f, 0.5f);
    [field: SerializeField] private GameObject scopeImage;
    private Vector3 originalSniperPosition;

    private bool isScoping = false;
    private bool hasSniperMoved = false;

    private int currentAmmoInMagazine;
    private bool isReloading = false;
    private bool canShoot = true;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    [Header("Audio")]
    [SerializeField] private SoundManager soundManager;

    void Start()
    {
        originalSniperPosition = sniper.transform.localPosition;
        scopeImage.SetActive(false);
        currentAmmoInMagazine = magazineSize;
        originalCameraPosition = playerCamera.transform.localPosition;
        originalCameraRotation = playerCamera.transform.localRotation;
    }

    void Update()
    {
        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            HandleScoping();
        }
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

            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.Euler(-90f, 0f, 0f));
            bullet.GetComponent<Rigidbody>().linearVelocity = playerCamera.transform.forward * bulletSpeed;
            bullet.GetComponent<BulletScript>().bulletSpeed = bulletSpeed; 

            int bulletLayer = LayerMask.NameToLayer("Bullet");
            int zombieLayer = LayerMask.NameToLayer("Zombie");
            bullet.GetComponent<BulletScript>().hitMask = ~(1 << bulletLayer);

            Destroy(bullet, 2f);

            StartCoroutine(ApplyRecoil());
            StartCoroutine(FireRateDelay());

            totalAmmo--;

            soundManager.PlayFireSound();
            // Play muzzle flash animation
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
            StartCoroutine(ReloadCoroutine());
        }
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        // If scoping, scope out
        if (isScoping)
        {
            isScoping = false;
            hasSniperMoved = false;
            sniper.SetActive(true);
            scopeImage.SetActive(false);

            // Reset camera and gun positions to default non-scoping view
            playerCamera.fieldOfView = normalFOV;
            sniper.transform.localPosition = originalSniperPosition;
            playerCamera.transform.localPosition = originalCameraPosition;
            playerCamera.transform.localRotation = originalCameraRotation;
        }

        soundManager.PlayActionSoundSequence("Reload");
        // anim

        yield return new WaitForSeconds(reloadDuration);

        int ammoToReload = Mathf.Min(magazineSize - currentAmmoInMagazine, totalAmmo);
        currentAmmoInMagazine += ammoToReload;
        totalAmmo -= ammoToReload;

        isReloading = false;

        // If the player is still holding the scoping key, scope back in
        if (Input.GetMouseButton(1))
        {
            isScoping = true;
            hasSniperMoved = false;
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
            if (isScoping)
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
            if (isScoping)
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
        if (isScoping)
        {
            playerCamera.transform.localPosition = originalCameraPosition;
            playerCamera.transform.localRotation = originalCameraRotation;
        }
    }

    private void HandleScoping()
    {
        if (isReloading) return; // Prevent scoping while reloading

        if (Input.GetMouseButtonDown(1))
        {
            isScoping = true;
            hasSniperMoved = false;
            scopeImage.SetActive(false);
        }
        if (Input.GetMouseButtonUp(1))
        {
            isScoping = false;
            hasSniperMoved = false;
            sniper.SetActive(true);
            scopeImage.SetActive(false);

            // Reset camera and gun positions to default non-scoping view
            playerCamera.fieldOfView = normalFOV;
            sniper.transform.localPosition = originalSniperPosition;
            playerCamera.transform.localPosition = originalCameraPosition;
            playerCamera.transform.localRotation = originalCameraRotation;
        }

        if (isScoping)
        {
            MoveSniperToScopedPosition();
            if (hasSniperMoved)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, scopedFOV, zoomSpeed * Time.deltaTime);
                sniper.SetActive(false);
                scopeImage.SetActive(true);
            }
        }
        else
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, normalFOV, zoomSpeed * Time.deltaTime);
            sniper.transform.localPosition = Vector3.Lerp(sniper.transform.localPosition, originalSniperPosition, zoomSpeed * Time.deltaTime);
        }
    }

    private void MoveSniperToScopedPosition()
    {
        sniper.transform.localPosition = Vector3.Lerp(sniper.transform.localPosition, scopedPosition, zoomSpeed * Time.deltaTime);
        if (Vector3.Distance(sniper.transform.localPosition, scopedPosition) < 0.01f)
        {
            hasSniperMoved = true;
        }
    }
}

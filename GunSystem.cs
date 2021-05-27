using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSystem : MonoBehaviour
{
    // Weapon stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;
    public AudioClip ReloadAudio;
    public AudioClip GunFire;
    public AudioClip GunEmpty;

    //bools
    bool shooting, readyToShoot, reloading;
    Vector2 lastrays, lastraye;

    //References
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;
    public GameObject FiringEffect, ContactEffect;
    public GameObject Flashlight;
    public AudioSource GunAudio;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
        if (Flashlight!=null)
        { Flashlight.SetActive(false); }
    }

    private void Update()
    {
        MyInput();
        Debug.DrawLine(lastrays, lastraye);
    }

    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();

        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
        }
    }
    public void ToggleFlashlight()
    {
        if (Flashlight != null)
        { Flashlight.SetActive(!Flashlight.activeSelf); }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Spread bullets
        Vector2 vspread = new Vector2(Random.Range(-1, 1),Random.Range(-1, 1));

        //Calculate Direction with spread
        Vector3 direction = attackPoint.transform.forward + (Vector3)vspread.normalized * spread;

        //If using rigid body on player character, add these for controlled spread when running.
        /*
            if (rigidbody.celocity.magnitude > 0)
            spread = spread * 1.5f;
            else spread = "normal spread";

        */

        //RayCast, add Enemy to ShootingAi
        Debug.DrawLine(attackPoint.position, attackPoint.position + attackPoint.rotation * Vector3.forward * range, Color.yellow, .1f);
        if (  Physics.Raycast(attackPoint.position, direction, out rayHit, range, whatIsEnemy))
        {
            Debug.DrawLine(attackPoint.position, rayHit.point, Color.red, .1f);
            if (rayHit.collider.CompareTag("Enemy")) {
                rayHit.collider.GetComponent<Hunter>().TakeDamage( damage );
            }
        }
        GameObject firingeffect = GameObject.Instantiate(FiringEffect);
        firingeffect.transform.SetParent(attackPoint.transform);
        firingeffect.transform.localPosition = Vector3.zero;
        firingeffect.transform.localRotation = Quaternion.Euler(Vector3.zero);
        GameObject.Destroy(firingeffect.transform.Find("Light").gameObject, .05f);
        if (rayHit.collider != null)
        {
            GameObject contacteffect = GameObject.Instantiate(ContactEffect);
            Destroy(contacteffect, 10f);
            contacteffect.transform.localPosition = rayHit.point- attackPoint.transform.forward*.1f;
            contacteffect.transform.localRotation = attackPoint.transform.rotation;
        }


        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);
        GunAudio.Stop();

        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            GunAudio.PlayOneShot(GunEmpty);
            Invoke("Shoot", timeBetweenShots);
        }
        else
            GunAudio.PlayOneShot(GunFire);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }
    private void Reload()
    {
        GunAudio.PlayOneShot(ReloadAudio);
        reloading = true;
        Invoke("ReloadFinished", reloadTime); 
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }

}

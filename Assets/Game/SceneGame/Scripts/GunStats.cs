using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunStats {
    public int gunIndex;
    public int maxAmmoInMag;
    public int startingReserveAmmo;
    public int currentAmmoInMag;
    public int currentAmmoInReserve;
    public float fireRate;
    public float range;
    public float damage;

    public GunStats(int gunChoice)
    {
        gunIndex = gunChoice;
        if (gunIndex == 2)
        {
            damage = 15;
            maxAmmoInMag = 5;
            startingReserveAmmo = 30;
            fireRate = 1f;
            range = 3000f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
        else if (gunIndex == 1)
        {
            damage = 10;
            maxAmmoInMag = 30;
            startingReserveAmmo = 150;
            fireRate = .1f;
            range = 2000f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
        else if (gunIndex == 3)
        {
            damage = 60;
            maxAmmoInMag = 1;
            startingReserveAmmo = 15;
            fireRate = 1f;
            range = 3000f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
        else if (gunIndex == 4)
        {
            damage = 60;
            maxAmmoInMag = 4;
            startingReserveAmmo = 20;
            fireRate = 1f;
            range = 2000f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
        else if (gunIndex == 5)
        {
            damage = 10;
            maxAmmoInMag = 60;
            startingReserveAmmo = 180;
            fireRate = .05f;
            range = 8000f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
        else if (gunIndex == 6)
        {
            damage = 30;
            maxAmmoInMag = 2;
            startingReserveAmmo = 10;
            fireRate = 1f;
            range = 600f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
        else if (gunIndex == 7)
        {
            damage = 60;
            maxAmmoInMag = 1;
            startingReserveAmmo = 15;
            fireRate = 1f;
            range = 8000f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
        else if (gunIndex == 8)
        {
            damage = 30;
            maxAmmoInMag = 3;
            startingReserveAmmo = 15;
            fireRate = 1f;
            range = 1200f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
        else
        {
            damage = 20;
            maxAmmoInMag = 20;
            startingReserveAmmo = 50;
            fireRate = .5f;
            range = 2000f;
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInReserve = startingReserveAmmo;
        }
    }

    public void ResetAmmo(bool refillMag)
    {
        currentAmmoInReserve = startingReserveAmmo;
        if (currentAmmoInReserve >= maxAmmoInMag)
        {
            if (refillMag)
            {
                currentAmmoInMag = maxAmmoInMag;
                currentAmmoInReserve -= maxAmmoInMag;
            }

        }
        else
        {
            if (refillMag)
            {
                currentAmmoInMag = currentAmmoInReserve;
                currentAmmoInReserve = 0;
            }
        }

    }
}

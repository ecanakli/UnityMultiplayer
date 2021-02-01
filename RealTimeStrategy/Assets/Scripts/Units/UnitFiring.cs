using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;
    [SerializeField] private float fireRange = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 20f;

    private Targetable target = null;
    private float lastFireTime;

    [ServerCallback]
    private void Update()
    {
        ProjectileFireProcess();
    }

    private void ProjectileFireProcess()
    {
        target = targeter.GetTarget();

        if (target == null) { return; }

        CalculatingFireRange();

        Fire();
    }

    private void CalculatingFireRange()
    {
        //To Do ---------Fix Range Value-----------

        //Is This Target In Range
        if (!CanFireAtTarget()) { return; }

        //If In Range, Rotate Between Current Rotation And Target Rotation
        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void Fire()
    {

        if (Time.time > (1 / fireRate) + lastFireTime)//Calculate Fire Rate
        {
            //Firing

            Quaternion projectileRotation = Quaternion.LookRotation(target.GetAimAtPoint().position - projectileSpawnPoint.position);

            //To Do -----------Instantiate Projectile After Rotation Process Finished-----------
            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);

            //Before That Part We Instantiating Projectile Only On Server, To Instantiate Also On Clients We Use NetworkServer Command
            NetworkServer.Spawn(projectileInstance, connectionToClient);

            lastFireTime = Time.time;
        }
    }

    [Server]
    //Calculating FireRange
    private bool CanFireAtTarget()
    {
        return ((targeter.GetTarget().transform.position - transform.position).sqrMagnitude <= fireRange * fireRange);
    }
}
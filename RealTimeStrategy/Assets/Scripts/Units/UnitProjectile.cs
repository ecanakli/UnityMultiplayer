using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private int damageToDeal = 20;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float launchForce = 10f;

    private void Start()
    {
        rb.velocity = transform.forward * launchForce;
    }

    public override void OnStartServer()
    {
        //Destroy Projectile After Destroy Delay
        Invoke(nameof(DestroyProjectile), destroyAfterSeconds);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        //If Other Gameobject Has The Networkidentity Component Then Check If It Belongs To Us.
        if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            //If Hitting Our Unit Return
            if(networkIdentity.connectionToClient == connectionToClient) { return; }
        }

        //If Hitted Object Has Health Then Deal Damage
        if(other.TryGetComponent<Health>(out Health health))
        {
            health.DealDamage(damageToDeal);
        }

        DestroyProjectile();
    }

    [Server]
    //Destroying Projectile
    private void DestroyProjectile()
    {
        NetworkServer.Destroy(gameObject);
    }
}

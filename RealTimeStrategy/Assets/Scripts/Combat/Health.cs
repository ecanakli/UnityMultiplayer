using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int currentHealth;

    //Dying
    public event Action ServerOnDie;

    //Health Change
    public event Action<int, int> ClientOnHealthUpdated;

    #region Server

    public override void OnStartServer()
    {
        currentHealth = maxHealth;

        UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
    }
    public override void OnStopServer()
    {
        UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
    }

    [Server]
    //If The Base Died And Player Id And Object Id Matches, Destroy All Matches Player Objects Because Player Base Died
    private void ServerHandlePlayerDie(int connectionId)
    {
        //If The Player Who Died Not Us Then Return, Dont Destroy Objects
        if(connectionToClient.connectionId != connectionId) { return; }
        //Destroy Died Player Objects
        DealDamage(currentHealth);
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        //If We Already Dead Return
        if(currentHealth == 0) { return; }

        //If The Health After You Have Taken Off The Damage, Lower Than Zero Then Stop Get Negative Number It Set to The Zero

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        /* It is Same As The Upper Code Segment
        
        currentHealth -= damageAmount;

        if(currentHealth < 0)
        {
            currentHealth = 0;
        }
        */

        if(currentHealth != 0) { return; }

        //If health == 0 We Died

        ServerOnDie?.Invoke();
    }

    #endregion

    #region Client

    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }

    #endregion
}

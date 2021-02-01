using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    //References
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10f;

    private Camera mainCamera;

    #region Server

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [ServerCallback]
    private void Update()
    {
        //Get Target
        Targetable target = targeter.GetTarget();

        //If We Do Have A Target
        if (target != null)
        {
            //If We Are Out Of The Chase Range Then Chase
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)//Same As Vector3.Destination But This Function More Efficient and Optimized
            {
                agent.SetDestination(target.transform.position);
            }
            //Stop Chase
            else if (agent.hasPath)
            {
                agent.ResetPath();
            }

            return;
        }

        //If One Unit Reaches It's Path Then Other Units Not Bumping Eachother, They Clear Their Path.
        AvoidBumping();
    }

    private void AvoidBumping()
    {
        //Trying To Clear Agent's Path In The Same Frame
        if (!agent.hasPath) { return; }

        //Trying to move that location
        if (agent.remainingDistance > agent.stoppingDistance) { return; }

        //To Avoid Bumping Units When They Are Trying To Get Close Locations.
        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        //Clear Target If It Has.
        targeter.ClearTarget();

        //Validation For Move
        if(!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)){ return; }

        //Move to hit Pos
        agent.SetDestination(hit.position);
    }

    [Server]
    //When The Game Is Over Clear The Path, Don't Move
    private void ServerHandleGameOver()
    {
        agent.ResetPath();
    }

    #endregion
}

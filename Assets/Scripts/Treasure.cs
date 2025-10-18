using Mirror;
using UnityEngine;

public class Treasure : NetworkBehaviour
{
    [Server]
    void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        player.points++;
        //Chamar o metodo de spawn do manager
        Destroy(gameObject);
    }
}

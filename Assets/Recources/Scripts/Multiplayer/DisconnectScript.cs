using Photon.Pun;
using UnityEngine;

public class DisconnectScript : MonoBehaviourPunCallbacks
{
    public void Disconnect()

    {
        PhotonNetwork.Disconnect();
    }
}


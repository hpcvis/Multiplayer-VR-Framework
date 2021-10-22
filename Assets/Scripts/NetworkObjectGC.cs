using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObjectGC : MonoBehaviour, IOnPhotonViewControllerChange
{
    public PhotonView selfView;

    private void Awake()
    {
        selfView.AddCallbackTarget(this);
    }

    public void OnControllerChange(Player newController, Player previousController)
    {
        Debug.Log("Controller change");
        PhotonNetwork.Destroy(this.gameObject);
    }
}

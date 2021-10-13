using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//This script is used to Enable Fallback onto the different types of players.
//This script is both on the CameraRig(aka VR player) and on the fallback player
//It will swap between disabling/enabling the fallback player or VR player
//On the CameraRig leave isFallBack false and on the fallback player turn isFallBack to true
//This script will also send in the RPC in the "disable.cs" to notify the other players of this change
public class EnableFallback : MonoBehaviour
{

    public Disable disable;
    public bool isFallBack;

    private void OnDisable()
    {
        disable = transform.root.gameObject.GetComponent<Disable>();
        if (isFallBack && disable.photonView.IsMine)
        {
            disable.useNotRpc();
        }
        else if (disable.photonView.IsMine)
        {
            disable.useAmRpc();
        }

    }
}
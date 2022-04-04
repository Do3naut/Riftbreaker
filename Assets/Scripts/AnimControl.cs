using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimControl : MonoBehaviour
{
    private CharacterController player;
    // Start is called before the first frame update
    void Start()
    {
        player = gameObject.GetComponentInParent<CharacterController>();
    }

    void CallTeleport()
    {
        player.Teleport();
    }

    void StartAnim()
    {
        player.animInProgress = true;
    }

    void EndAnim()
    {
        player.animInProgress = false;
    }
}

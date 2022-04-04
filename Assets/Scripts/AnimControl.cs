using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimControl : MonoBehaviour
{
    private CharacterController player;
    [Header("SFX")]
    [SerializeField] AudioSource jumpSound;
    [SerializeField] AudioSource riftSound;
    [SerializeField] AudioSource deathSound;
    [SerializeField] AudioSource teleportSound;
    // Start is called before the first frame update
    void Start()
    {
        player = gameObject.GetComponentInParent<CharacterController>();
    }

    // Mechanics

    void CallTeleport()
    {
        player.Teleport();
    }

    void PhasePrep()
    {
        player.PreStartPhase();
    }

    void CallStartPhase()
    {
        player.StartPhase();
    }

    void CallEndPhase()
    {
        player.EndPhase();
    }

    // Audio

    void PlayJump()
    {
        jumpSound.Play();
    }

    void PlayRift()
    {
        riftSound.Play();
    }

    void PlayTP()
    {
        teleportSound.Play();
    }

    void PlayDeath()
    {
        deathSound.Play();
    }

    // Control

    void StartAnim()
    {
        player.animInProgress = true;
    }

    void EndAnim()
    {
        player.animInProgress = false;
    }
}

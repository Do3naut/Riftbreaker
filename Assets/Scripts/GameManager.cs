using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] float startingSpeed = 100f;
    float speedScore;
    int difficulty;
    // Start is called before the first frame update
    void Start()
    {
        speedScore = startingSpeed;
        difficulty = 0;
    }

    private void FixedUpdate()
    {
    }

    public float GetSpeed() { return speedScore; }
    
    public void SetSpeed(float targ) { speedScore = targ; }

    public void DamagePlayer()  // No params bc fixed damage 
    {
        speedScore -= 10f;
    }

    public void IncreaseDifficulty() { difficulty++; }
    
    public int GetDifficulty() { return difficulty; }
}

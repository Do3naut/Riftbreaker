using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUI : MonoBehaviour
{

    [SerializeField] public Text playerSpeed;
    private float currentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public bool addSpeed(int toAdd)
    {
        currentSpeed += toAdd;
        playerSpeed.text = "Speed: " + currentSpeed;
        if (currentSpeed <= 0)
            return false;
        return true;
    }
    
    public bool setSpeed(float speed)
    {
        currentSpeed = speed;
        playerSpeed.text = "Speed: " + currentSpeed;
        return true;
    }
}

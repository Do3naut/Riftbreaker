using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUI : MonoBehaviour
{

    [SerializeField] public Text playerSpeed;
    [SerializeField] public Text scalingFactor;
    private float currentSpeed;
    private float currentScaleFactor;

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = 0;
        currentScaleFactor = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setScaleFactor(float scale)
    {
        currentScaleFactor = scale;
        scalingFactor.text = "Scaling Factor: " + currentScaleFactor; 
    }
    public bool addSpeed(float toAdd)
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

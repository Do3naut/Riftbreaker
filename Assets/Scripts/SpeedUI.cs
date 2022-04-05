using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedUI : MonoBehaviour
{

    [SerializeField] public Text playerSpeed;
    [SerializeField] public TextMeshProUGUI playerSpeed2;


    [SerializeField] public Text scalingFactor;
    [SerializeField] public TextMeshProUGUI scalingFactor2;

    [SerializeField] public Image scaleImage;

    [SerializeField] public Text deathTimer;
    [SerializeField] public TextMeshProUGUI deathTimer2;

    [SerializeField] Image timerImage;
    private float currentSpeed;
    private float currentScaleFactor;

    private float deathTimerValue;

    // Start is called before the first frame update
    void Start()
    {
        
        currentSpeed = 0;
        currentScaleFactor = 1;
        deathTimerValue = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentSpeed);
        //playerSpeed2.SetText("Speed: " + currentSpeed);
    }

    public bool setSpeed(float speed)
    {
        currentSpeed = speed;
        //Debug.Log(currentSpeed);
        this.playerSpeed2.SetText("Speed " + (Mathf.Round(currentSpeed * 100)) / 100.0);
        return true;
    }

    public void setScaleFactor(float scale)
    {
        
        currentScaleFactor = scale;
        Debug.Log(currentScaleFactor);
        scaleImage.fillAmount = currentScaleFactor/10 - .1f;
        //this.scalingFactor2.SetText("Scaling Factor " + (Mathf.Round(currentScaleFactor * 100)) / 100.0); 
    }
    public bool addSpeed(float toAdd)
    {
        currentSpeed += toAdd;
        //Debug.Log(currentSpeed);
        this.playerSpeed2.SetText("Speed " + (Mathf.Round(currentSpeed * 100)) / 100.0);
        return true;
    }
    


    public bool setDeathTimer(float time, bool isDecreasing, bool powerUp=false)
    {

        
        deathTimerValue = time;
        timerImage.fillAmount = deathTimerValue/60;
    
        
        this.deathTimer2.SetText("death: " + (Mathf.Round(deathTimerValue * 100)) / 100.0);
        return true;
    }
    
}

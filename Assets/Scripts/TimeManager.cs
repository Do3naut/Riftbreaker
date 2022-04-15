using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    /*
     This class controls the time slow effect, as well as applies any necessary Post Processing. Of course, 
     the methods have to be referenced by other scripts before taking place.
     */
    [SerializeField]
    public float slowTime = 0.5f;  // The threshold for slowed time
    [SerializeField]
    public float slowdownLength = 1.1f;  // Speed at which time slows
    [SerializeField]
    public float normalTime = 1f;  // Normal timescale

    [SerializeField] PostProcess postProcess;  // The post processing object

    [SerializeField] CharacterController characterController;  // The player

    bool changedTime = false;
    bool startChange = false;

    void Update()
    {
        if(startChange)
        {
            if(changedTime)
            {
                Time.timeScale -= (1f / slowdownLength) * Time.unscaledDeltaTime;
                if (Time.timeScale <= slowTime)
                    startChange = false;

            }

            else  
            {
                Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
                if (Time.timeScale >= normalTime)
                    startChange = false;
            }

            Time.timeScale = Mathf.Clamp(Time.timeScale, slowTime, normalTime);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }

        if(changedTime)
            characterController.BurnTime();
    }
    public void ToggleSlowMotion()
    {

        changedTime = !changedTime;
        startChange = true;
        postProcess.toggleShift();

    }

    public void StartSlowMotion()
    {
        changedTime = true;
        startChange = true;
        postProcess.BlueShift();
    }

    public void StopSlowMotion()
    {
        changedTime = false;
        startChange = true;
        postProcess.WhiteShift();
    }
    // Start is called before the first frame update
}

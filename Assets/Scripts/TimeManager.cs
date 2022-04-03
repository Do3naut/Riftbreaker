using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    public float slowTime = 0.5f;
    [SerializeField]
    public float slowdownLength = 1.1f;
    [SerializeField]
    public float normalTime = 1f;

    [SerializeField] PostProcess postProcess;

    [SerializeField] CharacterController characterController;

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
    // Start is called before the first frame update
}

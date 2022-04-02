using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    public float slowTime;
    [SerializeField]
    public float slowdownLength;
    [SerializeField]
    public float normalTime;

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
    }
    public void ToggleSlowMotion()
    {

        changedTime = !changedTime;
        startChange = true;

    }
    // Start is called before the first frame update
}

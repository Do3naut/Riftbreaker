using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class PostProcess : MonoBehaviour
{
    [SerializeField] float redMin = 0.175f;
    [SerializeField] float shiftSpeed = 3f;
    public PostProcessVolume volume;
    private ColorGrading color;

    bool shiftToBlue = false;
    bool shift = false;


    // Start is called before the first frame update
    void Start()
    {
        volume.profile.TryGetSettings(out color);
        color.colorFilter.value.b = 1;
        color.colorFilter.value.r = 1;
        color.colorFilter.value.g = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(shift)
        {
            if(shiftToBlue)
            {
                color.colorFilter.value.r -= (shiftSpeed * Time.unscaledDeltaTime);
                if(color.colorFilter.value.r < redMin)
                {
                    color.colorFilter.value.r = redMin;
                    shift = false;
                }
            }
            else
            {
                color.colorFilter.value.r += (shiftSpeed * Time.unscaledDeltaTime);
                if(color.colorFilter.value.r > 1)
                {
                    color.colorFilter.value.r = 1;
                    shift = false;
                }
            }
        }
    }

    public void BlueShift()
    {
        shiftToBlue = true;
        shift = true;
    }
    public void WhiteShift()
    {
        shiftToBlue = false;
        shift = true;
    }

    public void toggleShift()
    {
        shiftToBlue = !shiftToBlue;
        shift = true;
    }
}

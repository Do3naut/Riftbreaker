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
    private Vignette vignette;

    bool shiftToBlue = false;
    bool shiftToGray = false;
    bool shift = false;


    // Start is called before the first frame update
    void Start()
    {
        volume.profile.TryGetSettings(out color);
        volume.profile.TryGetSettings(out vignette);
        color.colorFilter.value.b = 1;
        color.colorFilter.value.r = 1;
        color.colorFilter.value.g = 1;

        
    }

    // Update is called once per frame
    void Update()
    {
        // All post-processing for the scene (Vignette & hue shifting)
        if(shift)
        {
            if(shiftToGray)  // Grayscale
            {
                // Vignette - "tunnel vision effect"
                vignette.intensity.value = Mathf.Clamp(vignette.intensity.value + shiftSpeed * Time.unscaledDeltaTime * .1f, 0, 0.37f);
                // Saturation goes to 0 to create grayscale effect
                color.saturation.value -= (shiftSpeed * Time.unscaledDeltaTime * 30);
                {
                    if(color.saturation.value < -100f)
                    {
                    color.saturation.value = -100f;
                    shift = false;
                    }
                    if(vignette.intensity.value > 0.37f)
                    {
                    color.saturation.value = 0.37f;
                    }
                }
            }
            else if(shiftToBlue)  // Hue shifting towards blue
            {
                if(vignette.intensity.value > 0)  // Undo any potential vignette remaining from other effects
                vignette.intensity.value = Mathf.Clamp(vignette.intensity.value - shiftSpeed * Time.unscaledDeltaTime * .1f, 0, 0.37f);
                // Blue effect: reduce red channel & increase intensity (feel free to tinker with values/colors)
                color.colorFilter.value.r -= (shiftSpeed * Time.unscaledDeltaTime);
                color.saturation.value += (shiftSpeed * Time.unscaledDeltaTime * 30);
                if(color.saturation.value > 1)
                {
                    color.saturation.value = 1;
                }
                if(color.colorFilter.value.r < redMin)
                {
                    color.colorFilter.value.r = redMin;
                    shift = false;
                }
            }
            else  // No Post Processing
            {
                if(vignette.intensity.value > 0)  // Undo vignette
                vignette.intensity.value = Mathf.Clamp(vignette.intensity.value - shiftSpeed * Time.unscaledDeltaTime * .1f, 0, 0.37f);
                // Reverse all shift effects
                color.saturation.value += (shiftSpeed * Time.unscaledDeltaTime * 30);
                color.colorFilter.value.r += (shiftSpeed * Time.unscaledDeltaTime);
                if(color.colorFilter.value.r > 1)
                {
                    color.colorFilter.value.r = 1;
                }
                if(color.saturation.value > 1)
                {
                    color.saturation.value = 1;
                }
                if(color.colorFilter.value.r > 1 && color.saturation.value > 1)
                    shift = false;
            }
        }
    }


    // Unity Event Handlers (to be called in the Animator)
    public void GrayShift()
    {
        shiftToGray = true;
        shift = true;
    }

    public void BlueShift()
    {
        shiftToBlue = true;
        shift = true;
    }
    public void WhiteShift()
    {
        shiftToBlue = false;
        shiftToGray = false;
        shift = true;
    }

    public void toggleShift()
    {
        shiftToBlue = !shiftToBlue;
        shift = true;
    }
}

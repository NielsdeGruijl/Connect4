using Cinemachine;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] CinemachineVirtualCamera cam;

    private CinemachineBasicMultiChannelPerlin perlinNoise;

    private bool canShake;
    private float timeElapsed;
    private float duration;

    float amplitude;
    float frequency;

    private void Start()
    {
        perlinNoise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    //apply screenshake which falls off over time
    private void Update()
    {
        if(canShake)
        { 
            timeElapsed += Time.deltaTime;
            
            perlinNoise.m_AmplitudeGain = Mathf.Lerp(amplitude, 0, timeElapsed / duration);
            perlinNoise.m_FrequencyGain = Mathf.Lerp(frequency, 0, timeElapsed / duration);

            if(timeElapsed >= duration)
                StopShake();
        }
    }

    //set screenshake values
    public void StartShake(float amplitude, float frequency, float duration = 1)
    {
        this.frequency = frequency;
        this.amplitude = amplitude;
        perlinNoise.m_AmplitudeGain  = amplitude;
        perlinNoise.m_FrequencyGain = frequency;
        this.duration = duration;
        timeElapsed = 0;
        canShake = true;
    }

    //stop screenshake
    private void StopShake()
    {
        perlinNoise.m_AmplitudeGain = 0;
        perlinNoise.m_FrequencyGain = 0;
        canShake = false;
    }
}

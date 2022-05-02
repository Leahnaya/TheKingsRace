using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footsteps : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] dirtStep;
    [SerializeField]
    private AudioClip[] rainStep;
    [SerializeField]
    private AudioClip[] snowStep;

    [SerializeField]
    private AudioClip[] landing;

    [SerializeField]
    private AudioClip[] rollerskate;

    public AudioSource audioSource;


    public AudioClip getRandomClipDirt()
    {
        return dirtStep[UnityEngine.Random.Range(0, (dirtStep.Length - 1))];
    }

    public AudioClip getRandomClipRain()
    {
        return rainStep[UnityEngine.Random.Range(0, (rainStep.Length - 1))];
    }

    public AudioClip getRandomClipSnow()
    {
        return dirtStep[UnityEngine.Random.Range(0, (snowStep.Length - 1))];
    }

    public AudioClip getSkate()
    {
        return rollerskate[UnityEngine.Random.Range(0, (rollerskate.Length - 1))];
    }

    public AudioClip getLanding()
    {
        return landing[UnityEngine.Random.Range(0, (landing.Length - 1))];
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Step()
    {
        if (GameObject.FindGameObjectWithTag("RainSystem").GetComponent<ParticleSystem>().isPlaying)
        {
            AudioClip clip = getRandomClipRain();
            //audioSource.PlayOneShot(clip, .25f);
        }

        else if (GameObject.FindGameObjectWithTag("SnowSystem").GetComponent<ParticleSystem>().isPlaying)
        {
            AudioClip clip = getRandomClipSnow();
            //audioSource.PlayOneShot(clip, .25f);
        }
        else
        {
            AudioClip clip = getRandomClipDirt();
            //audioSource.PlayOneShot(clip, .25f);
        }
        
    }

    private void Skate()
    {
        AudioClip clip = getSkate();
        //audioSource.PlayOneShot(clip, .25f);
    }

    private void Land()
    {
        AudioClip clip = getLanding();
        //audioSource.PlayOneShot(clip,.25f);
    }

}

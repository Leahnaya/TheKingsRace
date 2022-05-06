using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioHandler : MonoBehaviour
{

    public AudioSource mainTheme;

    
    public static AudioHandler Instance => instance;
    private static AudioHandler instance;

    

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Play()
    {
        if(mainTheme != null) mainTheme.Play();
    }

    void Stop()
    {
        if(mainTheme != null) mainTheme.Stop();
    }

    // Start is called before the first frame update
    void Start()
    {
        Play();

    }

    // Update is called once per frame
    void Update()
    {

        
        if (SceneManager.GetActiveScene().buildIndex == 3 || SceneManager.GetActiveScene().buildIndex == 4)
        {
            Stop();
        }
           
        
    }
}

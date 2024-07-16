using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class ItemAssets : MonoBehaviour
{
    private static ItemAssets instance;
    private static readonly object instanceLock = new object();

    public static ItemAssets Instance
    {
        get
        {
            lock (instanceLock)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<ItemAssets>();

                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject("ItemAssets");
                        instance = singletonObject.AddComponent<ItemAssets>();
                    }

                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
    }

    [SerializeField] private AudioSource menuMusic;
    [SerializeField] private ObjectPooler musicPooler;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string endlessRun;
    [SerializeField] private string gameOver;
    [SerializeField] private string soundFX;
    [SerializeField] private AudioClip[] deathSounds;
    [SerializeField] private AudioClip[] footSteps;

    [SerializeField] private float stepTime;
    private bool isPlaying = false;


    public Sprite superMagnetSprite;
    public Sprite emptyField;
    public Sprite doubleCoinsSprite;
    public Sprite flyToolSprite;


    public event EventHandler OnPlayerDeath;
    public event EventHandler OnDoubleCoinsTrigger;
    public event EventHandler OnSlowTimerTrigger;
    public event EventHandler OnJumping;
    public event EventHandler OnWalking;
    public event EventHandler OnCoinTrigger;

    private AudioSource instantiatedMusic;
    private GameObject pooledObject;
    private List<GameObject> activePooledObjects = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this as ItemAssets;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        SceneManager.sceneLoaded += StartPlayingMusic;
        SceneManager.sceneUnloaded += EndPlayingMusic;
        OnPlayerDeath += StartMusicOnDeath;
        OnWalking += PlayWalkingSound;

    }

    private void Start()
    {
        LoadVolumeSettings();
    }

    private void OnDestroy()
    {
        OnPlayerDeath = null;
        OnDoubleCoinsTrigger = null;
        OnSlowTimerTrigger = null;
        OnJumping = null;
        OnWalking = null;
        OnCoinTrigger = null;
        SceneManager.sceneLoaded -= StartPlayingMusic;
        SceneManager.sceneUnloaded -= EndPlayingMusic;
        OnWalking -= PlayWalkingSound;
        OnPlayerDeath -= StartMusicOnDeath;
        ReturnAllActivePooledObjects();
    }

    public void InvokeOnJumping() => OnJumping?.Invoke(this, EventArgs.Empty);
    public void InvokeOnWalking() => OnWalking?.Invoke(this, EventArgs.Empty);
    public void InvokeOnCoinTrigger() => OnCoinTrigger?.Invoke(this, EventArgs.Empty);
    
    public void InvokeOnSlowTimeTrigger() => OnSlowTimerTrigger?.Invoke(this, EventArgs.Empty);
    public void InvokeOnPlayerDeath() => OnPlayerDeath?.Invoke(this, EventArgs.Empty);
    public void InvokeOnDoubleCoinsTrigger() => OnDoubleCoinsTrigger?.Invoke(this, EventArgs.Empty);

    private void StartMusicOnDeath(object sender, EventArgs e){
        PlayRandomSoundFXClip(deathSounds, transform, 1f);
        Invoke(nameof(LoadGameOverScene), 1.5f);
    }

    private void PlayWalkingSound(object sender, EventArgs e){
       if(!isPlaying){
        isPlaying = true;
        PlayRandomSoundFXClip(footSteps, transform, 0.9f);
       StartCoroutine(SetisPlaying());
       }
    }

    private IEnumerator SetisPlaying(){
        yield return new WaitForSecondsRealtime(stepTime);
        isPlaying = false;
    }

    public void StartPlayingMusic(Scene newScene, LoadSceneMode mode)
    {
        if (newScene.name == "LevelSelect" || newScene.name == "Shop")
        {
            if (menuMusic != null && instantiatedMusic == null)
            {
                instantiatedMusic = Instantiate(menuMusic);
                DontDestroyOnLoad(instantiatedMusic.gameObject);
            }
        }
        else if (newScene.name == "Endless Runner")
        {
            DestroyCurrentMusic();
            if (pooledObject != null && pooledObject.activeInHierarchy)
            {
                musicPooler.ReturnToPool(pooledObject.tag, pooledObject);
                activePooledObjects.Remove(pooledObject);
            }
            pooledObject = musicPooler.GetPooledObject(endlessRun);
            if (pooledObject != null)
            {
                activePooledObjects.Add(pooledObject);
            }
        }
        else if (newScene.name == "GameOver")
        {
            if (pooledObject != null && pooledObject.activeInHierarchy)
            {
                musicPooler.ReturnToPool(pooledObject.tag, pooledObject);
                activePooledObjects.Remove(pooledObject);
            }
            pooledObject = musicPooler.GetPooledObject(gameOver);
            if (pooledObject != null)
            {
                activePooledObjects.Add(pooledObject);
            }
        }
    }

    public void EndPlayingMusic(Scene currentScene)
    {
        if (currentScene.name == "Endless Runner" || currentScene.name == "GameOver")
        {
           
                musicPooler.ReturnToPool(pooledObject.tag, pooledObject);
                activePooledObjects.Remove(pooledObject);
            
        }

        ReturnAllActivePooledObjects();
    }

    private void ReturnAllActivePooledObjects()
    {
        foreach (var pooledObj in activePooledObjects)
        {
            if (pooledObj != null && pooledObj.activeInHierarchy)
            {
                musicPooler.ReturnToPool(pooledObj.tag, pooledObj);
            }
        }
        activePooledObjects.Clear();
    }

    private void DestroyCurrentMusic()
    {
        if (instantiatedMusic != null)
        {
            Destroy(instantiatedMusic.gameObject);
            instantiatedMusic = null;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float value)
    {
        GameObject audioObj = musicPooler.GetPooledObject(soundFX);
        if (audioObj != null)
        {
            activePooledObjects.Add(audioObj);
            AudioSource audioSource = audioObj.GetComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.volume = value;
            audioSource.Play();
            float clipLength = audioSource.clip.length;
            Invoke(nameof(ReturnSoundFX), clipLength);
        }
        else
        {
            Debug.LogWarning("No available pooled audio object for sound FX.");
        }
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float value)
    {
        int rand = UnityEngine.Random.Range(0, audioClip.Length);
        GameObject audioObjRnd = musicPooler.GetPooledObject(soundFX);
        if (audioObjRnd != null)
        {
            activePooledObjects.Add(audioObjRnd);
            AudioSource audioSource = audioObjRnd.GetComponent<AudioSource>();
            audioSource.clip = audioClip[rand];
            audioSource.volume = value;
            audioSource.Play();
            float clipLength = audioSource.clip.length;
            Invoke(nameof(ReturnRndSoundFX), clipLength);
        }
        else
        {
            Debug.LogWarning("No available pooled audio object for random sound FX.");
        }
        
    }

    private void ReturnSoundFX()
    {
        foreach (var audioObj in activePooledObjects)
        {
            if (audioObj != null && audioObj.activeInHierarchy)
            {
                AudioSource audioSource = audioObj.GetComponent<AudioSource>();
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = null;
                    musicPooler.ReturnToPool(audioObj.tag, audioObj);
                }
            }
        }
        activePooledObjects.RemoveAll(obj => obj == null || !obj.activeInHierarchy);
    }

    private void ReturnRndSoundFX()
    {
        foreach (var audioObjRnd in activePooledObjects)
        {
            if (audioObjRnd != null && audioObjRnd.activeInHierarchy)
            {
                AudioSource audioSource = audioObjRnd.GetComponent<AudioSource>();
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = null;
                    musicPooler.ReturnToPool(audioObjRnd.tag, audioObjRnd);
                }
            }
        }
        activePooledObjects.RemoveAll(obj => obj == null || !obj.activeInHierarchy);
    }

    private void LoadGameOverScene()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void LoadVolumeSettings()
    {
        if (PlayerPrefs.HasKey("General"))
        {
            float generalVolume = PlayerPrefs.GetFloat("General");
            audioMixer.SetFloat("MainMixer", Mathf.Log10(generalVolume) * 20f);
        }

        if (PlayerPrefs.HasKey("Sound"))
        {
            float soundFXVolume = PlayerPrefs.GetFloat("Sound");
            audioMixer.SetFloat("SoundMixer", Mathf.Log10(soundFXVolume) * 20f);
        }

        if (PlayerPrefs.HasKey("Music"))
        {
            float musicVolume = PlayerPrefs.GetFloat("Music");
            audioMixer.SetFloat("MusicMixer", Mathf.Log10(musicVolume) * 20f);
        }
    }
}

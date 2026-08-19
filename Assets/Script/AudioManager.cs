using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SceneMusicMapping
{
    public string sceneName;
    public AudioClip musicClip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public SceneMusicMapping[] sceneMusicMappings;

    private AudioSource audioSource;
    private string currentSceneName;

    void Awake()
    {
        Application.targetFrameRate = 60;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = true;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentSceneName == scene.name) return;

        AudioClip targetClip = GetClipBySceneName(scene.name);

        if (targetClip != null)
        {
            if (audioSource.clip == targetClip && audioSource.isPlaying) return;

            audioSource.clip = targetClip;
            audioSource.Play();
            currentSceneName = scene.name;
        }

    }

    private AudioClip GetClipBySceneName(string name)
    {
        foreach (var mapping in sceneMusicMappings)
        {
            if (mapping.sceneName == name)
                return mapping.musicClip;
        }
        return null;
    }

    public void PlaySpecificClip(AudioClip clip, bool restart = true)
    {
        if (clip == null) return;
        if (audioSource.clip == clip && audioSource.isPlaying && !restart) return;

        audioSource.clip = clip;
        audioSource.Play();
        currentSceneName = "Special";
    }
}
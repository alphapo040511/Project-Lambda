using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine;

public class MenuScreenManager : MonoBehaviour
{
    public Material screenshader;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            EnableGlitch();
        }
        else
        {
            DisableGlitch();
        }
    }

    void EnableGlitch()
    {
        screenshader.DOKill();
        screenshader.DOFloat(2f, "_NoiseAmount", 0.1f);
        screenshader.DOFloat(1f, "_GlitchStrength", 0.1f);
        screenshader.DOFloat(1f, "_ScanLinesStrength", 0.1f);
    }

    void DisableGlitch()
    {
        screenshader.DOKill();
        screenshader.DOFloat(0f, "_NoiseAmount", 0.1f);
        screenshader.DOFloat(0f, "_GlitchStrength", 0.1f);
        screenshader.DOFloat(0f, "_ScanLinesStrength", 0.1f);
    }
}

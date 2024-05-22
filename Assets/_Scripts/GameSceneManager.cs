using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSceneManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource clickSound;

    public void LoadScene(string sceneName)
    {
        clickSound.Play();
        SceneManager.LoadScene(sceneName);
    }

    public void SetWorldPreference(int worldPreference)
    {
        clickSound.Play();
        PlayerPrefs.SetInt("WorldPreference", worldPreference);
    }

    public void QuitGame(){
        clickSound.Play();
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}

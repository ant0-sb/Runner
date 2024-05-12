using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSceneManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource sceneTransitionSound;

    public void LoadScene(string sceneName)
    {
        sceneTransitionSound.Play();
        SceneManager.LoadScene(sceneName);
    }
}

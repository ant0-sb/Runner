using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource buttonSound;
    public Animator transition;
    public float transitionTime = 1f;
    private GameObject lastSelectedButton;

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }
        else
        {
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        }
    }

    public void PlaySound()
    {
        StartCoroutine(PlaySoundandWait());
    }

    public IEnumerator PlaySoundandWait()
    {
        buttonSound.Play();
        yield return new WaitWhile(() => buttonSound.isPlaying);
    }

    public void SetWorldPreference(int worldPreference)
    {
        PlayerPrefs.SetInt("WorldPreference", worldPreference);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAfterSound(sceneName));
    }

    public IEnumerator LoadSceneAfterSound(string sceneName)
    {
        StartCoroutine(PlaySoundandWait());
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        StartCoroutine(PlaySoundandWait());
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
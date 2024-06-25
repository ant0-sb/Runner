using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScript : MonoBehaviour
{
    public Image bar;
    public TMP_Text tmpText;
    public float rotationSpeedX = 120f; // Vitesse de rotation de la caméra en degrés par seconde
    public Camera mainCamera; // Référence à la caméra que vous voulez faire tourner
    public float fallSpeedY = 50f; // Vitesse de chute selon l'axe Y


    void Start() 
    {
        StartCoroutine(LoadGameScene());
    }

    IEnumerator LoadGameScene()
    {
        AsyncOperation result = SceneManager.LoadSceneAsync("Game");
        
        while (!result.isDone)
        {
            float progress = Mathf.Clamp01(result.progress / 0.9f);
            bar.fillAmount = progress;
            tmpText.text = "Loading progress " + (progress * 100) + "%";

            // Rotation de la caméra autour de l'axe Y
            if (mainCamera != null)
            {
                mainCamera.transform.Rotate(Vector3.right, rotationSpeedX * progress);
            }
            mainCamera.transform.position -= new Vector3(0f,fallSpeedY*progress,0f);

            // Changement de texte à 80% de progression
            if (result.progress >= 0.8f)
            {
                tmpText.text = "Almost ready !";
            }

            yield return null;
        }
    }
}

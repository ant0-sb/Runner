using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LootLocker.Requests;

public class PlayerSkinItem : MonoBehaviour, IPointerClickHandler
{
    private static PlayerSkinItem currentlySelected;
    private string colorName;
    private Color originalColor;
    private bool isSelected = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        Image image = GetComponent<Image>();
        if (!isSelected)
        {
            // Désélectionne l'élément précédemment sélectionné
            if (currentlySelected != null && currentlySelected != this)
            {
                currentlySelected.Deselect();
            }

            originalColor = image.color;
            Color dimmedColor = originalColor * 0.8f; // Ternit la couleur de 20%
            image.color = dimmedColor;
            isSelected = true;

            // Met à jour l'élément actuellement sélectionné
            currentlySelected = this;
        }
        else
        {
            Deselect();
        }

        LootLockerSDKManager.UpdateOrCreateKeyValue("skin", colorName, (response) => {
            if (response.success) {
                Debug.Log("set player skin color key successfully");
            } else {
                Debug.Log("set player skin color key unsuccessfully");
            }
        });
    }

    public void Deselect()
    {
        Image image = GetComponent<Image>();
        image.color = originalColor; // Restaure la couleur originale
        isSelected = false;
    }

    public void SetColor(string color)
    {
        if (ColorUtility.TryParseHtmlString(color, out Color newColor))
        {
            GetComponent<Image>().color = newColor;
            originalColor = newColor; // Sauvegarde la couleur originale lors de la définition
        }
        else
        {
            Debug.Log("error parsing color");
        }
        colorName = color;
    }
}
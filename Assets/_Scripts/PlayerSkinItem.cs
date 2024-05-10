using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LootLocker.Requests;
public class PlayerSkinItem : MonoBehaviour, IPointerClickHandler
{
    private string colorName;
    public void OnPointerClick(PointerEventData eventData)
    {
        LootLockerSDKManager.UpdateOrCreateKeyValue("skin", colorName, (response) => {
            if (response.success) {
                Debug.Log("set player skin color key successfully");
            }else {
                Debug.Log("set player skin color key unsuccessfully");
            }

        });
    }
    public void SetColor(string color){
        if (ColorUtility.TryParseHtmlString(color, out Color newColor)){
            GetComponent<Image>().color = newColor;
        }
        else{
            Debug.Log("error parsing color");
        }
        colorName = color;
    }
}

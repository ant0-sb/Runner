using System.Collections;
using UnityEngine;
using LootLocker.Requests;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private UnityEvent playerConnected;

    private IEnumerator Start(){
        bool connected =false;
        LootLockerSDKManager.StartGuestSession((response)=>{
            if(!response.success) {
                Debug.Log("Error starting LootLocker session");
                return;

            }
            Debug.Log("Successfully Lootlocker Session");
            connected = true;
        
          });
          yield return new WaitUntil(()=> connected);
          playerConnected.Invoke(); 
    }

    public void QuitGame(){
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
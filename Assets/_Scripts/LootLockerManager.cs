using System.Collections;
using UnityEngine;
using LootLocker.Requests;
using UnityEngine.Events;

public class LootLockerManager : MonoBehaviour
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
            // If it's the player's first time, we need to register him in the skin progression system
            LootLockerSDKManager.RegisterPlayerProgression("skins", (response) =>
            {
                if(!response.success)
                {
                    Debug.Log("error regisering progression");
                    Debug.Log(response.errorData.ToString());
                    return;
                }
            
            Debug.Log("progression registered successfully");
        });
        
          });
          yield return new WaitUntil(()=> connected);
          playerConnected.Invoke(); 
    }

}
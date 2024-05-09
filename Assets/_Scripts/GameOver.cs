using System.Collections;
using UnityEngine;
using LootLocker.Requests;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverCanvas; 
    [SerializeField]
    private TMP_InputField inputField; 
    [SerializeField]
    private TextMeshProUGUI leaderboardScoreText;
    [SerializeField]
    private TextMeshProUGUI leaderboardNameText;
    [SerializeField]
    private TextMeshProUGUI scoreText;

    private int score = 0;
    private int leaderboardID = 22178;
    private int leaderboardTopCount = 10;


    public void StopGame(int score){
        gameOverCanvas.SetActive(true);
        this.score = score;
        scoreText.text =score.ToString();
        SubmitScore();

    }

    public void SubmitScore(){
        StartCoroutine(SubmitScoreToLeaderboard());
    }

    private IEnumerator SubmitScoreToLeaderboard(){
        bool? nameSet = null;
        LootLockerSDKManager.SetPlayerName(inputField.text,(response)=>{
            if(response.success){
                 Debug.Log("successfully set the player name");
                 nameSet = true;
            }
            else {
                Debug.Log("was not able to add the player name");
                nameSet = true;
            }
           
        });
        yield return new WaitUntil(()=> nameSet.HasValue);
        if(!nameSet.Value) yield break;
        bool? scoreSubmitted = null;
        LootLockerSDKManager.SubmitScore("",score,leaderboardID.ToString(), (reponse) =>{
            if(reponse.success){
                 Debug.Log("successfully submitted the score to the leaderboard");
                 scoreSubmitted = true;
            }
            else {
                Debug.Log("unsuccessfully submitted the score to the leaderboard");
                scoreSubmitted = true;
            }
           
        });
        yield return new  WaitUntil(()=> scoreSubmitted.HasValue);
        if(!scoreSubmitted.Value) yield break;
        GetLeaderboard();

    }
    private void GetLeaderboard(){
        LootLockerSDKManager.GetScoreList(leaderboardID.ToString(),leaderboardTopCount,(response)=>{
            if(response.success){
                Debug.Log("successfully retrieved the scores from the leaderboard");
                string leaderboardName = "";
                string leaderboardScore = "";
                LootLockerLeaderboardMember[] members= response.items;
                for(int i =0; i< members.Length; ++i){
                    LootLockerPlayer player = members[i].player;
                    if(player ==null) continue;

                    if(player.name!= ""){
                        leaderboardName += player.name + "\n";
    
                    }
                    else {
                        leaderboardName += player.id + "\n";
    
                    }
                    leaderboardScore += members[i].score + "\n";
    
                }
                leaderboardNameText.SetText(leaderboardName);
                leaderboardScoreText.SetText(leaderboardScore);
            }
            else{
                Debug.Log("unsuccessfully retrieved the scores from the leaderboard");
            }
        });
    }


    public void AddXP(int score){
    
    }

    public void ReloadScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    

}

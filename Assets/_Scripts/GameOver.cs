using System.Collections;
using UnityEngine;
using LootLocker.Requests;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameOver : MonoBehaviour
{
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
        this.score = score;
        scoreText.text =score.ToString();
        GetLeaderboard();
        AddXP(score);
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

        //if(!nameSet.Value) yield break;

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
                //TO DO: check members.Length in case leaderboard is empty
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
        string progressionKey = "skins";
        // If it's the player's first time, we need to register him in the skin progression system
        LootLockerSDKManager.RegisterPlayerProgression(progressionKey, (response) =>
        {
            if(!response.success)
            {
                Debug.Log("error regisering progression");
                Debug.Log(response.errorData.ToString());
                return;
            }
            
            Debug.Log("progression registered successfully");
        });

        LootLockerSDKManager.AddPointsToPlayerProgression(progressionKey, (ulong) score, response =>
        {
            if (!response.success) {
                Debug.Log("failed adding points to progression");
            }

            // If the awarded_tiers array contains any items that means the player leveled up
            // There can also be multiple level-ups at once
            if (response.awarded_tiers.Any())
            {
                foreach (var awardedTier in response.awarded_tiers)
                {
                    Debug.Log($"Reached level {awardedTier.step}!");

                    foreach (var assetReward in awardedTier.rewards.asset_rewards)
                    {
                        Debug.Log($"Rewarded with an asset, id: {assetReward.asset_id}!");
                    }
                    
                    foreach (var progressionPointsReward in awardedTier.rewards.progression_points_rewards)
                    {
                        Debug.Log($"Rewarded with {progressionPointsReward.amount} bonus points in {progressionPointsReward.progression_name} progression!");
                    }
                    
                    foreach (var progressionResetReward in awardedTier.rewards.progression_reset_rewards)
                    {
                        Debug.Log($"Progression {progressionResetReward.progression_name} has been reset as a reward!");
                    }
                }
            }
        });
        // LootLockerSDKManager.SubmitXp(score,(response)=>{
        //     if(response.success){
        //         Debug.Log("successfully added XP");
        //     }
        //     else{
        //         Debug.Log("unsuccessfully added XP");
        //     }
        // });
    }

    public void ReloadScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}

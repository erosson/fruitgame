using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.PPrefs;
using System.Collections;


namespace FrenzyGames.FruitGame {
	public class Leaderboard : MonoBehaviour {
		// This should match /Assets/Resources/LocalSocialSetting (until we get network highscores)
		public string leaderboardID;
		public Pause pause;

		void Start() {
			// Uncomment to reset local scores for debug builds.
			if (Debug.isDebugBuild) {
				//PlayerPrefs.DeleteAll();
			}

			// Local achievements and leaderboards. TODO: use ios gamecenter, android googleplay, ...
			// According to the docs, if I comment this out it will be used by default on most platforms except for iOS, where GameCenter will be used? Hm.
			// But we don't have the default leaderboardID configured in gamecenter yet, so force local leaderboards and let one of the ios guys figure out gamecenter.
			Social.Active = PPrefsSocialPlatform.Instance;

			// Log in. http://docs.unity3d.com/Documentation/Components/net-SocialAPI.html
			Social.localUser.Authenticate(success => {
				if (success) {
					Debug.Log ("Authenticated, checking achievements");
					
					// Request loaded achievements, and register a callback for processing them
					Social.LoadAchievements(ProcessAchievements);
					Social.LoadScores(leaderboardID, ProcessScores);
				}
				else {
					Debug.Log ("Failed to authenticate");
				}
			});
			
			pause.GameOverEvent += OnGameOver;
		}

		private void OnGameOver(Score score) {
			// Report a high score when the game ends.
			Social.localUser.Authenticate(success => {
				if (success) {
					Social.ReportScore(score.score, leaderboardID, result => {
						// TODO error handling if score failed to save
					});
				}
				else {
					Debug.Log ("Failed to authenticate");
				}
			});
		}

		private void ProcessAchievements(IAchievement[] cheevos) {
			Debug.Log("Achievements loaded: " + cheevos.Length);
		}

		private void ProcessScores(IScore[] scores) {
			Debug.Log("Scores loaded: " + scores.Length);
			foreach (var score in scores) {
				Debug.Log(score.date + ": " + score.formattedValue + "; user " + score.userID); 
			}
		}
	}
}
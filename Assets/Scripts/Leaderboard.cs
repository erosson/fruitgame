using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.PPrefs;
using System.Collections;

namespace FrenzyGames.FruitGame {
	public class Leaderboard : MonoBehaviour {
		// This should match /Assets/Resources/LocalSocialSetting (until we get network highscores)
		public string leaderboardID;
		public Pause pause;
		public GameObject achievementPopupPrefab;

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
			// TODO: don't do this right at the start once we get network achievements running. Prompt the user to log in,
			// because I think localUser.Authenticate() brings up a dialog
			Social.localUser.Authenticate(success => {
				if (success) {
					Debug.Log ("Authenticated");
				}
				else {
					Debug.Log ("Failed to authenticate");
				}/*
				// Test achievement GUI. Delete me!
				Social.LoadAchievementDescriptions(descs => {
					var popup = Instantiate(achievementPopupPrefab) as GameObject;
					popup.GetComponent<AchievementPopup>().achievement = descs[1];
					popup.SetActive(true);
				});*/
			});
			
			pause.GameOverEvent += OnGameOver;
		}

		private int PrefsAddInt(string key, int val) {
			val += PlayerPrefs.GetInt(key);
			PlayerPrefs.SetInt(key, val);
			return val;
		}

		private void ReportProgress(string key, double value) {
			ReportProgress(key, value, 1);
		}

		private void ReportProgress(string key, double value, double max) {
			var percent = 100 * value / max;
			percent = System.Math.Min(100, percent);
			// TODO this overwrites the date. Once it's at 100%, don't re-report it!
			Social.ReportProgress(key, percent, OnAchievementReport);
		}

		private void OnAchievementReport(bool result) {
			Debug.Log ("achievement reported "+result+"; "+PPrefsSocialPlatform.Instance._achievements.Count);
			// TODO error handling
		}

		private void OnGameOver(Score score) {
			// Report a high score when the game ends.
			Social.localUser.Authenticate(success => {
				if (success) {
					Social.ReportScore(score.score, leaderboardID, result => {
						// TODO error handling if score failed to save
					});
					var finished = PrefsAddInt("GamesFinished", 1);
					ReportProgress("Finish1", finished, 1);
					ReportProgress("Finish10", finished, 10);
					ReportProgress("Finish100", finished, 100);
					ReportProgress("Finish1000", finished, 1000);
				}
				else {
					Debug.Log ("Failed to authenticate");
				}
			});
		}

		public void LoadScores(System.Action<IScore[]> onload) {
			Social.localUser.Authenticate(success => {
				if (success) {
					Social.LoadScores(leaderboardID, onload);
				}
				else {
					Debug.Log ("Failed to authenticate");
				}
			});
		}
	}
}
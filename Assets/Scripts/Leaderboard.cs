using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.PPrefs;
using System.Collections;
using System.Collections.Generic;

namespace FrenzyGames.FruitGame {
	/**
	 * High score lists and achievements.
	 */
	public class Leaderboard : MonoBehaviour {
		// This should match /Assets/Resources/LocalSocialSetting (until we get network highscores)
		public string leaderboardID;
		public Pause pause;
		public GameObject achievementPopupPrefab;
		public AudioClip sfx;
		
		private Dictionary<string, AchievementData> achievementsByID;

		void Start() {
			// Uncomment to reset local scores for debug builds.
			if (Debug.isDebugBuild) {
				PlayerPrefs.DeleteAll();
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
					Social.LoadAchievementDescriptions(descs => {
						achievementsByID = new Dictionary<string, AchievementData>();
						foreach (IAchievementDescription desc in descs) {
							achievementsByID[desc.id] = new AchievementData(desc, 0);
						}

						Social.LoadAchievements(achieves => {
							foreach (IAchievement achieve in achieves) {
								achievementsByID[achieve.id].percentCompleted = achieve.percentCompleted;
							}
						});
						// Uncomment to test achievement GUI.
						//OnAchievementReport(achievementsByID["TestCheevo0"], 100.0, true);
					});
				}
				else {
					Debug.Log ("Failed to authenticate");
				}
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
			var achieve = achievementsByID[key];
			// Don't report already-achieved achievements, or lower/unchanged values
			if (!achieve.completed && percent > achieve.percentCompleted) {
				Social.ReportProgress(key, percent, result => {
					OnAchievementReport(achieve, percent, result);
				});
			}
		}

		private void OnAchievementReport(AchievementData achieve, double percent, bool result) {
			if (result) {
				achieve.percentCompleted = percent;
				if (achieve.completed) {
					// show achievement-complete popup
					var popup = Instantiate(achievementPopupPrefab) as GameObject;
					popup.GetComponent<AchievementPopup>().achievement = achieve.description;
					popup.SetActive(true);
					audio.PlayOneShot(sfx);
				}
			}
			else {
				// TODO error handling
			}
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

		private class AchievementData {
			public IAchievementDescription description;
			public double percentCompleted;

			public AchievementData(IAchievementDescription description, double percentCompleted) {
				this.percentCompleted = percentCompleted;
				this.description = description;
			}

			public bool completed {
				get {
					return percentCompleted >= 100;
				}
				set {
					percentCompleted = value ? 100 : 0;
				}
			}
		}
	}
}
using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Collections;
using System.Collections.Generic;

// TODO: delete mainmenu.cs in favor of this.
namespace FrenzyGames.FruitGame {
	public class GUIMainMenu : MonoBehaviour {
		public Leaderboard leaderboard;
		public SpawnScript spawn;
		public MainMenu mainMenu;
		public AudioClip sfx;
		public GUISkin skin;
		public int maxScoresShown;

		private enum State { Main, HighScoreList, AchievementsList };
		private State state = State.Main;

		private IScore[] highScores = null;
		private Vector2 scrollPosition = Vector2.zero;
		private List<IAchievement> achievements = null;
		private List<IAchievementDescription> achievementDescriptions = null;
		private List<DescribedAchievement> describedAchievements {
			get {
				if (achievements == null || achievementDescriptions == null) {
					return null;
				}
				Dictionary<string, IAchievement> cheevsById = new Dictionary<string, IAchievement>();
				foreach (var cheev in achievements) {
					if (cheevsById.ContainsKey(cheev.id)) {
						// Sometimes there are duplicates; show only the latest one
						if (cheev.lastReportedDate > cheevsById[cheev.id].lastReportedDate) {
							cheevsById.Remove(cheev.id);
							cheevsById.Add(cheev.id, cheev);
						}
					}
					else {
						cheevsById.Add(cheev.id, cheev);
					}
				}
				var ret = new List<DescribedAchievement>();
				foreach (IAchievementDescription desc in achievementDescriptions) {
					IAchievement cheev = null;
					if (cheevsById.ContainsKey(desc.id)) {
						cheev = cheevsById[desc.id];
					}
					ret.Add(new DescribedAchievement(desc, cheev));
				}
				return ret;
			}
		}

		void Start() {
			spawn.RestartEvent += OnRestart;
		}

		private string HighScoreText {
			get {
				if (highScores == null) {
					return "Loading...";
				}
				var ret = new List<string>();

				// limit number of scores shown. Negative or 0: no (practical) maximum
				var maxScoresShown = this.maxScoresShown > 0 ? this.maxScoresShown : long.MaxValue;

				for (var i=0; i < highScores.Length && i < maxScoresShown; i++) {
					var score = highScores[i];
					var rank = i+1;
					ret.Add(rank + ": " + score.formattedValue + " (" + score.date + ")"); 
				}
				return string.Join("\n", ret.ToArray());
			}
		}

		
		void Update() {
			// This also fires for Android's back button.
			if (Input.GetKeyDown(KeyCode.Escape)) {
				if (state == State.HighScoreList || state == State.AchievementsList) {
					BackToMain();
				}
			}
		}
		
		public void OnGUI() {
			GUI.skin = skin;
			// Never ever display any of this if the MainMenu, where state is currently tracked, thinks we're in the middle of the game.
			// TODO: delete MainMenu and maintain this state here, once this becomes the real MainMenu

			if (mainMenu.gameState == MainMenu.GameState.MainMenu) 
			{
				if (state == State.Main) {
					if (GUI.Button(new Rect(0, Screen.height * (1 - 0.12f), Screen.width * 0.4f, Screen.height * 0.12f), "High Scores")) {
						audio.PlayOneShot(sfx);
						scrollPosition = Vector2.zero;
						state = State.HighScoreList;
						leaderboard.LoadScores(scores => {
							this.highScores = scores;
						});
						// hack to break the "play" button while highscore gui is showing
						Time.timeScale = 0;
					}

					if (GUI.Button(new Rect(0, Screen.height * (1 - 0.36f), Screen.width * 0.4f, Screen.height * 0.12f), "Exit")) {
						Application.Quit(); 
						Debug.Log ("check quit");
					}
					if (GUI.Button(new Rect(0, Screen.height * (1 - 0.24f), Screen.width * 0.4f, Screen.height * 0.12f), "Achievements")) {
						audio.PlayOneShot(sfx);
						scrollPosition = Vector2.zero;
						state = State.AchievementsList;
						Social.localUser.Authenticate(result => {
							Social.LoadAchievements(cheevs => {
								foreach (var cheev in cheevs) {
									Debug.Log(cheev.id + "; " + cheev.percentCompleted + "; " + cheev.completed);
								}
								this.achievements = new List<IAchievement>(cheevs);
								this.achievements.Sort((a, b) => string.Compare(a.id, b.id));
							});
							Social.LoadAchievementDescriptions(cheevs => {
								this.achievementDescriptions = new List<IAchievementDescription>(cheevs);
								this.achievementDescriptions.Sort((a, b) => string.Compare(a.id, b.id));
							});
						});
						// hack to break the "play" button while highscore gui is showing
						Time.timeScale = 0;
					}
				}
				else if (state == State.HighScoreList) {
					GUILayout.Label("High Scores");
					scrollPosition = GUILayout.BeginScrollView(
						scrollPosition, false, true, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height * 0.7f));
					GUILayout.Box(HighScoreText);
					GUILayout.EndScrollView();
					if (GUILayout.Button("Exit")) {
						BackToMain();
					}
				}
				else if (state == State.AchievementsList) {
					GUILayout.Label("Achievements");
					//GUI.Box (new Rect(0, 0, Screen.width, Screen.height));
					scrollPosition = GUILayout.BeginScrollView(
						scrollPosition, false, true, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height * 0.7f));
					if (describedAchievements == null) {
						GUILayout.Box("Loading...");
					}
					else {
						foreach (DescribedAchievement cheev in describedAchievements) {
							if (!cheev.hidden) {
								GUILayout.BeginHorizontal();
								GUILayout.Box(cheev.description.image, GUILayout.Width(64), GUILayout.Height (64));
								//GUILayout.Label(cheev.description.id);
								GUILayout.Box(cheev.description.title);
								GUILayout.Box(cheev.currentDescription);
								//GUILayout.BeginVertical();
								//GUILayout.Box(cheev.completed ? "Complete" : "Incomplete");
								GUILayout.Box(cheev.percentCompletedString + " complete" + 
								              (cheev.lastReportedDate.HasValue ? ", " + cheev.lastReportedDate.Value : ""));
								//GUILayout.EndVertical();
								GUILayout.EndHorizontal();
							}
						}
					}
					GUILayout.EndScrollView();
					if (GUILayout.Button("Exit")) {
						BackToMain();
					}
				}
			}
		}

		private void BackToMain() {
			audio.PlayOneShot(sfx);
			state = State.Main;
			// hack to break the "play" button while achievements gui is showing
			Time.timeScale = 1;
		}
		
		private void OnRestart() {
			state = State.Main;
		}

		private class DescribedAchievement {
			public IAchievementDescription description;
			public IAchievement achievement;
			
			public DescribedAchievement(IAchievementDescription description, IAchievement achievement) {
				if (achievement != null && achievement.id != description.id) {
					// TODO assert
					throw new System.Exception();
				}
				this.description = description;
				this.achievement = achievement;
			}

			public string id {
				get {
					return description.id;
				}
			}

			public bool completed {
				get {
					return achievement != null && achievement.completed;
				}
			}

			public bool hidden {
				get {
					return !completed && description.hidden;
				}
			}
			
			public string currentDescription {
				get {
					if (completed && description.achievedDescription != "") {
						return description.achievedDescription;
					}
					return description.unachievedDescription;
				}
			}
			
			public double percentCompleted {
				get {
					return achievement != null ? achievement.percentCompleted : 0.0;
				}
			}
			
			public string percentCompletedString {
				get {
					// http://stackoverflow.com/questions/1790975/format-decimal-for-percentage-values
					return string.Format("{0:P0}", percentCompleted/100);
				}
			}

			// The "?" makes it nullable; datetime is a nonnull primitive.
			// http://stackoverflow.com/questions/1729553/system-datetime-vs-system-datetime
			public System.DateTime? lastReportedDate {
				get {
					return achievement != null ? achievement.lastReportedDate : (System.DateTime?)null;
				}
			}

			public string lastReportedDateString {
				get {
					return lastReportedDate != null ? lastReportedDate.Value + "" : "Never";
				}
			}
		}
	}
}
using UnityEngine;
using System.Collections;

namespace FrenzyGames.FruitGame {
	public class MatchData {
		public int NumRemoved;
		public Transform Touched;
		public int XTouched;
		public int YTouched;

		public MatchData(int numRemoved, Transform touched, int xTouched, int yTouched) {
			YTouched = yTouched;
			XTouched = xTouched;
			Touched = touched;
			NumRemoved = numRemoved;
		}
	}
}

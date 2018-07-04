using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoG.Map {

	#region DECO RULE
	[CreateAssetMenu (fileName = "Deco Rule", menuName = "Map/Deco Rule", order = 21)]
	public class DecoRule : ScriptableObject {

		#region PUBLIC VARIABLES
		public List<DecoType> Types;
		public List<string> Tags;
		#endregion
	}
	#endregion
}

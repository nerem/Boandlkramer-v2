using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoG.Map {

	#region MAP RULE
	[CreateAssetMenu (fileName = "Map Rule", menuName = "Map/Rule", order = 20)]
	public class MapRule : ScriptableObject {

		#region PUBLIC VARIABLES
		public List<NodeType> Types;
		public List<string> Tags;
		#endregion
	}
	#endregion
}

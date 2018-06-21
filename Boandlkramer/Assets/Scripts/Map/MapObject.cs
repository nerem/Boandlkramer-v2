using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LoG.Math2Int;
using Random = UnityEngine.Random;

namespace LoG.Map {

	#region MAP OBJECT
	[CreateAssetMenu(fileName = "Map Object", menuName = "Map/Object", order = 10)]
	public class MapObject : ScriptableObject {

		#region PUBLIC VARIABLES
		public GameObject Object;
		public List<string> Tags;
		public List<DirectedRule> Rules;
		#endregion

		#region CONSTRUCTOR
		public MapObject () {

		}

		public MapObject (MapObject obj) {

		}
		#endregion

		#region PUBLIC FUNCTIONS
		public void Build () {

		}

		public bool IsValid (Vector pos, Rotation rot, Grid2D<MapTile> grid) {
			foreach (DirectedRule rule in Rules)
				if (!rule.IsValid (pos, rot, grid))
					return false;
			return true;
		}
		#endregion
	}
	#endregion

	#region DIRECTED RULE
	[System.Serializable]
	public class DirectedRule {

		#region PUBLIC VARIABLES
		public Direction Direction;
		public MapRule Rule;
		#endregion

		#region PUBLIC FUNCTIONS
		public bool IsValid (Vector pos, Rotation rot, Grid2D<MapTile> grid) {
			foreach (NodeType type in Rule.Types) {
				List<MapNode> nodes = grid.Get (pos + rot * (Vector)new Rotation (Direction), new MapTile (null, null, Vector.Zero)).Nodes[type];
				if (nodes == null)
					return true;
				foreach (MapNode node in nodes)
					foreach (string tag in Rule.Tags)
						if (node.Object != null && !node.Object.Tags.Contains (tag))
							return false;
			}
			return true;
		}
		#endregion
	}
	#endregion
}

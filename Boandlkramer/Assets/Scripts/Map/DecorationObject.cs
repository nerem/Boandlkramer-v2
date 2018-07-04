using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LoG.Math2Int;
using Random = UnityEngine.Random;

namespace LoG.Map {

	#region MAP OBJECT
	[CreateAssetMenu (fileName = "Decoration Object", menuName = "Map/Deco Object", order = 11)]
	public class DecorationObject : ScriptableObject {

		#region PUBLIC VARIABLES
		public GameObject Object;
		public List<string> Tags;
		public List<ExtendedDirectedRule> Rules;
		public List<ExtendedDirectedDecoRule> DecoRules;
		public Vector3 MinPosition;
		public Vector3 MaxPosition;
		public Vector3 MinRotation;
		public Vector3 MaxRotation;
		#endregion

		#region CONSTRUCTOR

		#endregion

		#region PUBLIC FUNCTIONS
		public void Build () {

		}

		public bool IsValid (Vector pos, Rotation rot, Grid2D<MapTile> grid) {
			foreach (ExtendedDirectedRule rule in Rules)
				if (!rule.IsValid (pos, rot, grid))
					return false;
			foreach (ExtendedDirectedDecoRule rule in DecoRules)
				if (!rule.IsValid (pos, rot, grid))
					return false;
			return true;
		}

		public Vector3 GetPosition () {
			float x = Random.Range (MinPosition.x, MaxPosition.x);
			float y = Random.Range (MinPosition.y, MaxPosition.y);
			float z = Random.Range (MinPosition.z, MaxPosition.z);
			return new Vector3 (x, y, z);
		}

		public Quaternion GetRotation () {
			float x = Random.Range (MinRotation.x, MaxRotation.x);
			float y = Random.Range (MinRotation.y, MaxRotation.y);
			float z = Random.Range (MinRotation.z, MaxRotation.z);
			return Quaternion.Euler (x, y, z);
		}
		#endregion
	}
	#endregion

	#region ENUMS
	public enum ExtendedDirection { Up, Right, Down, Left, None }
	#endregion

	#region DIRECTED RULE
	[System.Serializable]
	public class ExtendedDirectedRule {

		#region PUBLIC VARIABLES
		public ExtendedDirection Direction;
		public MapRule Rule;
		#endregion

		#region PROPERTIES
		public Vector DirectionVector {
			get {
				if (Direction == ExtendedDirection.None)
					return Vector.Zero;
				return (Vector)new Rotation ((int)Direction);
			}
		}
		#endregion

		#region PUBLIC FUNCTIONS
		public bool IsValid (Vector pos, Rotation rot, Grid2D<MapTile> grid) {
			foreach (NodeType type in Rule.Types) {
				List<MapNode> nodes = grid.Get (pos + rot * DirectionVector, new MapTile (null, null, Vector.Zero)).Nodes[type];
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

	#region DIRECTED DECO RULE
	[System.Serializable]
	public class ExtendedDirectedDecoRule {

		#region PUBLIC VARIABLES
		public ExtendedDirection Direction;
		public DecoRule Rule;
		#endregion

		#region PROPERTIES
		public Vector DirectionVector {
			get {
				if (Direction == ExtendedDirection.None)
					return Vector.Zero;
				return (Vector)new Rotation ((int)Direction);
			}
		}
		#endregion

		#region PUBLIC FUNCTIONS
		public bool IsValid (Vector pos, Rotation rot, Grid2D<MapTile> grid) {
			List<MapNode> nodes;
			List<DecorationNode> decos;
			foreach (DecoType type in Rule.Types) {
				nodes = new List<MapNode> ();
				decos = new List<DecorationNode> ();
				foreach (NodeType t in Enum.GetValues (typeof (NodeType)))
					nodes.AddRange (grid.Get (pos + rot * DirectionVector, new MapTile (null, null, Vector.Zero)).Nodes[t]);
				foreach (MapNode node in nodes)
					if (node.Object != null && node.Object.Node != null && node.Object.Node.Type == type)
						decos.Add (node.Object.Node);
					foreach (string tag in Rule.Tags)
						foreach (DecorationNode deco in decos)
							if (deco.Object != null && !deco.Object.Tags.Contains (tag))
								return false;
			}
			return true;
		}
		#endregion
	}
	#endregion
}

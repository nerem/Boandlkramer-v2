using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LoG.Math2Int;
using Random = UnityEngine.Random;

namespace LoG.Map {

	#region MAP THEME
	[CreateAssetMenu (fileName = "Theme", menuName = "Map/Theme", order = 0)]
	public class MapTheme : ScriptableObject {

		#region PUBLIC VARIABLES
		public List<TypedObject> Objects;
		#endregion

		#region PUBLIC FUNCTIONS
		public void Build (Map map, Transform parent) {
			map.Build ();
			foreach (Vector key in map.Grid.Elements.Keys)
				foreach (MapNode node in map.Grid.Elements[key].Nodes.Values.SelectMany (x => x))
					Build (node, map, parent);
		}
		#endregion

		#region PRIVATE FUNCTIONS
		private void Build (MapNode node, Map map, Transform parent) {
			Dictionary<NodeType, List<MapObject>> dict = new Dictionary<NodeType, List<MapObject>> ();
			foreach (NodeType t in Enum.GetValues (typeof (NodeType)))
				dict.Add (t, new List<MapObject> ());
			foreach (TypedObject obj in Objects) {
				if (obj.Object.IsValid (node.Position, node.Rotation, map.Grid))
					dict[obj.Type].Add (obj.Object);
			}
			if (dict[node.Type].Count == 0)
				return;
			Create (dict[node.Type].OrderBy (x => Random.value).First (), node, parent);
		}

		private void Create (MapObject obj, MapNode node, Transform parent, Rotation? rot = null) {
			if (rot == null)
				rot = node.Rotation;
			node.Object = Instantiate (obj);
			GameObject instance = Instantiate (obj.Object, (Vector3)node.Position, (Quaternion)rot);
			if (parent != null)
				instance.transform.SetParent (parent, false);
		}
		#endregion
	}
	#endregion

	#region TYPED OBJECTS
	[System.Serializable]
	public class TypedObject {

		#region PUBLIC VARIABLES
		public NodeType Type;
		public MapObject Object;
		#endregion
	}
	#endregion
}

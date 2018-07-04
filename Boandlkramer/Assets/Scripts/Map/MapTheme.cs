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
		public List<TypedDeco> Decos;
		#endregion

		#region PUBLIC FUNCTIONS
		public void Build (Map map, Transform parent) {
			map.Build ();
			foreach (Vector key in map.Grid.Elements.Keys)
				foreach (MapNode node in map.Grid.Elements[key].Nodes.Values.SelectMany (x => x))
					Build (node, map, parent);
			foreach (Vector key in map.Grid.Elements.Keys)
				foreach (MapNode node in map.Grid.Elements[key].Nodes.Values.SelectMany (x => x))
					if (node.Object != null)
						Build (node.Object.Node, map, node.Object.Object.transform);
		}
		#endregion

		#region PRIVATE FUNCTIONS
		private void Build (MapNode node, Map map, Transform parent) {
			Dictionary<NodeType, List<MapObject>> dict = new Dictionary<NodeType, List<MapObject>> ();
			foreach (NodeType t in Enum.GetValues (typeof (NodeType)))
				dict.Add (t, new List<MapObject> ());
			foreach (TypedObject tobj in Objects) {
				foreach (MapObject obj in tobj.Objects)
					if (obj.IsValid (node.Position, node.Rotation, map.Grid))
						dict[tobj.Type].Add (obj);
			}
			if (dict[node.Type].Count == 0)
				return;
			Create (dict[node.Type].OrderBy (x => Random.value).First (), node, parent);
		}

		private void Build (DecorationNode node, Map map, Transform parent) {
			if (node == null)
				return;
			Dictionary<DecoType, List<DecorationObject>> dict = new Dictionary<DecoType, List<DecorationObject>> ();
			foreach (DecoType t in Enum.GetValues (typeof (DecoType)))
				dict.Add (t, new List<DecorationObject> ());
			foreach (TypedDeco tdeco in Decos) {
				foreach (DecorationObject deco in tdeco.Objects)
					if (deco.IsValid (node.Position, node.Rotation, map.Grid))
						dict[tdeco.Type].Add (deco);
			}
			if (dict[node.Type].Count == 0)
				return;
			Create (dict[node.Type].OrderBy (x => Random.value).First (), node, parent);
		}

		private void Create (MapObject obj, MapNode node, Transform parent, Rotation? rot = null) {
			if (rot == null)
				rot = node.Rotation;
			node.Object = Instantiate (obj);
			if (node.Type == NodeType.Floor)
				node.Object.Node = new DecorationNode (DecoType.FloorDeco, node.Position, node.Rotation, node.Map);
			else if (node.Type == NodeType.Wall)
				node.Object.Node = new DecorationNode (DecoType.WallDeco, node.Position, node.Rotation, node.Map);
			else
				node.Object.Node = new DecorationNode (DecoType.None, node.Position, node.Rotation, node.Map);
			GameObject instance = Instantiate (obj.Object, (Vector3)node.Position, (Quaternion)rot);
			if (parent != null)
				instance.transform.SetParent (parent, false);
			node.Object.Object = instance;
		}

		private void Create (DecorationObject obj, DecorationNode node, Transform parent) {
			node.Object = Instantiate (obj);
			GameObject instance = Instantiate (obj.Object);
			if (parent != null)
				instance.transform.SetParent (parent, false);
			instance.transform.localPosition = obj.GetPosition ();
			instance.transform.localRotation = obj.GetRotation ();
		}
		#endregion
	}
	#endregion

	#region TYPED OBJECTS
	[System.Serializable]
	public class TypedObject {

		#region PUBLIC VARIABLES
		public NodeType Type;
		public List<MapObject> Objects;
		#endregion
	}
	#endregion

	#region TYPED DECO
	[System.Serializable]
	public class TypedDeco {

		#region PUBLIC VARIABLES
		public DecoType Type;
		public List<DecorationObject> Objects;
		#endregion
	}
	#endregion
}

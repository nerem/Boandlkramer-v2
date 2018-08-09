using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DialogueCreator : EditorWindow
{
	List<Rect> windows = new List<Rect>();
	Rect window1;
	Rect window2;

	string myString;

	[MenuItem("Window/Dialogue Creator")]
	static void ShowEditor()
	{
		DialogueCreator editor = EditorWindow.GetWindow<DialogueCreator>();
		editor.Init();
	}

	public void Init()
	{
		window1 = new Rect(10, 10, 200, 100);
		window2 = new Rect(210, 210, 200, 100);
	}

	void OnGUI()
	{
		DrawNodeCurve(window1, window2); // Here the curve is drawn under the windows

		if (Event.current.type == EventType.MouseUp && Event.current.button == 1)
		{
			windows.Add(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 200, 100));
		}

		BeginWindows();
		window1 = GUI.Window(100, window1, DrawNodeWindow, "Window 1");   // Updates the Rect's when these are dragged
		window2 = GUI.Window(200, window2, DrawNodeWindow, "Window 2");
		for (int i=0; i < windows.Count; i++)
		{
			windows[i] = GUI.Window(i, windows[i], DrawNodeWindow, "Window " + i);
		}
		EndWindows();
	}

	void DrawNodeWindow(int id)
	{
		if (GUI.Button(new Rect(20, 60, 80, 20), "Hello World"))
		{
			Debug.Log("Got a click in window " + id);
		}

		GUILayout.Label("Base Settings", EditorStyles.boldLabel);
		myString = EditorGUILayout.TextField("Text Field", myString);

		GUI.DragWindow();
	}


	void DrawNodeCurve(Rect start, Rect end)
	{
		Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
		Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
		Vector3 startTan = startPos + Vector3.right * 50;
		Vector3 endTan = endPos + Vector3.left * 50;
		Color shadowCol = new Color(0, 0, 0, 0.06f);
		for (int i = 0; i < 3; i++) // Draw a shadow
			Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
		Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
	}
}
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[System.Serializable]
public struct Reply
{
	public string reply;
	public Dialogue replyDialogue;
}

[System.Serializable]
public class Dialogue {

	public string name;
	public string text;
	// public string[] requiredTags;
	public List<Reply> replies;
}

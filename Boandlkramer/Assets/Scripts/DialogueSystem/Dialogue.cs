using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[System.Serializable]
public struct Reply
{
    // text of this reply
	public string reply;

    // link to next dialogue block called when this reply is given by the player
	public Dialogue replyDialogue;
}

[System.Serializable]
public class Dialogue {

    // name of the character saying these lines
	public string name;

    // content of this dialogue block
	public string text;

	// public string[] requiredTags;

    // possible replies to this dialogue block
	public List<Reply> replies;

}

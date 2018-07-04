﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ChangeSizeByText : MonoBehaviour {

	RectTransform rt;
	Text txt;

	void Start()
	{
		rt = gameObject.GetComponent<RectTransform>(); // Acessing the RectTransform 
		txt = gameObject.GetComponent<Text>(); // Accessing the text component
	}

	void Update()
	{
		rt.sizeDelta = new Vector2(rt.rect.width, txt.preferredHeight); // Setting the height to equal the height of text
	}
}
	



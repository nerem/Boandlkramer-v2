using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour {

	Color colorStart;
	Color colorEnd;


	void Start()
	{
		//colorStart = renderer.material.color;
		//colorEnd = new Color(colorStart.r, colorStart.g, colorStart.b, 0f);
	}


	public void Fade(float fadeTime)
	{
		StartCoroutine(FadeImage(fadeTime));
	}


	IEnumerator FadeImage(float fadeTime)
	{
		for (float t = 0f; t < fadeTime; t += Time.deltaTime)
		{
			this.gameObject.GetComponent<CanvasRenderer>().SetAlpha(1f - t / fadeTime);
			//renderer.material.color = Color.Lerp(colorStart, colorEnd, t / fadeTime);
			yield return null;
		}
		this.gameObject.SetActive(false);
	}
}


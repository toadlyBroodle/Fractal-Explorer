using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIText : MonoBehaviour {

	public static Text zoom_level_text;

	void Start()
	{
		zoom_level_text = GetComponent<Text>();
	}

}

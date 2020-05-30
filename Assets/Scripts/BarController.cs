using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Mask))]
public class BarController : MonoBehaviour {

	public static BarController Instance { set; get; }

	private GameObject barF;
	private float width;
	public float value;

	private void Awake()
	{
		Instance = this;
	}

	public void UpdateValue(float _value)
	{
		barF = transform.GetChild(0).gameObject;
		width = barF.GetComponent<RectTransform>().rect.width;
		Vector3 pos = Vector3.zero;
		float a = width / 100;
		value = a * _value;
		pos.x = -width + value;
		barF.GetComponent<RectTransform>().anchoredPosition = pos;
	}
}

using System;
using UnityEngine;

public class Easing
{
	public static float EaseInOutQuad(float t, float b, float c, float d) {
		t /= d/2;
		if (t < 1) return c/2*t*t + b;
		t--;
		return -c/2 * (t*(t-2) - 1) + b;
	}
}

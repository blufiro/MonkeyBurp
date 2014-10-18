using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Anim
{
	private float m_timeElapsedSeconds = 0.0f;
	private float m_durationSeconds = 0.0f;
	private string m_onComplete = null;
	private object m_onCompleteParams = null;
	private float m_delaySeconds = 0.0f;
	private GameObject m_target;
	
	public Anim delay(float delaySeconds) {
		this.m_delaySeconds = delaySeconds;
		return this;
	}
	
	public Anim onComplete(string onComplete) {
		this.m_onComplete = onComplete;
		return this;
	}
	public Anim onCompleteParams(object onCompleteParams) {
		this.m_onCompleteParams = onCompleteParams;
		return this;
	}

	public void elapse(float elapsedSeconds) {
		m_timeElapsedSeconds += elapsedSeconds;
		if (isOver()) {
			if (m_onComplete != null && m_target != null) {
				m_target.SendMessage (m_onComplete, m_onCompleteParams, SendMessageOptions.RequireReceiver);
			}
		}
	}
	
	public bool isOver() {
		return m_timeElapsedSeconds >= (m_delaySeconds + m_durationSeconds);
	}

	public Anim (GameObject target)
	{
		this.m_target = target;
	}
}

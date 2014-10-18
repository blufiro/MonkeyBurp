using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Singleton animation master
 */
public class AnimMaster
{
	public List<Anim> anims;
	
	private static Anim createAnim(GameObject gob) {
		Anim anim = new Anim(gob);
		get().anims.Add(anim);
		return anim;
	}
	
	public static Anim delay(GameObject gob, float delaySeconds) {
		return createAnim(gob).delay(delaySeconds);
	}
	
	// Update the animations every frame
	public void update() {
		float deltaTime = Time.deltaTime;
		for (int i = anims.Count-1; i >= 0; i--) {
			Anim anim = anims[i];
			anim.elapse(deltaTime);
			if (anim.isOver()) {
				anims.RemoveAt (i);
			}
		}
	}

	private static AnimMaster m_instance = new AnimMaster();
	public static AnimMaster get() {
		return m_instance;
	}

	private AnimMaster ()
	{
		anims = new List<Anim>();
	}
}


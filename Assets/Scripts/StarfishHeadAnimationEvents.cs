using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StarfishHeadAnimationEvents : MonoBehaviour
{
	public UnityEvent onBigMunch;
	public UnityEvent onBigSlobber;
	public UnityEvent onSmallSlobber;
	public UnityEvent onDrool;
	public UnityEvent onCry;

	public void BigMunch()
	{
		onBigMunch.Invoke();
	}

	public void BigSlobber()
	{
		onBigSlobber.Invoke();
	}

	public void SmallSlobber()
	{
		onSmallSlobber.Invoke();
	}

	public void Drool()
	{
		onDrool.Invoke();
	}

	public void Cry()
	{
		onCry.Invoke();
	}
}

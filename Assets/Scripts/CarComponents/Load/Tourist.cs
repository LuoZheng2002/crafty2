using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tourist : LoadComponent
{
	public override Util.Component Component => Util.Component.Tourist;

	public override void Build()
	{
		Collider c = GetComponent<Collider>();
		RB.useGravity = true;
		c.enabled = true;
	}
}

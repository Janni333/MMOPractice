using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UIWorldElement : MonoBehaviour
{
	public Transform owner;
	private Vector3 offset = new Vector3(0, 2.2f, 0);
	void Update()
	{
		if (this.owner != null)
		{
			this.transform.position = this.owner.position + offset;
		}
		this.transform.forward = Camera.main.transform.forward;
	}
}

using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class FloorMark : Singleton<FloorMark>
{
	[SerializeField]
	private Animator animator;

	public GameObject markSlow;

	public GameObject markFast;

	public GameObject ripple;

	public int nowTag;

	private List<int> hashTag;

	private int hashRipple;

	protected override void Awake()
	{
		if (CheckInstance())
		{
		}
	}

	private void Start()
	{
		if (animator == null)
		{
			animator = GetComponentInParent<Animator>();
		}
		hashTag = new List<int>();
		hashTag.Add(Animator.StringToHash("mark_slow"));
		hashTag.Add(Animator.StringToHash("mark_fast"));
		hashRipple = Animator.StringToHash("ripple");
		(from _ in Observable.Interval(TimeSpan.FromSeconds(0.20000000298023224))
			where hashTag.Contains(nowTag)
			select _).Subscribe(delegate
		{
			UnityEngine.Object.Instantiate((hashTag.FindIndex((int v) => v == nowTag) != 0) ? markFast : markSlow, Vector3.zero, Quaternion.identity);
		}).AddTo(this);
		(from c in this.OnTriggerEnterAsObservable()
			where c.tag == "fxCol"
			select c into _
			where hashRipple == nowTag
			select _).Subscribe(delegate(Collider col)
		{
			UnityEngine.Object.Instantiate(ripple, col.transform.position, Quaternion.identity);
		}).AddTo(this);
	}
}

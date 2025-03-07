using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Cyalume : MonoBehaviour
{
	private class CyalumeInfo
	{
		private float y;

		private Vector3 rot;

		private Vector3 pos;

		public int type { get; private set; }

		public CyalumeInfo(int _type)
		{
			type = _type;
		}

		public void Init(ref ParticleSystem.Particle _particle, Transform _root, int _disMax)
		{
			rot = _particle.rotation3D;
			pos = _particle.position;
			y = pos.y;
			int num = (int)Vector3.Distance(_root.position, _particle.position);
			if (num > _disMax)
			{
				num = _disMax;
			}
			byte a = (byte)Mathf.Lerp(0f, 255f, Mathf.Clamp01(Mathf.InverseLerp(0f, _disMax, num)));
			Color32 startColor = _particle.startColor;
			_particle.startColor = new Color32(startColor.r, startColor.g, startColor.b, a);
		}

		public void Calc(ref ParticleSystem.Particle _particle, List<OffsetInfo> _list)
		{
			OffsetInfo offsetInfo = _list[type];
			rot.x = offsetInfo.rx;
			rot.z = offsetInfo.rz;
			pos.y = y + offsetInfo.py;
			_particle.rotation3D = rot;
			_particle.position = pos;
		}
	}

	private class AnimeInfo
	{
		public float speed { get; private set; }

		public float x { get; private set; }

		public float y { get; private set; }

		public AnimeInfo(float _s, float _x, float _y)
		{
			speed = _s;
			x = _x;
			y = _y;
		}
	}

	private class OffsetInfo
	{
		private float offset;

		public float rx { get; private set; }

		public float rz { get; private set; }

		public float py { get; private set; }

		public OffsetInfo(float _offset)
		{
			offset = _offset;
		}

		public void Calc(float _ts, float _ts3, float _ts5, float _x, float _y)
		{
			float num = offset * 2f;
			float num2 = Mathf.Sin(num + _ts3) * 5f;
			float num3 = Mathf.Cos(num + _ts3) * 15f;
			float num4 = Mathf.Sin(num + _ts5) * 0.1f;
			float num5 = Mathf.Sin(offset + _ts) * 30f;
			rx = num3 * _y;
			rz = num2 * _y + num5 * _x;
			num4 *= _y;
		}
	}

	public GameObject cyalumeParticle;

	private ParticleSystem ps;

	private ParticleSystem.Particle[] p;

	public float offset;

	public int max_Distance;

	public GameObject origin;

	private Animator cutAnim;

	private int offsetNum = 50;

	private List<CyalumeInfo> listCyalumeInfo;

	private Dictionary<int, AnimeInfo> dicAnimeInfo;

	private List<OffsetInfo> listOffsetInfo;

	private void Start()
	{
		cutAnim = GetComponent<Animator>();
		ps = cyalumeParticle.GetComponent<ParticleSystem>();
		p = new ParticleSystem.Particle[ps.main.maxParticles];
		listOffsetInfo = new List<OffsetInfo>();
		for (int i = 0; i < offsetNum; i++)
		{
			listOffsetInfo.Add(new OffsetInfo(Random.Range(0f, offset)));
		}
		int maxParticles = ps.main.maxParticles;
		listCyalumeInfo = new List<CyalumeInfo>();
		for (int j = 0; j < maxParticles; j++)
		{
			listCyalumeInfo.Add(new CyalumeInfo(Random.Range(0, offsetNum)));
		}
		dicAnimeInfo = new Dictionary<int, AnimeInfo>();
		dicAnimeInfo.Add(Animator.StringToHash("normal"), new AnimeInfo(5f, 1f, 0f));
		dicAnimeInfo.Add(Animator.StringToHash("fast"), new AnimeInfo(8f, 1f, 0f));
		dicAnimeInfo.Add(Animator.StringToHash("slow"), new AnimeInfo(2f, 1f, 0f));
		dicAnimeInfo.Add(Animator.StringToHash("vertical"), new AnimeInfo(4f, 0f, 1f));
		this.LateUpdateAsObservable().First().Subscribe(delegate
		{
			int particles = ps.GetParticles(p);
			for (int k = 0; k < particles; k++)
			{
				listCyalumeInfo[k].Init(ref p[k], origin.transform, max_Distance);
			}
			this.UpdateAsObservable().Subscribe(delegate
			{
				AnimatorStateInfo currentAnimatorStateInfo = cutAnim.GetCurrentAnimatorStateInfo(0);
				AnimeInfo animeInfo = dicAnimeInfo[currentAnimatorStateInfo.tagHash];
				float num = Time.time * animeInfo.speed;
				float ts = num * 3f;
				float ts2 = num * 5f;
				foreach (OffsetInfo item in listOffsetInfo)
				{
					item.Calc(num, ts, ts2, animeInfo.x, animeInfo.y);
				}
				int particles2 = ps.GetParticles(p);
				for (int l = 0; l < particles2; l++)
				{
					listCyalumeInfo[l].Calc(ref p[l], listOffsetInfo);
				}
				ps.SetParticles(p, particles2);
			}).AddTo(this);
		})
			.AddTo(this);
	}
}

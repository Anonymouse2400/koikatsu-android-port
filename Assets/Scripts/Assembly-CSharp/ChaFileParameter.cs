using System;
using Localize.Translate;
using MessagePack;
using UnityEngine;

[MessagePackObject(true)]
public class ChaFileParameter
{
	[MessagePackObject(true)]
	public class Awnser
	{
		public bool animal { get; set; }

		public bool eat { get; set; }

		public bool cook { get; set; }

		public bool exercise { get; set; }

		public bool study { get; set; }

		public bool fashionable { get; set; }

		public bool blackCoffee { get; set; }

		public bool spicy { get; set; }

		public bool sweet { get; set; }

		public Awnser()
		{
			MemberInit();
		}

		public void MemberInit()
		{
			animal = false;
			eat = false;
			cook = false;
			exercise = false;
			study = false;
			fashionable = false;
			blackCoffee = false;
			spicy = false;
			sweet = false;
		}

		public void Copy(Awnser src)
		{
			animal = src.animal;
			eat = src.eat;
			cook = src.cook;
			exercise = src.exercise;
			study = src.study;
			fashionable = src.fashionable;
			blackCoffee = src.blackCoffee;
			spicy = src.spicy;
			sweet = src.sweet;
		}
	}

	[MessagePackObject(true)]
	public class Denial
	{
		public bool kiss { get; set; }

		public bool aibu { get; set; }

		public bool anal { get; set; }

		public bool massage { get; set; }

		public bool notCondom { get; set; }

		public Denial()
		{
			MemberInit();
		}

		public void MemberInit()
		{
			kiss = false;
			aibu = false;
			anal = false;
			massage = false;
			notCondom = false;
		}

		public void Copy(Denial src)
		{
			kiss = src.kiss;
			aibu = src.aibu;
			anal = src.anal;
			massage = src.massage;
			notCondom = src.notCondom;
		}
	}

	[MessagePackObject(true)]
	public class Attribute
	{
		public bool hinnyo { get; set; }

		public bool harapeko { get; set; }

		public bool donkan { get; set; }

		public bool choroi { get; set; }

		public bool bitch { get; set; }

		public bool mutturi { get; set; }

		public bool dokusyo { get; set; }

		public bool ongaku { get; set; }

		public bool kappatu { get; set; }

		public bool ukemi { get; set; }

		public bool friendly { get; set; }

		public bool kireizuki { get; set; }

		public bool taida { get; set; }

		public bool sinsyutu { get; set; }

		public bool hitori { get; set; }

		public bool undo { get; set; }

		public bool majime { get; set; }

		public bool likeGirls { get; set; }

		public Attribute()
		{
			MemberInit();
		}

		public void MemberInit()
		{
			hinnyo = false;
			harapeko = false;
			donkan = false;
			choroi = false;
			bitch = false;
			mutturi = false;
			dokusyo = false;
			ongaku = false;
			kappatu = false;
			ukemi = false;
			friendly = false;
			kireizuki = false;
			taida = false;
			sinsyutu = false;
			hitori = false;
			undo = false;
			majime = false;
			likeGirls = false;
		}

		public void Copy(Attribute src)
		{
			hinnyo = src.hinnyo;
			harapeko = src.harapeko;
			donkan = src.donkan;
			choroi = src.choroi;
			bitch = src.bitch;
			mutturi = src.mutturi;
			dokusyo = src.dokusyo;
			ongaku = src.ongaku;
			kappatu = src.kappatu;
			ukemi = src.ukemi;
			friendly = src.friendly;
			kireizuki = src.kireizuki;
			taida = src.taida;
			sinsyutu = src.sinsyutu;
			hitori = src.hitori;
			undo = src.undo;
			majime = src.majime;
			likeGirls = src.likeGirls;
		}
	}

	[IgnoreMember]
	public static readonly string BlockName = "Parameter";

	public Version version { get; set; }

	public byte sex { get; set; }

	public string lastname { get; set; }

	public string firstname { get; set; }

	[IgnoreMember]
	public string fullname
	{
		get
		{
			if (Localize.Translate.Manager.Check(Localize.Translate.Manager.LanguageID.US))
			{
				return firstname + " " + lastname;
			}
			return lastname + " " + firstname;
		}
	}

	public string nickname { get; set; }

	public int callType { get; set; }

	public int personality { get; set; }

	public byte bloodType { get; set; }

	public byte birthMonth { get; set; }

	public byte birthDay { get; set; }

	[IgnoreMember]
	public string strBirthDay
	{
		get
		{
			int type = ((!Localize.Translate.Manager.Check(Localize.Translate.Manager.LanguageID.US)) ? 1 : 0);
			return string.Format(Localize.Translate.Manager.BirthdayText(type) ?? "{0}月{1}日", birthMonth, birthDay);
		}
	}

	public byte clubActivities { get; set; }

	public float voiceRate { get; set; }

	[IgnoreMember]
	public float voicePitch
	{
		get
		{
			return Mathf.Lerp(0.94f, 1.06f, voiceRate);
		}
	}

	public int weakPoint { get; set; }

	public Awnser awnser { get; set; }

	public Denial denial { get; set; }

	public Attribute attribute { get; set; }

	public int aggressive { get; set; }

	public int diligence { get; set; }

	public int kindness { get; set; }

	public ChaFileParameter()
	{
		MemberInit();
	}

	public void MemberInit()
	{
		version = ChaFileDefine.ChaFileParameterVersion;
		sex = 0;
		lastname = string.Empty;
		firstname = string.Empty;
		nickname = string.Empty;
		callType = -1;
		personality = 0;
		bloodType = 0;
		birthMonth = 1;
		birthDay = 1;
		clubActivities = 0;
		voiceRate = 0.5f;
		awnser = new Awnser();
		awnser.MemberInit();
		weakPoint = -1;
		denial = new Denial();
		denial.MemberInit();
		attribute = new Attribute();
		attribute.MemberInit();
		aggressive = 0;
		diligence = 0;
		kindness = 0;
	}

	public void Copy(ChaFileParameter src)
	{
		version = src.version;
		sex = src.sex;
		lastname = src.lastname;
		firstname = src.firstname;
		nickname = src.nickname;
		callType = src.callType;
		personality = src.personality;
		bloodType = src.bloodType;
		birthMonth = src.birthMonth;
		birthDay = src.birthDay;
		clubActivities = src.clubActivities;
		voiceRate = src.voiceRate;
		awnser.Copy(src.awnser);
		weakPoint = src.weakPoint;
		denial.Copy(src.denial);
		attribute.Copy(src.attribute);
		aggressive = src.aggressive;
		diligence = src.diligence;
		kindness = src.kindness;
	}

	public void ComplementWithVersion()
	{
		callType = -1;
		if (version.CompareTo(new Version("0.0.1")) == -1)
		{
			awnser = new Awnser();
			awnser.MemberInit();
		}
		if (version.CompareTo(new Version("0.0.2")) == -1)
		{
			denial = new Denial();
			denial.MemberInit();
		}
		if (version.CompareTo(new Version("0.0.3")) == -1)
		{
			attribute = new Attribute();
			attribute.MemberInit();
		}
		if (version.CompareTo(new Version("0.0.4")) == -1)
		{
			voiceRate = 0.5f;
		}
		version = ChaFileDefine.ChaFileParameterVersion;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using UnityEngine;

namespace ADV
{
	public class ScenarioData : ScriptableObject
	{
		[Serializable]
		public class Param
		{
			[SerializeField]
			private int _hash;

			[SerializeField]
			private int _version;

			[SerializeField]
			private bool _multi;

			[SerializeField]
			private Command _command;

			[SerializeField]
			private string[] _args;

			public int Hash
			{
				get
				{
					return _hash;
				}
			}

			public int Version
			{
				get
				{
					return _version;
				}
			}

			public bool Multi
			{
				get
				{
					return _multi;
				}
			}

			public Command Command
			{
				get
				{
					return _command;
				}
			}

			public string[] Args
			{
				get
				{
					return _args;
				}
			}

			public Param(bool multi, Command command, params string[] args)
			{
				_multi = multi;
				_command = command;
				_args = args;
			}

			public Param(params string[] args)
			{
				Initialize(args);
			}

			public void SetHash(int hash)
			{
				_hash = hash;
			}

			public IEnumerable<string> Output()
			{
				return new string[4]
				{
					_hash.ToString(),
					_version.ToString(),
					_multi.ToString(),
					_command.ToString()
				}.Concat(_args);
			}

			private void Initialize(params string[] args)
			{
				int count = 1;
				bool flag = bool.TryParse(args[count++], out _multi);
				string self = args.SafeGet(count++);
				try
				{
					_command = (Command)Enum.ToObject(typeof(Command), self.Check(true, Enum.GetNames(typeof(Command))));
				}
				catch (Exception)
				{
					throw new Exception("CommandError:" + string.Join(",", args.Select((string s) => (!s.IsNullOrEmpty()) ? s : "(null)").ToArray()));
				}
				if (!flag)
				{
					_multi |= MultiForce(_command);
				}
				_args = ConvertAnalyze(_command, args.Skip(count).ToArray().LastStringEmptySpaceRemove(), null);
			}

			private static string[] ConvertAnalyze(Command command, string[] args, string fileName)
			{
				CommandBase commandBase = CommandList.CommandGet(command);
				if (commandBase != null)
				{
					commandBase.Convert(fileName, ref args);
				}
				return args.LastStringEmptySpaceRemove();
			}
		}

		[SerializeField]
		public List<Param> list = new List<Param>();

		private static bool MultiForce(Command command)
		{
			switch (command)
			{
			case Command.VAR:
			case Command.RandomVar:
			case Command.Calc:
			case Command.Format:
			case Command.Voice:
			case Command.Motion:
			case Command.Expression:
			case Command.ExpressionIcon:
			case Command.FormatVAR:
			case Command.CharaKaraokePlay:
				return true;
			default:
				return false;
			}
		}
	}
}

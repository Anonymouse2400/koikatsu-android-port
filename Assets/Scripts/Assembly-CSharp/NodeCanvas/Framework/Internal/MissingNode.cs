using System;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using ParadoxNotion.Serialization.FullSerializer;

namespace NodeCanvas.Framework.Internal
{
	[Description("Please resolve the MissingNode issue by either replacing the node or importing the missing node type in the project")]
	[DoNotList]
	public sealed class MissingNode : Node, IMissingRecoverable
	{
		[fsProperty]
		public string missingType { get; set; }

		[fsProperty]
		public string recoveryState { get; set; }

		public override string name
		{
			get
			{
				return "<color=#ff6457>* Missing Node *</color>";
			}
		}

		public override Type outConnectionType
		{
			get
			{
				return null;
			}
		}

		public override int maxInConnections
		{
			get
			{
				return 0;
			}
		}

		public override int maxOutConnections
		{
			get
			{
				return 0;
			}
		}

		public override bool allowAsPrime
		{
			get
			{
				return false;
			}
		}

		public override bool showCommentsBottom
		{
			get
			{
				return false;
			}
		}
	}
}

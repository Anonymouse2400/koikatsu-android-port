using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Studio
{
	public class BoneLineCtrl : MonoBehaviour
	{
		[SerializeField]
		private Material material;

		private void Draw(OCIChar _oCIChar)
		{
			if (_oCIChar.charInfo.visibleAll && _oCIChar.oiCharInfo.enableFK)
			{
				List<OCIChar.BoneInfo> listBones = _oCIChar.listBones;
				if (_oCIChar.oiCharInfo.activeFK[0])
				{
					Draw(listBones, 100, 3, Studio.optionSystem.colorFKHair);
					Draw(listBones, 104, 3, Studio.optionSystem.colorFKHair);
					Draw(listBones, 108, 3, Studio.optionSystem.colorFKHair);
					Draw(listBones, 112, 3, Studio.optionSystem.colorFKHair);
					Draw(listBones, 116, 2, Studio.optionSystem.colorFKHair);
					Draw(listBones, 120, 6, Studio.optionSystem.colorFKHair);
					Draw(listBones, 127, 6, Studio.optionSystem.colorFKHair);
					Draw(listBones, 134, 10, Studio.optionSystem.colorFKHair);
					Draw(listBones, 145, 6, Studio.optionSystem.colorFKHair);
					Draw(listBones, 152, 6, Studio.optionSystem.colorFKHair);
					Draw(listBones, 159, 6, Studio.optionSystem.colorFKHair);
					Draw(listBones, 166, 2, Studio.optionSystem.colorFKHair);
					Draw(listBones, 169, 2, Studio.optionSystem.colorFKHair);
					Draw(listBones, 172, 4, Studio.optionSystem.colorFKHair);
					Draw(listBones, 177, 3, Studio.optionSystem.colorFKHair);
					Draw(listBones, 181, 3, Studio.optionSystem.colorFKHair);
					Draw(listBones, 185, 1, Studio.optionSystem.colorFKHair);
					Draw(listBones, 187, 1, Studio.optionSystem.colorFKHair);
					Draw(listBones, 189, 7, Studio.optionSystem.colorFKHair);
					Draw(listBones, 197, 7, Studio.optionSystem.colorFKHair);
					Draw(listBones, 205, 6, Studio.optionSystem.colorFKHair);
					Draw(listBones, 212, 6, Studio.optionSystem.colorFKHair);
				}
				if (_oCIChar.oiCharInfo.activeFK[1])
				{
					Draw(listBones, 1, 1, Studio.optionSystem.colorFKNeck);
				}
				if (_oCIChar.oiCharInfo.activeFK[2])
				{
					Draw(listBones, 53, 4, Studio.optionSystem.colorFKBreast);
					Draw(listBones, 59, 4, Studio.optionSystem.colorFKBreast);
				}
				if (_oCIChar.oiCharInfo.activeFK[3])
				{
					Draw(listBones, 3, 2, Studio.optionSystem.colorFKBody);
					DrawLine(listBones, 5, 6, Studio.optionSystem.colorFKBody);
					Draw(listBones, 6, 3, Studio.optionSystem.colorFKBody);
					DrawLine(listBones, 5, 10, Studio.optionSystem.colorFKBody);
					Draw(listBones, 10, 3, Studio.optionSystem.colorFKBody);
					DrawLine(listBones, 3, 14, Studio.optionSystem.colorFKBody);
					Draw(listBones, 14, 3, Studio.optionSystem.colorFKBody);
					DrawLine(listBones, 3, 18, Studio.optionSystem.colorFKBody);
					Draw(listBones, 18, 3, Studio.optionSystem.colorFKBody);
					DrawLine(listBones, 65, 66, Studio.optionSystem.colorFKBody);
				}
				if (_oCIChar.oiCharInfo.activeFK[4])
				{
					Draw(listBones, 22, 2, Studio.optionSystem.colorFKRightHand);
					Draw(listBones, 25, 2, Studio.optionSystem.colorFKRightHand);
					Draw(listBones, 28, 2, Studio.optionSystem.colorFKRightHand);
					Draw(listBones, 31, 2, Studio.optionSystem.colorFKRightHand);
					Draw(listBones, 34, 2, Studio.optionSystem.colorFKRightHand);
				}
				if (_oCIChar.oiCharInfo.activeFK[5])
				{
					Draw(listBones, 37, 2, Studio.optionSystem.colorFKLeftHand);
					Draw(listBones, 40, 2, Studio.optionSystem.colorFKLeftHand);
					Draw(listBones, 43, 2, Studio.optionSystem.colorFKLeftHand);
					Draw(listBones, 46, 2, Studio.optionSystem.colorFKLeftHand);
					Draw(listBones, 49, 2, Studio.optionSystem.colorFKLeftHand);
				}
				if (_oCIChar.oiCharInfo.activeFK[6])
				{
					Draw(listBones, 219, 5, Studio.optionSystem.colorFKSkirt);
					Draw(listBones, 225, 5, Studio.optionSystem.colorFKSkirt);
					Draw(listBones, 231, 5, Studio.optionSystem.colorFKSkirt);
					Draw(listBones, 237, 5, Studio.optionSystem.colorFKSkirt);
					Draw(listBones, 243, 5, Studio.optionSystem.colorFKSkirt);
					Draw(listBones, 249, 5, Studio.optionSystem.colorFKSkirt);
					Draw(listBones, 255, 5, Studio.optionSystem.colorFKSkirt);
					Draw(listBones, 261, 5, Studio.optionSystem.colorFKSkirt);
				}
			}
		}

		private void Draw(List<OCIChar.BoneInfo> _bones, int _start, int _num, Color _color)
		{
			for (int i = 0; i < _num; i++)
			{
				DrawLine(_bones, _start + i, _start + i + 1, _color);
			}
		}

		private void DrawLine(List<OCIChar.BoneInfo> _bones, int _start, int _end, Color _color)
		{
			OCIChar.BoneInfo boneInfo = _bones.Find((OCIChar.BoneInfo v) => v.boneID == _start);
			if (boneInfo != null)
			{
				OCIChar.BoneInfo boneInfo2 = _bones.Find((OCIChar.BoneInfo v) => v.boneID == _end);
				if (boneInfo2 != null)
				{
					DrawLine(boneInfo.posision, boneInfo2.posision, _color);
				}
			}
		}

		private void DrawLine(Vector3 _s, Vector3 _e, Color _color)
		{
			GL.Color(_color);
			GL.Vertex(_s);
			GL.Vertex(_e);
		}

		private void OnPostRender()
		{
			if (!Studio.optionSystem.lineFK)
			{
				return;
			}
			IEnumerable<OCIChar> enumerable = from v in Studio.GetSelectObjectCtrl()
				where v.kind == 0
				select v as OCIChar;
			material.SetPass(0);
			GL.PushMatrix();
			GL.Begin(1);
			foreach (OCIChar item in enumerable)
			{
				Draw(item);
			}
			GL.End();
			GL.PopMatrix();
		}
	}
}

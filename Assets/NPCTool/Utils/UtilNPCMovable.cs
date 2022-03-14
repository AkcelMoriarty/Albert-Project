using UnityEngine;

/* util that provide the necessary functions for the npc movable  */

namespace EdgarDev.NPCTool.Utils
{
	public class UtilNPCMovable : MonoBehaviour
	{
		public static bool IsMultiPathpoint(Transform tr)
		{
			// check if it is a multi pathpoint regarding the tag
			return tr.CompareTag(UtilNPC.HIERARCHY_STR_MULTIPATHPOINT);
		}
	}
}

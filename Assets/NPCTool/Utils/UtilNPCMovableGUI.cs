using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/* util that provide the necessary GUI functions for the npc movable */

namespace EdgarDev.NPCTool.Utils
{
	public class UtilNPCMovableGUI
	{
		// Draw button only if selection is a multi pathpoint or a child of it
		public static void HandleButtonSelectMultiPathpointChild()
		{
			// get selection
			// return if have not parent
			Transform selection = Selection.activeTransform;

			// check if it is a multi pathpoint
			bool IsMultiPathpoint = UtilNPCMovable.IsMultiPathpoint(selection);

			// check if it has no parent
			if (selection.parent == null)
			{
				// draw disabled button select multi pathpoint child 
				EditorGUI.BeginDisabledGroup(true);
				GUILayout.Button(UtilNPC.BUTTON_STR_SELECT_MULTI_CHILD_PATHPOINT, UtilEditor.BUTTON_FIXED_BIG_HEIGHT);
				EditorGUI.EndDisabledGroup();
				return;
			}

			// check if it is a multi pathpoint child
			bool IsMultiPathpointChild = UtilNPCMovable.IsMultiPathpoint(selection.parent);

			// check if it is not any kind of gameobject related to the multi pathpoint
			if (!IsMultiPathpointChild && !IsMultiPathpoint)
			{
				// draw disabled button select multi pathpoint child 
				EditorGUI.BeginDisabledGroup(true);
				GUILayout.Button(UtilNPC.BUTTON_STR_SELECT_MULTI_CHILD_PATHPOINT, UtilEditor.BUTTON_FIXED_BIG_HEIGHT);
				EditorGUI.EndDisabledGroup();
				return;
			}

			// check if it is a multipathpoint
			if (IsMultiPathpoint)
			{
				// draw button select multi pathpoint child 
				if (GUILayout.Button(UtilNPC.BUTTON_STR_SELECT_MULTI_CHILD_PATHPOINT, UtilEditor.BUTTON_FIXED_BIG_HEIGHT))
				{
					SelectMultiPathpointChild(selection);
				}
			}
			else if (UtilNPCMovable.IsMultiPathpoint(selection.parent))
			{
				// draw button select multi pathpoint child 
				if (GUILayout.Button(UtilNPC.BUTTON_STR_SELECT_MULTI_CHILD_PATHPOINT, UtilEditor.BUTTON_FIXED_BIG_HEIGHT))
				{
					SelectMultiPathpointChild(selection.parent, selection);
				}
			}
		}

		public static void SelectMultiPathpointChild(Transform parent, Transform child = null)
		{
			if (child == null)
			{
				// select first child of multi pathpoint
				Selection.activeGameObject = parent.GetChild(0).gameObject;
				return;
			}

			int currentIndex = child.GetSiblingIndex();
			int newIndex = currentIndex + 1;

			// check if it is the last index
			// if true new selection will be first child
			if (currentIndex == parent.childCount - 1) newIndex = 0;

			// select pathpoint child of multi pathpoint
			Selection.activeGameObject = parent.GetChild(newIndex).gameObject;
		}
	}
}


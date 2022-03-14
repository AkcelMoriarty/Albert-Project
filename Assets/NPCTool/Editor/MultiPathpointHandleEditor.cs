using EdgarDev.NPCTool.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace EdgarDev.NPCTool
{
	[CustomEditor(typeof(MultiPathpointHandle))]
	public class MultiPathpointHandleEditor : Editor
	{
		private NPCMovable _NPCMovable;

		private MultiPathpointHandle _InspectedMultiPathpoint;
		private Tool m_LastTool = Tool.None;

		private void OnEnable()
		{
			_InspectedMultiPathpoint = (MultiPathpointHandle)target;

			m_LastTool = Tools.current;
			Tools.current = Tool.None;

			SceneView.duringSceneGui += OnSceneGUI;
		}

		private void OnDisable()
		{
			Tools.current = m_LastTool;

			SceneView.duringSceneGui -= OnSceneGUI;
		}

		private void OnSceneGUI(SceneView sceneView)
		{
			// check if npc window editor is open
			// should return if it is closed
			if (!EditorWindow.HasOpenInstances<NPCWindowEditor>()) return;

			if (_InspectedMultiPathpoint == null) return;

			// get inspected pathpoint position
			Vector3 pos = _InspectedMultiPathpoint.transform.position;

			DrawMultiPathpointGUI(pos);

			// create handle position
			Handles.zTest = CompareFunction.Always;
			pos = Handles.PositionHandle(pos, Quaternion.identity);
			Handles.zTest = CompareFunction.LessEqual;

			if (GUI.changed || _InspectedMultiPathpoint.transform.hasChanged)
			{
				// for undo operation
				Undo.RecordObject(target, UtilNPC.UNDO_STR_MOVEPATHPOINT);

				// apply changes back to inspected pathpoint
				_InspectedMultiPathpoint.transform.position = pos;

				// apply gravity to inspected pathpoint
				ApplyGravity();

				_InspectedMultiPathpoint.transform.hasChanged = false;
			}
		}

		private void DrawMultiPathpointGUI(Vector3 pos)
		{
			Handles.zTest = CompareFunction.Less;

			Handles.color = UtilNPC.LINE_COLOR_MULTI_PATHPOINT;
			// loop into each pathpoint on multi pathpoint parent and draw intersections
			foreach (Transform pathpoint in _InspectedMultiPathpoint.transform)
			{
				Handles.DrawLine(pos, pathpoint.position, UtilNPC.LINE_SIZE_LINEAR);
			}

			Handles.color = Color.white;
		}

		private void ApplyGravity()
		{
			float maxY = UtilNPC.MAP_MAX_Y;
			float maxDistance = UtilNPC.MAP_MAX_Y - UtilNPC.MAP_MIN_Y;

			// get inspected pathpoint position
			// and set its Y value to skybox value

			// THIS IS NOT UPDATE CORRECTLY, HEIGHT IS SET ONLY THE FIRST TIME YOU HANDLE THE POSITION HANDLE
			Vector3 pos = _InspectedMultiPathpoint.transform.position;
			pos.y += 2;


			RaycastHit hitRoof;
			if (Physics.Raycast(pos, Vector3.up, out hitRoof, maxDistance))
			{
				pos.y = hitRoof.point.y;
			}
			else
			{
				pos.y = maxY;
			}

			// draw a raycast down and set the raycasthit value
			// set inspected pathpoint position to the hit position
			RaycastHit hit;
			if (Physics.Raycast(pos, Vector3.down, out hit, maxDistance))
			{
				_InspectedMultiPathpoint.transform.position = hit.point;
			}
		}
	}
}
using EdgarDev.NPCTool.Utils;
using UnityEditor;
using UnityEngine;

namespace EdgarDev.NPCTool
{
	[CustomEditor(typeof(NPCMovable))]
	public class NPCMovableEditor : Editor
	{
		private SerializedObject _so;
		private SerializedProperty _propPathpoints;
		private SerializedProperty _propMove;

		private Transform _PathpointParent;

		private void OnEnable()
		{
			_so = serializedObject;
			_propPathpoints = _so.FindProperty("m_Pathpoints");
			_propMove = _so.FindProperty("m_Move");
		}

		private void OnDisable()
		{

		}

		public override void OnInspectorGUI()
		{
			_so.Update();

			EditorGUILayout.PropertyField(_propMove);

			_so.ApplyModifiedProperties();
		}

		public void OnNPCMovableInspectorGUI(NPCMovable _NPCMovable)
		{
			if (_so.targetObject == null)
			{
				Debug.LogWarning("FATAL ERROR : _so.targetObject is null");
				return;
			}

			if (_propPathpoints.arraySize == 0)
			{
				HandlePullPathpointsData(_NPCMovable);
			}

			_so.Update();

			EditorGUILayout.PropertyField(_propMove);
			DrawPathpointsGUI(_NPCMovable);

			_so.ApplyModifiedProperties();
		}

		private void HandlePullPathpointsData(NPCMovable _NPCMovable)
		{
			if (_propPathpoints.arraySize == 0)
			{
				Transform tr = _NPCMovable.transform.Find(UtilNPC.HIERARCHY_STR_PARENT_PATHPOINT);
				if (tr != null)
				{
					_so.Update();

					foreach (Transform pathpoint in tr)
					{
						// create new element in array and set its value to new created pathpoint
						int index = _propPathpoints.arraySize;
						_propPathpoints.InsertArrayElementAtIndex(index);
						_propPathpoints.GetArrayElementAtIndex(index).objectReferenceValue = pathpoint;

					}

					Debug.Log("Pulled data of " + _NPCMovable.transform);

					_so.ApplyModifiedProperties();
				}
			}
		}

		private void DrawPathpointsGUI(NPCMovable _NPCMovable)
		{
			/*		EditorGUI.BeginDisabledGroup(true);
					EditorGUILayout.PropertyField(_propPathpoints);
					EditorGUI.EndDisabledGroup();*/

			using (new GUILayout.VerticalScope(EditorStyles.helpBox))
			{
				EditorGUILayout.LabelField(UtilNPC.LABEL_STR_PATHPOINTS + "(" + _propPathpoints.arraySize + ")", EditorStyles.boldLabel);

				// draw pathpoint tools
				using (new GUILayout.HorizontalScope())
				{
					// draw add pathpoint button
					// draw disabled delete button if empty
					// otherwise draw delete button
					if (GUILayout.Button(UtilNPC.BUTTON_STR_ADD_STATIC_PATHPOINT, UtilEditor.BUTTON_FIXED_BIG_HEIGHT))
					{
						// add pathpoint to array and in hierarchy
						AddPathpoint(_NPCMovable, _PathpointParent);
					}
					else
					{
						if (GetPathpointsArraySize() == 0)
						{
							EditorGUI.BeginDisabledGroup(true);
							GUILayout.Button(UtilNPC.BUTTON_STR_DELETE_LAST_PATHPOINT, UtilEditor.BUTTON_FIXED_BIG_HEIGHT);
							EditorGUI.EndDisabledGroup();
						}
						else
						{
							Transform lastPathpoint = GetLastPathpointValue();

							if (lastPathpoint != null)
							{
								// draw delete last pathpoint button
								if (GUILayout.Button(UtilNPC.BUTTON_STR_DELETE_LAST_PATHPOINT + "\n(" + lastPathpoint.name + ")", UtilEditor.BUTTON_FIXED_BIG_HEIGHT))
								{
									// remove last pathpoint to array and in hierarchy
									// try select last
									DeleteLastPathpoint();
									TrySelectLastPathpoint();
								}
							}
						}
					}
				}

				using (new GUILayout.HorizontalScope())
				{
					// draw add multi pathpoint button
					if (GUILayout.Button(UtilNPC.BUTTON_STR_ADD_MULTI_PATHPOINT, UtilEditor.BUTTON_FIXED_BIG_HEIGHT))
					{
						// add multi pathpoint to array and in hierarchy
						AddMultiPathpoint(_NPCMovable);
					}

					// draw the button select multi pathpoint child if necessary
					UtilNPCMovableGUI.HandleButtonSelectMultiPathpointChild();
				}

				using (new GUILayout.HorizontalScope())
				{
					Transform selection = null;
					int currentIndex = 0;

					if (GUILayout.Button(UtilIcon.ICON_FIRST, UtilEditor.BUTTON_FIXED_MID_HEIGHT))
					{
						TrySelectPathpointAtIndex(0);
					}
					if (GUILayout.Button(UtilIcon.ICON_PREVIOUS, UtilEditor.BUTTON_FIXED_MID_HEIGHT))
					{
						// get child index of current selection
						selection = Selection.activeTransform;
						if (selection.parent != _PathpointParent) TrySelectPathpointAtIndex(0);

						currentIndex = GetIndexOfPathpoint(selection.transform);
						// cannot found this pathpoint in array
						if (currentIndex == -1) TrySelectPathpointAtIndex(0);

						// try select previous pathpoint
						TrySelectPathpointAtIndex(currentIndex - 1);
					}
					if (GUILayout.Button(UtilIcon.ICON_NEXT, UtilEditor.BUTTON_FIXED_MID_HEIGHT))
					{
						// get child index of current selection
						selection = Selection.activeTransform;
						if (selection.parent != _PathpointParent) TrySelectLastPathpoint();

						currentIndex = GetIndexOfPathpoint(selection.transform);
						// cannot found this pathpoint in array
						if (currentIndex == -1) TrySelectPathpointAtIndex(0);

						// try select next pathpoint
						TrySelectPathpointAtIndex(currentIndex + 1);
					}
					if (GUILayout.Button(UtilIcon.ICON_LAST, UtilEditor.BUTTON_FIXED_MID_HEIGHT))
					{
						TrySelectLastPathpoint();
					}
				}
			}
		}

		private void AddPathpoint(NPCMovable _NPCMovable, Transform _Parent, bool _AddToArray = true)
		{
			// create new pathpoint and set its name
			// set its position
			GameObject newPathpoint = new GameObject(UtilNPC.HIERARCHY_STR_PATHPOINT + UtilNPC.HIERARCHY_STR_SEPARATOR + GetPathpointsArraySize(), typeof(PathpointHandle));
			SetNewPathpointPosition(_NPCMovable, newPathpoint);
			newPathpoint.tag = UtilNPC.HIERARCHY_STR_PATHPOINT;

			if (_Parent == null)
			{
				// try get pathpoint parent
				// if exists set it as parent of new pathpoint
				_PathpointParent = TryGetPathpointsParent(_NPCMovable);
				newPathpoint.transform.SetParent(_PathpointParent);
			}
			else
			{
				// usually used for multipathpoint
				// set arg parent as parent of new pathpoint
				newPathpoint.transform.SetParent(_Parent);
			}

			if (_AddToArray)
			{
				// create new element in array and set its value to new created pathpoint
				int index = _propPathpoints.arraySize;
				_propPathpoints.InsertArrayElementAtIndex(index);
				_propPathpoints.GetArrayElementAtIndex(index).objectReferenceValue = newPathpoint.transform;
			}

			// record new path point
			Undo.RegisterCreatedObjectUndo(newPathpoint, UtilNPC.UNDO_STR_ADDPATHPOINT);

			// unlock inspector to avoid pathpoint handle bug
			ActiveEditorTracker.sharedTracker.isLocked = false;

			// select new created pathpoint
			Selection.activeGameObject = newPathpoint;
		}

		private void SetNewPathpointPosition(NPCMovable _NPCMovable, GameObject newPathpoint)
		{
			// get last path point transform
			// set new path point to last path point position
			Transform lastPathpoint = GetLastPathpointValue();
			newPathpoint.transform.position = lastPathpoint == null ? _NPCMovable.transform.position + _NPCMovable.transform.forward : lastPathpoint.position + Vector3.up;
		}

		private void AddMultiPathpoint(NPCMovable _NPCMovable)
		{
			// create new multi pathpoint and set its name
			// set its position
			GameObject newMultiPathpoint = new GameObject(UtilNPC.HIERARCHY_STR_MULTIPATHPOINT + UtilNPC.HIERARCHY_STR_SEPARATOR + GetPathpointsArraySize(), typeof(MultiPathpointHandle));
			SetNewPathpointPosition(_NPCMovable, newMultiPathpoint);
			newMultiPathpoint.tag = UtilNPC.HIERARCHY_STR_MULTIPATHPOINT;

			/*newPathpoint.hideFlags = HideFlags.HideInHierarchy;*/

			AddPathpoint(_NPCMovable, newMultiPathpoint.transform, false);
			AddPathpoint(_NPCMovable, newMultiPathpoint.transform, false);

			// try get pathpoint parent
			// if exists set it as parent of new multipathpoint
			_PathpointParent = TryGetPathpointsParent(_NPCMovable);
			newMultiPathpoint.transform.SetParent(_PathpointParent);

			// create new element in array and set its value to new created pathpoint
			int index = _propPathpoints.arraySize;
			_propPathpoints.InsertArrayElementAtIndex(index);
			_propPathpoints.GetArrayElementAtIndex(index).objectReferenceValue = newMultiPathpoint.transform;

			// record new path point
			Undo.RegisterCreatedObjectUndo(newMultiPathpoint, UtilNPC.UNDO_STR_ADDMULTIPATHPOINT);

			// unlock inspector to avoid pathpoint handle bug
			ActiveEditorTracker.sharedTracker.isLocked = false;

			// select new created path point
			Selection.activeGameObject = newMultiPathpoint;
		}

		private Transform GetPathpointValueAtIndex(int index)
		{
			// return index out of bounds
			if (index < 0) return null;

			int arraySize = GetPathpointsArraySize();

			// return null if array is empty
			if (arraySize == 0) return null;

			// return index out of bounds
			if (index > arraySize - 1) return null;

			// find the value of last element in array
			// the array must have atleast 1 element
			return _propPathpoints.GetArrayElementAtIndex(index).objectReferenceValue as Transform;
		}

		// use that function to get the array size of index with value
		// otherwise it can create bug because of actualisation time of DeletePathpointAtIndex function
		private int GetPathpointsArraySize()
		{
			Transform tr;
			int size = 0;

			for (int i = 0; i < _propPathpoints.arraySize; i++)
			{
				tr = _propPathpoints.GetArrayElementAtIndex(i).objectReferenceValue as Transform;
				if (tr != null) size++;
			}

			return size;
		}

		private int GetIndexOfPathpoint(Transform pathpoint)
		{
			Transform tr = null;
			int index = -1;

			for (int i = 0; i < _propPathpoints.arraySize; i++)
			{
				tr = _propPathpoints.GetArrayElementAtIndex(i).objectReferenceValue as Transform;
				if (tr == pathpoint)
				{
					index = i;
				}
			}

			return index;
		}

		private Transform GetLastPathpointValue()
		{
			return GetPathpointValueAtIndex(_propPathpoints.arraySize - 1);
		}

		private void TrySelectPathpointAtIndex(int index)
		{
			// try get last pathpoint
			Transform pathPoint = GetPathpointValueAtIndex(index);
			if (pathPoint != null)
			{
				// select new last pathpoint
				Selection.activeGameObject = pathPoint.gameObject;
			}
		}

		private void TrySelectLastPathpoint()
		{
			// try get last pathpoint
			Transform lastPathpoint = GetLastPathpointValue();
			if (lastPathpoint != null)
			{
				// select new last pathpoint
				Selection.activeGameObject = lastPathpoint.gameObject;
			}
		}

		private void TrySelectMultiPathpointChild(Transform multiPathpoint, int index)
		{
			// select new last pathpoint
			Selection.activeGameObject = multiPathpoint.GetChild(index).gameObject;
		}

		public void DeletePathpointAtIndex(int index)
		{
			// out of range
			if (index < 0 || index >= _propPathpoints.arraySize)
				return;

			_so.Update();

			// get the value of element at index in array
			Transform pathPoint = GetPathpointValueAtIndex(index);

			// record and destroy element's value in scene if found 
			// destroy element on pathpoints property
			if (pathPoint != null) Undo.DestroyObjectImmediate(pathPoint.gameObject);
			_propPathpoints.DeleteArrayElementAtIndex(index);

			_so.ApplyModifiedProperties();
		}

		private void DeletePathpointDynamic(Transform pathPoint)
		{
			int index = GetIndexOfPathpoint(pathPoint);

			// cannot found this pathpoint in array
			if (index == -1) return;

			DeletePathpointAtIndex(index);

			Debug.Log("deleted path point at index " + index);
		}

		private void DeleteLastPathpoint()
		{
			DeletePathpointAtIndex(_propPathpoints.arraySize - 1);
		}

		private Transform TryGetPathpointsParent(NPCMovable _NPCMovable)
		{
			if (_PathpointParent != null) return _PathpointParent;

			// look if a parent already exists
			Transform tr = _NPCMovable.transform.Find(UtilNPC.HIERARCHY_STR_PARENT_PATHPOINT);
			if (tr == null)
			{
				tr = CreatePathpointsParent(_NPCMovable);
			}

			return tr;
		}

		private Transform CreatePathpointsParent(NPCMovable _NPCMovable)
		{
			Transform tr;
			// parent has not been found, create it
			GameObject tmp = new GameObject(UtilNPC.HIERARCHY_STR_PARENT_PATHPOINT);
			tr = tmp.transform;
			tr.SetParent(_NPCMovable.transform);
			return tr;
		}
	}
}
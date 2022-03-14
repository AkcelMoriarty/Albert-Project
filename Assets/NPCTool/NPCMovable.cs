using EdgarDev.NPCTool.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EdgarDev.NPCTool
{
	[System.Serializable]
	public class NPCMoveEvent : UnityEvent<Vector2>
	{
	}

	public class NPCMovable : NPC
	{
		private enum NPCMovableState { Idle, Moving }

		[Header("Movement")]
		[Tooltip("Move speed of the character in m/s")]
		public float m_MoveSpeed = 2.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float m_SprintSpeed = 5.335f;
		[Tooltip("How fast the character turns to face movement direction")]
		[Range(0.0f, 0.3f)]
		public float m_RotationSmoothTime = 0.12f;
		[Tooltip("Acceleration and deceleration")]
		public float m_SpeedChangeRate = 10.0f;
		[Tooltip("Where you want your character to go")]
		public Transform[] m_Pathpoints = new Transform[0];

		public NPCMoveEvent m_Move;

		private CharacterController m_Controller;

		private NPCMovableState m_State;
		private List<Vector3> m_PathList;
		private Vector3 m_TargetPosition;
		private int m_MovePointIndex;

		public override void Start()
		{
			base.Start();
			m_Controller = GetComponent<CharacterController>();

			InitPathList();
			m_State = NPCMovableState.Moving;
		}

		private void InitPathList()
		{
			// store path points values in a list
			m_PathList = new List<Vector3>();
			foreach (Transform point in m_Pathpoints)
			{
				if (point.childCount == 0)
				{
					m_PathList.Add(point.position);
				}
				else
				{
					int rand = Random.Range(0, point.childCount);
					m_PathList.Add(point.GetChild(rand).position);
				}
			}


			// set index and first target
			m_MovePointIndex = 0;
			m_TargetPosition = m_PathList[m_MovePointIndex];
		}

		private void Update()
		{
			switch (m_State)
			{
				// represent the actions of character on Idle state
				case NPCMovableState.Idle:
					m_Move.Invoke(Vector2.zero);
					break;
				// represent the actions of character on Moving state
				case NPCMovableState.Moving:
					Vector2 move = new Vector2(m_TargetPosition.x - transform.position.x, m_TargetPosition.z - transform.position.z);
					m_Move.Invoke(move);
					CheckPosition();
					break;
			}
		}

		public void CheckPosition()
		{
			float distance = Vector3.Distance(transform.position, m_TargetPosition);
			if (distance < 0.1f)
			{
				TargetNextPathpoint();
			}
		}

		private void TargetNextPathpoint()
		{
			if (m_MovePointIndex == m_PathList.Count - 1)
			{
				// finished the path
				// switch state to idle state
				m_TargetPosition = transform.position;
				m_State = NPCMovableState.Idle;
			}
			else
			{
				// increment and set new target position
				m_MovePointIndex++;
				m_TargetPosition = m_PathList[m_MovePointIndex];
			}
		}

		public void Sort()
		{
			Debug.Log("Sort");

			Transform trParent = transform.Find(UtilNPC.HIERARCHY_STR_PARENT_PATHPOINT);
			Transform tr;

			for (int i = 0; i < m_Pathpoints.Length; i++)
			{
				tr = m_Pathpoints[i];
				if (tr == null) continue;

				tr.SetParent(null);
				tr.SetParent(trParent);
			}
		}
	}
}
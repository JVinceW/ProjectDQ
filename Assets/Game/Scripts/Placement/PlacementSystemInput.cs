using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Game.Placement
{
	/// <summary>
	/// The input system for the placement system.
	/// To separate the actual placement system logic from the input and query logic.
	/// </summary>
	public class PlacementSystemInput : MonoBehaviour
	{
		[SerializeField]
		private Camera _sceneCamera;

		[SerializeField]
		private LayerMask _layerMask;

		[SerializeField]
		private float _detectionMaxDistance = 100f;

		[Header("Input Setup")]
		[Space(10)]
		[SerializeField]
		private InputAction _mousePosition;

		[SerializeField]
		private InputAction _mouseClickLeft;

		[SerializeField]
		private InputAction _mouseClickRight;

		[SerializeField]
		private InputAction _exitPlacementMode;

		public event Action OnLeftClicked, OnRightClicked, OnExitPlacementModeClicked;

		private Vector3 _lastPosition;

		public bool IsPointerUI() => EventSystem.current.IsPointerOverGameObject();

		#region Lifecycle
		private void OnEnable()
		{
			_mousePosition.Enable();
			_mouseClickLeft.Enable();
			_mouseClickRight.Enable();
			_exitPlacementMode.Enable();
			_mouseClickLeft.performed += OnMouseClick;
			_mouseClickRight.performed += OnMouseClickRight;
			_exitPlacementMode.performed += OnExitPlacementMode;
		}

		private void OnDisable()
		{
			_mousePosition.Disable();
			_mouseClickLeft.Disable();
			_mouseClickRight.Disable();
			_exitPlacementMode.Disable();
			_mouseClickLeft.performed -= OnMouseClick;
			_mouseClickRight.performed -= OnMouseClickRight;
			_exitPlacementMode.performed -= OnExitPlacementMode;
		}
		#endregion Lifecycle

		#region Methods
		// ReSharper disable once MemberCanBePrivate.Global
		public void GetHitObjectInfoOnMousePosition(out RaycastHit[] hits, out int hitCount)
		{
			Vector3 mousePosition = _mousePosition.ReadValue<Vector2>();
			mousePosition.z = _sceneCamera.nearClipPlane;
			var ray = _sceneCamera.ScreenPointToRay(mousePosition);
			hits = new RaycastHit[10];
			hitCount = Physics.RaycastNonAlloc(ray, hits, _detectionMaxDistance, _layerMask);
		}

		// ReSharper disable once UnusedMember.Global
		public bool IsNearestHitOtherPlacementObject(in RaycastHit[] hits, out RaycastHit nearestHit)
		{
			nearestHit = new RaycastHit();
			var nearestHitOrder = hits.Where(x => x.distance > 0)
				.OrderBy(x => x.distance)
				.ToArray();
			if (nearestHitOrder.Length == 0)
			{
				return false;
			}

			nearestHit = nearestHitOrder[0];
			var isHitOtherPlacementObject =
				nearestHit.transform.root.transform.CompareTag(Constant.PLACEMENT_OBJECT_TAG);
			return isHitOtherPlacementObject;
		}

		public Vector3 GetPlaceableSurfaceMapPosition()
		{
			GetHitObjectInfoOnMousePosition(out var hits, out var hitCount);
			if (hitCount <= 0)
			{
				return _lastPosition;
			}

			var groundHit = hits[0];
			_lastPosition = groundHit.point;
			return _lastPosition;
		}
		#endregion Methods

		#region Callbacks
		private void OnExitPlacementMode(InputAction.CallbackContext obj)
		{
			Debug.Log("Exit placement mode");
			OnExitPlacementModeClicked?.Invoke();
		}

		private void OnMouseClickRight(InputAction.CallbackContext ctx)
		{
			OnRightClicked?.Invoke();
		}

		private void OnMouseClick(InputAction.CallbackContext ctx)
		{
			OnLeftClicked?.Invoke();
		}
		#endregion Callbacks
	}
}
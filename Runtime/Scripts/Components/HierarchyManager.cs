#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
#endif
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnnamedTeam.UnamedParkGolf.General.Components
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public sealed class HierarchyManager : MonoBehaviour
	{
#if UNITY_EDITOR
		// - Class Struct Enum -
		[Serializable]
		public sealed class HierarchyExpansionController
		{
			// - Statics -
			const double AutoExpandInterval = 0.2d;

			// - Public Methods -
			public void Initialize(HierarchyManager owner) {
				_owner = owner;
			}
			public void Release() {
				_owner = null;
			}
			public void Tick(double currentTime) {
				if (!_autoExpand || _owner == null || currentTime < _nextAutoExpandTime) {
					return;
				}

				_nextAutoExpandTime = currentTime + AutoExpandInterval;
				if (!IsTargetValid()) {
					return;
				}
				ApplyHierarchyState(false);
			}

			// - Internals -
			[SerializeField] Transform _targetChild;
			[SerializeField, LabelText("자동 적용")] bool _autoExpand;
			[NonSerialized] HierarchyManager _owner;
			double _nextAutoExpandTime;

			[Button("지정 자식만 펼치기")]
			void ExpandOnlyTargetChild() {
				ApplyHierarchyState(true);
			}

			bool IsTargetValid() {
				return _owner != null && _targetChild != null && _targetChild != _owner.transform && _targetChild.IsChildOf(_owner.transform);
			}

			void ApplyHierarchyState(bool selectTarget) {
				if (!IsTargetValid()) {
					Debug.LogError("지정한 오브젝트는 HierarchyManager가 붙은 오브젝트의 자식이어야 합니다.", _owner);
					return;
				}

				Type hierarchyWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
				MethodInfo setExpandedRecursiveMethod = hierarchyWindowType?.GetMethod("SetExpandedRecursive", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				MethodInfo setExpandedMethod = hierarchyWindowType?.GetMethod("SetExpanded", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(EntityId), typeof(bool) }, null);
				if (hierarchyWindowType == null || setExpandedRecursiveMethod == null || setExpandedMethod == null) {
					Debug.LogError("Hierarchy 창의 접힘 상태를 변경하는 기능을 찾지 못했습니다.", _owner);
					return;
				}

				List<Transform> targetPath = new List<Transform>();
				Transform currentTransform = _targetChild;
				while (currentTransform != null) {
					targetPath.Add(currentTransform);
					if (currentTransform == _owner.transform) {
						break;
					}
					currentTransform = currentTransform.parent;
				}
				targetPath.Reverse();

				UnityEngine.Object[] hierarchyWindows = Resources.FindObjectsOfTypeAll(hierarchyWindowType);
				for (int i = 0; i < hierarchyWindows.Length; i++) {
					setExpandedRecursiveMethod.Invoke(hierarchyWindows[i], new object[] { _owner.gameObject.GetEntityId(), false });
					for (int j = 0; j < targetPath.Count; j++) {
						setExpandedMethod.Invoke(hierarchyWindows[i], new object[] { targetPath[j].gameObject.GetEntityId(), true });
					}
				}

				if (selectTarget) {
					Selection.activeGameObject = _targetChild.gameObject;
					EditorGUIUtility.PingObject(_targetChild.gameObject);
				}
			}
		}

		[Serializable]
		public sealed class SceneVisibilityController
		{
			// - Public Methods -
			public void Apply(GameObject target) {
				SetSceneVisibility(target, _sceneVisibility, true);
				SetScenePicking(target, _scenePicking, true);
				EditorApplication.RepaintHierarchyWindow();
			}
			public void Restore(GameObject target) {
				SetSceneVisibility(target, true, true);
				SetScenePicking(target, true, true);
				EditorApplication.RepaintHierarchyWindow();
			}

			// - Internals -
			[SerializeField] bool _sceneVisibility = true;
			[SerializeField] bool _scenePicking = true;

			void SetSceneVisibility(GameObject target, bool visible, bool includeDescendants) {
				SceneVisibilityManager manager = SceneVisibilityManager.instance;
				if (visible) {
					manager.Show(target, includeDescendants);
				}
				else {
					manager.Hide(target, includeDescendants);
				}
			}

			void SetScenePicking(GameObject target, bool visible, bool includeDescendants) {
				SceneVisibilityManager manager = SceneVisibilityManager.instance;
				if (visible) {
					manager.EnablePicking(target, includeDescendants);
				}
				else {
					manager.DisablePicking(target, includeDescendants);
				}
			}
		}

		// - Handler -
		void OnEnable() {
			_hierarchyExpansionController.Initialize(this);
			_sceneVisibilityController.Apply(gameObject);
			EditorApplication.update -= HandleEditorUpdate;
			EditorApplication.update += HandleEditorUpdate;
		}
		void OnDisable() {
			EditorApplication.update -= HandleEditorUpdate;
			_hierarchyExpansionController.Release();
			_sceneVisibilityController.Restore(gameObject);
		}
		void OnDestroy() {
			EditorApplication.update -= HandleEditorUpdate;
			_hierarchyExpansionController.Release();
			_sceneVisibilityController.Restore(gameObject);
		}
		void OnTransformChildrenChanged() {
			_sceneVisibilityController.Apply(gameObject);
		}
		void OnValidate() {
			_hierarchyExpansionController.Initialize(this);
			_sceneVisibilityController.Apply(gameObject);
		}

		// - Internals -
		[SerializeField, BoxGroup("Hierarchy Expansion"), HideLabel] HierarchyExpansionController _hierarchyExpansionController = new HierarchyExpansionController();
		[SerializeField, BoxGroup("Scene Visibility"), HideLabel] SceneVisibilityController _sceneVisibilityController = new SceneVisibilityController();

		void HandleEditorUpdate() {
			if (Application.isPlaying) {
				return;
			}

			_hierarchyExpansionController.Tick(EditorApplication.timeSinceStartup);
			_sceneVisibilityController.Apply(gameObject);
		}
#endif
	}
}

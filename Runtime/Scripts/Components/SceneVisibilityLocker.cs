using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ParkMinPackages.Foundation.Components
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public sealed class SceneVisibilityLocker : MonoBehaviour
	{
#if UNITY_EDITOR
		// - Unity -
		void OnEnable() {
			Apply(true);
		}

		void OnTransformChildrenChanged() {
			Apply(true);
		}

		void Update() {
			Apply(true);
		}

		void OnDisable() {
			Apply(false);
		}

		void OnDestroy() {
			Apply(false);
		}

		void OnValidate() {
			Apply(_sceneVisibility);
		}

		// - Internal -
		void Apply(bool value) {
			if (value) {
				SetSceneVisibility(gameObject, _sceneVisibility, true);
				SetScenePicking(gameObject, _scenePicking, true);
			}
			else {
				SetSceneVisibility(gameObject, true, true);
				SetScenePicking(gameObject, true, true);
			}

			EditorApplication.RepaintHierarchyWindow();
		}

		void SetSceneVisibility(
			GameObject target,
			bool visible,
			bool includeDescendants) {
			SceneVisibilityManager manager = SceneVisibilityManager.instance;
			if (visible) {
				manager.Show(target, includeDescendants);
			}
			else {
				manager.Hide(target, includeDescendants);
			}
		}

		void SetScenePicking(
			GameObject target,
			bool visible,
			bool includeDescendants) {
			SceneVisibilityManager manager = SceneVisibilityManager.instance;
			if (visible) {
				manager.EnablePicking(target, includeDescendants);
			}
			else {
				manager.DisablePicking(target, includeDescendants);
			}
		}

		// - Field -
		[SerializeField] bool _sceneVisibility = true;
		[SerializeField] bool _scenePicking = true;
#endif
	}
}
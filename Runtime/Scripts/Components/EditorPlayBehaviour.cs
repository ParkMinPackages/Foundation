using UnityEngine;
using UnityEngine.SceneManagement;

namespace ParkMinPackages.Foundation.Components
{
	public abstract class EditorPlayBehaviour : MonoBehaviour
	{
		static Scene _initialScene;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void Initialize() {
			_initialScene = SceneManager.GetActiveScene();
		}


		void Awake() {
#if UNITY_EDITOR
			if (WasStartedFrom())
				EditorPlayAwake();
#endif
		}

		void Start() {
#if UNITY_EDITOR
			if (WasStartedFrom())
				EditorPlayStart();
#endif
		}

		protected abstract void EditorPlayAwake();
		protected abstract void EditorPlayStart();

		bool WasStartedFrom() {
			return SceneManager.GetActiveScene().handle == _initialScene.handle;
		}
	}
}
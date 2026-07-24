#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace com.parkminpackages.expansion.Editor
{
	[CreateAssetMenu(fileName = "BuildSettingProfile", menuName = nameof(parkminpackages) + "/BuildSettingProfile")]
	public class BuildSettingProfile : ScriptableObject
	{
		#region Public - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		public virtual void CopyFromBuildSetting() {
			editorBuildSettingsScenes = EditorBuildSettings.scenes;


			scenes = new SerializableEditorBuildSettingsScene[editorBuildSettingsScenes.Length];
			for (int i = 0; i < scenes.Length; i++) {
				scenes[i] = new SerializableEditorBuildSettingsScene();

				scenes[i].sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(editorBuildSettingsScenes[i].path);
				scenes[i].enable = editorBuildSettingsScenes[i].enabled;
			}
		}

		[Button, PropertyOrder(0)]
		public virtual void Use() {
			if (scenes == null || scenes.Length == 0) {
				EditorBuildSettings.scenes = null;
				return;
			}

			EditorBuildSettingsScene[] newEditorBuildSettingsScene = new EditorBuildSettingsScene[scenes.Length];

			for (int i = 0; i < newEditorBuildSettingsScene.Length; i++) {
				newEditorBuildSettingsScene[i] = new EditorBuildSettingsScene();

				if (scenes[i].sceneAsset == null)
					continue;

				newEditorBuildSettingsScene[i].path = AssetDatabase.GetAssetPath(scenes[i].sceneAsset);
				newEditorBuildSettingsScene[i].enabled = scenes[i].enable;
			}

			EditorBuildSettings.scenes = newEditorBuildSettingsScene;

			PlayerSettings.companyName = companyName;
			PlayerSettings.productName = productName;
			PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com." + PlayerSettings.companyName + "." + PlayerSettings.productName);
		}
		#endregion - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

		#region Private - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		[SerializeField] protected string companyName;
		[SerializeField] protected string productName;
		[SerializeField, PropertyOrder(2)] protected SerializableEditorBuildSettingsScene[] scenes;

		EditorBuildSettingsScene[] editorBuildSettingsScenes;

		[System.Serializable]
		protected class SerializableEditorBuildSettingsScene
		{
			[HorizontalGroup("HorizontalGroup1"), LabelText("")]
			public bool enable;

			[HorizontalGroup("HorizontalGroup1"), LabelText("")]
			public SceneAsset sceneAsset;
		}
		#endregion - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
	}
}
#endif
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.parkminpackages.foundation.Editor
{
	public class SetIconWindow : EditorWindow
	{
		#region 메뉴
		const string _menuPath = "Assets/" + nameof(parkminpackages) + "/스크립트 아이콘 변경";

		[MenuItem(_menuPath, priority = -100)]
		public static void ShowMenuItem() {
			SetIconWindow window = (SetIconWindow)EditorWindow.GetWindow(typeof(SetIconWindow));
			window.titleContent = new GUIContent("Set Icon");
			window.Show();
		}

		[MenuItem(_menuPath, validate = true)]
		public static bool ValidateShowMenuItem() {
			foreach (Object asset in Selection.objects) {
				if (asset.GetType() != typeof(MonoScript))
					return false;
			}
			return true;
		}
		#endregion

		void OnEnable() {
			GetIcons();
			_scroll = new Vector2();
			_selectedIcon = 0;
		}
		void OnDisable() {
			_icons.Clear();
		}
		void OnGUI() {
			SetDesign();

			bool isApply = GUILayout.Button("Apply", GUILayout.Width(100));
			_scroll = GUILayout.BeginScrollView(_scroll);
			_selectedIcon = GUILayout.SelectionGrid(
				_selectedIcon,
				_icons.ToArray(),
				_gridCellCount,
				_guiStyle,
				GUILayout.Width(_gridCellWidth), GUILayout.Height(_gridCellHeight)
			);
			GUILayout.EndScrollView();

			if (isApply) {
				ApplyIcon(_icons[_selectedIcon]);
				Close();
			}
		}

		List<Texture2D> _icons;

		//디자인
		int _gridCellCount;
		int _gridCellSize;
		int _gridCellWidth;
		int _gridCellHeight;
		GUIStyle _guiStyle;

		//레이아웃
		Vector2 _scroll;
		int _selectedIcon;

		void GetIcons() {
			_icons = new List<Texture2D>();
			_icons.Add(null);

			string[] assetGuids = AssetDatabase.FindAssets("t:texture2d l:ScriptIcon");
			foreach (string assetGuid in assetGuids) {
				string path = AssetDatabase.GUIDToAssetPath(assetGuid);
				_icons.Add(AssetDatabase.LoadAssetAtPath<Texture2D>(path));
			}
		}
		void SetDesign() {
			_gridCellCount = 12;
			_gridCellSize = 70;
			_gridCellWidth = _icons.Count <= _gridCellCount ? _icons.Count * _gridCellSize : _gridCellCount * _gridCellSize;
			_gridCellHeight = (_icons.Count / _gridCellCount + (_icons.Count % _gridCellCount == 0 ? 0 : 1)) * _gridCellSize;
			_guiStyle = new GUIStyle(GUI.skin.button);
			_guiStyle.fixedWidth = _gridCellSize;
			_guiStyle.fixedHeight = _gridCellSize;
		}
		void ApplyIcon(Texture2D icon) {
			AssetDatabase.StartAssetEditing();
			foreach (Object asset in Selection.objects) {
				string path = AssetDatabase.GetAssetPath(asset);
				MonoImporter monoImporter = AssetImporter.GetAtPath(path) as MonoImporter;
				monoImporter.SetIcon(icon);
				AssetDatabase.ImportAsset(path);
			}
			AssetDatabase.StopAssetEditing();
			AssetDatabase.Refresh();
		}
	}
}
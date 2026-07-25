using UnityEngine;

namespace ParkMinPackages.Foundation.Extensions
{
	public static class ComponentExtensions
	{
		public static bool IsSceneObject(this Component component) {
			if (component == null)
				return false;

			return component.gameObject.scene.IsValid();
		}

		public static T GetOrAddComponent<T>(this Component component) where T : Component {
			if (component.TryGetComponent<T>(out T t))
				return t;
			else
				return component.gameObject.AddComponent<T>();
		}
	}
}
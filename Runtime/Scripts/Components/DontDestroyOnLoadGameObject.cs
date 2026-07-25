using UnityEngine;

namespace ParkMinPackages.Foundation.Components
{
	public sealed class DontDestroyOnLoadGameObject : MonoBehaviour
	{
		void Awake() {
			transform.SetParent(null);
			DontDestroyOnLoad(gameObject);
		}
	}
}
using System;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace ParkMinPackages.Foundation.Components
{
	[DefaultExecutionOrder(int.MinValue)]
	public partial class InstantiateOnceOnRuntime : MonoBehaviour
	{
		[AutoStaticsCleanup]
		static bool IsInstantiated = false;

		void Awake() {
			if (IsInstantiated)
				return;

			foreach (GameObject prefab in _prefabs) {
				try {
					Instantiate(prefab);
				}
				catch (Exception e) {
					Debug.LogError(e);
				}
			}
			IsInstantiated = true;
		}


		[SerializeField] GameObject[] _prefabs;
	}
}
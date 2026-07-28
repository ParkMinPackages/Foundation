using UnityEngine;

namespace ParkMinPackages.Foundation.Components
{
	public class TargetFrameSetting : MonoBehaviour
	{
		void Awake() {
			switch (Application.platform) {
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
					Application.targetFrameRate = windowsTargetFrameRate;
					break;
				case RuntimePlatform.Android:
					Application.targetFrameRate = androidTargetFrameRate;
					break;
				case RuntimePlatform.WebGLPlayer:
					Application.targetFrameRate = webGLTargetFrameRate;
					break;
			}
		}

		[SerializeField, Min(-1)] int windowsTargetFrameRate = 60;
		[SerializeField, Min(-1)] int androidTargetFrameRate = 60;
		[SerializeField, Min(-1)] int webGLTargetFrameRate = 60;
	}
}
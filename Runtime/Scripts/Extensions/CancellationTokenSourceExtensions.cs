using System;
using System.Threading;

namespace com.parkminpackages.expansion.Extensions
{
	public static class CancellationTokenSourceExtensions
	{
		public static void CancelAndDispose(this CancellationTokenSource cancellationTokenSource) {
			if (cancellationTokenSource != null) {
				try {
					cancellationTokenSource.Cancel();
				}
				catch (ObjectDisposedException) { }
				finally {
					cancellationTokenSource.Dispose();
				}
			}
		}
	}
}
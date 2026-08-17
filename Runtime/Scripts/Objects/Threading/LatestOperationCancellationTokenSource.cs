using System;
using System.Threading;
using ParkMinPackages.Foundation.Extensions;

namespace ParkMinPackages.Foundation.Objects.Threading
{
	public sealed class LatestOperationCancellationTokenSource : IDisposable
	{
		// - Public Methods -
		public CancellationToken CreateToken(
			CancellationToken cancellationToken = default
		) {
			CancellationTokenSource nextCancellationTokenSource = cancellationToken.CanBeCanceled
				? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken)
				: new CancellationTokenSource();
			CancellationToken nextCancellationToken = nextCancellationTokenSource.Token;
			CancellationTokenSource previousCancellationTokenSource;

			lock (_gate) {
				if (_isDisposed) {
					nextCancellationTokenSource.Dispose();
					throw new ObjectDisposedException(nameof(LatestOperationCancellationTokenSource));
				}
				previousCancellationTokenSource = _cancellationTokenSource;
				_cancellationTokenSource = nextCancellationTokenSource;
			}

			previousCancellationTokenSource.CancelAndDispose();
			return nextCancellationToken;
		}

		public void Cancel() {
			CancellationTokenSource cancellationTokenSource;

			lock (_gate) {
				if (_isDisposed) {
					throw new ObjectDisposedException(nameof(LatestOperationCancellationTokenSource));
				}
				cancellationTokenSource = _cancellationTokenSource;
				_cancellationTokenSource = null;
			}

			cancellationTokenSource.CancelAndDispose();
		}

		public void Dispose() {
			CancellationTokenSource cancellationTokenSource;

			lock (_gate) {
				if (_isDisposed) {
					return;
				}
				_isDisposed = true;
				cancellationTokenSource = _cancellationTokenSource;
				_cancellationTokenSource = null;
			}

			cancellationTokenSource.CancelAndDispose();
		}

		// - Internals -
		readonly object _gate = new object();
		CancellationTokenSource _cancellationTokenSource;
		bool _isDisposed;
	}
}

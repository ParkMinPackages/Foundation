using System;
using ParkMinPackages.Foundation.Interfaces;
using R3;
using UnityEngine;

namespace ParkMinPackages.Foundation.Components
{
	public abstract class ExtendedBehaviour : MonoBehaviour, IDisposable
	{
		//Public Methods-------------------------------------------------------------------------------------------
		public bool IsStarted
		{
			get { return _isStarted; }
		}
		public void Dispose() {
			if (_isDisposed) {
				return;
			}

			_isDisposed = true;
			enabled = false;
			Destroy(this);
		}
		//Public Properties----------------------------------------------------------------------------------------

		//Events---------------------------------------------------------------------------------------------------

		//Handlers-------------------------------------------------------------------------------------------------
		protected virtual void OnEnable() {
			if (_isStarted) {
				OnReady();
			}
		}
		protected virtual void OnReady() {
			_r3UpdateMethodsDisposable?.Dispose();
			_r3UpdateMethodsDisposable = null;

			if (this is IR3EarlyUpdatable earlyUpdatable) {
				_r3UpdateMethodsDisposable ??= new CompositeDisposable();
				Observable.EveryUpdate(UnityFrameProvider.EarlyUpdate).Subscribe(_ => earlyUpdatable.R3EarlyUpdate()).AddTo(_r3UpdateMethodsDisposable);
			}
			if (this is IR3FixedUpdatable fixedUpdatable) {
				_r3UpdateMethodsDisposable ??= new CompositeDisposable();
				Observable.EveryUpdate(UnityFrameProvider.FixedUpdate).Subscribe(_ => fixedUpdatable.R3FixedUpdate()).AddTo(_r3UpdateMethodsDisposable);
			}
			if (this is IR3PreUpdatable preUpdatable) {
				_r3UpdateMethodsDisposable ??= new CompositeDisposable();
				Observable.EveryUpdate(UnityFrameProvider.PreUpdate).Subscribe(_ => preUpdatable.R3PreUpdate()).AddTo(_r3UpdateMethodsDisposable);
			}
			if (this is IR3Updatable updatable) {
				_r3UpdateMethodsDisposable ??= new CompositeDisposable();
				Observable.EveryUpdate(UnityFrameProvider.Update).Subscribe(_ => updatable.R3Update()).AddTo(_r3UpdateMethodsDisposable);
			}
			if (this is IR3PreLateUpdatable preLateUpdatable) {
				_r3UpdateMethodsDisposable ??= new CompositeDisposable();
				Observable.EveryUpdate(UnityFrameProvider.PreLateUpdate).Subscribe(_ => preLateUpdatable.R3PreLateUpdate()).AddTo(_r3UpdateMethodsDisposable);
			}
			if (this is IR3PostLateUpdatable postLateUpdatable) {
				_r3UpdateMethodsDisposable ??= new CompositeDisposable();
				Observable.EveryUpdate(UnityFrameProvider.PostLateUpdate).Subscribe(_ => postLateUpdatable.R3PostLateUpdate()).AddTo(_r3UpdateMethodsDisposable);
			}
			if (this is IR3TimeUpdatable timeUpdatable) {
				_r3UpdateMethodsDisposable ??= new CompositeDisposable();
				Observable.EveryUpdate(UnityFrameProvider.TimeUpdate).Subscribe(_ => timeUpdatable.R3TimeUpdate()).AddTo(_r3UpdateMethodsDisposable);
			}
			if (this is IR3PostFixedUpdatable postFixedUpdatable) {
				_r3UpdateMethodsDisposable ??= new CompositeDisposable();
				Observable.EveryUpdate(UnityFrameProvider.PostFixedUpdate).Subscribe(_ => postFixedUpdatable.R3PostFixedUpdate()).AddTo(_r3UpdateMethodsDisposable);
			}
		}
		protected virtual void Start() {
			OnReady();
			_isStarted = true;
		}
		protected virtual void OnDisable() {
			_r3UpdateMethodsDisposable?.Dispose();
			_r3UpdateMethodsDisposable = null;
			if (Application.exitCancellationToken.IsCancellationRequested == false) {
				OnDisableDuringRuntime();
			}
		}
		protected virtual void OnDestroy() {
			if (Application.exitCancellationToken.IsCancellationRequested == false) {
				OnDestroyDuringRuntime();
			}
		}
		//Internals---------------------------------------------------------------------------------------
		bool _isDisposed;
		bool _isStarted;
		CompositeDisposable _r3UpdateMethodsDisposable;

		protected virtual void OnDisableDuringRuntime() { }
		protected virtual void OnDestroyDuringRuntime() { }
	}
}

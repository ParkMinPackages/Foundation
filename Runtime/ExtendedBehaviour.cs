using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

namespace com.parkminpackages.expansion
{
	public abstract class ExtendedBehaviour : MonoBehaviour
	{
		//Statics-------------------------------------------------------------------------------------------
		protected enum R3UpdateMethod
		{
			EarlyUpdateR3,
			FixedUpdateR3,
			PreUpdateR3,
			UpdateR3,
			PreLateUpdateR3,
			PostLateUpdateR3,
			TimeUpdateR3,
			PostFixedUpdateR3
		}

		//Public Methods-------------------------------------------------------------------------------------------
		public bool IsStarted
		{
			get { return _isStarted; }
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
			HashSet<R3UpdateMethod> r3UpdateMethods = DefineR3UpdateMethod()?.ToHashSet();
			if (r3UpdateMethods != null && 1 <= r3UpdateMethods.Count) {
				_r3UpdateMethodsDisposable?.Dispose();
				_r3UpdateMethodsDisposable = new CompositeDisposable();
				if (r3UpdateMethods.Contains(R3UpdateMethod.EarlyUpdateR3))
					Observable.EveryUpdate(UnityFrameProvider.EarlyUpdate).Subscribe(unit => { EarlyUpdateR3(); }).AddTo(_r3UpdateMethodsDisposable);
				if (r3UpdateMethods.Contains(R3UpdateMethod.FixedUpdateR3))
					Observable.EveryUpdate(UnityFrameProvider.FixedUpdate).Subscribe(unit => { FixedUpdateR3(); }).AddTo(_r3UpdateMethodsDisposable);
				if (r3UpdateMethods.Contains(R3UpdateMethod.PreUpdateR3))
					Observable.EveryUpdate(UnityFrameProvider.PreUpdate).Subscribe(unit => { PreUpdateR3(); }).AddTo(_r3UpdateMethodsDisposable);
				if (r3UpdateMethods.Contains(R3UpdateMethod.UpdateR3))
					Observable.EveryUpdate(UnityFrameProvider.Update).Subscribe(unit => { UpdateR3(); }).AddTo(_r3UpdateMethodsDisposable);
				if (r3UpdateMethods.Contains(R3UpdateMethod.PreLateUpdateR3))
					Observable.EveryUpdate(UnityFrameProvider.PreLateUpdate).Subscribe(unit => { PreLateUpdateR3(); }).AddTo(_r3UpdateMethodsDisposable);
				if (r3UpdateMethods.Contains(R3UpdateMethod.PostLateUpdateR3))
					Observable.EveryUpdate(UnityFrameProvider.PostLateUpdate).Subscribe(unit => { PostLateUpdateR3(); }).AddTo(_r3UpdateMethodsDisposable);
				if (r3UpdateMethods.Contains(R3UpdateMethod.TimeUpdateR3))
					Observable.EveryUpdate(UnityFrameProvider.TimeUpdate).Subscribe(unit => { TimeUpdateR3(); }).AddTo(_r3UpdateMethodsDisposable);
				if (r3UpdateMethods.Contains(R3UpdateMethod.PostFixedUpdateR3))
					Observable.EveryUpdate(UnityFrameProvider.PostFixedUpdate).Subscribe(unit => { PostFixedUpdateR3(); }).AddTo(_r3UpdateMethodsDisposable);
			}
		}
		protected virtual void Start() {
			OnReady();
			_isStarted = true;
		}
		protected virtual void OnDisable() {
			_r3UpdateMethodsDisposable?.Dispose();
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
		bool _isStarted;
		CompositeDisposable _r3UpdateMethodsDisposable;

		protected virtual R3UpdateMethod[] DefineR3UpdateMethod() {
			return new R3UpdateMethod[] { };
		}
		protected virtual void EarlyUpdateR3() { }
		protected virtual void FixedUpdateR3() { }
		protected virtual void PreUpdateR3() { }
		protected virtual void UpdateR3() { }
		protected virtual void PreLateUpdateR3() { }
		protected virtual void PostLateUpdateR3() { }
		protected virtual void TimeUpdateR3() { }
		protected virtual void PostFixedUpdateR3() { }

		protected virtual void OnDisableDuringRuntime() { }
		protected virtual void OnDestroyDuringRuntime() { }
	}
}
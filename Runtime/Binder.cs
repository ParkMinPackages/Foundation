using System;
using System.ComponentModel;
using Sirenix.OdinInspector;
using UnityEngine;

namespace com.mutant.expansion
{
	public abstract class Binder<T> : ExtendedBehaviour where T : INotifyPropertyChanged
	{
		//Statics-------------------------------------------------------------------------------------------
		public static void BindTo(GameObject gameObject, in T value) {
			foreach (Binder<T> binder in gameObject.GetComponentsInChildren<Binder<T>>(true)) {
				binder.Bind(value);
			}
		}
		//Public Methods-------------------------------------------------------------------------------------------
		public void Bind(T value) {
			_value = value;

			if (_value != null) {
				_bindDisposable?.Dispose();
				_bindDisposable = SetupBinding(value);
			}
			else {
				_bindDisposable?.Dispose();
				SetupUnBinding();
			}
		}
		//Public Properties----------------------------------------------------------------------------------------
		public T Value
		{
			get { return _value; }
		}
		//Handlers-------------------------------------------------------------------------------------------------
		protected abstract IDisposable SetupBinding(T value);
		protected abstract void SetupUnBinding();
		protected override void OnDestroy() {
			base.OnDestroy();
			_bindDisposable?.Dispose();
		}
		//Internals---------------------------------------------------------------------------------------
		[ShowInInspector, Sirenix.OdinInspector.ReadOnly] protected T _value;
		IDisposable _bindDisposable;
	}
}
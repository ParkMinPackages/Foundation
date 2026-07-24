using System;
using System.ComponentModel;

namespace com.parkminpackages.expansion
{
	public abstract class Binding<T> : ExtendedBehaviour where T : class, INotifyPropertyChanged
	{
		public void Bind(T value) {
			Unbind();

			if (value == null)
				return;

			_value = value;
			_bindDisposable = SetupBinding(value);
			OnBound(value);
		}

		public void Unbind() {
			if (_value == null && _bindDisposable == null)
				return;

			_bindDisposable?.Dispose();
			_bindDisposable = null;

			_value = default;

			OnUnbound();
		}

		public T Value
		{
			get { return _value; }
		}
		public bool IsBound
		{
			get { return _value != null; }
		}

		protected abstract IDisposable SetupBinding(T value);

		protected virtual void OnBound(T value) { }
		protected abstract void OnUnbound();

		protected override void OnDestroy() {
			base.OnDestroy();
			Unbind();
		}

#if ODIN_INSPECTOR
		[Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly]
#endif
		protected T _value;

		IDisposable _bindDisposable;
	}
}
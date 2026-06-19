using System.ComponentModel;

namespace com.mutant.expansion
{
	public class Binder<T> : ExtendedBehaviour where T : class, INotifyPropertyChanged
	{
		public void Bind(T value)
		{
			_current = value;

			foreach (Binding<T> binding in gameObject.GetComponentsInChildren<Binding<T>>(true))
			{
				binding.Bind(value);
			}
		}

		public void Unbind()
		{
			_current = default;

			foreach (Binding<T> binding in gameObject.GetComponentsInChildren<Binding<T>>(true))
			{
				binding.Unbind();
			}
		}

		public T Current => _current;
		public bool IsBound => _current != null;

		T _current;
	}
}
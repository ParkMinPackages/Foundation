using System.Collections.Generic;

namespace com.mutant.expansion
{
	public abstract class TreeNode<T> : ExtendedBehaviour where T : TreeNode<T>
	{
		// ===================== Public API =====================

		public IEnumerable<T> ParentNodesEnumerable(bool includeSelf = true) {
			T current = includeSelf ? (T)this : _parentNode;

			while (current != null) {
				yield return current;
				current = current._parentNode;
			}
		}

		public IEnumerable<T> ChildNodesEnumerable(bool includeSelf = true) {
			Stack<T> stack = new Stack<T>();

			if (includeSelf) {
				stack.Push((T)this);
			}
			else {
				foreach (T child in _childNodes) {
					if (child != null) stack.Push(child);
				}
			}

			while (stack.Count > 0) {
				T node = stack.Pop();

				yield return node;

				foreach (T child in node._childNodes) {
					stack.Push(child);
				}
			}
		}

		// ===================== Public Property =====================

		public T ParentNode
		{
			get { return _parentNode; }
		}

		public IReadOnlyCollection<T> ChildNodes
		{
			get { return _childNodes; }
		}

		// ===================== Unity Messages =====================
		protected virtual void Awake() {
			Init();
		}

		protected virtual void OnBeforeTransformParentChanged() {
			if (_parentNode != null) {
				_parentNode._childNodes.Remove((T)this);
				_parentNode = null;
			}
		}

		protected virtual void OnTransformParentChanged() {
			_isInit = false;
			Init();
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			if (_parentNode != null) {
				_parentNode._childNodes.Remove((T)this);
				_parentNode = null;
			}
		}

		// ===================== Internals =====================
		T _parentNode;
		HashSet<T> _childNodes = new HashSet<T>();
		bool _isInit;

		protected virtual void Init() {
			if (_isInit) return;
			_isInit = true;

			if (_parentNode != null) {
				_parentNode._childNodes.Remove((T)this);
				_parentNode = null;
			}

			if (transform.parent == null) {
				_isInit = true;
				return;
			}

			_parentNode = transform.parent.GetComponentInParent<T>(true);

			if (_parentNode != null) {
				_parentNode._childNodes.Add((T)this);
			}
		}
	}
}
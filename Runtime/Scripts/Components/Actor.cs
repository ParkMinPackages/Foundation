using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace com.parkminpackages.expansion.Components
{
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(-10)]
	public class Actor : ExtendedBehaviour
	{
		// ===================== Static storages =====================
		// 타입(구체/부모/인터페이스) -> 인스턴스 집합
		static readonly Dictionary<Type, HashSet<Actor>> _cacheDic = new Dictionary<Type, HashSet<Actor>>();

		// 스폰 알림 이벤트
		static readonly Dictionary<Type, UnityEvent<Actor>> _eventDic = new Dictionary<Type, UnityEvent<Actor>>();
		static readonly Dictionary<Type, int> _eventSubscriberCountDic = new Dictionary<Type, int>();

		//역인덱스
		static Dictionary<Type, Type[]> _typeClosureCache = new Dictionary<Type, Type[]>();
		static Type[] GetOrBuildTypeClosure(Type concrete) {
			Type[] cached;
			if (_typeClosureCache.TryGetValue(concrete, out cached) && cached != null) return cached;

			HashSet<Type> seen = new HashSet<Type>();
			List<Type> result = new List<Type>();

			Type current = concrete;
			while (current != null) {
				// 클래스 타입 추가
				if (seen.Add(current)) {
					result.Add(current);
				}

				// 인터페이스들 추가
				Type[] ifaces = current.GetInterfaces();
				for (int i = 0; i < ifaces.Length; i++) {
					Type itf = ifaces[i];
					if (seen.Add(itf)) {
						result.Add(itf);
					}
				}

				// RootObject 까지만 포함하고 종료
				if (current == typeof(Actor)) {
					break;
				}

				current = current.BaseType;
			}

			Type[] built = result.ToArray();
			_typeClosureCache[concrete] = built;
			return built;
		}
		// ===================== Initialize =====================
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void SubsystemRegistrationInit() {
			_cacheDic.Clear();
			_eventDic.Clear();
			_eventSubscriberCountDic.Clear();
			_typeClosureCache.Clear();

			SceneManager.sceneLoaded -= InitScene;
			SceneManager.sceneLoaded -= InitScene;
			SceneManager.sceneLoaded += InitScene;
		}

		static void InitScene(Scene scene, LoadSceneMode mode) {
			List<Actor> list = ListPool<Actor>.Get();
			try {
				GameObject[] roots = scene.GetRootGameObjects();
				for (int i = 0; i < roots.Length; i++) {
					list.Clear();
					roots[i].GetComponentsInChildren(includeInactive: true, results: list);
					for (int j = 0; j < list.Count; j++) list[j].Register();
				}
			}
			finally {
				list.Clear();
				ListPool<Actor>.Release(list);
			}
		}

		// ===================== Public API =====================
		public static IDisposable SubscribeAction<T>(UnityAction<T> callback) where T : class {
			if (callback == null) throw new ArgumentNullException(nameof(callback));

			Type type = typeof(T);

			UnityEvent<Actor> evt;
			if (_eventDic.TryGetValue(type, out evt) == false || evt == null) {
				evt = new UnityEvent<Actor>();
				_eventDic[type] = evt;
			}

			// 현재 존재하는 모든 T(인터페이스/부모 타입 포함) 즉시 실행
			foreach (T item in GetEnumerable<T>()) {
				callback(item);
			}

			// 구독자 수 증가
			int count;
			if (_eventSubscriberCountDic.TryGetValue(type, out count) == false) count = 0;
			_eventSubscriberCountDic[type] = count + 1;

			// 이후 스폰되는 항목 구독
			IDisposable disposable = evt.AsObservable().Subscribe(rootObject =>
			{
				T casted = rootObject as T;
				if (casted != null) {
					callback.Invoke(casted);
				}
			});

			// 해제 핸들러
			return Disposable.Create(() =>
			{
				disposable.Dispose();

				int left;
				if (_eventSubscriberCountDic.TryGetValue(type, out left) == false) left = 0;
				left -= 1;

				if (left <= 0) {
					_eventSubscriberCountDic.Remove(type);
					_eventDic.Remove(type);
				}
				else {
					_eventSubscriberCountDic[type] = left;
				}
			});
		}

		public static T Get<T>() where T : class {
			HashSet<Actor> set;
			if (_cacheDic.TryGetValue(typeof(T), out set) && set != null && set.Count > 0) {
				foreach (Actor r in set) {
					T casted = r as T;
					if (casted != null) return casted;
				}
			}
			throw new InvalidOperationException("RootObject : " + typeof(T) + "이 없습니다.");
		}

		public static T GetOrDefault<T>() where T : class {
			HashSet<Actor> set;
			if (_cacheDic.TryGetValue(typeof(T), out set) && set != null && set.Count > 0) {
				foreach (Actor r in set) {
					T casted = r as T;
					if (casted != null) return casted;
				}
			}
			return null;
		}

		public static bool TryGet<T>(out T result) where T : class {
			result = null;
			HashSet<Actor> set;
			if (_cacheDic.TryGetValue(typeof(T), out set) && set != null && set.Count > 0) {
				foreach (Actor r in set) {
					T casted = r as T;
					if (casted != null) {
						result = casted;
						return true;
					}
				}
			}
			return false;
		}

		public static List<T> GetAll<T>() where T : class {
			return GetEnumerable<T>().ToList();
		}

		public static IEnumerable<T> GetEnumerable<T>() where T : class {
			HashSet<Actor> set;
			if (_cacheDic.TryGetValue(typeof(T), out set) && set != null) {
				foreach (Actor r in set) {
					if (r != null) {
						T casted = r as T;
						if (casted != null) yield return casted;
					}
				}
			}
		}

		// ===================== MonoBehaviour hooks =====================

		protected virtual void Awake() {
			Register();
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			Unregister();
		}

#if UNITY_EDITOR
		protected virtual void OnValidate() {
			while (UnityEditorInternal.ComponentUtility.MoveComponentUp(this)) { }
		}
#endif

		// ===================== Instance fields =====================
		[SerializeField] protected string _id;
		bool _isRegistered;

		public string ID
		{
			get { return _id; }
			set { _id = value; }
		}

		// ===================== Register/Unregister =====================
		void Register() {
			if (_isRegistered) return;
			_isRegistered = true;
			Register(this);
		}

		void Unregister() {
			if (_isRegistered == false) return;
			_isRegistered = false;
			Unregister(this);
		}

		void Register(Actor instance) {
			Type instanceType = instance.GetType();
			Type[] registerTypes = GetOrBuildTypeClosure(instanceType);

			for (int i = 0; i < registerTypes.Length; i++) {
				Type t = registerTypes[i];

				if (_cacheDic.TryGetValue(t, out HashSet<Actor> set) == false || set == null) {
					set = new HashSet<Actor>();
					_cacheDic[t] = set;
				}
				set.Add(instance);

				// 구독자 알림
				if (_eventDic.TryGetValue(t, out UnityEvent<Actor> evt) && evt != null) {
					evt.Invoke(instance);
				}
			}
		}

		void Unregister(Actor instance) {
			Type instanceType = instance.GetType();
			Type[] registerTypes = GetOrBuildTypeClosure(instanceType);

			for (int i = 0; i < registerTypes.Length; i++) {
				Type t = registerTypes[i];
				if (_cacheDic.TryGetValue(t, out HashSet<Actor> set) && set != null) {
					set.Remove(instance);
					if (set.Count == 0) {
						_cacheDic.Remove(t);
					}
				}
			}
		}
	}
}
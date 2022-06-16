using System.Collections;
using UnityEngine;

namespace CustomFunctionality
{
	public class CoroutineWithResult
	{
		public Coroutine coroutine;
		public object result;
		private IEnumerator target;

		public CoroutineWithResult(MonoBehaviour owner, IEnumerator t)
		{
			target = t;
			coroutine = owner.StartCoroutine(Run());
		}

		private IEnumerator Run()
		{
			while (target.MoveNext())
			{
				result = target.Current;
				yield return result;
			}
		}
	}
}
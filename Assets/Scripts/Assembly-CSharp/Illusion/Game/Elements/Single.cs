using System;
using UniRx;

namespace Illusion.Game.Elements
{
	public class Single : IObservable<Unit>, IDisposable
	{
		private readonly AsyncSubject<Unit> _asyncSubject = new AsyncSubject<Unit>();

		private readonly object lockObject = new object();

		public void Done()
		{
			lock (lockObject)
			{
				if (!_asyncSubject.IsCompleted)
				{
					_asyncSubject.OnNext(Unit.Default);
					_asyncSubject.OnCompleted();
				}
			}
		}

		public IDisposable Subscribe(IObserver<Unit> observer)
		{
			lock (lockObject)
			{
				return _asyncSubject.Subscribe(observer);
			}
		}

		public void Dispose()
		{
			lock (lockObject)
			{
				_asyncSubject.Dispose();
			}
		}
	}
}

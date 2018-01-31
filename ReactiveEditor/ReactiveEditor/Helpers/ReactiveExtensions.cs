using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace ReactiveEditor.Helpers
{
    public static class ReactiveExtensions
    {
        /// <summary>
        /// Helper method to transform an IObservable{T} to IObservable{Unit}.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="observable"></param>
        /// <returns></returns>
        public static IObservable<Unit> ToUnit<T>(this IObservable<T> observable)
        {
            return observable.Select(_ => Unit.Default);
        }
    }
}
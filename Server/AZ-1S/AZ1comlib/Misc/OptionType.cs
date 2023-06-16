using System;


interface IOption<T> {
  TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone);
  IOption<TResult> Bind<TResult>(Func<T, IOption<TResult>> f);
  IOption<TResult> Map<TResult>(Func<T, TResult> f);
  T Or(T aDefault);
}

class Some<T> : IOption<T> {
  private T _data;

  private Some(T data) {
    _data = data;
  }

  public static IOption<T> Of(T data) => new Some<T>(data);

  public TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> _) =>
    onSome(_data);

  public IOption<TResult> Bind<TResult>(Func<T, IOption<TResult>> f) => f(_data);

  public IOption<TResult> Map<TResult>(Func<T, TResult> f) => new Some<TResult>(f(_data));

  public T Or(T _) => _data;
}

class None<T> : IOption<T> {
  public TResult Match<TResult>(Func<T, TResult> _, Func<TResult> onNone) =>
    onNone();

  public IOption<TResult> Bind<TResult>(Func<T, IOption<TResult>> f) => new None<TResult>();

  public IOption<TResult> Map<TResult>(Func<T, TResult> f) => new None<TResult>();

  public T Or(T aDefault) => aDefault;
}


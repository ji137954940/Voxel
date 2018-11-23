﻿using System;
using System.Collections.Generic;

/// <summary>
/// Implements a C# promise.
/// https://developer.mozilla.org/en/docs/Web/JavaScript/Reference/Global_Objects/Promise
/// </summary>
public interface IPromise<PromisedT>
{
	/// <summary>
	/// Catch any execption that is thrown while the promise is being resolved.
	/// </summary>
	IPromise<PromisedT> Catch(Action<Exception> onError);

	/// <summary>
	/// Handle completion of the promise.
	/// </summary>
	IPromise<PromisedT> Done(Action<PromisedT> onCompleted);

	/// <summary>
	/// Handle completion of the promise.
	/// </summary>
	IPromise<PromisedT> Done(Action onCompleted);

	/// <summary>
	/// Chains another asynchronous operation. 
	/// May also change the type of value that is being fulfilled.
	/// </summary>
	IPromise<ConvertedT> Then<ConvertedT>(Func<PromisedT, IPromise<ConvertedT>> chain);

	/// <summary>
	/// Return a new promise with a different value.
	/// May also change the type of the value.
	/// </summary>
	IPromise<ConvertedT> Transform<ConvertedT>(Func<PromisedT, ConvertedT> transform);
}

/// <summary>
/// Interface for a promise that can be rejected or resolved.
/// </summary>
public interface IPendingPromise<PromisedT>
{
	/// <summary>
	/// Reject the promise with an exception.
	/// </summary>
	void Reject(Exception ex);

	/// <summary>
	/// Resolve the promise with a particular value.
	/// </summary>
	void Resolve(PromisedT value);
}

/// <summary>
/// Specifies the state of a promise.
/// </summary>
public enum PromiseState
{
	Pending,    // The promise is in-flight.
	Rejected,   // The promise has been rejected.
	Resolved    // The promise has been resolved.
};

/// <summary>
/// Implements a C# promise.
/// https://developer.mozilla.org/en/docs/Web/JavaScript/Reference/Global_Objects/Promise
/// </summary>
public class Promise<PromisedT> : IPromise<PromisedT>, IPendingPromise<PromisedT>
{
	/// <summary>
	/// The exception when the promise is rejected.
	/// </summary>
	private Exception rejectionException;

	/// <summary>
	/// The value when the promises is resolved.
	/// </summary>
	private PromisedT resolveValue;

	/// <summary>
	/// Error handlers.
	/// </summary>
	private List<Action<Exception>> errorHandlers;

	/// <summary>
	/// Completed handlers that accept a value.
	/// </summary>
	private List<Action<PromisedT>> valueCompletedHandlers;

	/// <summary>
	/// Completed handlers that accept no value.
	/// </summary>
	private List<Action> completedHandlers;

	/// <summary>
	/// Tracks the current state of the promise.
	/// </summary>
	public PromiseState CurState { get; private set; }

	public Promise()
	{
		this.CurState = PromiseState.Pending;
	}

	/// <summary>
	/// Helper function clear out all handlers after resolution or rejection.
	/// </summary>
	private void ClearHandlers()
	{
		errorHandlers = null;
		valueCompletedHandlers = null;
		completedHandlers = null;
	}

	/// <summary>
	/// Reject the promise with an exception.
	/// </summary>
	public void Reject(Exception ex)
	{
		if (CurState != PromiseState.Pending)
		{
			throw new ApplicationException("Attempt to reject a promise that is already in state: " + CurState + ", a promise can only be rejected when it is still in state: " + PromiseState.Pending + ex.ToString());
		}

		rejectionException = ex;

		CurState = PromiseState.Rejected;

		if (errorHandlers != null)
		{
			errorHandlers.ForEach(handler => handler(rejectionException));
		}

		ClearHandlers();
	}


	/// <summary>
	/// Resolve the promise with a particular value.
	/// </summary>
	public void Resolve(PromisedT value)
	{
		if (CurState != PromiseState.Pending)
		{
			throw new ApplicationException("Attempt to resolve a promise that is already in state: " + CurState + ", a promise can only be resolved when it is still in state: " + PromiseState.Pending);
		}

		resolveValue = value;

		CurState = PromiseState.Resolved;

		if (valueCompletedHandlers != null)
		{
			valueCompletedHandlers.ForEach(handler => handler(resolveValue));
		}

		if (completedHandlers != null)
		{
			completedHandlers.ForEach(handler => handler());
		}

		ClearHandlers();
	}

	/// <summary>
	/// Catch any execption that is thrown while the promise is being resolved.
	/// </summary>
	public IPromise<PromisedT> Catch(Action<Exception> onError)
	{
		if (CurState == PromiseState.Pending)
		{
			// Promise is in flight, queue handler for possible call later.
			if (errorHandlers == null)
			{
				errorHandlers = new List<Action<Exception>>();
			}

			errorHandlers.Add(onError);
		}
		else if (CurState == PromiseState.Rejected)
		{
			// Promise has already been rejected, immediately call handler.
			onError(rejectionException);
		}

		return this;
	}

	/// <summary>
	/// Handle completion of the promise.
	/// </summary>
	public IPromise<PromisedT> Done(Action<PromisedT> onCompleted)
	{
		if (CurState == PromiseState.Pending)
		{
			// Promise is in flight, queue handler for possible call later.
			if (valueCompletedHandlers == null)
			{
				valueCompletedHandlers = new List<Action<PromisedT>>();
			}
			valueCompletedHandlers.Add(onCompleted);
		}
		else if (CurState == PromiseState.Resolved)
		{
			// Promise has already been rejected, immediately call handler.
			onCompleted(resolveValue);
		}

		return this;
	}

	/// <summary>
	/// Handle completion of the promise.
	/// </summary>
	public IPromise<PromisedT> Done(Action onCompleted)
	{
		if (CurState == PromiseState.Pending)
		{
			// Promise is in flight, queue handler for possible call later.
			if (completedHandlers == null)
			{
				completedHandlers = new List<Action>();
			}
			completedHandlers.Add(onCompleted);
		}
		else if (CurState == PromiseState.Resolved)
		{
			// Promise has already been rejected, immediately call handler.
			onCompleted();
		}

		return this;
	}

	/// <summary>
	/// Chains another asynchronous operation. 
	/// May also change the type of value that is being fulfilled.
	/// </summary>
	public IPromise<ConvertedT> Then<ConvertedT>(Func<PromisedT, IPromise<ConvertedT>> chain)
	{
		var resultPromise = new Promise<ConvertedT>();

		Catch(e => resultPromise.Reject(e));
		Done(v =>
		{
			try
			{
				var chainedPromise = chain(v);
				chainedPromise.Catch(e => resultPromise.Reject(e));
				chainedPromise.Done(chainedValue => resultPromise.Resolve(chainedValue));
			}
			catch (Exception ex)
			{
				resultPromise.Reject(ex);
			}
		});

		return resultPromise;
	}

	/// <summary>
	/// Return a new promise with a different value.
	/// May also change the type of the value.
	/// </summary>
	public IPromise<ConvertedT> Transform<ConvertedT>(Func<PromisedT, ConvertedT> transform)
	{
		var resultPromise = new Promise<ConvertedT>();
		Catch(e => resultPromise.Reject(e));
		Done(v =>
		{
			try
			{
				var transformedValue = transform(v);
				resultPromise.Resolve(transformedValue);
			}
			catch (Exception ex)
			{
				resultPromise.Reject(ex);
			}
		});
		return resultPromise;
	}

	/// <summary>
	/// Returns a promise that resolves when all of the promises in the enumerable argument have resolved.
	/// Returns a promise of a collection of the resolved results.
	/// </summary>
	public static IPromise<PromisedT[]> All(params IPromise<PromisedT>[] promises)
	{
		var promisesArray = promises;
		var remainingCount = promisesArray.Length;
		var results = new PromisedT[remainingCount];
		var resultPromise = new Promise<PromisedT[]>();

		promisesArray.Each((promise, index) =>
		{
			promise
				.Catch(ex =>
				{
					if (resultPromise.CurState == PromiseState.Pending)
					{
						// If a promise errorred and the result promise is still pending, reject it.
						resultPromise.Reject(ex);
					}
				})
				.Done(result =>
				{
					results[index] = result;

					--remainingCount;
					if (remainingCount <= 0)
					{
						// This will never happen if any of the promises errorred.
						resultPromise.Resolve(results);
					}
				});
		});

		return resultPromise;
	}

	/// <summary>
	/// Convert a simple value directly into a resolved promise.
	/// </summary>
	public static IPromise<PromisedT> Resolved(PromisedT promisedValue)
	{
		var promise = new Promise<PromisedT>();
		promise.Resolve(promisedValue);
		return promise;
	}

	/// <summary>
	/// Convert an exception directly into a rejected promise.
	/// </summary>
	public static IPromise<PromisedT> Rejected(Exception ex)
	{
		var promise = new Promise<PromisedT>();
		promise.Reject(ex);
		return promise;
	}
}

/// <summary>
/// 所有创建Promise 需要走这里.
/// </summary>
/// <typeparam name="PromisedT"></typeparam>
public static class PromiseCache<PromisedT>
{
	public static Promise<PromisedT> Get()
	{
		return new Promise<PromisedT>();
	}
}

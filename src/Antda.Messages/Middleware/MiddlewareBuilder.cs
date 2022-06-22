﻿using Antda.Core.Helpers;

namespace Antda.Messages.Middleware;

public class MiddlewareBuilder : IMiddlewareBuilder, IMiddlewareProvider
{
  private readonly IList<(Type MessageType, Func<MessageDelegate, MessageDelegate> Factory)> _middlewares = new List<(Type Type, Func<MessageDelegate, MessageDelegate> Delegate)>();

  public IMiddlewareBuilder Use(Type messageType, Func<MessageDelegate, MessageDelegate> factory)
  {
    _middlewares.Add((messageType, factory));
    return this;
  }

  public MessageDelegate Create(Type messageType)
  {
    MessageDelegate invokeDelegate = _ => Task.CompletedTask;

    var middlewares = GetMiddlewares(messageType);

    foreach (var middleware in middlewares.Reverse())
    {
      invokeDelegate = middleware(invokeDelegate);
    }

    return invokeDelegate;
  }

  private IEnumerable<Func<MessageDelegate, MessageDelegate>> GetMiddlewares(Type messageType)
  {
    foreach (var (type, middleware) in _middlewares)
    {
      if (TypeHelper.FindTypes(messageType, type).Any())
      {
        yield return middleware;
      }
    }
  }
}
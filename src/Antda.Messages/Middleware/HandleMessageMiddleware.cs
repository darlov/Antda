﻿namespace Antda.Messages.Middleware;

public class HandleMessageMiddleware<TMessage, TResult> : MessageMiddleware<TMessage, TResult>
  where TMessage : IMessage<TResult>
{
  private readonly IMessageHandler<TMessage, TResult> _messageHandler;

  public HandleMessageMiddleware(IMessageHandler<TMessage, TResult> messageHandler)
  {
    _messageHandler = messageHandler;
  }

  public override async Task InvokeAsync(IMessageContext<TMessage, TResult> context, MessageDelegate next, CancellationToken cancellationToken)
  {
    var result = await _messageHandler.HandleAsync(context.Message, cancellationToken).ConfigureAwait(false);
    context.Result = result;

    await next(context).ConfigureAwait(false);
  }
}
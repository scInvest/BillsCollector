using System;
using System.Threading;
using System.Threading.Tasks;
using Pgr.AIHub.API.ChatInterface;
using Pgr.AIHub.API.ChatInterface.Dummy;

namespace Pgr.AIHub.Tests;

[TestClass]
public class DummyImplementationTests
{
    [TestMethod]
    public async Task CreateChat_AddsChatToManagerCollection()
    {
        var manager = new AiChatManagerDummyImp();

        var chat = manager.CreateChat();

        Assert.IsNotNull(chat);
        Assert.AreEqual(1, manager.Chats.Count);
        Assert.AreSame(chat, manager.Chats[0]);
    }

    [TestMethod]
    public async Task SendAsync_AddsUserMessage_AndRaisesAssistantEvent()
    {
        var chat = new AiChatClientWorkerDummyImp();
        var received = new TaskCompletionSource<IAiChatMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
        var options = new AiChatOptionsDummyImp
        {
            Model = "GPT-5.4",
            Mode = "Agent",
            ApiKey = "test-key"
        };

        chat.MessageReceived += (_, args) => received.TrySetResult(args.Message);

        await chat.SendAsync("hello", options);

        var message = await received.Task.WaitAsync(TimeSpan.FromSeconds(2));

        Assert.AreEqual(2, chat.Messages.Count);
        Assert.AreEqual(AiChatRole.User, chat.Messages[0].Role);
        Assert.AreEqual("hello", chat.Messages[0].Content);
        Assert.AreEqual(AiChatRole.Assistant, message.Role);
        Assert.AreEqual("AI: hello", message.Content);
    }

    [TestMethod]
    public async Task CancelAsync_PreventsAssistantReply()
    {
        var chat = new AiChatClientWorkerDummyImp();
        var eventRaised = false;
        var options = new AiChatOptionsDummyImp
        {
            Model = "GPT-5.4",
            Mode = "Question"
        };

        chat.MessageReceived += (_, _) => eventRaised = true;

        var sendTask = chat.SendAsync("stop me", options, CancellationToken.None);
        await chat.CancelAsync();
        await sendTask;

        Assert.IsFalse(eventRaised);
        Assert.AreEqual(1, chat.Messages.Count);
        Assert.AreEqual(AiChatRole.User, chat.Messages[0].Role);
    }

    [TestMethod]
    public async Task RateLimiter_DoesNotThrow_AndReturnsTrue()
    {
        var rateLimiter = new ChatRateLimiterDummyImp();

        Assert.IsTrue(rateLimiter.RequestStarted("GPT-5.4"));
        Assert.IsTrue(rateLimiter.RequestFinished("GPT-5.4", 12.5));

        await rateLimiter.DelayIfNeeded();
    }

    [TestMethod]
    public void ModelSupport_ReturnsExpectedResults()
    {
        var modelSupport = new ModelSpecificImplementationDummyImp();

        Assert.IsTrue(modelSupport.IsModelSupported("GPT-5.4"));
        Assert.IsTrue(modelSupport.IsModelSupported("gpt-5.4 mini"));
        Assert.IsFalse(modelSupport.IsModelSupported("Unknown-model"));
    }
}

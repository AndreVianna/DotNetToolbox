namespace DotNetToolbox.Graph.Nodes;

public class ActionNodeTests {
    private readonly NodeFactory _factory;

    public ActionNodeTests() {
        var services = new ServiceCollection();
        services.AddTransient<IPolicy, RetryPolicy>();
        var provider = services.BuildServiceProvider();
        _factory = new(provider);
    }

    [Fact]
    public void CreateAction_WithoutLabel_ReturnsActionNodeWithDefaultLabel() {
        var node = _factory.CreateAction(1, _ => { });

        node.Should().NotBeNull();
        node.Should().BeOfType<ActionNode>();
        node.Label.Should().Be("action");
    }

    [Fact]
    public void CreateAction_WithCustomLabel_ReturnsActionNodeWithCustomLabel() {
        const string customLabel = "CustomAction";
        var node = _factory.CreateAction(1, customLabel, _ => { });

        node.Should().NotBeNull();
        node.Should().BeOfType<ActionNode>();
        node.Label.Should().Be(customLabel);
    }

    [Fact]
    public void CreateAction_WithGenericType_ReturnsCustomActionNode() {
        var node = _factory.CreateAction<CustomActionNode>(1);

        node.Should().NotBeNull();
        node.Should().BeOfType<CustomActionNode>();
    }

    [Fact]
    public void CreateAction_WithPolicy_AppliesPolicyToNode() {
        var services = new ServiceCollection();
        services.AddTransient<IPolicy, TestPolicy>();
        var provider = services.BuildServiceProvider();
        var factory = new NodeFactory(provider);

        var node = factory.CreateAction(1, _ => { });

        node.Should().NotBeNull();
        node.Should().BeOfType<ActionNode>();
    }

    [Fact]
    public async Task Run_WithPolicy_ExecutesPolicyAndAction() {
        var actionExecuted = false;
        var node = _factory.CreateAction(1, _ => actionExecuted = true);
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();
        var context = new Context(provider);

        await node.Run(context);

        actionExecuted.Should().BeTrue();
    }

    [Fact]
    public async Task Run_WithSuccessfulRetry_ExecutesPolicyAndAction() {
        var actionExecuted = false;
        var policy = new TestPolicy(failedTries: 2);
        var services = new ServiceCollection();
        services.AddTransient<IPolicy>(_ => policy);
        var provider = services.BuildServiceProvider();
        var factory = new NodeFactory(provider);
        var node = factory.CreateAction(1, _ => actionExecuted = true);
        var context = new Context(provider);

        await node.Run(context);

        policy.TryCount.Should().Be(3);
        actionExecuted.Should().BeTrue();
    }

    [Fact]
    public async Task Run_WithTooManyRetries_ExecutesPolicyAndAction() {
        var actionExecuted = false;
        var services = new ServiceCollection();
        var policy = new TestPolicy(failedTries: RetryPolicy.DefaultMaximumRetries + 1);
        services.AddTransient<IPolicy>(_ => policy);
        var provider = services.BuildServiceProvider();
        var factory = new NodeFactory(provider);

        var node = factory.CreateAction(1, _ => actionExecuted = true);
        var context = new Context(provider);

        var action = () => node.Run(context);

        await action.Should().ThrowAsync<PolicyException>();
        policy.TryCount.Should().Be(RetryPolicy.DefaultMaximumRetries);
        actionExecuted.Should().BeTrue();
    }

    [Fact]
    public async Task Run_WithCustomRetries_ExecutesPolicyAndAction() {
        var actionExecuted = false;
        var services = new ServiceCollection();
        var policy = new TestPolicy(maxRetries: 10, failedTries: 11);
        services.AddTransient<IPolicy>(_ => policy);
        var provider = services.BuildServiceProvider();
        var factory = new NodeFactory(provider);
        var node = factory.CreateAction(1, _ => actionExecuted = true);
        var context = new Context(provider);

        var action = () => node.Run(context);

        await action.Should().ThrowAsync<PolicyException>();
        policy.TryCount.Should().Be(10);
        actionExecuted.Should().BeTrue();
    }

    [Fact]
    public async Task Run_RetryOnException_ExecutesPolicyAndAction() {
        var services = new ServiceCollection();
        var policy = new TestPolicy(failedTries: RetryPolicy.DefaultMaximumRetries + 1);
        services.AddTransient<IPolicy>(_ => policy);
        var provider = services.BuildServiceProvider();
        var factory = new NodeFactory(provider);
        var node = factory.CreateAction(1, _ => throw new());
        var context = new Context(provider);

        var action = () => node.Run(context);

        await action.Should().ThrowAsync<PolicyException>();
        policy.TryCount.Should().Be(RetryPolicy.DefaultMaximumRetries);
    }

    [Fact]
    public async Task Run_WithRetryAsMax_IgnoresPolicyAndTryAsManyTimesAsNeeded() {
        var services = new ServiceCollection();
        var policy = new TestPolicy(maxRetries: byte.MaxValue);
        services.AddTransient<IPolicy>(_ => policy);
        var provider = services.BuildServiceProvider();
        var factory = new NodeFactory(provider);
        var node = factory.CreateAction(1, _ => {
            if (policy.TryCount < 10) throw new();
        });
        var context = new Context(provider);

        var action = () => node.Run(context);

        await action.Should().NotThrowAsync();
        policy.TryCount.Should().Be(10);
    }

    [Fact]
    public async Task CRun_RunMethod_UpdatesContextAndReturnsNextNode() {
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();
        var context = new CustomContext(provider);
        var nextNode = _factory.CreateAction(1, _ => { });
        var node = _factory.CreateAction(2, ctx => ctx["key"] = "value");
        node.Next = nextNode;

        var result = await node.Run(context);

        result.Should().BeSameAs(nextNode);
        context["key"].Should().Be("value");
    }

    private sealed class CustomContext(IServiceProvider provider)
        : Context(provider);

    // ReSharper disable once ClassNeverInstantiated.Local - Test class
    private sealed class CustomActionNode
        : ActionNode<CustomActionNode> {
        public CustomActionNode(uint id, string label, IServiceProvider services)
            : base(id, label, services) {
        }

        public CustomActionNode(uint id, IServiceProvider services)
            : base(id, services) { }

        protected override Task Execute(Context context, CancellationToken ct) => Task.CompletedTask;
    }

    private sealed class TestPolicy(byte maxRetries = RetryPolicy.DefaultMaximumRetries, uint failedTries = 0)
        : RetryPolicy(maxRetries) {
        protected override async Task<bool> TryExecute(Func<Context, CancellationToken, Task> action, Context ctx, CancellationToken ct) {
            TryCount++;
            await action(ctx, ct);
            return TryCount > failedTries;
        }
        public uint TryCount { get; private set; }
    }
}

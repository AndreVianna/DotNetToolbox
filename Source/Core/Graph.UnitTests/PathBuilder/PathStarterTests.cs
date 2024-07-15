namespace DotNetToolbox.Graph.PathBuilder;

public class NodeBuilderTests {
    [Fact]
    public void If_WithPredicate_AddsIfNode() {
        var path = Start.If(_ => true);
        path.Should().NotBeNull();
        path.Should().BeOfType<IThenBuilder>();
    }

    [Fact]
    public void Then_WithAction_SetsTruePath() {
        var path = Start.If(_ => true).Then(_ => { });
        path.Should().NotBeNull();
        path.Should().BeAssignableTo<IElseBuilder>();
    }

    [Fact]
    public void Then_WithNode_SetsTruePath() {
        var path = Start.If(_ => true).Then(Start.End);
        path.Should().NotBeNull();
        path.Should().BeAssignableTo<IElseBuilder>();
    }

    [Fact]
    public void Else_WithNode_SetsFalsePath() {
        var path = Start.If(_ => false).Then(_ => End).Else(_ => End);
        path.Should().NotBeNull();
        path.Should().BeAssignableTo<INodeBuilder>();
    }
    [Fact]
    public void ElseIf_WithPredicate_AddsElseIfNode() {
        var path = Start.If(_ => false).Then(_ => End).ElseIf(_ => true);
        path.Should().NotBeNull();
        path.Should().BeAssignableTo<IThenBuilder>();
    }
}
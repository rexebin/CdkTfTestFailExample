using FluentAssertions;
using HashiCorp.Cdktf;

namespace IteratorStack.Tests;

public class IteratorStackTests
{
    private static readonly IteratorStack MainStack = new(Testing.App(), "iterator-infra");

    private static readonly string Synthesized = Testing.Synth(MainStack);

    [Fact]
    public void CheckValidity()
    {
        Testing.ToBeValidTerraform(Testing.FullSynth(MainStack)).Should().BeTrue();
    }

}
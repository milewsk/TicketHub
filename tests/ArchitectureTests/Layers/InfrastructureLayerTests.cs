using NetArchTest.Rules;

using Shouldly;

namespace ArchitectureTests.Layers;

public class InfrastructureLayerTests : BaseTest
{
    [Fact]
    public void InfrastructureLayer_Should_NotHaveDependencyOn_PresentationLayer()
    {
        TestResult result = Types.InAssembly(InfrastructureAssembly)
            .Should()
            .NotHaveDependencyOn("Web")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }
}
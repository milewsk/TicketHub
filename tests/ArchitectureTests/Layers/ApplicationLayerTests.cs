using NetArchTest.Rules;

using Shouldly;

namespace ArchitectureTests.Layers;

public class ApplicationLayerTests : BaseTest
{
    [Fact]
    public void ApplicationLayer_Should_NotHaveDependencyOn_InfrastructureLayer()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn("Infrastructure")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void ApplicationLayer_Should_NotHaveDependencyOn_PresentationLayer()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn("Web")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }
}
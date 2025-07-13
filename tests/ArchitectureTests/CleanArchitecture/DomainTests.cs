using System.Reflection;

using Common;

using NetArchTest.Rules;

using Shouldly;

namespace ArchitectureTests.CleanArchitecture;

public class DomainTests : BaseTest
{
    [Fact]
    public void DomainEvents_Should_BeSealed()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .BeSealed()
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void DomainEvents_Should_HaveDomainEventPostfix()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .HaveNameEndingWith("DomainEvent")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Entities_Should_HaveParameterlessConstructor()
    {
        var entityTypes = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes();

        var failures = new List<Type>();
        foreach (var entityType in entityTypes)
        {
            var constructors = entityType.GetConstructors(BindingFlags.Instance
                                                          | BindingFlags.NonPublic);

            if (constructors.Any(c => c.IsPrivate && c.GetParameters().Length == 0))
            {
                failures.Add(entityType);
            }
        }

        failures.ShouldBeEmpty();
    }
}
#nullable enable

namespace johnverbiest.belgian.insz.tests.TestArchitecture
{
    public sealed record TestCase(
        string? TestName,
        string? TestDescription,
        string Input,
        Expected Expected,
        string Because,
        string? Name = null,
        string? Description = null
    );
}
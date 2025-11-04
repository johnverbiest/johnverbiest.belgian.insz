namespace johnverbiest.belgian.insz.tests.TestArchitecture
{
    public sealed record TestCase(
        string Input,
        Expected Expected,
        string Because
    );
}
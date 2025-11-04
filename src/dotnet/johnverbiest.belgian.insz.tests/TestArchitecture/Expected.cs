namespace johnverbiest.belgian.insz.tests.TestArchitecture
{
    public sealed record Expected(
        bool? IsValid = null,
        string? Sex = null,
        bool? IsBis = null,
        string? BirthDate = null,
        int? Century = null
    );
}
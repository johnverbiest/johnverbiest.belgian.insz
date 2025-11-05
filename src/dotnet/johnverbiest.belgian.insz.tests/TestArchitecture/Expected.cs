#nullable enable

using System;
using System.Globalization;

namespace johnverbiest.belgian.insz.tests.TestArchitecture
{
    public sealed record Expected
    {
        public bool? IsValid { get; init; }
        public string? BirthDate { get; init; }
        public DateTime? BirthDateTime => BirthDate != null ? DateTime.ParseExact(BirthDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) : null;
        public short? BirthYear { get; init; }
        public bool? IsBis { get; init; }
    }
}
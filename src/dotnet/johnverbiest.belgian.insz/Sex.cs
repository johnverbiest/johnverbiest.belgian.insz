﻿namespace johnverbiest.belgian.insz;

/// <summary>
/// Represents the sex/gender that can be determined from a Belgian INSZ number.
/// </summary>
public enum Sex
{
    /// <summary>
    /// Sex is unknown or cannot be determined (regular INSZ numbers, or BIS numbers with unknown sex).
    /// </summary>
    Unknown,
    
    /// <summary>
    /// Female (determined from BIS numbers with known sex when sequence number is even).
    /// </summary>
    Female,
    
    /// <summary>
    /// Male (determined from BIS numbers with known sex when sequence number is odd).
    /// </summary>
    Male
}
using System;

namespace BWChaos;

internal class ChaosModStartupException : Exception
{
    public ChaosModStartupException() : base($"Illegal environment path '{MelonLoader.MelonUtils.GameDirectory}'", new Exception("Failed validating local path, try installing BONEWORKS on C:")) { }
}

internal class ChaosModRuntimeException : ChaosModStartupException
{
    public ChaosModRuntimeException() : base() { }
}

internal class ChaosModDependencyFailedException : Exception
{
    public ChaosModDependencyFailedException(string expected, string got) : base($"A dependency failed to return an expected value (exptcted \"{expected}\", got \"{got}\")") { }
}


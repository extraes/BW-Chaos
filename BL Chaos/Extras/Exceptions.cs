namespace BLChaos;

internal class ChaosModStartupException : Exception
{
    public ChaosModStartupException() : base($"Illegal environment path '{MelonLoader.Utils.MelonEnvironment.GameRootDirectory}'", new Exception("Failed validating local path, try installing the game on your C: drive.")) { }
}

internal class ChaosModRuntimeException : ChaosModStartupException
{
    public ChaosModRuntimeException() : base() { }
}

internal class ChaosModDependencyFailedException : Exception
{
    public ChaosModDependencyFailedException(string expected, string got) : base($"A dependency failed to return an expected value (expected \"{expected}\", got \"{got}\")") { }
}


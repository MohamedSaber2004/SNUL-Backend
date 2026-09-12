using SNUL.Shared.Common.Options;

namespace SNUL.Shared.Common.Interfaces
{
    /// <summary>
    /// Resolves the per-system Welco target (endpoint, secret, market) from a dashboard
    /// discriminator. Single Responsibility: system -&gt; target mapping only.
    /// </summary>
    public interface IWelcoSystemResolver
    {
        /// <summary>Normalizes raw input (?system= / X-System) to a known system key.</summary>
        string Normalize(string? system);

        /// <summary>Returns the target for a system key, falling back to legacy flat config.</summary>
        WelcoSystemTarget Resolve(string? system);
    }
}

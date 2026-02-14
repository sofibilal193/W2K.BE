namespace W2K.Common.Application.Auth;

/// <summary>
/// Abstraction for session authentication throttling storage.
/// Handles tracking of failures across user, fingerprint and composite partitions.
/// </summary>
public interface IThrottlingStore
{
    /// <summary>
    /// Checks whether any partition (user, fingerprint or composite) is currently locked for the supplied <paramref name="context"/>.
    /// </summary>
    /// <param name="context">The throttling partition key set (user may be null for anonymous scenarios).</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns><c>true</c> when at least one partition is locked and the request should be short‑circuited; otherwise <c>false</c>.</returns>
    Task<bool> IsLockedAsync(ThrottlingContext context, CancellationToken cancel = default);

    /// <summary>
    /// Registers a failed validation attempt across all relevant partitions and applies / extends any lock state.
    /// Implementations should:
    /// <list type="number">
    /// <item><description>Load (or create) the failure state per partition.</description></item>
    /// <item><description>Increment counters and reset windows as needed.</description></item>
    /// <item><description>Apply exponential backoff (capped) once thresholds are crossed.</description></item>
    /// </list>
    /// </summary>
    /// <param name="context">The throttling partition key set (user may be null).</param>
    /// <param name="cancel">Cancellation token.</param>
    Task RegisterFailureAsync(ThrottlingContext context, CancellationToken cancel = default);

    /// <summary>
    /// Clears failure / lock state for all partitions in the supplied <paramref name="context"/>.
    /// Typically invoked after a successful session validation so that subsequent failures start a fresh window.
    /// </summary>
    /// <param name="context">The throttling partition key set to clear.</param>
    /// <param name="cancel">Cancellation token.</param>
    Task ClearAsync(ThrottlingContext context, CancellationToken cancel = default);
}

/// <summary>
/// Immutable set of throttling partition keys for a single request. Keys are pre‑hashed / normalized by the caller.
/// <para>
/// Partitions:
/// <list type="bullet">
/// <item><description><see cref="UserKey"/>: Per authenticated user (null for anonymous)</description></item>
/// <item><description><see cref="FingerprintKey"/>: Per client fingerprint (device / browser signature)</description></item>
/// <item><description><see cref="CompositeKey"/>: Combined user + fingerprint for tighter coupling when both are present</description></item>
/// </list>
/// </para>
/// </summary>
public readonly record struct ThrottlingContext(string? UserKey, string FingerprintKey, string CompositeKey)
{
    /// <summary>
    /// Indicates whether this context has a user partition (i.e. the request is authenticated).
    /// </summary>
    public bool HasUser => UserKey is not null;
}

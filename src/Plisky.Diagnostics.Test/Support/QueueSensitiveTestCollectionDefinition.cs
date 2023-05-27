namespace Plisky.Diagnostics.Test {
    using Xunit;


#pragma warning disable SA1402 // File may only contain a single type

    /// <summary>
    /// Required to single file the test cases
    /// </summary>
    [CollectionDefinition(nameof(QueueSensitiveTestCollectionDefinition), DisableParallelization = true)]
    public class QueueSensitiveTestCollectionDefinition {
    }

    /// <summary>
    /// Required to enable paralell test cases
    /// </summary>
    [CollectionDefinition(nameof(QueueSensitiveTestCollectionDefinition))]

    public class ParalellEnabledTestCollection {

    }

#pragma warning restore SA1402 // File may only contain a single type
}

namespace WebApp.Test;

[CollectionDefinition(CollectionName, DisableParallelization = true)]
#pragma warning disable S1118 // Utility classes should not have public constructors
public class NonParallelCollectionDefinitionClass
#pragma warning restore S1118 // Utility classes should not have public constructors
{
    public const string CollectionName = "Non-Parallel Collection";
}

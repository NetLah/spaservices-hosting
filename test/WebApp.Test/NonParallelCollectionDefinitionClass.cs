namespace WebApp.Test;

[CollectionDefinition(CollectionName, DisableParallelization = true)]
public class NonParallelCollectionDefinitionClass
{
    public const string CollectionName = "Non-Parallel Collection";
}

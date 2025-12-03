using System.Collections.Generic;
using Xunit;
using Yaml2Doc.Core.Models;

namespace Yaml2Doc.Core.Tests;

public class PipelineDocumentTests
{
    [Fact]
    public void CanInstantiateAndSetName()
    {
        // arrange & act
        var doc = new PipelineDocument
        {
            Name = "sample-pipeline"
        };

        // assert
        Assert.Equal("sample-pipeline", doc.Name);
    }

    [Fact]
    public void Root_IsInitializedAndCanStoreValues()
    {
        // arrange
        var doc = new PipelineDocument();

        // act
        doc.Root["jobs"] = new[] { "build", "test" };
        doc["trigger"] = "main";

        // assert
        Assert.NotNull(doc.Root);
        Assert.True(doc.Root.ContainsKey("jobs"));
        Assert.Equal(new[] { "build", "test" }, doc.Root["jobs"]);

        Assert.Equal("main", doc["trigger"]);
    }

    [Fact]
    public void Indexer_ReturnsNullForMissingKey()
    {
        // arrange
        var doc = new PipelineDocument();

        // act
        var value = doc["doesNotExist"];

        // assert
        Assert.Null(value);
    }
}

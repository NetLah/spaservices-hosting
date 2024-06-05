using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace NetLah.Extensions.SpaServices.Hosting.Test;

public class ResponseHeadersHelperTest
{
    private static ResponseHeadersOptions Parse(IConfigurationRoot? configurationRoot, string sectionName)
        => ResponseHeadersHelper.Parse(configurationRoot, sectionName);

    [Fact]
    public void DisabledTest()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:IsEnabled"] = "false",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.False(options.IsEnabled);
        Assert.Null(options.DefaultHandler);
        Assert.Empty(options.Handlers);
    }

    [Fact]
    public void EnabledTest()
    {
        var configuration = new ConfigurationManager();

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.True(options.IsEnabled);
        Assert.Null(options.DefaultHandler);
        Assert.Empty(options.Handlers);
    }

    [Fact]
    public void DefaultEmptyTest()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:x-header1"] = "value1",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        var defaultHandler = options.DefaultHandler;
        Assert.NotNull(defaultHandler);
        Assert.Empty(defaultHandler.ContentTypeMatchEq);
        Assert.Empty(defaultHandler.ContentTypeMatchContain);
        Assert.Empty(defaultHandler.ContentTypeMatchStartWith);
        Assert.Empty(defaultHandler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header1"] = "value1"
        }, defaultHandler.Headers);
    }

    [Fact]
    public void DefaultFullTest()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:ContentType"] = "text/html",
            ["ResponseHeaders:ContentTypeContain"] = "text/html2",
            ["ResponseHeaders:ContentTypeStartWith"] = "text/html3",
            ["ResponseHeaders:StatusCode"] = "201",
            ["ResponseHeaders:x-header2"] = "value2",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        var defaultHandler = options.DefaultHandler;
        Assert.NotNull(defaultHandler);
        Assert.Equal(["text/html"], defaultHandler.ContentTypeMatchEq);
        Assert.Equal<string[]>(["text/html2"], defaultHandler.ContentTypeMatchContain);
        Assert.Equal<string[]>(["text/html3"], defaultHandler.ContentTypeMatchStartWith);
        Assert.Equal([201], defaultHandler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header2"] = "value2"
        }, defaultHandler.Headers);
    }

    [Fact]
    public void EntryEmptyTest()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:0:x-header3"] = "value3",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.Null(options.DefaultHandler);
        var handler = Assert.Single(options.Handlers);

        Assert.Empty(handler.ContentTypeMatchEq);
        Assert.Empty(handler.ContentTypeMatchContain);
        Assert.Empty(handler.ContentTypeMatchStartWith);
        Assert.Empty(handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header3"] = "value3"
        }, handler.Headers);
    }

    [Fact]
    public void EntryFullTest()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:0:ContentType"] = "text/html4",
            ["ResponseHeaders:0:ContentTypeContain"] = "text/html5",
            ["ResponseHeaders:0:ContentTypeStartWith"] = "text/html6",
            ["ResponseHeaders:0:StatusCode"] = "202",
            ["ResponseHeaders:0:x-header4"] = "value4",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.Null(options.DefaultHandler);
        var handler = Assert.Single(options.Handlers);

        Assert.Equal(["text/html4"], handler.ContentTypeMatchEq);
        Assert.Equal<string[]>(["text/html5"], handler.ContentTypeMatchContain);
        Assert.Equal<string[]>(["text/html6"], handler.ContentTypeMatchStartWith);
        Assert.Equal([202], handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header4"] = "value4"
        }, handler.Headers);
    }

    [Fact]
    public void FullTest()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:ContentType"] = "text/html7",
            ["ResponseHeaders:ContentTypeContain"] = "text/html8",
            ["ResponseHeaders:ContentTypeStartWith"] = "text/html9",
            ["ResponseHeaders:StatusCode"] = "203",
            ["ResponseHeaders:x-header5"] = "value5"
            ,
            ["ResponseHeaders:0:ContentType"] = "text/html10",
            ["ResponseHeaders:0:ContentTypeContain"] = "text/html11",
            ["ResponseHeaders:0:ContentTypeStartWith"] = "text/html12",
            ["ResponseHeaders:0:StatusCode"] = "204",
            ["ResponseHeaders:0:x-header6"] = "value6",

            ["ResponseHeaders:1:ContentType"] = "text/html13",
            ["ResponseHeaders:1:ContentTypeContain"] = "text/html14",
            ["ResponseHeaders:1:ContentTypeStartWith"] = "text/html15",
            ["ResponseHeaders:1:StatusCode"] = "205",
            ["ResponseHeaders:1:x-header7"] = "value7",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        var defaultHandler = options.DefaultHandler;
        Assert.NotNull(defaultHandler);
        Assert.Equal(["text/html7"], defaultHandler.ContentTypeMatchEq);
        Assert.Equal<string[]>(["text/html8"], defaultHandler.ContentTypeMatchContain);
        Assert.Equal<string[]>(["text/html9"], defaultHandler.ContentTypeMatchStartWith);
        Assert.Equal([203], defaultHandler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header5"] = "value5"
        }, defaultHandler.Headers);

        Assert.Equal(2, options.Handlers.Length);

        var handler0 = options.Handlers[0];
        Assert.Equal(["text/html10"], handler0.ContentTypeMatchEq);
        Assert.Equal<string[]>(["text/html11"], handler0.ContentTypeMatchContain);
        Assert.Equal<string[]>(["text/html12"], handler0.ContentTypeMatchStartWith);
        Assert.Equal([204], handler0.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header6"] = "value6"
        }, handler0.Headers);


        var handler1 = options.Handlers[1];
        Assert.Equal(["text/html13"], handler1.ContentTypeMatchEq);
        Assert.Equal<string[]>(["text/html14"], handler1.ContentTypeMatchContain);
        Assert.Equal<string[]>(["text/html15"], handler1.ContentTypeMatchStartWith);
        Assert.Equal([205], handler1.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header7"] = "value7"
        }, handler1.Headers);
    }

    [Fact]
    public void ContentTypeMatchEq1Test()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:ContentType"] = "text/html16",
            ["ResponseHeaders:x-header8"] = "value8",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.NotNull(options.DefaultHandler);
        Assert.Empty(options.Handlers);

        var handler = options.DefaultHandler;
        Assert.NotNull(handler);
        Assert.Equal(["text/html16"], handler.ContentTypeMatchEq);
        Assert.Empty(handler.ContentTypeMatchContain);
        Assert.Empty(handler.ContentTypeMatchStartWith);
        Assert.Empty(handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header8"] = "value8"
        }, handler.Headers);
    }

    [Fact]
    public void ContentTypeMatchEq2Test()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:0:ContentType:0"] = "text/html17",
            ["ResponseHeaders:0:x-header9"] = "value9",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.Null(options.DefaultHandler);
        var handler = Assert.Single(options.Handlers);

        Assert.NotNull(handler);
        Assert.Equal(["text/html17"], handler.ContentTypeMatchEq);
        Assert.Empty(handler.ContentTypeMatchContain);
        Assert.Empty(handler.ContentTypeMatchStartWith);
        Assert.Empty(handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header9"] = "value9"
        }, handler.Headers);
    }

    [Fact]
    public void ContentTypeMatchEq3Test()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:ContentType:0"] = "text/html18",
            ["ResponseHeaders:ContentType:1"] = "text/plain19",
            ["ResponseHeaders:x-header10"] = "value10",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.NotNull(options.DefaultHandler);
        Assert.Empty(options.Handlers);

        var handler = options.DefaultHandler;
        Assert.NotNull(handler);
        Assert.Equal(["text/html18", "text/plain19"], handler.ContentTypeMatchEq);
        Assert.Empty(handler.ContentTypeMatchContain);
        Assert.Empty(handler.ContentTypeMatchStartWith);
        Assert.Empty(handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header10"] = "value10"
        }, handler.Headers);
    }

    [Fact]
    public void ContentTypeContain1Test()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:0:ContentTypeContain"] = "text/html20",
            ["ResponseHeaders:0:x-header11"] = "value11",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.Null(options.DefaultHandler);
        var handler = Assert.Single(options.Handlers);

        Assert.NotNull(handler);
        Assert.Empty(handler.ContentTypeMatchEq);
        Assert.Equal<string[]>(["text/html20"], handler.ContentTypeMatchContain);
        Assert.Empty(handler.ContentTypeMatchStartWith);
        Assert.Empty(handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header11"] = "value11"
        }, handler.Headers);
    }

    [Fact]
    public void ContentTypeContain2Test()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:ContentTypeContain:0"] = "text/html21",
            ["ResponseHeaders:x-header12"] = "value12",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.NotNull(options.DefaultHandler);
        Assert.Empty(options.Handlers);

        var handler = options.DefaultHandler;
        Assert.NotNull(handler);
        Assert.Empty(handler.ContentTypeMatchEq);
        Assert.Equal<string[]>(["text/html21"], handler.ContentTypeMatchContain);
        Assert.Empty(handler.ContentTypeMatchStartWith);
        Assert.Empty(handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header12"] = "value12"
        }, handler.Headers);
    }

    [Fact]
    public void ContentTypeContain3Test()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:0:ContentTypeContain:0"] = "text/html22",
            ["ResponseHeaders:0:ContentTypeContain:1"] = "text/plain23",
            ["ResponseHeaders:0:x-header13"] = "value13",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.Null(options.DefaultHandler);
        var handler = Assert.Single(options.Handlers);

        Assert.NotNull(handler);
        Assert.Empty(handler.ContentTypeMatchEq);
        Assert.Equal<string[]>(["text/html22", "text/plain23"], handler.ContentTypeMatchContain);
        Assert.Empty(handler.ContentTypeMatchStartWith);
        Assert.Empty(handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header13"] = "value13"
        }, handler.Headers);
    }

    [Fact]
    public void ContentTypeStartWith1Test()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:ContentTypeStartWith"] = "text/html24",
            ["ResponseHeaders:x-header14"] = "value14",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.NotNull(options.DefaultHandler);
        Assert.Empty(options.Handlers);

        var handler = options.DefaultHandler;
        Assert.NotNull(handler);
        Assert.Empty(handler.ContentTypeMatchEq);
        Assert.Empty(handler.ContentTypeMatchContain);
        Assert.Equal<string[]>(["text/html24"], handler.ContentTypeMatchStartWith);
        Assert.Empty(handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header14"] = "value14"
        }, handler.Headers);
    }

    [Fact]
    public void ContentTypeStartWith2Test()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:0:ContentTypeStartWith:0"] = "text/html25",
            ["ResponseHeaders:0:x-header15"] = "value15",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.Null(options.DefaultHandler);
        var handler = Assert.Single(options.Handlers);

        Assert.NotNull(handler);
        Assert.Empty(handler.ContentTypeMatchEq);
        Assert.Empty(handler.ContentTypeMatchContain);
        Assert.Equal<string[]>(["text/html25"], handler.ContentTypeMatchStartWith);
        Assert.Empty(handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header15"] = "value15"
        }, handler.Headers);
    }

    [Fact]
    public void ContentTypeStartWith3Test()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:ContentTypeStartWith:0"] = "text/html26",
            ["ResponseHeaders:ContentTypeStartWith:1"] = "text/plain27",
            ["ResponseHeaders:x-header16"] = "value16",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.NotNull(options.DefaultHandler);
        Assert.Empty(options.Handlers);

        var handler = options.DefaultHandler;
        Assert.NotNull(handler);
        Assert.Empty(handler.ContentTypeMatchEq);
        Assert.Empty(handler.ContentTypeMatchContain);
        Assert.Equal<string[]>(["text/html26", "text/plain27"], handler.ContentTypeMatchStartWith);
        Assert.Empty(handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header16"] = "value16"
        }, handler.Headers);
    }

    [Fact]
    public void StatusCode1Test()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:0:StatusCode"] = "206",
            ["ResponseHeaders:0:x-header17"] = "value17",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.Null(options.DefaultHandler);
        var handler = Assert.Single(options.Handlers);

        Assert.NotNull(handler);
        Assert.Empty(handler.ContentTypeMatchEq);
        Assert.Empty(handler.ContentTypeMatchContain);
        Assert.Empty(handler.ContentTypeMatchStartWith);
        Assert.Equal([206], handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header17"] = "value17"
        }, handler.Headers);
    }

    [Fact]
    public void StatusCode2Test()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:StatusCode:0"] = "207",
            ["ResponseHeaders:x-header18"] = "value18",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.NotNull(options.DefaultHandler);
        Assert.Empty(options.Handlers);

        var handler = options.DefaultHandler;
        Assert.NotNull(handler);
        Assert.Empty(handler.ContentTypeMatchEq);
        Assert.Empty(handler.ContentTypeMatchContain);
        Assert.Empty(handler.ContentTypeMatchStartWith);
        Assert.Equal([207], handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header18"] = "value18"
        }, handler.Headers);
    }

    [Fact]
    public void StatusCode3Test()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:0:StatusCode:0"] = "208",
            ["ResponseHeaders:0:StatusCode:1"] = "209",
            ["ResponseHeaders:0:x-header19"] = "value19",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.Null(options.DefaultHandler);
        var handler = Assert.Single(options.Handlers);

        Assert.NotNull(handler);
        Assert.Empty(handler.ContentTypeMatchEq);
        Assert.Empty(handler.ContentTypeMatchContain);
        Assert.Empty(handler.ContentTypeMatchStartWith);
        Assert.Equal([208, 209], handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header19"] = "value19"
        }, handler.Headers);
    }

    [Fact]
    public void Header2Test()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:Headers:x-header20"] = "value20",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.NotNull(options.DefaultHandler);
        Assert.Empty(options.Handlers);

        var handler = options.DefaultHandler;
        Assert.NotNull(handler);
        Assert.Empty(handler.ContentTypeMatchEq);
        Assert.Empty(handler.ContentTypeMatchContain);
        Assert.Empty(handler.ContentTypeMatchStartWith);
        Assert.Empty(handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header20"] = "value20"
        }, handler.Headers);
    }

    [Fact]
    public void Header3Test()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:0:Headers:0"] = "x-header21=value21",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.Null(options.DefaultHandler);
        var handler = Assert.Single(options.Handlers);

        Assert.NotNull(handler);
        Assert.Empty(handler.ContentTypeMatchEq);
        Assert.Empty(handler.ContentTypeMatchContain);
        Assert.Empty(handler.ContentTypeMatchStartWith);
        Assert.Empty(handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header21"] = "value21"
        }, handler.Headers);
    }

    [Fact]
    public void Header4Test()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:Headers:0"] = "x-header22=value22",
            ["ResponseHeaders:Headers:1"] = "x-header23=value23",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.NotNull(options.DefaultHandler);
        Assert.Empty(options.Handlers);

        var handler = options.DefaultHandler;
        Assert.NotNull(handler);
        Assert.Empty(handler.ContentTypeMatchEq);
        Assert.Empty(handler.ContentTypeMatchContain);
        Assert.Empty(handler.ContentTypeMatchStartWith);
        Assert.Empty(handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header22"] = "value22",
            ["x-header23"] = "value23"
        }, handler.Headers);
    }

    [Fact]
    public void HeaderAllTest()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:0:Headers:1"] = "x-header26=value25",
            ["ResponseHeaders:0:Headers:0"] = "x-header27=value24",
            ["ResponseHeaders:0:x-header25"] = "value26",
            ["ResponseHeaders:0:Headers:x-header24"] = "value27",
            ["ResponseHeaders:0:header28"] = "value28",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.Null(options.DefaultHandler);
        var handler = Assert.Single(options.Handlers);

        Assert.NotNull(handler);
        Assert.Empty(handler.ContentTypeMatchEq);
        Assert.Empty(handler.ContentTypeMatchContain);
        Assert.Empty(handler.ContentTypeMatchStartWith);
        Assert.Empty(handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header27"] = "value24",
            ["x-header26"] = "value25",
            ["header28"] = "value28",
            ["x-header24"] = "value27",
            ["x-header25"] = "value26",
        }, handler.Headers);
    }

    [Fact]
    public void HeaderKeyValueWithEqualTest()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ResponseHeaders:Headers:x-header28"] = "value29=hex",
        });

        var options = Parse(configuration, "ResponseHeaders");

        Assert.NotNull(options);
        Assert.NotNull(options.DefaultHandler);
        Assert.Empty(options.Handlers);

        var handler = options.DefaultHandler;
        Assert.NotNull(handler);
        Assert.Empty(handler.ContentTypeMatchEq);
        Assert.Empty(handler.ContentTypeMatchContain);
        Assert.Empty(handler.ContentTypeMatchStartWith);
        Assert.Empty(handler.StatusCode);
        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header28"] = "value29=hex",
        }, handler.Headers);
    }
}

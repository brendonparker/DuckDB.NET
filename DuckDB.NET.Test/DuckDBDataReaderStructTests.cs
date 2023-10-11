﻿using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace DuckDB.NET.Test;

public class DuckDBDataReaderStructTests : DuckDBTestBase
{
    public DuckDBDataReaderStructTests(DuckDBDatabaseFixture db) : base(db)
    {
    }

    [Fact]
    public void ReadBasicStruct()
    {
        Command.CommandText = "SELECT {'x': 1, 'y': 2, 'z': 'test'};";
        using var reader = Command.ExecuteReader();
        reader.Read();

        var value = reader.GetFieldValue<Struct1>(0);
        value.Should().BeEquivalentTo(new Struct1
        {
            X = 1,
            Y = 2,
            Z = "test"
        });
    }

    [Fact]
    public void ReadBasicStructWithGetValue()
    {
        Command.CommandText = "SELECT {'x': 1, 'y': 2, 'z': 'test'};";
        using var reader = Command.ExecuteReader();
        reader.Read();

        var value = reader.GetValue(0);
        value.Should().BeEquivalentTo(new Dictionary<string, object> { { "x", 1 }, { "y", 2 }, { "z", "test" } });
    }

    [Fact]
    public void ReadBasicStructList()
    {
        Command.CommandText = "SELECT [{'x': 1, 'y': 2, 'z': 'test'}, {'x': 4, 'y': 3, 'z': 'tset'}, NULL];";
        using var reader = Command.ExecuteReader();
        reader.Read();

        var value = reader.GetFieldValue<List<Struct1>>(0);
        value.Should().BeEquivalentTo(new List<Struct1>
        {
            new() { X = 1, Y = 2, Z = "test" },
            new() { X = 4, Y = 3, Z = "tset" },
            null
        });
    }

    [Fact]
    public void ReadBasicStructListWithGetValue()
    {
        Command.CommandText = "SELECT [{'x': 1, 'y': 2, 'z': 'test'}, {'x': 4, 'y': 3, 'z': 'tset'}, NULL];";
        using var reader = Command.ExecuteReader();
        reader.Read();

        var value = reader.GetValue(0);
        value.Should().BeEquivalentTo(new List<Dictionary<string, object>>
        {
            new() { { "x", 1 }, { "y", 2 }, { "z", "test" } },
            new() { { "x", 4 }, { "y", 3 }, { "z", "tset" } },
            null
        });
    }

    [Fact]
    public void ReadStructWithNull()
    {
        Command.CommandText = "SELECT {'yes': 'duck', 'maybe': 'goose', 'huh': NULL, 'no': 'heron', 'type': 0} Union All " +
                              "SELECT {'yes': 'duck', 'maybe': 'goose', 'huh': 'bird', 'no': 'heron', 'type':1};";
        using var reader = Command.ExecuteReader();

        reader.Read();
        var value = reader.GetFieldValue<Struct2>(0);

        value.Should().BeEquivalentTo(new Struct2
        {
            Huh = null,
            Maybe = "goose",
            No = "heron",
            Yes = "duck",
            Type = 0
        });

        reader.Read();
        value = reader.GetFieldValue<Struct2>(0);

        value.Should().BeEquivalentTo(new Struct2
        {
            Huh = "bird",
            Maybe = "goose",
            No = "heron",
            Yes = "duck",
            Type = 1
        });
    }

    class Struct1
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Z { get; set; }
        public int XY { get; }
    }

    class Struct2
    {
        public string Yes { get; set; }
        public string Maybe { get; set; }
        public string Huh { get; set; }
        public string No { get; set; }
        public int Type { get; set; }
    }
}
namespace Quarrel.FSharp.UnitTests

module Tests = 

    open System.Text.Json
    open Xunit
    open Quarrel

    [<Fact>]
    let ``Test OfElements``() = 
        use d1 = JsonDocument.Parse("true")
        use d2 = JsonDocument.Parse("false")
        let e1 = d1.RootElement
        let e2 = d2.RootElement
        let diffs = JsonDiff.OfElements(e1, e2) |> Seq.toList
        Assert.NotEmpty(diffs);
        match diffs.Head with 
        | Kind kindDiff -> 
            Assert.Equal("$", kindDiff.Path);
            Assert.True(kindDiff.Left.GetBoolean());
            Assert.False(kindDiff.Right.GetBoolean());
        | _ -> failwith "Wrong diff"

    [<Fact>]
    let ``Test OfDocuments``() = 
        use d1 = JsonDocument.Parse("true")
        use d2 = JsonDocument.Parse("false")
        let diffs = JsonDiff.OfDocuments(d1, d2) |> Seq.toList
        Assert.NotEmpty(diffs);
        match diffs.Head with 
        | Kind kindDiff -> 
            Assert.Equal("$", kindDiff.Path);
            Assert.True(kindDiff.Left.GetBoolean());
            Assert.False(kindDiff.Right.GetBoolean());
        | _ -> failwith "Wrong diff"
    
    [<Fact>]
    let ``Test OfStrings true vs false``() = 
        let diffs = JsonDiff.OfStrings("true", "false") |> Seq.toList
        Assert.NotEmpty(diffs);
        match diffs.Head with 
        | Kind kindDiff -> 
            Assert.Equal("$", kindDiff.Path);
            Assert.True(kindDiff.Left.GetBoolean());
            Assert.False(kindDiff.Right.GetBoolean());
        | _ -> failwith "Wrong diff"


    [<Fact>]
    let ``Test OfStrings 1 vs 2``() = 
        let diffs = JsonDiff.OfStrings("1", "2") |> Seq.toList
        Assert.NotEmpty(diffs);
        match diffs.Head with 
        | Value valueDiff -> 
            Assert.Equal("$", valueDiff.Path);
            Assert.Equal(1, valueDiff.Left.GetInt32());
            Assert.Equal(2, valueDiff.Right.GetInt32());
        | _ -> failwith "Wrong diff"

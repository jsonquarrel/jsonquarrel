namespace Quarrel.FSharp.UnitTests

module JsonStringsTests = 

    open Xunit
    open Quarrel

    [<Fact>]
    let ``true vs false``() = 
        let diffMessage = JsonStrings.Diff("true", "false") |> Seq.head
        Assert.Equal("Boolean value mismatch at $.\nExpected false but was true.", diffMessage)

    [<Fact>]
    let ``1 vs 2``() = 
        let diffMessage = JsonStrings.Diff("1", "2") |> Seq.head
        Assert.Equal("Number value mismatch at $.\nExpected 2 but was 1.", diffMessage)

    [<Fact>]
    let ``true vs 1``() = 
        let diffMessage = JsonStrings.Diff("true", "1") |> Seq.head
        Assert.Equal("Kind mismatch at $.\nExpected the number 1 but was the boolean true.", diffMessage)

    [<Fact>]
    let ``null vs 1``() = 
        let diffMessage = JsonStrings.Diff("null", "1") |> Seq.head
        Assert.Equal("Kind mismatch at $.\nExpected the number 1 but was null.", diffMessage)

    [<Fact>]
    let ``Empty array vs null``() = 
        let diffMessage = JsonStrings.Diff("[]", "null") |> Seq.head
        Assert.Equal("Kind mismatch at $.\nExpected null but was an empty array.", diffMessage)
            
    [<Fact>]
    let ``Empty array vs empty object``() = 
        let diffMessage = JsonStrings.Diff("[]", "{}") |> Seq.head
        Assert.Equal("Kind mismatch at $.\nExpected an object but was an empty array.", diffMessage)

    [<Fact>]
    let ``[ 1 ] vs [ 2 ]``() = 
        let diffMessage = JsonStrings.Diff("[ 1 ]", "[ 2 ]") |> Seq.head
        Assert.Equal("Number value mismatch at $[0].\nExpected 2 but was 1.", diffMessage)

    [<Fact>]
    let ``[ 1 ] vs [ 1, 2 ]``() =
        let diffs = JsonStrings.Diff("[ 1 ]", "[ 1, 2 ]") |> Seq.toList
        match diffs with 
        | [ diffMessage ] -> Assert.Equal("Array length mismatch at $.\nExpected 2 items but was 1.", diffMessage)
        | _ -> failwith "Wrong number of diffs"

    [<Fact>]
    let ``[ 1 ] vs [ 2, 1 ]``() = 
        let diffMessage = JsonStrings.Diff("[ 1 ]", "[ 2, 1 ]") |> Seq.head
        Assert.Equal("Array length mismatch at $.\nExpected 2 items but was 1.", diffMessage)
            
    [<Fact>]
    let ``[ 2, 1 ] vs [ 1, 2 ]``() =
        let diffs = JsonStrings.Diff("[ 2, 1 ]", "[ 1, 2 ]") |> Seq.toList
        match diffs with
        | [ diff1; diff2 ] ->
            Assert.Equal("Number value mismatch at $[0].\nExpected 1 but was 2.", diff1)        
            Assert.Equal("Number value mismatch at $[1].\nExpected 2 but was 1.", diff2)        
        | _ -> failwith "Wrong number of diffs"

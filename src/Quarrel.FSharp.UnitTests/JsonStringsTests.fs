namespace Quarrel.FSharp.UnitTests

module JsonStringsTests = 

    open Xunit
    open Quarrel

    [<Fact>]
    let ``true = true``() = 
        let diffs = JsonStrings.Diff("true", "true")
        Assert.Empty(diffs)

    [<Fact>]
    let ``false = false``() = 
        let diffs = JsonStrings.Diff("false", "false")
        Assert.Empty(diffs)

    [<Fact>]
    let ``0 = 0``() = 
        let diffs = JsonStrings.Diff("0", "0")
        Assert.Empty(diffs)

    [<Fact>]
    let ``0 = -0``() = 
        let diffs = JsonStrings.Diff("0", "-0")
        Assert.Empty(diffs)
        
    [<Fact>]
    let ``1000 = 1E3``() = 
        let diffs = JsonStrings.Diff("1000", "1E3")
        Assert.Empty(diffs)

    [<Fact>]
    let ``1 != 2``() = 
        let diffMessage = JsonStrings.Diff("1", "2") |> Seq.head
        Assert.Equal("Number value mismatch at $.\nExpected 2 but was 1.", diffMessage)
        
    [<Fact>]
    let ``1 != 1E1``() = 
        let diffMessage = JsonStrings.Diff("1", "1E1") |> Seq.head
        Assert.Equal("Number value mismatch at $.\nExpected 1E1 but was 1.", diffMessage)
        
    [<Fact>]
    let ``1 = 1.0``() =
        let diffs = JsonStrings.Diff("1", "1.0")
        Assert.Empty(diffs)

    [<Fact>]
    let ``true vs 1``() =
        let diffs = JsonStrings.Diff("true", "1") |> Seq.toList
        match diffs with
        | [ diff ] -> Assert.Equal("Kind mismatch at $.\nExpected the number 1 but was the boolean true.", diff)
        | _ -> failwith "Wrong number of diffs"

    [<Fact>]
    let ``"foo" = "foo"``() = 
        let diffs = JsonStrings.Diff("\"foo\"", "\"foo\"")
        Assert.Empty(diffs)

    [<Fact>]
    let ``"foo" != "bar"``() = 
        let diffs = JsonStrings.Diff("\"foo\"", "\"bar\"") |> Seq.toList
        match diffs with
        | [ diff ] -> Assert.Equal("String value mismatch at $.\nExpected bar but was foo.", diff)
        | _ -> failwith "Expected 1 diff but was %d." (List.length diffs)
        
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
        let diffs = JsonStrings.Diff("[]", "{}") |> Seq.toList
        match diffs with
        | [ diff ] -> Assert.Equal("Kind mismatch at $.\nExpected an object but was an empty array.", diff)
        | _ -> failwith "Wrong number of diffs"

    [<Fact>]
    let ``[ 1 ] vs [ 2 ]``() = 
        let diffs = JsonStrings.Diff("[ 1 ]", "[ 2 ]") |> Seq.toList
        match diffs with
        | [ diff ] -> Assert.Equal("Number value mismatch at $[0].\nExpected 2 but was 1.", diff)
        | _ -> failwith "Wrong number of diffs"

    [<Fact>]
    let ``[ 1 ] vs [ 1, 2 ]``() =
        let diffs = JsonStrings.Diff("[ 1 ]", "[ 1, 2 ]") |> Seq.toList
        match diffs with 
        | [ diffMessage ] -> Assert.Equal("Array length mismatch at $.\nExpected 2 items but was 1.", diffMessage)
        | _ -> failwith "Wrong number of diffs"

    [<Fact>]
    let ``[ 1 ] vs [ 2, 1 ]``() = 
        let diffs = JsonStrings.Diff("[ 1 ]", "[ 2, 1 ]") |> Seq.toList
        match diffs with
        | [ diffMessage ] -> Assert.Equal("Array length mismatch at $.\nExpected 2 items but was 1.", diffMessage)
        | _ -> failwith "Wrong number of diffs"
            
    [<Fact>]
    let ``[ 2, 1 ] vs [ 1, 2 ]``() =
        let diffs = JsonStrings.Diff("[ 2, 1 ]", "[ 1, 2 ]") |> Seq.toList
        match diffs with
        | [ diff1; diff2 ] ->
            Assert.Equal("Number value mismatch at $[0].\nExpected 1 but was 2.", diff1)        
            Assert.Equal("Number value mismatch at $[1].\nExpected 2 but was 1.", diff2)
        | _ -> failwithf "Expected 2 diffs but was %d." (List.length diffs)
        
    [<Fact>]
    let ``{} != { "count": 0 }``() =
        let diffs = JsonStrings.Diff("{}", "{ \"count\": 0 }") |> Seq.toList
        match diffs with
        | [ diff ] ->
            Assert.Equal("Object mismatch at $.\nMissing property:\ncount (number).", diff)        
        | _ -> failwithf "Expected 1 diff but got %d." (List.length diffs)

    [<Fact>]
    let ``{ "count": 0 } != {}``() =
        let diffs = JsonStrings.Diff("{ \"count\": 0 }", "{}") |> Seq.toList
        match diffs with
        | [ diff ] ->
            Assert.Equal("Object mismatch at $.\nAdditional property:\ncount (number).", diff)        
        | _ -> failwithf "Expected 1 diff but was %d." (List.length diffs)
        
    [<Fact>]
    let ``{ "age": 20, "name": "Don" } = { "name": "Don", "age": 20 }``() =
        let diffs = JsonStrings.Diff("{ \"age\": 20, \"name\": \"Don\" }", "{ \"name\": \"Don\", \"age\": 20 }")
        Assert.Empty(diffs)

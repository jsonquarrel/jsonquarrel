module JsonQuarrel

open System.Text.Json
open System.Text.RegularExpressions

type PathElement =
    | PropertyPathElement of string
    | IndexPathElement of int

type PropertyMismatch =
    | MissingProperty of string
    | AdditionalProperty of string

type Diff =
    | KindDiff of (string * JsonElement * JsonElement)
    | ValueDiff of (string * JsonElement * JsonElement)
    | PropertiesDiff of (string * JsonElement * JsonElement * PropertyMismatch list)
    | ItemCountDiff of (string * JsonElement * JsonElement)

let private toJsonPath (path : PathElement list) : string =
    let (|Dot|Bracket|) s =
        if Regex.IsMatch(s, "^[a-zA-Z0-9]+$") then Dot
        else Bracket

    let elementToString =
        function
        | PropertyPathElement p ->
            match p with
            | Dot -> sprintf ".%s" p
            | Bracket -> sprintf "['%s']" p
        | IndexPathElement i -> sprintf "[%d]" i

    path
    |> List.fold (fun str elm -> sprintf "%s%s" str (elementToString elm)) ""
    |> sprintf "$%s"

let rec private findDiff (path : PathElement list) (element1 : JsonElement)
        (element2 : JsonElement) : Diff list =
    if (element1.ValueKind <> element2.ValueKind) then
        [ KindDiff(toJsonPath path, element1, element2) ]
    else
        match element1.ValueKind with
        | JsonValueKind.Array ->
            (* order matters. *)
            let itemsOf (e : JsonElement) : JsonElement list =
                e.EnumerateArray() |> Seq.toList
            let children1 = itemsOf element1
            let children2 = itemsOf element2
            if List.length children1 <> List.length children2 then
                [ ItemCountDiff(toJsonPath path, element1, element2) ]
            else
                let itemDiff i e1 e2 =
                    findDiff (path @ [ IndexPathElement i ]) e1 e2
                let childDiffs =
                    List.mapi2 itemDiff children1 children2 |> List.collect id
                childDiffs
        | JsonValueKind.Object ->
            (* order doesn't matter. *)
            let keys (e : JsonElement) : string list =
                e.EnumerateObject()
                |> Seq.map (fun jp -> jp.Name)
                |> Seq.toList

            let keys1 = keys element1
            let keys2 = keys element2
            let missingKeys = keys2 |> List.except keys1
            let missingProperties = missingKeys |> List.map MissingProperty
            let additionalKeys = keys1 |> List.except keys2
            let additionalProperties =
                additionalKeys |> List.map AdditionalProperty
            let mismatches = missingProperties @ additionalProperties

            let objectDiff =
                match mismatches with
                | [] -> []
                | ms ->
                    [ PropertiesDiff(toJsonPath path, element1, element2, ms) ]

            let sharedKeys : string list = keys2 |> List.except missingKeys

            let propDiff (key : string) =
                let child1 = element1.GetProperty(key)
                let child2 = element2.GetProperty(key)
                findDiff (path @ [ PropertyPathElement key ]) child1 child2

            let childDiffs = sharedKeys |> List.collect propDiff
            objectDiff @ childDiffs
        | JsonValueKind.Number ->
            let rawText1 = element1.GetRawText()
            let rawText2 = element2.GetRawText()
            if rawText1 = rawText2 then []
            else [ ValueDiff(toJsonPath path, element1, element2) ]
        | JsonValueKind.String ->
            let string1 = element1.GetString()
            let string2 = element2.GetString()
            if string1 = string2 then []
            else [ ValueDiff(toJsonPath path, element1, element2) ]
        | _ -> []

let diff = findDiff []

#r "../packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"
#r "../packages/FSharp.Data/lib/net45/FSharp.Data.dll"

open Newtonsoft.Json
open Newtonsoft.Json.Serialization

module JsonConvert =
    let toJson v =
        let jsonSerializerSettings = new JsonSerializerSettings()
        jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()

        JsonConvert.SerializeObject(v, jsonSerializerSettings)

    let fromJson<'a> json =
        JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a

open FSharp.Data

type HttpVerb = | GET | POST | PUT | DELETE | HEAD

let http (verb:HttpVerb) url data =
    let response =
        match data with
        | Some data ->
            Http.Request(url, httpMethod = sprintf "%A" verb, body = HttpRequestBody.TextRequest data)
        | None ->
            Http.Request(url, httpMethod = sprintf "%A" verb)

    match response.Body with
    | HttpResponseBody.Text text -> text
    | HttpResponseBody.Binary _ -> "<Binary data>"

type Person = {
    Id : int
    Name : string
    Age : int
    Email : string
}

// Get the list
http GET "http://localhost:8083/people" None
|> JsonConvert.fromJson<Person array>

// Create user foo
Some """{ "name": "foo", "age": 16, "email": "foo@bar.com" }"""
|> http POST "http://localhost:8083/people"
|> JsonConvert.fromJson<Person>

// Create user baz
{
    Id = 0
    Name = "baz"
    Age = 22
    Email = "qux@corge.com"
}
|> JsonConvert.toJson
|> Some
|> http POST "http://localhost:8083/people"
|> JsonConvert.fromJson<Person>

// Update user foo
{
    Id = 1
    Name = "foo'"
    Age = 24
    Email = "foo@bar.com"
}
|> JsonConvert.toJson
|> Some
|> http PUT "http://localhost:8083/people"

// Get only user baz
http GET "http://localhost:8083/people/2" None
|> JsonConvert.fromJson<Person>

// Update user baz by Id
Some """{ "name": "baz'", "age": 21, "email": "foo@bar.com" }"""
|> http PUT "http://localhost:8083/people/2"

// Check if user 2 exists
http HEAD "http://localhost:8083/people/2" None

// Delete user 2
http DELETE "http://localhost:8083/people/2" None
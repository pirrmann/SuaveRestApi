﻿namespace SuaveRestApi.Step4

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave
open Suave.Operators
open Suave.Http
open Suave.Successful
open SuaveRestApi

module People =
    open Suave.RequestErrors
    open Suave.Filters

    let private getResourceFromReq<'a> (req : HttpRequest) =
        let getString rawForm = System.Text.Encoding.UTF8.GetString(rawForm)
        req.rawForm |> getString |> fromJson<'a>

    let PeopleWebPart (repository: Repository<Person>) =
        let badRequest = BAD_REQUEST "Resource not found"

        let handleResource requestError resource =
            match resource with
            | Some r -> r |> JSON
            | _ -> requestError

        let getAll _ = repository.GetAll () |> JSON

        choose [
            path "/people" >=> choose [
                GET >=> warbler getAll
                POST >=> request (getResourceFromReq >> repository.Create >> JSON)
                PUT >=> request (getResourceFromReq >> repository.Update >> handleResource badRequest)
            ]
            GET >=> __
        ]
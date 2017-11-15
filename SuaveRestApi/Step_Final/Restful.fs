namespace SuaveRestApi.StepFinal

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave
open Suave.Operators
open Suave.Http
open Suave.Successful
open SuaveRestApi

[<AutoOpen>]
module RestFul =
    open Suave.RequestErrors
    open Suave.Filters

    let getResourceFromReq<'a> (req : HttpRequest) =
        let getString rawForm = System.Text.Encoding.UTF8.GetString(rawForm)
        req.rawForm |> getString |> fromJson<'a>

    let rest resourceName repository =

        let resourcePath = "/" + resourceName
        let resourceIdPath = new PrintfFormat<(int -> string),unit,string,string,int>(resourcePath + "/%d")

        let badRequest = BAD_REQUEST "Resource not found"

        let handleResource requestError resource =
            match resource with
            | Some r -> r |> JSON
            | _ -> requestError

        let getAll _ = repository.GetAll () |> JSON

        let getResourceById =
            repository.GetById >> handleResource (NOT_FOUND "Resource not found")

        let updateResourceById id =
            request (getResourceFromReq >> (repository.UpdateById id) >> handleResource badRequest)

        let deleteResourceById id =
            repository.Delete id
            NO_CONTENT

        let resourceExists id =
            if repository.Exists id then OK "" else NOT_FOUND ""

        choose [
            path resourcePath >=> choose [
                GET >=> warbler getAll
                POST >=> request (getResourceFromReq >> repository.Create >> JSON)
                PUT >=> request (getResourceFromReq >> repository.Update >> handleResource badRequest)
            ]
            DELETE >=> pathScan resourceIdPath deleteResourceById
            GET >=> pathScan resourceIdPath getResourceById
            PUT >=> pathScan resourceIdPath updateResourceById
            HEAD >=> pathScan resourceIdPath resourceExists
        ]
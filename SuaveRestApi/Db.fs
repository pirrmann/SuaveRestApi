namespace SuaveRestApi

open System.Collections.Generic

module Db =

    let peopleStorage = new Dictionary<int, Person>()

    let getPeople () =
        peopleStorage.Values :> seq<Person>

    let getPerson id =
        if peopleStorage.ContainsKey(id) then
            Some peopleStorage.[id]
        else
            None

    let createPerson person =
        let id = peopleStorage.Values.Count + 1
        let newPerson = {person with Id = id}
        peopleStorage.Add(id, newPerson)
        newPerson

    let updatePersonById personId personToBeUpdated =
        if peopleStorage.ContainsKey(personId) then
            let updatedPerson = {personToBeUpdated with Id = personId}
            peopleStorage.[personId] <- updatedPerson
            Some updatedPerson
        else
            None

    let updatePerson personToBeUpdated =
        updatePersonById personToBeUpdated.Id personToBeUpdated

    let deletePerson personId =
        peopleStorage.Remove(personId) |> ignore

    let personExists  = peopleStorage.ContainsKey

namespace SuaveRestApi

module App =
    open Suave
    open Suave.Web

    [<EntryPoint>]
    let main argv =

        let repository = {
            GetAll = Db.getPeople
            GetById = Db.getPerson
            Create = Db.createPerson
            Update = Db.updatePerson
            UpdateById = Db.updatePersonById
            Delete = Db.deletePerson
            Exists = Db.personExists
        }

        let app =
            choose [
                Step1.People.PeopleWebPart repository
                RequestErrors.NOT_FOUND "Found no handlers"
            ]

        startWebServer defaultConfig app

        0

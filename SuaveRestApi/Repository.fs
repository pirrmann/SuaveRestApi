namespace SuaveRestApi

type Repository<'a> = {
    GetAll : unit -> 'a seq
    GetById : int -> 'a option
    Exists : int -> bool
    Create : 'a -> 'a
    Update : 'a -> 'a option
    UpdateById : int -> 'a -> 'a option
    Delete : int -> unit
}
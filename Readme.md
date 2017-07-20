## About
[StackExchange.Exceptional](https://github.com/NickCraver/StackExchange.Exceptional) is the error handler used internally by [Stack Exchange](http://stackexchange.com) and [Stack Overflow](http://stackoverflow.com).

After working with (and tweaking) the standard `SqlErrorStore` for a while, I decided that I wanted to customize things a bit more without having to edit the SQL statements embedded in `SqlErrorStore`. I was already using Entity Framework (Code First, specifically) to access my database and I thought it would be much cleaner to have all of my database access handled through EF. So I set out to create a different kind of `ErrorStore` that would provide an abstraction layer where I would be free to just create a simple entity class that would represent my errors in the database and exclusively use EF for communicating with the database. This was how the `EntityErrorStore` came to be.

The `EntityErrorStore` inherits from `ErrorStore` and performs all the functions of the other stores, but it does so by communicating with a user created object that implements the `IPersistenceProvider` interface, which largely imitates the abstract functions of `ErrorStore`, except that instead of creating `Error` objects, it returns objects that implement the `IPersistedError` interface, which would be the entity class.

Even though I personally use this with Entity Framework, it's not dependent on that in any way. This could be used just as well with NHibernate, or any other ORM, as long as your entity objects can be made to implement the `IPersistedError` interface.

## Documentation
The few bits of code in here are commented fairly well. If anyone has any interest in this project though, I could add a sample project and/or documentation.

## License

Dual-licensed under:
 * Apache License, Version 2.0, ([LICENSE-APACHE](LICENSE-APACHE) or https://www.apache.org/licenses/LICENSE-2.0)
 * MIT license ([LICENSE-MIT](LICENSE-MIT) or https://opensource.org/licenses/MIT)

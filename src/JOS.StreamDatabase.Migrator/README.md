## Local DB

```
docker run --name local-postgres -e POSTGRES_PASSWORD=power -p 5432:5432 -d postgres
brew install libpq
echo 'export PATH="/opt/homebrew/opt/libpq/bin:$PATH"' >> ~/.zshrc
psql -h localhost -p 5432 -U postgres -d postgres
CREATE USER jos_stream_database WITH PASSWORD 'power' CREATEDB;
```

## Create migrations

```sh
dotnet ef migrations add Initial --project src/JOS.StreamDatabase.Database --startup-project src/JOS.StreamDatabase.Migrator --context MyDbContext
```

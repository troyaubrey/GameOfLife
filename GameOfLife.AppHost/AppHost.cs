
var builder = DistributedApplication.CreateBuilder(args);


var sql = builder.AddSqlServer("sql")
  .WithDataVolume()
  .WithLifetime(ContainerLifetime.Persistent);

//create the table for boards in the database if it does not exist
var databaseName = "GameOfLife";
var creationScript = $$"""
    IF DB_ID('{{databaseName}}') IS NULL
        CREATE DATABASE [{{databaseName}}];
    GO

    -- Use the database
    USE [{{databaseName}}];
    GO

    -- Create the Boards table
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Boards')
    BEGIN
    CREATE TABLE Boards (
        Id UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,                     -- Unique ID for each board
        BoardState NVARCHAR(MAX) NOT NULL                                              -- Board state JSON format
    );
    END
    GO

    """;

var db = sql.AddDatabase(databaseName)
            .WithCreationScript(creationScript);


builder.AddProject<Projects.GameOfLife_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(db)
    .WaitFor(db);



builder.Build().Run();

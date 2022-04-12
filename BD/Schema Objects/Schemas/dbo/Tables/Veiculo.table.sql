CREATE TABLE [dbo].[Veiculo] (
    [VeiculoID]    BIGINT        IDENTITY (1, 1) NOT NULL,
    [CondominioID] BIGINT        NOT NULL,
    [MoradorID]    BIGINT        NOT NULL,
    [FotoID]       BIGINT        NULL,
    [Marca]        NVARCHAR (40) NOT NULL,
    [Modelo]       NVARCHAR (40) NOT NULL,
    [Matricula]    NVARCHAR (20) NOT NULL
);






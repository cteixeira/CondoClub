CREATE TABLE [dbo].[Funcionario] (
    [FuncionarioID]  BIGINT         IDENTITY (1, 1) NOT NULL,
    [CondominioID]   BIGINT         NOT NULL,
    [Nome]           NVARCHAR (200) NOT NULL,
    [FotoID]         BIGINT         NULL,
    [DataNascimento] SMALLDATETIME  NOT NULL,
    [Masculino]      BIT            NOT NULL,
    [Identificacao]  NVARCHAR (200) NOT NULL,
    [Funcao]         NVARCHAR (200) NOT NULL,
    [Horario]        NVARCHAR (200) NOT NULL,
    [Telefone]       NVARCHAR (20)  NOT NULL,
    [Email]          NVARCHAR (200) NOT NULL
);




CREATE TABLE [dbo].[Questionario] (
    [QuestionarioID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [CondominioID]   BIGINT         NOT NULL,
    [Questao]        NVARCHAR (200) NOT NULL,
    [Inicio]         SMALLDATETIME  NOT NULL,
    [Fim]            SMALLDATETIME  NOT NULL,
    [Opcao1]         NVARCHAR (200) NOT NULL,
    [Opcao2]         NVARCHAR (200) NOT NULL,
    [Opcao3]         NVARCHAR (200) NULL,
    [Opcao4]         NVARCHAR (200) NULL,
    [Opcao5]         NVARCHAR (200) NULL,
    [Opcao6]         NVARCHAR (200) NULL,
    [Opcao7]         NVARCHAR (200) NULL,
    [Opcao8]         NVARCHAR (200) NULL,
    [AutorID]        BIGINT         NOT NULL
);




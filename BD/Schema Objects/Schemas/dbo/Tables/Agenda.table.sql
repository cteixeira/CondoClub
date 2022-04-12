CREATE TABLE [dbo].[Agenda] (
    [AgendaID]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [CondominioID] BIGINT         NOT NULL,
    [Designacao]   NVARCHAR (200) NOT NULL,
    [Telefone]     NVARCHAR (20)  NOT NULL,
    [Email]        NVARCHAR (200) NULL,
    [URL]          NVARCHAR (200) NULL,
    [Endereco]     NVARCHAR (400) NULL,
    [Localidade]   NVARCHAR (80)  NULL,
    [Cidade]       NVARCHAR (80)  NULL,
    [CodigoPostal] NVARCHAR (20)  NULL,
    [Estado]       NVARCHAR (80)  NULL,
    [PaisID]       INT            NULL,
    [Latitude]     NVARCHAR (40)  NULL,
    [Longitude]    NVARCHAR (40)  NULL
);






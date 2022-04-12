CREATE TABLE [dbo].[Categoria] (
    [CategoriaID]    INT            IDENTITY (1, 1) NOT NULL,
    [CategoriaPaiID] INT            NULL,
    [Designacao]     NVARCHAR (200) COLLATE Latin1_General_CI_AI NOT NULL
);






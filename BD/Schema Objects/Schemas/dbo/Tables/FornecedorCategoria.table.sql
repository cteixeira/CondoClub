CREATE TABLE [dbo].[FornecedorCategoria] (
    [FornecedorCategoriaID] BIGINT IDENTITY (1, 1) NOT NULL,
    [FornecedorID]          BIGINT NOT NULL,
    [CategoriaID]           INT    NOT NULL
);


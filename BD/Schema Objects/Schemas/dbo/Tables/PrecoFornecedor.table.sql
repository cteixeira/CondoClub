CREATE TABLE [dbo].[PrecoFornecedor] (
    [PrecoFornecedorID] INT             IDENTITY (1, 1) NOT NULL,
    [PaisID]            INT             NOT NULL,
    [OpcaoPagamentoID]  INT             NOT NULL,
    [FraccoesAte]       BIGINT          NOT NULL,
    [Valor]             DECIMAL (18, 2) NOT NULL
);




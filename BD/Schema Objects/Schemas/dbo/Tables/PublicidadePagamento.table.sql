/*CREATE TABLE [dbo].[PublicidadePagamento] (
    [PublicidadePagamentoID] BIGINT          IDENTITY (1, 1) NOT NULL,
    [PublicidadeID]          BIGINT          NOT NULL,
    [Inicio]                 SMALLDATETIME   NOT NULL,
    [Fim]                    SMALLDATETIME   NOT NULL,
    [Valor]                  DECIMAL (18, 2) NOT NULL,
    [Pago]                   BIT             NOT NULL,
    [DataEmissao]            SMALLDATETIME   NOT NULL,
    [FormaPagamentoID]       INT             NOT NULL,
    [ReferenciaPagamento]    NVARCHAR (200)  NULL,
    [DataPagamento]          SMALLDATETIME   NULL,
    [UtilizadorPagamentoID]  BIGINT          NULL
);*/


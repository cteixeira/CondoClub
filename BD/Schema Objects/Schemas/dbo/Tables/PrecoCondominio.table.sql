CREATE TABLE [dbo].[PrecoCondominio] (
    [PrecoCondominioID] INT             IDENTITY (1, 1) NOT NULL,
    [PaisID]            INT             NOT NULL,
    [OpcaoPagamentoID]  INT             NOT NULL,
    [ExtratoSocialID]   INT             NOT NULL,
    [FraccoesAte]       BIGINT          NOT NULL,
    [Valor]             DECIMAL (18, 2) NOT NULL
);




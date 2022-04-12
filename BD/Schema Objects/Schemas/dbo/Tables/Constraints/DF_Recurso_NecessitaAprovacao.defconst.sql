ALTER TABLE [dbo].[Recurso]
    ADD CONSTRAINT [DF_Recurso_NecessitaAprovacao] DEFAULT ((0)) FOR [RequerAprovacao];


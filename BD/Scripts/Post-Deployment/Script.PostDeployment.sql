insert into Pais values ('Brasil', 'pt-BR')
insert into Pais values ('Portugal', 'pt-PT')

insert into TabelaLocalizada values ('ExtratoSocial')
insert into TabelaLocalizada values ('PerfilUtilizador')
insert into TabelaLocalizada values ('ZonaPublicidade')
insert into TabelaLocalizada values ('FormaPagamento')
insert into TabelaLocalizada values ('OpcaoPagamento')

insert into PerfilUtilizador values ('CondoClub')
insert into PerfilUtilizador values ('Empresa')
insert into PerfilUtilizador values ('Síndico')
insert into PerfilUtilizador values ('Morador')
insert into PerfilUtilizador values ('Portaria')
insert into PerfilUtilizador values ('Consulta')
insert into PerfilUtilizador values ('Fornecedor')

insert into ExtratoSocial values ('Baixo')
insert into ExtratoSocial values ('Médio')
insert into ExtratoSocial values ('Alto')

insert into ZonaPublicidade values ('Topo')
insert into ZonaPublicidade values ('Lateral')

insert into FormaPagamento values ('Cartão Crédito')
insert into FormaPagamento values ('Boleto')

insert into OpcaoPagamento values ('Mensal')
insert into OpcaoPagamento values ('Anual')

insert into Utilizador values (1, 'condoclub@simplesolutions.pt', 'Um8+i34Ej/8=', 'CondoClub', null, 1, null, null, null)

insert into Empresa values ('Gestora de Condomínios SA', '505', null, 'rua', null, 'Estoril', '2775', null, 2, 1, '2013-06-11')
insert into Utilizador values (2, 'empresa@simplesolutions.pt', 'Um8+i34Ej/8=', 'Administrador', null, 1, 1, null, null)

insert into Condominio values (1, 'Condomínio Terra Américas', '123123123123', 1, 1, 10, 1, null, 'Recreio dos Bandeirantes', 'Rio de Janeiro', 'Rio de Janeiro', '22790-701', 'Rio de Janeiro', 1, 'todo', 'todo', 1, null)
insert into Condominio values (1, 'Ilhas Oceânicas', '129186423623', 2, 1, 30, 1, null, 'Rua Dalva Rodrigues, 150', 'Fortaleza', 'Fortaleza', ' 60177-335', 'Fortaleza', 1, 'todo', 'todo', 1, null)
insert into Condominio values (1, 'Barra D''oro', '736454573453', 2, 2, 15, 2, null, 'Rua Jornalista Henrique Cordeiro, 120 Administração', 'Barra da Tijuca', 'Barra da Tijuca', '24900-000', 'Rio de Janeiro', 1, 'todo', 'todo', 1, null)
insert into Condominio values (1, 'Sítio Guararema', '123123123123', 1, 2, 20, 2, null, 'Rua Benjamim Gallotti, 797', 'Santa Mônica', 'Santa Mônica', '25745-170', 'Itaipava', 1, 'todo', 'todo', 1, null)
insert into Condominio values (null, 'Aldeia Mar', '123876234873', 1, 1, 27, 2, null, 'Rua José Fontes Romero, 120', 'Barra da Tijuca', 'Barra da Tijuca', '22630-030', 'Rio de Janeiro', 1, 'todo', 'todo', 1, null)
insert into Condominio values (null, 'Residencial Ubatã I', '293675232123', 1, 2, 36, 2, null, 'Etr Estrada do Caxito, 540', 'Caxito', 'Caxito', '24900-000', 'Maricá', 1, 'todo', 'todo', 1, null)
insert into Condominio values (null, 'Residencial Praia da Barra', '123128741123', 1, 2, 41, 3, null, 'Avenida Sernambetiba, 3500 Bloco 1 e 2', 'Barra da Tijuca', 'Barra da Tijuca', '22630-010', 'Rio de Janeiro', 1, 'todo', 'todo', 1, null)
insert into Condominio values (null, 'Edificio Platinum Icarai', '926345123123', 1, 2, 11, 3, null, 'Rua General Pereira Silva, 302', 'Icaraí', 'Icaraí', '24220-031', 'Niterói', 1, 'todo', 'todo', 1, null)
insert into Condominio values (null, 'Edfício Concorde', '1231239875623', 2, 2, 13, 3, null, 'Avenida Deputado Bartolomeu Lizandro, 1010', 'Jrd Carioca', 'Jrd Carioca', '28080-390', 'Campos dos Goytacazes', 1, 'todo', 'todo', 1, null)
insert into Condominio values (null, 'Condado de Alenquer', '1231212313123', 2, 2, 16, 1, null, 'Rua Prof Gabiso, 192 an M', 'Tijuca', 'Tijuca', '20271-061', 'Rio de Janeiro', 1, 'todo', 'todo', 1, null)
insert into Condominio values (null, 'Dário de A Magalhães', '9079797098123', 2, 2, 33, 1, null, 'Rua General Polidoro, 15 SALA ADMINISTRACAO', 'Botafogo', 'Botafogo', '22280-004', 'Rio de Janeiro', 1, 'todo', 'todo', 1, null)
insert into Condominio values (null, 'Maestro Francisco B', '6345839682423', 2, 1, 23, 1, null, 'Rua Maestro Francisco Braga, 350 ap 105', 'Copacabana', 'Copacabana', '22041-070', 'Rio de Janeiro', 1, 'todo', 'todo', 1, null)
insert into Condominio values (null, 'Palladium Residence Service', '9656572036832', 2, 1, 32, 2, null, 'Rua General Artigas, 200 MIG', 'Leblon', 'Leblon', '22441-140', 'Rio de Janeiro', 1, 'todo', 'todo', 1, null)
insert into Condominio values (null, 'Lélis', '8746534520394', 2, 1, 100, 2, null, 'Rua Barão Ipanema, 8 pt', 'Copacabana', 'Copacabana', '22050-032', 'Rio de Janeiro', 1, 'todo', 'todo', 1, null)
insert into Condominio values (null, 'Milton', '9386753253482', 1, 1, 8, 2, null, 'Rua Do Russel, 710 pt', 'Glória', 'Glória', '22210-010', 'Rio de Janeiro', 1, 'todo', 'todo', 1, null)
insert into Condominio values (null, 'Estilo XV', '0263574383421', 1, 1, 66, 3, null, 'Rua Potiguara, 4', 'Andaraí', 'Andaraí', '22210-030', 'Rio de Janeiro', 1, 'todo', 'todo', 1, null)

insert into Utilizador values (3, 'sindico@simplesolutions.pt', 'Um8+i34Ej/8=', 'Síndico', null, 1, null, 1, null)
insert into Utilizador values (4, 'cibelly@condoclub.com.br', 'Um8+i34Ej/8=', 'Cibelly Delfino', null, 1, null, 1, null)
insert into Utilizador values (4, 'waltherdorighelo@condoclub.com.br', 'Um8+i34Ej/8=', 'Walther Dorighelo', null, 1, null, 1, null)
insert into Utilizador values (4, 'ufscosta@gmail.com', 'Um8+i34Ej/8=', 'Fernando Costa', null, 1, null, 1, null)
insert into Utilizador values (4, 'amon.libanio@gmail.com', 'Um8+i34Ej/8=', 'Amon Libanio', null, 1, null, 1, null)
insert into Utilizador values (4, 'c.teixeira@simplesolutions.pt', 'Um8+i34Ej/8=', 'Carlos Teixeira', null, 1, null, 1, null)
insert into Utilizador values (4, 'r.taborda@simplesolutions.pt', 'Um8+i34Ej/8=', 'Rui Taborda', null, 1, null, 1, null)
insert into Utilizador values (4, 'r.cardoso@simplesolutions.pt', 'Um8+i34Ej/8=', 'Rodolfo Cardoso', null, 1, null, 1, null)
insert into Utilizador values (4, 'r.araujo@simplesolutions.pt', 'Um8+i34Ej/8=', 'Rita Araújo', null, 1, null, 1, null)
insert into Utilizador values (5, 'portaria@simplesolutions.pt', 'Um8+i34Ej/8=', 'Portaria', null, 1, null, 1, null)
insert into Utilizador values (6, 'consulta@simplesolutions.pt', 'Um8+i34Ej/8=', 'Consulta', null, 1, null, 1, null)

insert into Categoria values (null, 'Animal')
insert into Categoria values (null, 'Automóvel')
insert into Categoria values (null, 'Bem-Estar & Social')
insert into Categoria values (null, 'Comércio')
insert into Categoria values (null, 'Cultura & Lazer')
insert into Categoria values (null, 'Infantil')
insert into Categoria values (null, 'Obras & Reparações')
insert into Categoria values (null, 'Restaurantes')
insert into Categoria values (null, 'Saúde')
insert into Categoria values (null, 'Serviços Administrativos')
insert into Categoria values (null, 'Transportes')
insert into Categoria values (1, 'Pet Shop')
insert into Categoria values (1, 'Veterinário')
insert into Categoria values (2, 'Concessionárias')
insert into Categoria values (2, 'Electricista')
insert into Categoria values (2, 'Lavagem')
insert into Categoria values (2, 'Lojas e Revendedoras')
insert into Categoria values (2, 'Oficina')
insert into Categoria values (2, 'Pneus')
insert into Categoria values (3, 'Barbeiro')
insert into Categoria values (3, 'Ginásio')
insert into Categoria values (3, 'Piscina')
insert into Categoria values (3, 'Quadra de Ténis')
insert into Categoria values (3, 'Salão de Beleza')
insert into Categoria values (3, 'SPA')
insert into Categoria values (4, 'Electrodomésticos')
insert into Categoria values (4, 'Informática')
insert into Categoria values (4, 'Joalharia')
insert into Categoria values (4, 'Lavanderia')
insert into Categoria values (4, 'Livraria')
insert into Categoria values (4, 'Mercearia')
insert into Categoria values (4, 'Padaria & Docerias')
insert into Categoria values (4, 'Perfumaria')
insert into Categoria values (4, 'Quitandas e Frutarias')
insert into Categoria values (4, 'Roupa')
insert into Categoria values (4, 'Supermercado')
insert into Categoria values (4, 'Telecomunicações')
insert into Categoria values (4, 'Vidraceiro')
insert into Categoria values (4, 'Chaveiro')
insert into Categoria values (5, 'Bar')
insert into Categoria values (5, 'Casa de Espectáculos')
insert into Categoria values (5, 'Cinema')
insert into Categoria values (5, 'Clubes Esportivos')
insert into Categoria values (5, 'Discoteca')
insert into Categoria values (5, 'Museu')
insert into Categoria values (5, 'Teatro')
insert into Categoria values (6, 'Baby-sitter')
insert into Categoria values (6, 'Creche')
insert into Categoria values (6, 'Escola')
insert into Categoria values (6, 'Espectáculos')
insert into Categoria values (6, 'Parque Temático')
insert into Categoria values (7, 'Electricista')
insert into Categoria values (7, 'Elevadores')
insert into Categoria values (7, 'Encanador')
insert into Categoria values (7, 'Pedreiro')
insert into Categoria values (7, 'Pintor')
insert into Categoria values (8, 'Italiano')
insert into Categoria values (8, 'Japonês')
insert into Categoria values (8, 'Regional')
insert into Categoria values (8, 'Português')
insert into Categoria values (8, 'Chinês')
insert into Categoria values (8, 'Pizzaria')
insert into Categoria values (8, 'Fast-Food')
insert into Categoria values (9, 'Hospitais')
insert into Categoria values (9, 'Clínica Médica')
insert into Categoria values (9, 'Dentista')
insert into Categoria values (9, 'Farmácia')
insert into Categoria values (9, 'Homeopatia')
insert into Categoria values (9, 'Pronto Socorro')
insert into Categoria values (9, 'Centro de Saúde')
insert into Categoria values (9, 'Fisioterapia')
insert into Categoria values (10, 'Cartórios e Notários')
insert into Categoria values (10, 'Advogados')
insert into Categoria values (10, 'Solicitador')
insert into Categoria values (10, 'Seguros')
insert into Categoria values (10, 'Bancos')
insert into Categoria values (11, 'Táxi')
insert into Categoria values (11, 'Mudanças')


insert into Fornecedor values ('Restaurante Hong Kong I', '1287361235', 1, 1, null, 'Restaurante chinês de grande qualidade', '912531623', 'hongkongI@condoclub.br', null, 'Rua dummy', null, 'Cidade dummy', '1234-567', null, 1, '38.70580275672991', '-9.443172985076899', 10, 1, null, 3, 0)
insert into Fornecedor values ('Restaurante Hong Kong II', '1287361235', 2, 1, null, 'Restaurante chinês de grande qualidade', '912531623', 'hongkongII@condoclub.br', null, 'Rua dummy', null, 'Cidade dummy', '1234-567', null, 1, '38.70580275672991', '-9.443172985076899', 15, 1, null, 3, 0)
insert into Fornecedor values ('Telepizza', '1287361235', 2, 1, null, 'Pizzaria com take away', '912531623', 'telepizza@condoclub.br', null, 'Rua dummy', null, 'Cidade dummy', '1234-567', null, 1, '38.70580275672991', '-9.443172985076899', 20, 1, null, 3, 0)
insert into Fornecedor values ('Pizza Hut', '1287361235', 2, 1, null, 'Pizzaria com take away', '912531623', 'pizzahut@condoclub.br', null, 'Rua dummy', null, 'Cidade dummy', '1234-567', null, 1, '38.70580275672991', '-9.443172985076899', 30, 1, null, 3, 0)
insert into Fornecedor values ('MacDonalds', '1287361235', 1, 2, null, 'Hamburgueres', '912531623', 'macdonalds@condoclub.br', null, 'Rua dummy', null, 'Cidade dummy', '1234-567', null, 1, '38.70580275672991', '-9.443172985076899', 15, 1, null, 3, 0)
insert into Fornecedor values ('Burguer King', '1287361235', 2, 2, null, 'Hamburgueres', '912531623', 'burguerking@condoclub.br', null, 'Rua dummy', null, 'Cidade dummy', '1234-567', null, 1, '38.70580275672991', '-9.443172985076899', 10, 1, null, 3, 0)
insert into Fornecedor values ('Sushi Rio', '1287361235', 2, 2, null, 'Sushi!', '912531623', 'sushirio@condoclub.br', null, 'Rua dummy', null, 'Cidade dummy', '1234-567', null, 1, '38.70580275672991', '-9.443172985076899', 5, 1, null, 3, 0)
insert into Fornecedor values ('Portugalia', '1287361235', 2, 2, null, 'Venha provar os nossos bifes com molho especial', '912531623', 'portugalia@condoclub.br', null, 'Rua dummy', null, 'Cidade dummy', '1234-567', null, 1, '38.70580275672991', '-9.443172985076899', 5, 1, null, 3, 0)
insert into Fornecedor values ('Joaquim Machado', '1287361235', 2, 2, null, 'Reparações de aparelhos e instalações eléctricas', '912531623', 'j.machado@condoclub.br', null, 'Rua dummy', null, 'Cidade dummy', '1234-567', null, 1, '38.70580275672991', '-9.443172985076899', 50, 1, null, 3, 0)
insert into Fornecedor values ('Manuel Coelho', '1287361235', 1, 1, null, 'Reparações e instalações de canalizações', '912531623', 'm.coelho@condoclub.br', null, 'Rua dummy', null, 'Cidade dummy', '1234-567', null, 1, '38.70580275672991', '-9.443172985076899', 5, 1, null, 3, 0)
insert into Fornecedor values ('João Antunes', '1287361235', 1, 1, null, 'Reparações de móveis', '912531623', 'j.antunes@condoclub.br', null, 'Rua dummy', null, 'Cidade dummy', '1234-567', null, 1, '38.70580275672991', '-9.443172985076899', 5, 1, null, 3, 0)

insert into Utilizador values (7, 'hongkongI@condoclub.br', 'Um8+i34Ej/8=', 'Hong Kong I', null, 1, null, null, 1)
insert into Utilizador values (7, 'hongkongII@condoclub.br', 'Um8+i34Ej/8=', 'Hong Kong II', null, 1, null, null, 2)
insert into Utilizador values (7, 'telepizza@condoclub.br', 'Um8+i34Ej/8=', 'Telepizza', null, 1, null, null, 3)
insert into Utilizador values (7, 'pizzahut@condoclub.br', 'Um8+i34Ej/8=', 'Pizza Hut', null, 1, null, null, 4)
insert into Utilizador values (7, 'macdonalds@condoclub.br', 'Um8+i34Ej/8=', 'MacDonalds', null, 1, null, null, 5)
insert into Utilizador values (7, 'burguerking@condoclub.br', 'Um8+i34Ej/8=', 'Burguer King', null, 1, null, null, 6)
insert into Utilizador values (7, 'sushirio@condoclub.br', 'Um8+i34Ej/8=', 'Sushi Rio', null, 1, null, null, 7)
insert into Utilizador values (7, 'portugalia@condoclub.br', 'Um8+i34Ej/8=', 'Portugalia', null, 1, null, null, 8)
insert into Utilizador values (7, 'j.machado@condoclub.br', 'Um8+i34Ej/8=', 'Joaquim Machado', null, 1, null, null, 9)
insert into Utilizador values (7, 'm.coelho@condoclub.br', 'Um8+i34Ej/8=', 'Manuel Coelho', null, 1, null, null, 10)
insert into Utilizador values (7, 'j.antunes@condoclub.br', 'Um8+i34Ej/8=', 'João Antunes', null, 1, null, null, 11)

/*insert into FornecedorCategoria values (1, 4)
insert into FornecedorCategoria values (2, 4)
insert into FornecedorCategoria values (3, 5)
insert into FornecedorCategoria values (4, 5)
insert into FornecedorCategoria values (5, 6)
insert into FornecedorCategoria values (6, 6)
insert into FornecedorCategoria values (7, 7)
insert into FornecedorCategoria values (8, 8)
insert into FornecedorCategoria values (9, 9)
insert into FornecedorCategoria values (10, 10)
insert into FornecedorCategoria values (11, 11)*/

insert into FornecedorAlcance values (1, 1, null)
insert into FornecedorAlcance values (1, 2, null)
insert into FornecedorAlcance values (1, 3, null)
insert into FornecedorAlcance values (1, 4, null)
insert into FornecedorAlcance values (1, 5, null)
insert into FornecedorAlcance values (1, 6, null)
insert into FornecedorAlcance values (1, 7, null)
insert into FornecedorAlcance values (1, 8, null)
insert into FornecedorAlcance values (1, 9, null)
insert into FornecedorAlcance values (1, 10, null)
insert into FornecedorAlcance values (1, 11, null)
insert into FornecedorAlcance values (1, 12, null)
insert into FornecedorAlcance values (1, 13, null)
insert into FornecedorAlcance values (1, 14, null)
insert into FornecedorAlcance values (1, 15, null)
insert into FornecedorAlcance values (1, 16, null)
insert into FornecedorAlcance values (2, 1, null)
insert into FornecedorAlcance values (2, 2, null)
insert into FornecedorAlcance values (2, 3, null)
insert into FornecedorAlcance values (2, 4, null)
insert into FornecedorAlcance values (2, 5, null)
insert into FornecedorAlcance values (2, 6, null)
insert into FornecedorAlcance values (2, 7, null)
insert into FornecedorAlcance values (2, 8, null)
insert into FornecedorAlcance values (2, 9, null)
insert into FornecedorAlcance values (2, 10, null)
insert into FornecedorAlcance values (2, 11, null)
insert into FornecedorAlcance values (2, 12, null)
insert into FornecedorAlcance values (2, 13, null)
insert into FornecedorAlcance values (2, 14, null)
insert into FornecedorAlcance values (2, 15, null)
insert into FornecedorAlcance values (2, 16, null)
insert into FornecedorAlcance values (3, 1, null)
insert into FornecedorAlcance values (3, 2, null)
insert into FornecedorAlcance values (3, 3, null)
insert into FornecedorAlcance values (3, 4, null)
insert into FornecedorAlcance values (3, 5, null)
insert into FornecedorAlcance values (3, 6, null)
insert into FornecedorAlcance values (3, 7, null)
insert into FornecedorAlcance values (3, 8, null)
insert into FornecedorAlcance values (3, 9, null)
insert into FornecedorAlcance values (3, 10, null)
insert into FornecedorAlcance values (3, 11, null)
insert into FornecedorAlcance values (3, 12, null)
insert into FornecedorAlcance values (3, 13, null)
insert into FornecedorAlcance values (3, 14, null)
insert into FornecedorAlcance values (3, 15, null)
insert into FornecedorAlcance values (3, 16, null)
insert into FornecedorAlcance values (4, 1, null)
insert into FornecedorAlcance values (4, 2, null)
insert into FornecedorAlcance values (4, 3, null)
insert into FornecedorAlcance values (4, 4, null)
insert into FornecedorAlcance values (4, 5, null)
insert into FornecedorAlcance values (4, 6, null)
insert into FornecedorAlcance values (4, 7, null)
insert into FornecedorAlcance values (4, 8, null)
insert into FornecedorAlcance values (4, 9, null)
insert into FornecedorAlcance values (4, 10, null)
insert into FornecedorAlcance values (4, 11, null)
insert into FornecedorAlcance values (4, 12, null)
insert into FornecedorAlcance values (4, 13, null)
insert into FornecedorAlcance values (4, 14, null)
insert into FornecedorAlcance values (4, 15, null)
insert into FornecedorAlcance values (4, 16, null)
insert into FornecedorAlcance values (5, 1, null)
insert into FornecedorAlcance values (5, 2, null)
insert into FornecedorAlcance values (5, 3, null)
insert into FornecedorAlcance values (5, 4, null)
insert into FornecedorAlcance values (5, 5, null)
insert into FornecedorAlcance values (5, 6, null)
insert into FornecedorAlcance values (5, 7, null)
insert into FornecedorAlcance values (5, 8, null)
insert into FornecedorAlcance values (5, 9, null)
insert into FornecedorAlcance values (5, 10, null)
insert into FornecedorAlcance values (5, 11, null)
insert into FornecedorAlcance values (5, 12, null)
insert into FornecedorAlcance values (5, 13, null)
insert into FornecedorAlcance values (5, 14, null)
insert into FornecedorAlcance values (5, 15, null)
insert into FornecedorAlcance values (5, 16, null)
insert into FornecedorAlcance values (6, 1, null)
insert into FornecedorAlcance values (6, 2, null)
insert into FornecedorAlcance values (6, 3, null)
insert into FornecedorAlcance values (6, 4, null)
insert into FornecedorAlcance values (6, 5, null)
insert into FornecedorAlcance values (6, 6, null)
insert into FornecedorAlcance values (6, 7, null)
insert into FornecedorAlcance values (6, 8, null)
insert into FornecedorAlcance values (6, 9, null)
insert into FornecedorAlcance values (6, 10, null)
insert into FornecedorAlcance values (6, 11, null)
insert into FornecedorAlcance values (6, 12, null)
insert into FornecedorAlcance values (6, 13, null)
insert into FornecedorAlcance values (6, 14, null)
insert into FornecedorAlcance values (6, 15, null)
insert into FornecedorAlcance values (6, 16, null)
insert into FornecedorAlcance values (7, 1, null)
insert into FornecedorAlcance values (7, 2, null)
insert into FornecedorAlcance values (7, 3, null)
insert into FornecedorAlcance values (7, 4, null)
insert into FornecedorAlcance values (7, 5, null)
insert into FornecedorAlcance values (7, 6, null)
insert into FornecedorAlcance values (7, 7, null)
insert into FornecedorAlcance values (7, 8, null)
insert into FornecedorAlcance values (7, 9, null)
insert into FornecedorAlcance values (7, 10, null)
insert into FornecedorAlcance values (7, 11, null)
insert into FornecedorAlcance values (7, 12, null)
insert into FornecedorAlcance values (7, 13, null)
insert into FornecedorAlcance values (7, 14, null)
insert into FornecedorAlcance values (7, 15, null)
insert into FornecedorAlcance values (7, 16, null)
insert into FornecedorAlcance values (8, 1, null)
insert into FornecedorAlcance values (8, 2, null)
insert into FornecedorAlcance values (8, 3, null)
insert into FornecedorAlcance values (8, 4, null)
insert into FornecedorAlcance values (8, 5, null)
insert into FornecedorAlcance values (8, 6, null)
insert into FornecedorAlcance values (8, 7, null)
insert into FornecedorAlcance values (8, 8, null)
insert into FornecedorAlcance values (8, 9, null)
insert into FornecedorAlcance values (8, 10, null)
insert into FornecedorAlcance values (8, 11, null)
insert into FornecedorAlcance values (8, 12, null)
insert into FornecedorAlcance values (8, 13, null)
insert into FornecedorAlcance values (8, 14, null)
insert into FornecedorAlcance values (8, 15, null)
insert into FornecedorAlcance values (8, 16, null)
insert into FornecedorAlcance values (9, 1, null)
insert into FornecedorAlcance values (9, 2, null)
insert into FornecedorAlcance values (9, 3, null)
insert into FornecedorAlcance values (9, 4, null)
insert into FornecedorAlcance values (9, 5, null)
insert into FornecedorAlcance values (9, 6, null)
insert into FornecedorAlcance values (9, 7, null)
insert into FornecedorAlcance values (9, 8, null)
insert into FornecedorAlcance values (9, 9, null)
insert into FornecedorAlcance values (9, 10, null)
insert into FornecedorAlcance values (9, 11, null)
insert into FornecedorAlcance values (9, 12, null)
insert into FornecedorAlcance values (9, 13, null)
insert into FornecedorAlcance values (9, 14, null)
insert into FornecedorAlcance values (9, 15, null)
insert into FornecedorAlcance values (9, 16, null)
insert into FornecedorAlcance values (10, 1, null)
insert into FornecedorAlcance values (10, 2, null)
insert into FornecedorAlcance values (10, 3, null)
insert into FornecedorAlcance values (10, 4, null)
insert into FornecedorAlcance values (10, 5, null)
insert into FornecedorAlcance values (10, 6, null)
insert into FornecedorAlcance values (10, 7, null)
insert into FornecedorAlcance values (10, 8, null)
insert into FornecedorAlcance values (10, 9, null)
insert into FornecedorAlcance values (10, 10, null)
insert into FornecedorAlcance values (10, 11, null)
insert into FornecedorAlcance values (10, 12, null)
insert into FornecedorAlcance values (10, 13, null)
insert into FornecedorAlcance values (10, 14, null)
insert into FornecedorAlcance values (10, 15, null)
insert into FornecedorAlcance values (10, 16, null)
insert into FornecedorAlcance values (11, 1, null)
insert into FornecedorAlcance values (11, 2, null)
insert into FornecedorAlcance values (11, 3, null)
insert into FornecedorAlcance values (11, 4, null)
insert into FornecedorAlcance values (11, 5, null)
insert into FornecedorAlcance values (11, 6, null)
insert into FornecedorAlcance values (11, 7, null)
insert into FornecedorAlcance values (11, 8, null)
insert into FornecedorAlcance values (11, 9, null)
insert into FornecedorAlcance values (11, 10, null)
insert into FornecedorAlcance values (11, 11, null)
insert into FornecedorAlcance values (11, 12, null)
insert into FornecedorAlcance values (11, 13, null)
insert into FornecedorAlcance values (11, 14, null)
insert into FornecedorAlcance values (11, 15, null)
insert into FornecedorAlcance values (11, 16, null)

insert into FornecedorKeyword values (1, 'restaurantes', 10)
insert into FornecedorKeyword values (1, 'chinês', 15)
insert into FornecedorKeyword values (1, 'hong', 20)
insert into FornecedorKeyword values (1, 'kong', 20)
insert into FornecedorKeyword values (1, 'I', 20)
insert into FornecedorKeyword values (2, 'restaurantes', 10)
insert into FornecedorKeyword values (2, 'chinês', 15)
insert into FornecedorKeyword values (2, 'hong', 20)
insert into FornecedorKeyword values (2, 'kong', 20)
insert into FornecedorKeyword values (2, 'II', 20)
insert into FornecedorKeyword values (3, 'restaurantes', 10)
insert into FornecedorKeyword values (3, 'pizzaria', 15)
insert into FornecedorKeyword values (3, 'telepizza', 20)
insert into FornecedorKeyword values (3, 'take', 25)
insert into FornecedorKeyword values (3, 'away', 25)
insert into FornecedorKeyword values (4, 'restaurantes', 10)
insert into FornecedorKeyword values (4, 'pizzaria', 15)
insert into FornecedorKeyword values (4, 'pizza', 20)
insert into FornecedorKeyword values (4, 'hut', 20)
insert into FornecedorKeyword values (4, 'take', 25)
insert into FornecedorKeyword values (4, 'away', 25)
insert into FornecedorKeyword values (5, 'restaurantes', 10)
insert into FornecedorKeyword values (5, 'fast-food', 15)
insert into FornecedorKeyword values (5, 'macdonalds', 20)
insert into FornecedorKeyword values (5, 'take', 25)
insert into FornecedorKeyword values (5, 'away', 25)
insert into FornecedorKeyword values (6, 'restaurantes', 10)
insert into FornecedorKeyword values (6, 'fast-food', 15)
insert into FornecedorKeyword values (6, 'burguer', 20)
insert into FornecedorKeyword values (6, 'king', 20)
insert into FornecedorKeyword values (6, 'take', 25)
insert into FornecedorKeyword values (6, 'away', 25)
insert into FornecedorKeyword values (7, 'restaurantes', 10)
insert into FornecedorKeyword values (7, 'japoneses', 15)
insert into FornecedorKeyword values (7, 'sushi', 20)
insert into FornecedorKeyword values (7, 'rio', 20)
insert into FornecedorKeyword values (8, 'restaurantes', 10)
insert into FornecedorKeyword values (8, 'portugueses', 15)
insert into FornecedorKeyword values (8, 'portugalia', 20)
insert into FornecedorKeyword values (8, 'bifes', 25)
insert into FornecedorKeyword values (8, 'molho', 25)
insert into FornecedorKeyword values (8, 'especial', 25)
insert into FornecedorKeyword values (9, 'reparações', 10)
insert into FornecedorKeyword values (9, 'eletricistas', 15)
insert into FornecedorKeyword values (9, 'joaquim', 20)
insert into FornecedorKeyword values (9, 'machado', 20)
insert into FornecedorKeyword values (10, 'reparações', 10)
insert into FornecedorKeyword values (10, 'canalizadores', 15)
insert into FornecedorKeyword values (10, 'manuel', 20)
insert into FornecedorKeyword values (10, 'coelho', 20)
insert into FornecedorKeyword values (11, 'reparações', 10)
insert into FornecedorKeyword values (11, 'marceneiros', 15)
insert into FornecedorKeyword values (11, 'joão', 20)
insert into FornecedorKeyword values (11, 'antunes', 20)

/*
	Recursos
*/

--SALA FESTAS
DECLARE @SalaFestasId BIGINT

INSERT INTO [CondoClub].[dbo].[Recurso]([CondominioID],[Designacao],[Activo],[RequerAprovacao],[DiasMinAprovacao],[MaxSlotsReserva],[IntervaloReserva])
VALUES (1, 'Sala Festas', 1,1,1,1,24)
 
SET @SalaFestasId = @@IDENTITY
     
INSERT INTO [CondoClub].[dbo].[RecursoHorario]([RecursoID],[DiaSemana],[Inicio],[NumeroSlots],[DuracaoSlot])
     VALUES(@SalaFestasId,1,'08:00',4,240)
INSERT INTO [CondoClub].[dbo].[RecursoHorario]([RecursoID],[DiaSemana],[Inicio],[NumeroSlots], [DuracaoSlot])
     VALUES(@SalaFestasId,7,'08:00',4,240)
     
-- SAUNA
DECLARE @SaunaId BIGINT
     
INSERT INTO [CondoClub].[dbo].[Recurso]([CondominioID],[Designacao],[Activo],[RequerAprovacao],[DiasMinAprovacao],[MaxSlotsReserva],[IntervaloReserva])
VALUES (1, 'Sauna', 1,0,null,2,12)

SET @SaunaId = @@IDENTITY

INSERT INTO [CondoClub].[dbo].[RecursoHorario]([RecursoID],[DiaSemana],[Inicio],[NumeroSlots], [DuracaoSlot])
     VALUES(@SaunaId,1,'08:00',16,30)
INSERT INTO [CondoClub].[dbo].[RecursoHorario]([RecursoID],[DiaSemana],[Inicio],[NumeroSlots], [DuracaoSlot])
     VALUES(@SaunaId,2,'08:00',16,30)
INSERT INTO [CondoClub].[dbo].[RecursoHorario]([RecursoID],[DiaSemana],[Inicio],[NumeroSlots], [DuracaoSlot])
     VALUES(@SaunaId,3,'08:00',16,30)
INSERT INTO [CondoClub].[dbo].[RecursoHorario]([RecursoID],[DiaSemana],[Inicio],[NumeroSlots], [DuracaoSlot])
     VALUES(@SaunaId,4,'08:00',16,30)
INSERT INTO [CondoClub].[dbo].[RecursoHorario]([RecursoID],[DiaSemana],[Inicio],[NumeroSlots], [DuracaoSlot])
     VALUES(@SaunaId,5,'08:00',16,30)     
INSERT INTO [CondoClub].[dbo].[RecursoHorario]([RecursoID],[DiaSemana],[Inicio],[NumeroSlots], [DuracaoSlot])
     VALUES(@SaunaId,6,'08:00',16,30)     
INSERT INTO [CondoClub].[dbo].[RecursoHorario]([RecursoID],[DiaSemana],[Inicio],[NumeroSlots], [DuracaoSlot])
     VALUES(@SaunaId,7,'08:00',16,30)

INSERT INTO [CondoClub].[dbo].[RecursoHorario]([RecursoID],[DiaSemana],[Inicio],[NumeroSlots], [DuracaoSlot])
     VALUES(@SaunaId,1,'20:00',4,15)
INSERT INTO [CondoClub].[dbo].[RecursoHorario]([RecursoID],[DiaSemana],[Inicio],[NumeroSlots], [DuracaoSlot])
     VALUES(@SaunaId,2,'20:00',4,15)
INSERT INTO [CondoClub].[dbo].[RecursoHorario]([RecursoID],[DiaSemana],[Inicio],[NumeroSlots], [DuracaoSlot])
     VALUES(@SaunaId,3,'20:00',4,15)
INSERT INTO [CondoClub].[dbo].[RecursoHorario]([RecursoID],[DiaSemana],[Inicio],[NumeroSlots], [DuracaoSlot])
     VALUES(@SaunaId,4,'20:00',4,15)
INSERT INTO [CondoClub].[dbo].[RecursoHorario]([RecursoID],[DiaSemana],[Inicio],[NumeroSlots], [DuracaoSlot])
     VALUES(@SaunaId,5,'20:00',4,15)     
INSERT INTO [CondoClub].[dbo].[RecursoHorario]([RecursoID],[DiaSemana],[Inicio],[NumeroSlots], [DuracaoSlot])
     VALUES(@SaunaId,6,'20:00',4,15)     
INSERT INTO [CondoClub].[dbo].[RecursoHorario]([RecursoID],[DiaSemana],[Inicio],[NumeroSlots], [DuracaoSlot])
     VALUES(@SaunaId,7,'20:00',4,15)
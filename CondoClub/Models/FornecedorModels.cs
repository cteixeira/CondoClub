using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;

namespace CondoClub.Web.Models {

    public class FornecedorDropDown
    {
        public FornecedorDropDown(Regras.BD.Fornecedor obj)
        {
            ID = obj.FornecedorID;
            Nome = obj.Nome;
        }


        public long? ID { get; set; }

        public string Nome { get; set; }


        public Regras.BD.Fornecedor ToBDModel()
        {
            return new Regras.BD.Fornecedor()
            {
                FornecedorID = (ID != null ? Convert.ToInt64(ID) : 0),
                Nome = Nome
            };

        }
    }


    public class Fornecedor
    {
        public Fornecedor()
        {
            Activo = false;
            Permissoes = Regras.Fornecedor.Permissoes(ControladorSite.Utilizador);
            RaioAccao = 20;
        }

        public Fornecedor(Regras.BD.Fornecedor obj)
        {
            ID = obj.FornecedorID;
            Nome = obj.Nome;
            Contribuinte = obj.Contribuinte;
            OpcaoPagamentoID = obj.OpcaoPagamentoID;
            FormaPagamentoID = obj.FormaPagamentoID;
            AvatarID = obj.AvatarID;
            Descricao = obj.Descricao;
            Telefone = obj.Telefone;
            Email = obj.Email;
            URL = obj.URL;
            Endereco = obj.Endereco;
            Localidade = obj.Localidade;
            Cidade = obj.Cidade;
            CodigoPostal = obj.CodigoPostal;
            Estado = obj.Estado;
            PaisID = obj.PaisID;
            Latitude = obj.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Longitude = obj.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            RaioAccao = obj.RaioAccao;
            Activo = obj.Activo;
            DataActivacao = obj.DataActivacao;
            ClassificacaoMedia = obj.ClassificacaoMedia;
            CssClassificacaoMedia = Controllers.ServicoController.ObterCssClassificacao(obj.ClassificacaoMedia);
            TotalComentarios = obj.TotalComentarios;

            PaisDesignacao = obj.Pais.Designacao;
            OpcaoPagamentoDesignacao = obj.OpcaoPagamento.Designacao;
            FormaPagamentoDesignacao = obj.FormaPagamento.Designacao;

            Permissoes = Regras.Fornecedor.Permissoes(ControladorSite.Utilizador, obj);

            Categorias = new Dictionary<Models.FornecedorCategoria, Regras.Fornecedor.Categoria>();

            if (obj.FornecedorCategoria.IsLoaded)
            {
                foreach (Regras.BD.FornecedorCategoria item in obj.FornecedorCategoria)
                {
                    Categorias.Add(new Models.FornecedorCategoria(item), new Regras.Fornecedor.Categoria {
                        CategoriaID = item.CategoriaID,
                        Designacao = String.Concat(item.Categoria.Categoria2.Designacao, " | ", item.Categoria.Designacao)
                    });
                }
            }
            
            Classificacoes = new List<FornecedorClassificacao>();
            
            if (obj.FornecedorClassificacao.IsLoaded)
            {
                obj.FornecedorClassificacao.ToList().ForEach(x => Classificacoes.Add(new Models.FornecedorClassificacao(x)));
                Classificacoes.Reverse();
            }

            Utilizadores = new List<FornecedorUtilizador>();
            if (obj.Utilizador.IsLoaded)
            {
                foreach (Regras.BD.Utilizador ut in obj.Utilizador)
                {
                    Utilizadores.Add(new FornecedorUtilizador()
                    {
                        Nome = ut.Nome,
                        Email = ut.Email,
                        Activo = ut.Activo
                    });
                }
            }
        }


        public long? ID { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "Nome")]
        [RequiredLocalizado(typeof(Resources.Servico), "Nome")]
        [MaxStringLocalizado(200, typeof(Resources.Servico), "Nome")]
        public string Nome { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "Contribuinte")]
        [RequiredLocalizado(typeof(Resources.Servico), "Contribuinte")]
        [MaxStringLocalizado(20, typeof(Resources.Servico), "Contribuinte")]
        public string Contribuinte { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "OpcaoPagamento")]
        [RequiredLocalizado(typeof(Resources.Servico), "OpcaoPagamento")]
        public int? OpcaoPagamentoID { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "FormaPagamento")]
        [RequiredLocalizado(typeof(Resources.Servico), "FormaPagamento")]
        public int? FormaPagamentoID { get; set; }

        public long? AvatarID { get; set; }

        [RequiredLocalizado(typeof(Resources.Servico), "Descricao")]
        public string Descricao { get; set; }

        [RequiredLocalizado(typeof(Resources.Servico), "Telefone")]
        public string Telefone { get; set; }

        [RequiredLocalizado(typeof(Resources.Servico), "Email")]
        [FormatoEmailLocalizado(typeof(Resources.Servico), "Email")]
        public string Email { get; set; }

        public string URL { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "Endereco")]
        [RequiredLocalizado(typeof(Resources.Servico), "Endereco")]
        [MaxStringLocalizado(400, typeof(Resources.Servico), "Endereco")]
        public string Endereco { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "Localidade")]
        [MaxStringLocalizado(80, typeof(Resources.Servico), "Localidade")]
        public string Localidade { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "Cidade")]
        [RequiredLocalizado(typeof(Resources.Servico), "Cidade")]
        [MaxStringLocalizado(80, typeof(Resources.Servico), "Cidade")]
        public string Cidade { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "CodigoPostal")]
        [RequiredLocalizado(typeof(Resources.Servico), "CodigoPostal")]
        [MaxStringLocalizado(20, typeof(Resources.Servico), "CodigoPostal")]
        public string CodigoPostal { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "Estado")]
        [MaxStringLocalizado(80, typeof(Resources.Servico), "Estado")]
        public string Estado { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "Pais")]
        [RequiredLocalizado(typeof(Resources.Servico), "Pais")]
        public int? PaisID { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "Latitude")]
        [RequiredLocalizado(typeof(Resources.Servico), "Latitude")]
        [MaxStringLocalizado(40, typeof(Resources.Servico), "Latitude")]
        public string Latitude { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "Longitude")]
        [RequiredLocalizado(typeof(Resources.Servico), "Longitude")]
        [MaxStringLocalizado(40, typeof(Resources.Servico), "Longitude")]
        public string Longitude { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "RaioAccao")]
        [RequiredLocalizado(typeof(Resources.Servico), "RaioAccao")]
        public int RaioAccao { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "Activo")]
        [RequiredLocalizado(typeof(Resources.Servico), "Activo")]
        public bool Activo { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "DataActivacao")]
        public DateTime? DataActivacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "ClassificacaoMedia")]
        public short ClassificacaoMedia { get; set; }

        public string CssClassificacaoMedia { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "TotalComentarios")]
        public int TotalComentarios { get; set; }

        public List<Regras.Fornecedor.Permissao> Permissoes { get; set; }

        public Dictionary<Models.FornecedorCategoria, Regras.Fornecedor.Categoria> Categorias { get; set; }

        public List<FornecedorClassificacao> Classificacoes { get; set; }

        /*Visualização*/
        public string PaisDesignacao { get; set; }
        public string OpcaoPagamentoDesignacao { get; set; }
        public string FormaPagamentoDesignacao { get; set; }
        public List<FornecedorUtilizador> Utilizadores { get; set; }

        /*Segurança*/
        public string IDHash { get; set; }
        public string RaioAccaoHash { get; set; }
        public string ActivoHash { get; set; }
        public string DataActivacaoHash { get; set; }
        public string EnderecoHash { get; set; }
        public string LocalidadeHash { get; set; }
        public string CidadeHash { get; set; }
        public string CodigoPostalHash { get; set; }
        public string EstadoHash { get; set; }
        public string PaisIDHash { get; set; }
        public string LatitudeHash { get; set; }
        public string LongitudeHash { get; set; }


        public Regras.BD.Fornecedor ToBDModel()
        {
            return new Regras.BD.Fornecedor()
            {
                FornecedorID = (this.ID != null ? Convert.ToInt64(this.ID) : 0),
                Nome = this.Nome,
                Contribuinte = this.Contribuinte,
                OpcaoPagamentoID = (this.OpcaoPagamentoID.HasValue ? this.OpcaoPagamentoID.Value : 0),
                FormaPagamentoID = (this.FormaPagamentoID.HasValue ? this.FormaPagamentoID.Value : 0),
                AvatarID = this.AvatarID,
                Descricao = this.Descricao,
                Telefone = this.Telefone,
                Email = this.Email,
                URL = this.URL,
                Endereco = this.Endereco,
                Localidade = this.Localidade,
                Cidade = this.Cidade,
                CodigoPostal = this.CodigoPostal,
                Estado = this.Estado,
                PaisID = (this.PaisID.HasValue ? this.PaisID.Value : 0),
                Latitude = Convert.ToDouble(this.Latitude, System.Globalization.CultureInfo.InvariantCulture),
                Longitude = Convert.ToDouble(this.Longitude, System.Globalization.CultureInfo.InvariantCulture),
                RaioAccao = this.RaioAccao,
                Activo = this.Activo,
                DataActivacao = this.DataActivacao,
                ClassificacaoMedia = this.ClassificacaoMedia,
                TotalComentarios = this.TotalComentarios
            };
        }

    }


    public class FornecedorConvite
    {
        public FornecedorConvite()
        {
            Assunto = Resources.Servico.ConviteAssuntoDefault;
            Mensagem = Resources.Servico.ConviteMensagemDefault;
        }

        [DisplayLocalizado(typeof(Resources.Servico), "Destinatarios")]
        [RequiredLocalizado(typeof(Resources.Servico), "Destinatarios")]
        public string Destinatarios { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "Assunto")]
        [RequiredLocalizado(typeof(Resources.Servico), "Assunto")]
        public string Assunto { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "Mensagem")]
        [RequiredLocalizado(typeof(Resources.Servico), "Mensagem")]
        public string Mensagem { get; set; }
    }


    public class FornecedorClassificacao {

        public FornecedorClassificacao() { }

        public FornecedorClassificacao(Regras.BD.FornecedorClassificacao obj) {
            ID = obj.FornecedorClassificacaoID;
            FornecedorID = obj.FornecedorID;
            UtilizadorNome = obj.Utilizador.Nome;
            UtilizadorAvatarID = obj.Utilizador.AvatarID;
            Classificacao = obj.Classificacao;
            CssClassificacao = Controllers.ServicoController.ObterCssClassificacao(obj.Classificacao);
            Comentario = obj.Comentario;
            DataHora = obj.DataHora;
            
            Permissoes = Regras.Fornecedor.PermissoesClassificacao(ControladorSite.Utilizador, obj);
        }

        public long? ID { get; set; }

        public long FornecedorID { get; set; }

        public string UtilizadorNome { get; set; }

        public long? UtilizadorAvatarID { get; set; }

        public short Classificacao { get; set; }

        public string CssClassificacao { get; set; }
        
        public string Comentario { get; set; }

        public DateTime DataHora { get; set; }

        public List<Regras.Enum.Permissao> Permissoes { get; set; }

        public Regras.BD.FornecedorClassificacao ToBDModel() {
            Regras.BD.FornecedorClassificacao obj = new Regras.BD.FornecedorClassificacao();
            obj.FornecedorID = FornecedorID;
            obj.UtilizadorID = ControladorSite.Utilizador.ID;
            obj.Classificacao = Classificacao;
            obj.Comentario = !String.IsNullOrEmpty(Comentario) ? Comentario.Trim() : null;
            obj.DataHora = DateTime.Now;
            return obj;
        }
    }


    public class FornecedorCategoria
    {
        public FornecedorCategoria() { }

        public FornecedorCategoria(Regras.BD.FornecedorCategoria obj)
        {
            FornecedorCategoriaID = obj.FornecedorCategoriaID;
            FornecedorID = obj.FornecedorID;
            CategoriaID = obj.CategoriaID;
            CategoriaPaiID = obj.Categoria.CategoriaPaiID;
            Designacao = obj.Categoria.Designacao;
        }


        public long? FornecedorCategoriaID { get; set; }

        public long? FornecedorID { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "Categoria")]
        public int? CategoriaID { get; set; }

        public int? CategoriaPaiID { get; set; }

        public string Designacao { get; set; }


        public Regras.BD.FornecedorCategoria ToBDModel()
        {
            return new Regras.BD.FornecedorCategoria()
            {
                FornecedorCategoriaID = (this.FornecedorCategoriaID.HasValue ? this.FornecedorCategoriaID.Value : 0),
                FornecedorID = (this.FornecedorID.HasValue ? this.FornecedorID.Value : 0),
                CategoriaID = (this.CategoriaID.HasValue ? this.CategoriaID.Value : 0)
            };
        }
    }


    public class FornecedorUtilizador
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public bool Activo { get; set; }
    }

    
    public class Categoria
    {
        public Categoria(Regras.BD.Categoria obj)
        {
            ID = obj.CategoriaID;
            CategoriaPaiID = obj.CategoriaPaiID;
            Designacao = obj.Designacao;
        }


        public int? ID { get; set; }

        public int? CategoriaPaiID { get; set; }

        public string Designacao { get; set; }


        public Regras.BD.Categoria ToBDModel()
        {
            return new Regras.BD.Categoria()
            {
                CategoriaID = (this.ID.HasValue ? this.ID.Value : 0),
                CategoriaPaiID = (this.CategoriaPaiID.HasValue ? this.CategoriaPaiID.Value : 0),
                Designacao = this.Designacao
            };
        }
    }


    public class Servico {

        public Servico(Regras.BD.Fornecedor obj) {
            ID = obj.FornecedorID;
            Nome = obj.Nome;
            AvatarID = obj.AvatarID;
            Descricao = obj.Descricao;
            Endereco = obj.Endereco;
            Localidade = obj.Localidade;
            Cidade = obj.Cidade;
            CodigoPostal = obj.CodigoPostal;
            Estado = obj.Estado;
            Telefone = obj.Telefone;
            Email = obj.Email;
            URL = obj.URL;
            ClassificacaoMedia = obj.ClassificacaoMedia;
            CssClassificacaoMedia = Controllers.ServicoController.ObterCssClassificacao(obj.ClassificacaoMedia);
            TotalComentarios = obj.TotalComentarios;
        }

        public long ID { get; set; }

        public string Nome { get; set; }

        public long? AvatarID { get; set; }

        public string Descricao { get; set; }

        public string Telefone { get; set; }

        public string Email { get; set; }

        public string URL { get; set; }

        public string Endereco { get; set; }

        public string Localidade { get; set; }

        public string Cidade { get; set; }

        public string CodigoPostal { get; set; }

        public string Estado { get; set; }

        public short ClassificacaoMedia { get; set; }

        public string CssClassificacaoMedia { get; set; }

        public int TotalComentarios { get; set; }

    }


    public class ServicoFornecedor
    {
        public ServicoFornecedor(Regras.BD.Fornecedor obj)
        {
            ID = obj.FornecedorID;
            Nome = obj.Nome;
            AvatarID = obj.AvatarID;
            Descricao = obj.Descricao;
            Telefone = obj.Telefone;
            Email = obj.Email;
            URL = obj.URL;
            Endereco = obj.Endereco;
            Localidade = obj.Localidade;
            Cidade = obj.Cidade;
            CodigoPostal = obj.CodigoPostal;
            Estado = obj.Estado;
            Latitude = obj.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Longitude = obj.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            ClassificacaoMedia = obj.ClassificacaoMedia;
            CssClassificacaoMedia = Controllers.ServicoController.ObterCssClassificacao(obj.ClassificacaoMedia);
            TotalComentarios = obj.TotalComentarios;

            Permissoes = Regras.Fornecedor.Permissoes(ControladorSite.Utilizador, obj);

            Categorias = new List<string>();
            if (obj.FornecedorCategoria.IsLoaded)
            {
                foreach (var item in obj.FornecedorCategoria)
                {
                    Categorias.Add(item.Categoria.Designacao);
                }
            }

            Classificacoes = new List<FornecedorClassificacao>();
            if (obj.FornecedorClassificacao.IsLoaded)
            {
                foreach (var item in obj.FornecedorClassificacao)
                {
                    Classificacoes.Add(new FornecedorClassificacao(item));
                }
                Classificacoes = Classificacoes.OrderByDescending(o => o.DataHora).ToList();
            }
            Publicidades = new List<PublicidadeVisualizar>();

            if (obj.Publicidade.IsLoaded) {
                foreach (Regras.BD.Publicidade pub in obj.Publicidade.Where(p => p.Aprovado && p.Inicio.Date <= DateTime.Now.Date && p.Fim.Date >= DateTime.Now.Date)) {
                    Publicidades.Add(new PublicidadeVisualizar(pub));
                }
            }
        }


        public long? ID { get; set; }

        public string Nome { get; set; }

        public long? AvatarID { get; set; }

        public string Descricao { get; set; }

        public string Telefone { get; set; }

        public string Email { get; set; }

        public string URL { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Endereco { get; set; }

        public string Localidade { get; set; }

        public string Cidade { get; set; }

        public string CodigoPostal { get; set; }

        public string Estado { get; set; }

        public List<Regras.Fornecedor.Permissao> Permissoes { get; set; }

        public List<string> Categorias { get; set; }

        public short ClassificacaoMedia { get; set; }

        public string CssClassificacaoMedia { get; set; }

        public int TotalComentarios { get; set; }

        public List<FornecedorClassificacao> Classificacoes { get; set; }

        public List<PublicidadeVisualizar> Publicidades { get; set; }

    }

}

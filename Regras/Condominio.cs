using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CondoClub.Regras {

    public enum CondominioPermissao {
        Visualizar,
        Gravar,
        Apagar,
        EnviarConvites,
        ActualizarPerfil
    }

    public enum CondominioEstado {
        Activo,
        Inactivo,
        PorValidar
    }

    public class Condominio {

        private _Base<BD.Condominio> _base = new _Base<BD.Condominio>();

        private static List<string> _forbiddenKeywords = new List<string>() { "condominio" };

        #region Seleccionar

        public IEnumerable<BD.Condominio> Lista(UtilizadorAutenticado utilizador) {

            if (!Permissoes(utilizador).Contains(CondominioPermissao.Visualizar))
                throw new Exceptions.SemPermissao();

            return _base.Lista();
        }


        public IEnumerable<BD.Condominio> Lista(string nome, UtilizadorAutenticado utilizador) {
            if (!Permissoes(utilizador).Contains(CondominioPermissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {
                return ctx.Condominio.Where(c => (nome == null || c.Nome.Equals(nome))).ToList();
            }
        }


        public IEnumerable<BD.Condominio> Pesquisa(long? empresaID, CondominioEstado? estado, string termo, UtilizadorAutenticado utilizador) {
            if (!Permissoes(utilizador).Contains(CondominioPermissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {
                return ctx.Condominio.Where(
                    c =>
                        (empresaID == null || c.EmpresaID == empresaID) &&
                        (!estado.HasValue ||
                            (estado == CondominioEstado.Activo && c.Activo) ||
                            (estado == CondominioEstado.Inactivo && !c.Activo && c.DataActivacao.HasValue) ||
                            (estado == CondominioEstado.PorValidar && !c.Activo && !c.DataActivacao.HasValue)
                        ) &&
                        (string.IsNullOrEmpty(termo) || c.Nome.Contains(termo))
                ).ToList();
            }
        }


        public IEnumerable<BD.Condominio> Pesquisa(long? empresaID, CondominioEstado? estado, string termo, int skip,
            int take, UtilizadorAutenticado utilizador) {
            if (!Permissoes(utilizador).Contains(CondominioPermissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {
                return ctx.Condominio
                    .Include("Empresa")
                    .Include("ExtratoSocial")
                    .Include("OpcaoPagamento")
                    .Include("FormaPagamento")
                    .Include("Pais")
                    .Include("Utilizador")
                    .Where(c =>
                        (empresaID == null || c.EmpresaID == empresaID) &&
                        (!estado.HasValue ||
                            (estado == CondominioEstado.Activo && c.Activo) ||
                            (estado == CondominioEstado.Inactivo && !c.Activo && c.DataActivacao.HasValue) ||
                            (estado == CondominioEstado.PorValidar && !c.Activo && !c.DataActivacao.HasValue)
                        ) &&
                        (string.IsNullOrEmpty(termo) || c.Nome.Contains(termo) || c.Contribuinte.Contains(termo) ||
                        c.Cidade.Contains(termo) || c.Estado.Contains(termo))
                    )
                    .OrderBy(c => c.Nome)
                    .Skip(skip)
                    .Take(take)
                    .ToList();
            }
        }


        public BD.Condominio Abrir(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                BD.Condominio obj = ctx.Condominio
                    .Include("Empresa")
                    .Include("ExtratoSocial")
                    .Include("OpcaoPagamento")
                    .Include("FormaPagamento")
                    .Include("Pais")
                    .Include("Utilizador")
                    .FirstOrDefault(c => c.CondominioID == id);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(CondominioPermissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                return obj;
            }
        }

        internal BD.Condominio Abrir(long id) {
            return _base.Abrir(id);
        }

        #endregion

        #region Inserir/Actualizar

        public void Inserir(BD.Condominio obj, UtilizadorAutenticado utilizador) {
            if (!Permissoes(utilizador).Contains(CondominioPermissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            if (Lista(obj.Nome, utilizador).Count() > 0)
                throw new Exceptions.DesignacaoRepetida();

            if (obj.Activo)
                obj.DataActivacao = DateTime.Now;

            obj.DataCriacao = DateTime.Now;
            obj.Nome = FiltrarKeywords(obj.Nome);

            using (BD.Context ctx = new BD.Context()) {

                //verificar se tem utilizador associado para poder activar
                if (obj.Activo && (obj.Utilizador == null || obj.Utilizador.Count == 0)) {
                    throw new Exceptions.CondominioSemSindico();
                }

                _base.Inserir(obj, ctx);

                if (obj.AvatarID.HasValue)
                    obj.Ficheiro.Temporario = false;

                ctx.SaveChanges();
            }

            if (obj.Activo) {
                Notificacao.Processa(obj.CondominioID, Notificacao.Evento.NovoCondominioActivo, utilizador);
                new Regras.Fornecedor().DefineAlcanceFornecedor(null, obj.CondominioID, utilizador);
            }
        }

        public void Actualizar(BD.Condominio obj, UtilizadorAutenticado utilizador) {

            BD.Condominio original = _base.Abrir(obj.CondominioID);

            if (original == null)
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador, original).Contains(CondominioPermissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            IEnumerable<BD.Condominio> lista = Lista(obj.Nome, utilizador);
            if (lista.Count() > 0 && lista.Where(x => x.CondominioID == obj.CondominioID).Count() == 0)
                throw new Exceptions.DesignacaoRepetida();

            if (!original.Activo && obj.Activo)
                obj.DataActivacao = DateTime.Now;

            obj.Nome = FiltrarKeywords(obj.Nome);

            using (BD.Context ctx = new BD.Context()) {

                if (!original.Activo && obj.Activo) {

                    //verificar se tem utilizador associado (e activo) para poder activar
                    if (obj.Activo && !ctx.Utilizador.Any(u => u.PerfilUtilizadorID == (int)Enum.Perfil.Síndico && u.CondominioID == obj.CondominioID && u.Activo)) {
                        throw new Exceptions.CondominioSemSindico();
                    }

                    obj.DataActivacao = DateTime.Now;

                }

                //Apagar foto anterior
                if (original.AvatarID.HasValue && original.AvatarID.Value != obj.AvatarID.Value)
                    new Regras.Ficheiro().Apagar(original.AvatarID.Value, ctx);


                //Data de criação não é editavel
                obj.DataCriacao = original.DataCriacao;

                _base.Actualizar(obj, ctx);

                if (obj.AvatarID.HasValue)
                    obj.Ficheiro.Temporario = false;

                ctx.SaveChanges();

                if (!original.DataActivacao.HasValue && obj.Activo) {
                    try {
                        Notificacao.Processa(obj.CondominioID, Notificacao.Evento.NovoCondominioActivo, utilizador);
                        new Regras.Pagamento().ProcessaPagamentoCondominio(obj, utilizador, ctx);
                    } catch { throw; }
                }
            }

            if (obj.Activo) {
                new Regras.Fornecedor().DefineAlcanceFornecedor(null, obj.CondominioID, utilizador);
            }
        }

        public void ActualizarPerfil(BD.Condominio obj, UtilizadorAutenticado utilizador) {

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            BD.Condominio original = _base.Abrir(obj.CondominioID);

            if (!Permissoes(utilizador, original).Contains(CondominioPermissao.ActualizarPerfil))
                throw new Exceptions.SemPermissao();

            IEnumerable<BD.Condominio> lista = Lista(obj.Nome, utilizador);
            if (lista.Count() > 0 && lista.Where(x => x.CondominioID == obj.CondominioID).Count() == 0)
                throw new Exceptions.DesignacaoRepetida();

            obj.Nome = FiltrarKeywords(obj.Nome);

            SubreporCamposPerfilReadonly(obj);

            using (BD.Context ctx = new BD.Context()) {

                //Apagar foto anterior
                if (original.AvatarID.HasValue && original.AvatarID.Value != obj.AvatarID.Value)
                    new Regras.Ficheiro().Apagar(original.AvatarID.Value, ctx);

                //Data de criação não é editavel
                obj.DataCriacao = original.DataCriacao;
                
                _base.Actualizar(obj, ctx);

                if (obj.AvatarID.HasValue)
                    obj.Ficheiro.Temporario = false;

                ctx.SaveChanges();
            }
        }

        #endregion

        #region Apagar

        public void Apagar(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                Regras.BD.Condominio obj = _base.Abrir(id, ctx);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(CondominioPermissao.Apagar))
                    throw new Exceptions.SemPermissao();

                if (obj.ArquivoDirectoria.Any() || obj.ArquivoFicheiro.Any() ||
                    obj.CondominioPagamento.Any() || obj.Funcionario.Any() || obj.Questionario.Any() ||
                    obj.Recurso.Any() || obj.Veiculo.Any() || obj.PublicidadeImpressao.Any() ||
                    obj.PublicidadeVisualizacao.Any())
                    throw new Exceptions.TemDependencias();

                if (obj.AvatarID.HasValue)
                    new Regras.Ficheiro().Apagar(obj.AvatarID.Value, ctx);

                //Apagar registos das tabelas associadas
                foreach (var comunicado in obj.Comunicado.ToList())
                    ctx.DeleteObject(comunicado);
                foreach (var alcance in obj.FornecedorAlcance.ToList())
                    ctx.DeleteObject(alcance);

                _base.Apagar(obj.CondominioID, ctx);
                ctx.SaveChanges();
            }
        }

        #endregion

        #region Registar

        public void Registar(BD.Condominio obj) {
            obj.ExtratoSocialID = (int)Regras.Enum.ExtratoSocial.Media;

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            obj.Nome = FiltrarKeywords(obj.Nome);

            using (BD.Context ctx = new BD.Context()) {
                if (obj.CondominioID == 0) {
                    if (ctx.Condominio.Where(c => c.Nome.Equals(obj.Nome)).ToList().Count > 0)
                        throw new Exceptions.DesignacaoRepetida();

                    obj.DataCriacao = DateTime.Now;

                    _base.Inserir(obj, ctx);
                } else
                    _base.Actualizar(obj, ctx);

                if (obj.AvatarID.HasValue)
                    obj.Ficheiro.Temporario = false;

                ctx.SaveChanges();
            }
        }

        public BD.Condominio RegistarRetroceder(long id) {
            Regras.BD.Condominio obj = _base.Abrir(id);

            if (obj == null || obj.Activo)
                throw new Regras.Exceptions.DadosIncorrectos();

            if (new Regras.Morador().Lista(id, string.Empty, false, new List<Regras.Enum.Perfil>()).Count() > 0)
                throw new Regras.Exceptions.DadosIncorrectos();

            return obj;
        }

        #endregion

        #region convites

        public bool EnviarConvite(string remetente, string destinatarios, string assunto, string mensagem,
            UtilizadorAutenticado utilizador) {
            if (!Permissoes(utilizador).Contains(CondominioPermissao.EnviarConvites))
                throw new Exceptions.SemPermissao();

            if (!string.IsNullOrEmpty(destinatarios) && !string.IsNullOrEmpty(assunto) &&
                !string.IsNullOrEmpty(mensagem)) {
                Regras.Util.EnviaEmailAssincrono(remetente, destinatarios, assunto, mensagem, true, true, utilizador);
                return true;
            } else {
                throw new Exceptions.DadosIncorrectos();
            }
        }

        #endregion

        #region Permissoes

        public static List<CondominioPermissao> Permissoes(UtilizadorAutenticado utilizador) {
            if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub) {
                return new List<CondominioPermissao>() { CondominioPermissao.Visualizar, 
                    CondominioPermissao.Gravar, CondominioPermissao.EnviarConvites };
            }

            if (utilizador.Perfil == Regras.Enum.Perfil.Empresa) {
                return new List<CondominioPermissao>() { CondominioPermissao.Visualizar, CondominioPermissao.EnviarConvites };
            }

            if (utilizador.Perfil == Regras.Enum.Perfil.Síndico) {
                return new List<CondominioPermissao>() { CondominioPermissao.Visualizar };
            }

            return new List<CondominioPermissao>();
        }


        public static List<CondominioPermissao> Permissoes(UtilizadorAutenticado utilizador, BD.Condominio obj) {
            if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub) {
                return new List<CondominioPermissao>() { CondominioPermissao.Visualizar, CondominioPermissao.Gravar, 
                    CondominioPermissao.Apagar, CondominioPermissao.EnviarConvites };
            }

            if (utilizador.Perfil == Regras.Enum.Perfil.Empresa && utilizador.EmpresaID.Value == obj.EmpresaID.Value) {
                return new List<CondominioPermissao>() { CondominioPermissao.Visualizar, CondominioPermissao.EnviarConvites };
            }

            if (utilizador.Perfil == Enum.Perfil.Síndico && utilizador.CondominioID.HasValue &&
                utilizador.CondominioID.Value == obj.CondominioID) {
                return new List<CondominioPermissao>() { CondominioPermissao.Visualizar, CondominioPermissao.ActualizarPerfil };
            }

            return new List<CondominioPermissao>();
        }

        #endregion

        #region Métodos auxiliares

        private bool ObjectoValido(BD.Condominio obj) {
            return !string.IsNullOrEmpty(obj.Nome) && !string.IsNullOrEmpty(obj.Contribuinte) && obj.OpcaoPagamentoID != 0 &&
                obj.FormaPagamentoID != 0 && !string.IsNullOrEmpty(obj.Endereco) && !string.IsNullOrEmpty(obj.Cidade) &&
                !string.IsNullOrEmpty(obj.CodigoPostal) && obj.PaisID != 0 && obj.Latitude != 0 && obj.Longitude != 0;
        }

        private void SubreporCamposPerfilReadonly(BD.Condominio obj) {
            BD.Condominio objAnterior = Abrir(obj.CondominioID);
            obj.ExtratoSocialID = objAnterior.ExtratoSocialID;
            obj.Fraccoes = objAnterior.Fraccoes;
            obj.EmpresaID = objAnterior.EmpresaID;
            obj.Activo = objAnterior.Activo;
            obj.DataActivacao = objAnterior.DataActivacao;
            obj.Endereco = objAnterior.Endereco;
            obj.Localidade = objAnterior.Localidade;
            obj.Cidade = objAnterior.Cidade;
            obj.CodigoPostal = objAnterior.CodigoPostal;
            obj.Estado = objAnterior.Estado;
            obj.PaisID = objAnterior.PaisID;
            obj.Latitude = objAnterior.Latitude;
            obj.Longitude = objAnterior.Longitude;
        }

        private string FiltrarKeywords(string nome) {
            string aux = nome;
            foreach (string keyword in _forbiddenKeywords) {
                int idx = aux.ToLower().ReplaceAccents().Trim().IndexOf(keyword);
                if (idx > -1) {
                    aux = aux.Remove(idx, keyword.Length);
                }
            }
            return aux.Trim();
        }

        #endregion

    }
}

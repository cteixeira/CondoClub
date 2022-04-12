using System;
using System.Data;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CondoClub.Regras
{

    public class UtilizadorAutenticado
    {
        public UtilizadorAutenticado(Regras.BD.Utilizador obj)
        {
            this.ID = obj.UtilizadorID;
            this.PerfilOriginal = this.Perfil = (Enum.Perfil)obj.PerfilUtilizadorID;
            this.Email = obj.Email;
            this.Nome = obj.Nome;
            this.AvatarID = obj.AvatarID;
            this.EmpresaID = obj.EmpresaID;
            this.CondominioID = obj.CondominioID;
            this.FornecedorID = obj.FornecedorID;
            this.Impersonating = false;
        }

        public long ID { get; set; }
        public Regras.Enum.Perfil Perfil { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
        public long? AvatarID { get; set; }
        public long? EmpresaID { get; set; }
        public long? CondominioID { get; set; }
        public long? FornecedorID { get; set; }
        public string EntidadeNome { get; set; }

        public bool Impersonating { get; set; }
        public Regras.Enum.Perfil PerfilOriginal { get; set; }
    }


    public enum UtilizadorEstado {
        Activo,
        Inactivo
    }


    public class Utilizador
    {

        private _Base<BD.Utilizador> _base = new _Base<BD.Utilizador>();

        public BD.Utilizador Login(string email, string password)
        {
            BD.Utilizador obj = null;

            using (BD.Context ctx = new BD.Context())
            {
                password = new SimpleSolutions.wcSecure.Cifra().Cifrar(password);
                obj = ctx.Utilizador.SingleOrDefault(i => i.Email == email && i.Password == password && i.Activo == true);

                if (obj == null)
                    throw new Exceptions.Autenticacao();

                if (ValidaUtilizadorActivo(obj))
                    return obj;
                else
                    throw new Exceptions.RegistoPorAprovar();
            }
        }


        public bool ValidarPassword(long id, string password)
        {
            password = new SimpleSolutions.wcSecure.Cifra().Cifrar(password);

            using (BD.Context ctx = new BD.Context())
            {
                return ctx.Utilizador.SingleOrDefault(i => i.UtilizadorID == id && i.Password == password) != null;
            }
        }

        public IEnumerable<BD.Utilizador> Lista(long? condominioID, bool? activo) 
        {
            using (BD.Context ctx = new BD.Context()) {

                return ctx.Utilizador.Where(i =>
                    (condominioID == null || i.CondominioID == condominioID) &&
                    (activo == null || i.Activo == activo)
                ).ToList();
            }
        }


        public IEnumerable<BD.Utilizador> Lista(long? condominioID, string nome, bool? activo, List<Regras.Enum.Perfil> perfis)
        {
            using (BD.Context ctx = new BD.Context())
            {
                List<int> perfilIDs = new List<int>();

                if (perfis != null && perfis.Count > 0)
                {
                    perfis.ForEach(p => perfilIDs.Add((int)p));
                }

                return ctx.Utilizador.Where(i =>
                    (condominioID == null || i.CondominioID == condominioID) &&
                    (nome == null || i.Nome.Contains(nome)) &&
                    (activo == null || i.Activo == activo) &&
                    (perfilIDs.Count == 0 || perfilIDs.Contains(i.PerfilUtilizadorID))
                ).ToList();
            }
        }


        //utilizadores da empresa
        public IEnumerable<BD.Utilizador> ListaUtilizadoresEmpresa(long empresaID) {
            using (BD.Context ctx = new BD.Context()) {
                return ctx.Utilizador.Where(u => u.PerfilUtilizadorID == (int)Enum.Perfil.Empresa && u.EmpresaID == empresaID).ToList();
            }
        }

        //utilizadores do fornecedor
        public IEnumerable<BD.Utilizador> ListaUtilizadoresFornecedor(long fornecedorID) {
            using (BD.Context ctx = new BD.Context()) {
                return ctx.Utilizador.Where(u => u.PerfilUtilizadorID == (int)Enum.Perfil.Fornecedor && u.FornecedorID == fornecedorID).ToList();
            }
        }

        public IEnumerable<BD.Utilizador> Pesquisa(UtilizadorEstado? estado, string termo, Regras.Enum.Perfil? perfil, long? condominio, 
            long? empresa, long? fornecedor, int skip, int take, UtilizadorAutenticado utilizador)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context())
            {
                return ctx.Utilizador
                    .Include("PerfilUtilizador")
                    .Include("Condominio")
                    .Include("Empresa")
                    .Include("Fornecedor")
                    .Where(u =>
                        (!estado.HasValue ||
                            (estado == UtilizadorEstado.Activo && u.Activo) ||
                            (estado == UtilizadorEstado.Inactivo && !u.Activo)
                        ) &&
                        (   !perfil.HasValue ||
                            (perfil == Enum.Perfil.Morador && 
                                (
                                u.PerfilUtilizadorID == (int)Enum.Perfil.Morador || 
                                u.PerfilUtilizadorID == (int)Enum.Perfil.Síndico)
                                ) ||
                            (perfil != Enum.Perfil.Morador && u.PerfilUtilizadorID == (int?)perfil)) &&
                        (condominio == null || u.CondominioID == condominio) &&
                        (empresa == null || u.EmpresaID == empresa) &&
                        (fornecedor == null || u.FornecedorID == fornecedor) &&
                        (string.IsNullOrEmpty(termo) || u.Nome.Contains(termo))
                    )
                    .OrderBy(e => e.Nome)
                    .Skip(skip)
                    .Take(take)
                    .ToList();
            }
        }


        public BD.Utilizador Abrir(long id)
        {
            using (BD.Context ctx = new BD.Context())
            {

                BD.Utilizador utilizador = ctx.Utilizador
                    .FirstOrDefault(i => i.UtilizadorID == id);

                return utilizador;
            }
        }


        public virtual BD.Utilizador Abrir(long id, UtilizadorAutenticado utilizador)
        {
            using (BD.Context ctx = new BD.Context())
            {
                BD.Utilizador obj = ctx.Utilizador
                    .Include("PerfilUtilizador")
                    .Include("Condominio")
                    .Include("Empresa")
                    .Include("Fornecedor")
                    .FirstOrDefault(f => f.UtilizadorID == id);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                return obj;
            }
        }


        public UtilizadorAutenticado AbrirUtilizadorAutenticado(long id)
        {
            using (BD.Context ctx = new BD.Context())
            {
                BD.Utilizador utilizador = ctx.Utilizador.Include("Condominio").Include("Empresa")
                    .Include("Fornecedor").FirstOrDefault(i => i.UtilizadorID == id);

                UtilizadorAutenticado ut = new UtilizadorAutenticado(utilizador);

                switch (ut.Perfil)
                {
                    case CondoClub.Regras.Enum.Perfil.CondoClub:
                        ut.EntidadeNome = "Ximob"; break;
                    case CondoClub.Regras.Enum.Perfil.Empresa:
                        ut.EntidadeNome = utilizador.Empresa.Nome; break;
                    case CondoClub.Regras.Enum.Perfil.Fornecedor:
                        ut.EntidadeNome = utilizador.Fornecedor.Nome; break;
                    default:
                        ut.EntidadeNome = utilizador.Condominio.Nome; break;
                }

                return ut;
            }
        }


        public virtual void Inserir(BD.Utilizador obj, UtilizadorAutenticado utilizador)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            if (VerificaUserNameRepetido(null, obj.Email))
                throw new Exceptions.EmailRepetido();

            obj.Password = new SimpleSolutions.wcSecure.Cifra().Cifrar(obj.Password);
            obj.DataCriacao = DateTime.Now;

            using (BD.Context ctx = new BD.Context())
            {
                _base.Inserir(obj, ctx);

                if (obj.AvatarID.HasValue)
                    obj.Avatar.Temporario = false;

                ctx.SaveChanges();
            }

            if (obj.PerfilUtilizadorID == (int)CondoClub.Regras.Enum.Perfil.Morador ||
                obj.PerfilUtilizadorID == (int)CondoClub.Regras.Enum.Perfil.Síndico)
                Notificacao.Processa(obj.UtilizadorID, Notificacao.Evento.NovoMorador, utilizador);
        }


        public void Registar(BD.Utilizador obj)
        {
            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            if (VerificaUserNameRepetido(null, obj.Email))
                throw new Exceptions.EmailRepetido();

            if (obj.PerfilUtilizadorID == (int)Regras.Enum.Perfil.Síndico) {
                //apenas se pode registar um sindico por condominio. Os restantes são adicionados em backoffice
                IEnumerable<Regras.BD.Utilizador> users = Lista(obj.CondominioID, null);
                if (users != null && users.Count() > 0) {
                    throw new Exception("Apenas pode ser registado um sindico por condominio");
                }
            }

            if (obj.PerfilUtilizadorID == (int)Regras.Enum.Perfil.Empresa) {
                //apenas se pode registar um utilizador por empresa. Os restantes são adicionados em backoffice
                IEnumerable<Regras.BD.Utilizador> users = ListaUtilizadoresEmpresa(obj.EmpresaID.Value);
                if (users != null && users.Count() > 0) {
                    throw new Exception("Apenas pode ser registado um utilizador por empresa");
                }
            }

            if (obj.PerfilUtilizadorID == (int)Regras.Enum.Perfil.Fornecedor) {
                //apenas se pode registar um utilizador por fornecedor. Os restantes são adicionados em backoffice
                IEnumerable<Regras.BD.Utilizador> users = ListaUtilizadoresFornecedor(obj.FornecedorID.Value);
                if (users != null && users.Count() > 0) {
                    throw new Exception("Apenas pode ser registado um utilizador por fornecedor");
                }
            }

            obj.Password = new SimpleSolutions.wcSecure.Cifra().Cifrar(obj.Password);
            obj.DataCriacao = DateTime.Now;

            using (BD.Context ctx = new BD.Context())
            {

                _base.Inserir(obj, ctx);

                if (obj.AvatarID.HasValue)
                    obj.Avatar.Temporario = false;

                ctx.SaveChanges();
            }

            NotificacoesRegistoUtilizador(obj);
        }


        public virtual void Actualizar(BD.Utilizador obj, UtilizadorAutenticado utilizador)
        {
            Regras.BD.Utilizador original = Abrir(obj.UtilizadorID);

            if (original == null)
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador, original).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            if (VerificaUserNameRepetido(obj.UtilizadorID, obj.Email))
                throw new Exceptions.NomeRepetido();

            ApagaFotoAnterior(obj);

            using (BD.Context ctx = new BD.Context())
            {

                //validar se pode activar ou alterar perfil consoante ser o último utilizador do fornecedor, empresa ou sindico de condominio
                ValidarUltimoUtilizadorFornecedorEmpresaSindico(obj, original, false, ctx);

                //a password não é alterada
                obj.Password = original.Password;
            
                _base.Actualizar(obj, ctx);

                if (obj.AvatarID.HasValue)
                    obj.Avatar.Temporario = false;

                ctx.SaveChanges();
            }
        }


        public virtual void ActualizarPassword(BD.Utilizador obj, UtilizadorAutenticado utilizador)
        {
            if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (string.IsNullOrEmpty(obj.Password))
                throw new Exceptions.DadosIncorrectos();

            obj.Password = new SimpleSolutions.wcSecure.Cifra().Cifrar(obj.Password);

            BD.Utilizador objAnterior = Abrir(obj.UtilizadorID);
            objAnterior.Password = obj.Password;

            _base.Actualizar(objAnterior);
        }


        public virtual void Apagar(long id, UtilizadorAutenticado utilizador)
        {
            using (BD.Context ctx = new BD.Context())
            {
                Regras.BD.Utilizador obj = _base.Abrir(id, ctx);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Apagar))
                    throw new Exceptions.SemPermissao();

                //validar se pode apagar consoante ser o último utilizador do fornecedor, empresa ou sindico de condominio
                ValidarUltimoUtilizadorFornecedorEmpresaSindico(obj, obj, true, ctx);

                if (obj.ArquivoDirectoria.Any() || obj.ArquivoFicheiro.Any() || obj.Ficheiro.Any() ||
                    obj.CondominioPagamento.Any() || obj.FornecedorPagamento.Any() || obj.Publicidade.Any() || 
                    obj.Publicidade1.Any() || obj.Publicidade2.Any() || obj.PublicidadeVisualizacao.Any() ||
                    obj.FornecedorClassificacao.Any() || obj.Mensagem.Any() || obj.MensagemDestinatario.Any() ||
                    obj.Questionario.Any() || obj.QuestionarioResposta.Any() || obj.RecursoReserva.Any() ||
                    obj.RecursoReserva1.Any() || obj.Veiculo.Any())
                    throw new Exceptions.TemDependencias();

                if (obj.AvatarID.HasValue)
                    new Regras.Ficheiro().Apagar(obj.AvatarID.Value, ctx);

                //Apagar registos das tabelas associadas
                foreach (var comunicado in obj.Comunicado.ToList())
                    ctx.DeleteObject(comunicado);
                foreach (var comentario in obj.ComunicadoComentario.ToList())
                    ctx.DeleteObject(comentario);

                _base.Apagar(obj.UtilizadorID, ctx);
                ctx.SaveChanges();
            }
        }


        public void ActualizarPerfil(BD.Utilizador obj, UtilizadorAutenticado utilizador)
        {

            Regras.BD.Utilizador original = Abrir(obj.UtilizadorID);

            if (!Permissoes(utilizador, original).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (VerificaUserNameRepetido(obj.UtilizadorID, obj.Email))
                throw new Exceptions.EmailRepetido();

            obj.Password = original.Password;

            ApagaFotoAnterior(obj);

            //a password não é alterada
            obj.Password = original.Password;

            using (BD.Context ctx = new BD.Context())
            {
                _base.Actualizar(obj, ctx);

                if (obj.AvatarID.HasValue)
                    obj.Avatar.Temporario = false;

                ctx.SaveChanges();
            }
        }


        public bool ValidaUtilizadorActivo(BD.Utilizador obj)
        {
            using (BD.Context ctx = new BD.Context())
            {
                // Valida se o perfil do utilizador não é CondoClub, Empresa ou Fornecedor (se for não terá nenhum condomínio associado)
                if (obj != null && obj.PerfilUtilizadorID != (int)Enum.Perfil.CondoClub &&
                    obj.PerfilUtilizadorID != (int)Enum.Perfil.Empresa &&
                    obj.PerfilUtilizadorID != (int)Enum.Perfil.Fornecedor)
                {
                    // Valida se o utilizador tem condomínio e se este se encontra activo
                    return obj.CondominioID.HasValue && ctx.Condominio.SingleOrDefault(
                        c => c.CondominioID == obj.CondominioID.Value && c.Activo) != null;
                }

                if (obj.PerfilUtilizadorID == (int)Enum.Perfil.Fornecedor)
                {
                    // Valida se o utilizador está associado a um fornecedor e se este se encontra activo
                    return obj.FornecedorID.HasValue && ctx.Fornecedor.SingleOrDefault(
                        f => f.FornecedorID == obj.FornecedorID.Value && f.Activo) != null;
                }

                if (obj.PerfilUtilizadorID == (int)Enum.Perfil.Empresa)
                {
                    // Valida se o utilizador está associado a uma empresa e se esta se encontra activo
                    return obj.EmpresaID.HasValue && ctx.Empresa.SingleOrDefault(
                        e => e.EmpresaID == obj.EmpresaID.Value && e.Activo) != null;
                }

                if (obj.PerfilUtilizadorID == (int)Enum.Perfil.CondoClub)
                {
                    return true;
                }

                return false;
            }
        }


        protected bool VerificaUserNameRepetido(long? id, string email)
        {
            using (BD.Context ctx = new BD.Context())
            {
                return ctx.Utilizador.Any(i => (id == null || i.UtilizadorID != id) && i.Email == email);
            }
        }
        

        protected void ApagaFotoAnterior(BD.Utilizador obj)
        {
            using (BD.Context ctx = new BD.Context())
            {
                BD.Utilizador previous = _base.Abrir(obj.UtilizadorID, ctx);

                if (previous.AvatarID.HasValue && previous.AvatarID.Value != obj.AvatarID.Value)
                    new Regras.Ficheiro().Apagar(previous.AvatarID.Value, ctx);

                ctx.SaveChanges();
            }
        }


        private void NotificacoesRegistoUtilizador(BD.Utilizador obj)
        {
            UtilizadorAutenticado utilizador = new UtilizadorAutenticado(obj);

            Enum.Perfil perfil;
            if (!System.Enum.TryParse(obj.PerfilUtilizadorID.ToString(), out perfil))
                throw new Exceptions.DadosIncorrectos();

            switch (perfil)
            {
                case CondoClub.Regras.Enum.Perfil.Empresa:
                    Notificacao.Processa(obj.EmpresaID.Value, Notificacao.Evento.NovaEmpresa, utilizador);
                    break;
                case CondoClub.Regras.Enum.Perfil.Síndico:
                    Notificacao.Processa(obj.CondominioID.Value, Notificacao.Evento.NovoCondominio, utilizador);
                    break;
                case CondoClub.Regras.Enum.Perfil.Morador:
                    Notificacao.Processa(obj.UtilizadorID, Notificacao.Evento.NovoMorador, utilizador);
                    break;
                case CondoClub.Regras.Enum.Perfil.Fornecedor:
                    Notificacao.Processa(obj.FornecedorID.Value, Notificacao.Evento.NovoFornecedor, utilizador);
                    break;
            }
        }


        public static IEnumerable<BD.Utilizador> ConstroiMosaico(int numeroUtilizadores, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {

                if (utilizador.Perfil == Enum.Perfil.CondoClub) {
                    //ao condoclub são apresentados moradores de todos os condominio
                    return ctx.Utilizador.
                        Where(u => 
                            (u.PerfilUtilizadorID == (int)Enum.Perfil.Morador || u.PerfilUtilizadorID == (int)Enum.Perfil.Síndico)).
                        OrderBy(x => Guid.NewGuid()).Take(numeroUtilizadores).ToList();    
                }

                if (utilizador.Perfil == Enum.Perfil.Empresa) {
                    //ás empresas são apresentados moradores de todos os seus condominios

                    IEnumerable<BD.Condominio> condominiosEmpresa = ctx.Condominio.Where(c => c.EmpresaID.HasValue && c.EmpresaID == utilizador.EmpresaID.Value);

                    return ctx.Utilizador.
                        Where(u => 
                        condominiosEmpresa.Contains(u.Condominio) &&
                        (u.PerfilUtilizadorID == (int)Enum.Perfil.Morador || u.PerfilUtilizadorID == (int)Enum.Perfil.Síndico)).
                        OrderBy(x => Guid.NewGuid()).Take(numeroUtilizadores).ToList();
                }

                //os restantes apenas vêem moradores do seu condominio
                return ctx.Utilizador.
                    Where(u => 
                                u.CondominioID == utilizador.CondominioID &&
                                (u.PerfilUtilizadorID == (int)Enum.Perfil.Morador || u.PerfilUtilizadorID == (int)Enum.Perfil.Síndico)).
                    OrderBy(x => Guid.NewGuid()).Take(numeroUtilizadores).ToList();    
            }           
        }


        public void RecuperarPassword(string email) {
            using (BD.Context ctx = new BD.Context()) {

                BD.Utilizador ut = ctx.Utilizador.Where(u => u.Email == email).FirstOrDefault();
                if (ut != null) {
                    string pass = new SimpleSolutions.wcSecure.Decifra().Decifrar(ut.Password);
                    string emailMsg = String.Format(Resources.Utilizador.RecuperarPasswordEmail, pass);
                    Util.EnviaEmailAssincrono(ConfigurationManager.AppSettings["AppEmail"], email, Resources.Utilizador.RecuperarPasswordAssunto, emailMsg, true, true, null);
                }

            }
        }


        public bool EmailJaExistente(string email)
        {
            using (BD.Context ctx = new BD.Context())
            {
                return ctx.Utilizador.FirstOrDefault(u => u.Email.Equals(email)) != null;
            }
        }


        #region Permissões

        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador)
        {
            if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub)
            {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };
            }

            return new List<Enum.Permissao>();
        }


        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador, BD.Utilizador obj)
        {
            if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub)
            {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
            }

            if (utilizador.ID == obj.UtilizadorID)
            {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };
            }

            return new List<Enum.Permissao>();
        }

        #endregion


        #region Validações

        protected bool ObjectoValido(BD.Utilizador obj)
        {
            if (obj.PerfilUtilizadorID == 0 || string.IsNullOrEmpty(obj.Nome) || string.IsNullOrEmpty(obj.Email)){
                return false;
            }

            if(obj.UtilizadorID == 0 && string.IsNullOrEmpty(obj.Password)){
                //nos novos registos é obrigatória a password
                return false;
            }

            Enum.Perfil perfil;
            if(!System.Enum.TryParse(obj.PerfilUtilizadorID.ToString(), out perfil))
                return false;

            switch (perfil)
            {
                case CondoClub.Regras.Enum.Perfil.CondoClub:
                    return PerfilCondoClubValido(obj);
                case CondoClub.Regras.Enum.Perfil.Empresa:
                    return PerfilEmpresaValido(obj);
                case CondoClub.Regras.Enum.Perfil.Síndico:
                    return PerfilSíndicoValido(obj);
                case CondoClub.Regras.Enum.Perfil.Morador:
                    return PerfilMoradorValido(obj);
                case CondoClub.Regras.Enum.Perfil.Portaria:
                    return PerfilPortariaValido(obj);
                case CondoClub.Regras.Enum.Perfil.Consulta:
                    return PerfilConsultaValido(obj);
                case CondoClub.Regras.Enum.Perfil.Fornecedor:
                    return PerfilFornecedorValido(obj);
            }

            return false;
        }


        private bool PerfilCondoClubValido(BD.Utilizador obj)
        {
            return !obj.CondominioID.HasValue && !obj.EmpresaID.HasValue && !obj.FornecedorID.HasValue;
        }


        private bool PerfilEmpresaValido(BD.Utilizador obj)
        {
            return obj.EmpresaID.HasValue && !obj.CondominioID.HasValue && !obj.FornecedorID.HasValue;
        }


        private bool PerfilSíndicoValido(BD.Utilizador obj)
        {
            return obj.CondominioID.HasValue && !obj.EmpresaID.HasValue && !obj.FornecedorID.HasValue && 
                !string.IsNullOrEmpty(obj.Fraccao);
        }


        private bool PerfilMoradorValido(BD.Utilizador obj)
        {
            return obj.CondominioID.HasValue && !obj.EmpresaID.HasValue && !obj.FornecedorID.HasValue &&
                !string.IsNullOrEmpty(obj.Fraccao);
        }


        private bool PerfilPortariaValido(BD.Utilizador obj)
        {
            return obj.CondominioID.HasValue && !obj.EmpresaID.HasValue && !obj.FornecedorID.HasValue;
        }


        private bool PerfilConsultaValido(BD.Utilizador obj)
        {
            return obj.CondominioID.HasValue && !obj.EmpresaID.HasValue && !obj.FornecedorID.HasValue;
        }


        private bool PerfilFornecedorValido(BD.Utilizador obj)
        {
            return obj.FornecedorID.HasValue && !obj.CondominioID.HasValue && !obj.EmpresaID.HasValue;
        }


        private static void ValidarUltimoUtilizadorFornecedorEmpresaSindico(BD.Utilizador obj, Regras.BD.Utilizador original, bool apagar, BD.Context ctx) {

            if (original.PerfilUtilizadorID == (int)Enum.Perfil.Fornecedor) {

                bool ultimoUtilizadorFornecedor = ctx.Utilizador.Where(u => u.FornecedorID == original.FornecedorID && u.Activo).Count() == 1;

                if (ultimoUtilizadorFornecedor && original.Activo && !obj.Activo) {
                    throw new Exceptions.FornecedorSemUtilizador();
                }

                if (ultimoUtilizadorFornecedor && obj.PerfilUtilizadorID != (int)Enum.Perfil.Fornecedor) {
                    throw new Exceptions.FornecedorSemUtilizador();
                }

                if (apagar && ultimoUtilizadorFornecedor) {
                    throw new Exceptions.FornecedorSemUtilizador();
                }

            }

            if (original.PerfilUtilizadorID == (int)Enum.Perfil.Empresa) {

                bool ultimoUtilizadorEmpresa = ctx.Utilizador.Where(u => u.EmpresaID == original.EmpresaID && u.Activo).Count() == 1;

                if (ultimoUtilizadorEmpresa && original.Activo && !obj.Activo) {
                    throw new Exceptions.EmpresaSemUtilizador();
                }

                if (ultimoUtilizadorEmpresa && obj.PerfilUtilizadorID != (int)Enum.Perfil.Empresa) {
                    throw new Exceptions.EmpresaSemUtilizador();
                }

                if (apagar && ultimoUtilizadorEmpresa) {
                    throw new Exceptions.EmpresaSemUtilizador();
                }

            }

            if (original.PerfilUtilizadorID == (int)Enum.Perfil.Síndico) {

                bool ultimoUtilizadorSindico = ctx.Utilizador.Where(u => u.CondominioID == original.CondominioID &&
                    u.PerfilUtilizadorID == (int)Enum.Perfil.Síndico && u.Activo).Count() == 1;

                if (ultimoUtilizadorSindico && original.Activo && !obj.Activo) {
                    throw new Exceptions.CondominioSemSindico();
                }

                if (ultimoUtilizadorSindico && obj.PerfilUtilizadorID != (int)Enum.Perfil.Síndico) {
                    throw new Exceptions.CondominioSemSindico();
                }

                if (apagar && ultimoUtilizadorSindico) {
                    throw new Exceptions.CondominioSemSindico();
                }

            }
        }

        #endregion
    }


    public class Morador : Utilizador
    {

        private _Base<BD.Utilizador> _base = new _Base<BD.Utilizador>();

        public IEnumerable<BD.Utilizador> Pesquisa(long condominioID, Regras.UtilizadorEstado? estado, string termo,
            int skip, int take, UtilizadorAutenticado utilizador)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context())
            {
                return ctx.Utilizador
                    .Include("Condominio")
                    .Include("PerfilUtilizador")
                    .Where(u =>
                        u.CondominioID == condominioID &&
                        (u.PerfilUtilizadorID == (int)Enum.Perfil.Morador || u.PerfilUtilizadorID == (int)Enum.Perfil.Síndico
                            || u.PerfilUtilizadorID == (int)Enum.Perfil.Portaria) &&
                        (!estado.HasValue ||
                            (estado == UtilizadorEstado.Activo && u.Activo) ||
                            (estado == UtilizadorEstado.Inactivo && !u.Activo)
                        ) &&
                        (string.IsNullOrEmpty(termo) || u.Nome.Contains(termo) ||
                        u.Fraccao.Contains(termo) || u.Email.Contains(termo))
                    )
                    .OrderBy(c => c.Nome)
                    .Skip(skip)
                    .Take(take)
                    .ToList();
            }
        }


        public override BD.Utilizador Abrir(long id, UtilizadorAutenticado utilizador)
        {
            using (BD.Context ctx = new BD.Context())
            {
                BD.Utilizador obj = ctx.Utilizador
                    .Include("Condominio")
                    .Include("PerfilUtilizador")
                    .FirstOrDefault(u => u.UtilizadorID == id);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                return obj;
            }
        }


        public override void Inserir(BD.Utilizador obj, UtilizadorAutenticado utilizador)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            obj.CondominioID = utilizador.CondominioID.Value;

            if (!base.ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            if (base.VerificaUserNameRepetido(obj.UtilizadorID, obj.Email))
                throw new Exceptions.EmailRepetido();

            using (BD.Context ctx = new BD.Context()){

                obj.Password = new SimpleSolutions.wcSecure.Cifra().Cifrar(obj.Password);
                obj.DataCriacao = DateTime.Now;

                _base.Inserir(obj, ctx);

                if (obj.AvatarID.HasValue)
                    obj.Avatar.Temporario = false;

                ctx.SaveChanges();
            }

            if (obj.PerfilUtilizadorID == (int)CondoClub.Regras.Enum.Perfil.Morador ||
                obj.PerfilUtilizadorID == (int)CondoClub.Regras.Enum.Perfil.Síndico)
                Notificacao.Processa(obj.UtilizadorID, Notificacao.Evento.NovoMorador, utilizador);
        }


        public override void Actualizar(BD.Utilizador obj, UtilizadorAutenticado utilizador)
        {
            Regras.BD.Utilizador original = Abrir(obj.UtilizadorID);

            if (original == null)
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador, original).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!base.ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            if (base.VerificaUserNameRepetido(obj.UtilizadorID, obj.Email))
                throw new Exceptions.EmailRepetido();

            Exception excepcao;
            if (!ValidarAlteracaoSindico(obj, utilizador, out excepcao))
                throw excepcao;

            base.ApagaFotoAnterior(obj);

            using (BD.Context ctx = new BD.Context())
            {
                _base.Actualizar(obj, ctx);

                if (obj.AvatarID.HasValue)
                    obj.Avatar.Temporario = false;

                ctx.SaveChanges();
            }
        }


        public override void Apagar(long id, UtilizadorAutenticado utilizador)
        {
            using (BD.Context ctx = new BD.Context())
            {
                Regras.BD.Utilizador obj = _base.Abrir(id, ctx);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Apagar))
                    throw new Exceptions.SemPermissao();

                if (obj.ArquivoDirectoria.Any() || obj.ArquivoFicheiro.Any() || obj.Ficheiro.Any() ||
                    obj.CondominioPagamento.Any() || obj.FornecedorPagamento.Any() || obj.Publicidade.Any() ||
                    obj.Publicidade1.Any() || obj.Publicidade2.Any() || obj.PublicidadeVisualizacao.Any() ||
                    obj.FornecedorClassificacao.Any() || obj.Mensagem.Any() || obj.MensagemDestinatario.Any() ||
                    obj.Questionario.Any() || obj.QuestionarioResposta.Any() || obj.RecursoReserva.Any() ||
                    obj.RecursoReserva1.Any() || obj.Veiculo.Any())
                    throw new Exceptions.TemDependencias();

                if (obj.AvatarID.HasValue)
                    new Regras.Ficheiro().Apagar(obj.AvatarID.Value, ctx);

                //Apagar registos das tabelas associadas
                foreach (var comunicado in obj.Comunicado.ToList())
                    ctx.DeleteObject(comunicado);
                foreach (var comentario in obj.ComunicadoComentario.ToList())
                    ctx.DeleteObject(comentario);

                _base.Apagar(obj.UtilizadorID, ctx);
                ctx.SaveChanges();
            }
        }


        #region Permissões

        public new static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador)
        {
            if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub || utilizador.Perfil == Regras.Enum.Perfil.Empresa || 
                utilizador.Perfil == Regras.Enum.Perfil.Síndico)
            {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };
            }

            return new List<Enum.Permissao>();
        }


        public new static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador, BD.Utilizador obj)
        {
            if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub)
            {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
            }

            if (utilizador.Perfil == Regras.Enum.Perfil.Empresa && utilizador.EmpresaID.Value == obj.EmpresaID.Value)
            {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };
            }

            if (utilizador.Perfil == Regras.Enum.Perfil.Síndico && utilizador.CondominioID.Value == obj.CondominioID.Value)
            {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };
            }

            return new List<Enum.Permissao>();
        }

        #endregion


        public bool EnviarConvite(string remetente, string destinatarios, string assunto, string mensagem, 
            UtilizadorAutenticado utilizador)
        {
            if (!string.IsNullOrEmpty(destinatarios) && !string.IsNullOrEmpty(assunto) &&
                !string.IsNullOrEmpty(mensagem))
            {
                Regras.Util.EnviaEmailAssincrono(remetente, destinatarios, assunto, mensagem, true, true, utilizador);
                return true;
            }
            else
            {
                throw new Exceptions.DadosIncorrectos();
            }
        }


        private bool ValidarAlteracaoSindico(BD.Utilizador obj, UtilizadorAutenticado utilizador, out Exception erro)
        {
            using (BD.Context ctx = new BD.Context())
            {
                erro = null;

                List<BD.Utilizador> sindicos = ctx.Utilizador.Where(u =>
                    u.CondominioID == utilizador.CondominioID.Value &&
                    u.PerfilUtilizadorID == (int)Enum.Perfil.Síndico &&
                    u.UtilizadorID != obj.UtilizadorID
                ).ToList();

                if (!obj.Activo && obj.PerfilUtilizadorID == (int)Enum.Perfil.Síndico && sindicos.Count == 0)
                {
                    //Não pode inactivar sem nomear outro síndico
                    erro = new Exceptions.CondominioSemSindico();
                    return false;
                }


                BD.Utilizador objAnterior = _base.Abrir(obj.UtilizadorID, ctx);

                if (objAnterior.PerfilUtilizadorID == (int)Enum.Perfil.Síndico &&
                    obj.PerfilUtilizadorID == (int)Enum.Perfil.Morador && sindicos.Count == 0)
                {
                    //Não pode alterar o perfil sem nomear outro síndico
                    erro = new Exceptions.CondominioSemSindico();
                    return false;
                }

                return true;
            }
        }

    }


}

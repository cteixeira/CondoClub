using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CondoClub.Web.Models {
    public class CondominioRegisto {
        public CondominioRegisto() {
            Activo = false;
        }

        public CondominioRegisto(Regras.BD.Condominio obj) {
            ID = obj.CondominioID;
            IDCifrado = CondoClub.Regras.Util.UrlEncode(CondoClub.Regras.Util.Cifra(obj.CondominioID.ToString()));
            EmpresaID = obj.EmpresaID;
            Nome = obj.Nome;
            Contribuinte = obj.Contribuinte;
            OpcaoPagamentoID = obj.OpcaoPagamentoID;
            FormaPagamentoID = obj.FormaPagamentoID;
            Fraccoes = obj.Fraccoes;
            AvatarID = obj.AvatarID;
            Endereco = obj.Endereco;
            Localidade = obj.Localidade;
            Cidade = obj.Cidade;
            CodigoPostal = obj.CodigoPostal;
            Estado = obj.Estado;
            PaisID = obj.PaisID;
            Latitude = obj.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Longitude = obj.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Activo = obj.Activo;
        }


        public string IDCifrado { get; set; }
        public long? ID {
            get {
                if (!String.IsNullOrEmpty(IDCifrado)) {
                    return long.Parse(Regras.Util.Decifra(Regras.Util.UrlDecode(IDCifrado)));
                }
                return 0;
            }
            set { IDCifrado = CondoClub.Regras.Util.UrlEncode(CondoClub.Regras.Util.Cifra(value.ToString())); }
        }

        [DisplayLocalizado(typeof(Resources.Condominio), "Empresa")]
        public long? EmpresaID { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Nome")]
        [RequiredLocalizado(typeof(Resources.Condominio), "Nome")]
        [MaxStringLocalizado(200, typeof(Resources.Condominio), "Nome")]
        public string Nome { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Contribuinte")]
        [RequiredLocalizado(typeof(Resources.Condominio), "Contribuinte")]
        [MaxStringLocalizado(20, typeof(Resources.Condominio), "Contribuinte")]
        public string Contribuinte { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "OpcaoPagamento")]
        [RequiredLocalizado(typeof(Resources.Condominio), "OpcaoPagamento")]
        public int? OpcaoPagamentoID { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "FormaPagamento")]
        [RequiredLocalizado(typeof(Resources.Condominio), "FormaPagamento")]
        public int? FormaPagamentoID { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Fraccoes")]
        [RequiredLocalizado(typeof(Resources.Condominio), "Fraccoes")]
        public short? Fraccoes { get; set; }

        public long? AvatarID { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Endereco")]
        [RequiredLocalizado(typeof(Resources.Condominio), "Endereco")]
        [MaxStringLocalizado(400, typeof(Resources.Condominio), "Endereco")]
        public string Endereco { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Localidade")]
        [MaxStringLocalizado(80, typeof(Resources.Condominio), "Localidade")]
        public string Localidade { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Cidade")]
        [RequiredLocalizado(typeof(Resources.Condominio), "Cidade")]
        [MaxStringLocalizado(80, typeof(Resources.Condominio), "Cidade")]
        public string Cidade { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "CodigoPostal")]
        [RequiredLocalizado(typeof(Resources.Condominio), "CodigoPostal")]
        [MaxStringLocalizado(20, typeof(Resources.Condominio), "CodigoPostal")]
        public string CodigoPostal { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Estado")]
        [MaxStringLocalizado(80, typeof(Resources.Condominio), "Estado")]
        public string Estado { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Pais")]
        [RequiredLocalizado(typeof(Resources.Condominio), "Pais")]
        public int? PaisID { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Latitude")]
        [MaxStringLocalizado(40, typeof(Resources.Condominio), "Latitude")]
        public string Latitude { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Longitude")]
        [MaxStringLocalizado(40, typeof(Resources.Condominio), "Longitude")]
        public string Longitude { get; set; }

        public bool Activo { get; set; }

        [RequiredLocalizado(typeof(Resources.Registo), "AceitarCondicoesGerais")]
        [BooleanMustBeTrueAttribute(typeof(Resources.Registo), "AceitarCondicoesGerais")]
        public bool AceitarCondicoesGerais { get; set; }


        public Regras.BD.Condominio ToBDModel() {
            return new Regras.BD.Condominio() {
                CondominioID = (this.ID.HasValue ? Convert.ToInt64(this.ID) : 0),
                EmpresaID = this.EmpresaID,
                Nome = this.Nome,
                Contribuinte = this.Contribuinte,
                OpcaoPagamentoID = (this.OpcaoPagamentoID.HasValue ? this.OpcaoPagamentoID.Value : 0),
                FormaPagamentoID = (this.FormaPagamentoID.HasValue ? this.FormaPagamentoID.Value : 0),
                Fraccoes = this.Fraccoes,
                AvatarID = this.AvatarID,
                Endereco = this.Endereco,
                Localidade = this.Localidade,
                Cidade = this.Cidade,
                CodigoPostal = this.CodigoPostal,
                Estado = this.Estado,
                PaisID = (this.PaisID.HasValue ? this.PaisID.Value : 0),
                Latitude = Convert.ToDouble(this.Latitude, System.Globalization.CultureInfo.InvariantCulture),
                Longitude = Convert.ToDouble(this.Longitude, System.Globalization.CultureInfo.InvariantCulture),
                Activo = this.Activo
            };
        }
    }


    public class UtilizadorRegisto {
        public UtilizadorRegisto() {
            this.Activo = true;
        }


        public long? ID { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "PerfilUtilizador")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "PerfilUtilizador")]
        public string PerfilIDCifrado { get; set; }
        public int PerfilID {
            get {
                return int.Parse(Regras.Util.Decifra(Regras.Util.UrlDecode(PerfilIDCifrado)));
            }
            set { PerfilIDCifrado = CondoClub.Regras.Util.UrlEncode(CondoClub.Regras.Util.Cifra(value.ToString())); }
        }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Email")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Email")]
        [FormatoEmailLocalizado(typeof(Resources.Utilizador), "Email")]
        [MaxStringLocalizado(200, typeof(Resources.Utilizador), "Email")]
        [DataType(DataType.Text)]
        public string Email { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Password")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Password")]
        [MaxStringLocalizado(200, typeof(Resources.Utilizador), "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "ConfirmarPassword")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "ConfirmarPassword")]
        [MaxStringLocalizado(200, typeof(Resources.Utilizador), "ConfirmarPassword")]
        [DataType(DataType.Password)]
        public string ConfirmarPassword { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Nome")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Nome")]
        [MaxStringLocalizado(200, typeof(Resources.Utilizador), "Nome")]
        public string Nome { get; set; }

        public long? AvatarID { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Activo")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Activo")]
        public bool Activo { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Fraccao")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Fraccao")]
        [MaxStringLocalizado(50, typeof(Resources.Utilizador), "Fraccao")]
        public string Fraccao { get; set; }

        public string EmpresaIDCifrado { get; set; }
        public long? EmpresaID {
            get {
                if (!String.IsNullOrEmpty(EmpresaIDCifrado)) {
                    return long.Parse(Regras.Util.Decifra(Regras.Util.UrlDecode(EmpresaIDCifrado)));
                }
                return null;
            }
            set { EmpresaIDCifrado = CondoClub.Regras.Util.UrlEncode(CondoClub.Regras.Util.Cifra(value.ToString())); }
        }

        public string CondominioIDCifrado { get; set; }
        public long? CondominioID {
            get {
                if (!String.IsNullOrEmpty(CondominioIDCifrado)) {
                    return long.Parse(Regras.Util.Decifra(Regras.Util.UrlDecode(CondominioIDCifrado)));
                }
                return null;
            }
            set { CondominioIDCifrado = CondoClub.Regras.Util.UrlEncode(CondoClub.Regras.Util.Cifra(value.ToString())); }
        }

        public string FornecedorIDCifrado { get; set; }
        public long? FornecedorID {
            get {
                if (!String.IsNullOrEmpty(FornecedorIDCifrado)) {
                    return long.Parse(Regras.Util.Decifra(Regras.Util.UrlDecode(FornecedorIDCifrado)));
                }
                return null;
            }
            set { FornecedorIDCifrado = CondoClub.Regras.Util.UrlEncode(CondoClub.Regras.Util.Cifra(value.ToString())); }
        }

        [BooleanMustBeTrueAttribute(typeof(Resources.Registo), "AceitarCondicoesGerais")]
        public bool AceitarCondicoesGerais { get; set; }

        public string CifraUrl() {
            string cifra = string.Empty;
            if (PerfilID == (int)CondoClub.Regras.Enum.Perfil.Síndico) {
                cifra = CondoClub.Regras.Util.UrlEncode(CondoClub.Regras.Util.Cifra(PerfilID + ";" + CondominioID));
            } else if (PerfilID == (int)CondoClub.Regras.Enum.Perfil.Empresa) {
                cifra = CondoClub.Regras.Util.UrlEncode(CondoClub.Regras.Util.Cifra(PerfilID + ";" + EmpresaID));
            } else if (PerfilID == (int)CondoClub.Regras.Enum.Perfil.Fornecedor) {
                cifra = CondoClub.Regras.Util.UrlEncode(CondoClub.Regras.Util.Cifra(PerfilID + ";" + FornecedorID));
            }
            return cifra;
        }

        public Regras.BD.Utilizador ToBDModel() {
            Regras.BD.Utilizador obj = new Regras.BD.Utilizador();

            obj.UtilizadorID = (this.ID != null ? Convert.ToInt64(this.ID) : obj.UtilizadorID);
            obj.PerfilUtilizadorID = Convert.ToInt32(this.PerfilID);
            obj.Email = this.Email;
            obj.Password = this.Password;
            obj.Nome = this.Nome;
            obj.AvatarID = this.AvatarID;
            obj.Activo = this.Activo;
            obj.Fraccao = this.Fraccao;
            obj.EmpresaID = this.EmpresaID;
            obj.CondominioID = this.CondominioID;
            obj.FornecedorID = this.FornecedorID;

            return obj;
        }
    }


    public class EmpresaRegisto {
        public EmpresaRegisto() {
            Activo = false;
        }

        public EmpresaRegisto(Regras.BD.Empresa obj) {
            ID = obj.EmpresaID;
            Nome = obj.Nome;
            Contribuinte = obj.Contribuinte;
            AvatarID = obj.AvatarID;
            Endereco = obj.Endereco;
            Localidade = obj.Localidade;
            Cidade = obj.Cidade;
            CodigoPostal = obj.CodigoPostal;
            Estado = obj.Estado;
            PaisID = obj.PaisID;
            Latitude = obj.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Longitude = obj.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Activo = obj.Activo;
        }


        public string IDCifrado { get; set; }
        public long? ID {
            get {
                if (!String.IsNullOrEmpty(IDCifrado)) {
                    return long.Parse(Regras.Util.Decifra(Regras.Util.UrlDecode(IDCifrado)));
                }
                return 0;
            }
            set { IDCifrado = CondoClub.Regras.Util.UrlEncode(CondoClub.Regras.Util.Cifra(value.ToString())); }
        }

        [DisplayLocalizado(typeof(Resources.Empresa), "Nome")]
        [RequiredLocalizado(typeof(Resources.Empresa), "Nome")]
        [MaxStringLocalizado(200, typeof(Resources.Empresa), "Nome")]
        public string Nome { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Contribuinte")]
        [RequiredLocalizado(typeof(Resources.Empresa), "Contribuinte")]
        [MaxStringLocalizado(20, typeof(Resources.Empresa), "Contribuinte")]
        public string Contribuinte { get; set; }

        public long? AvatarID { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Endereco")]
        [RequiredLocalizado(typeof(Resources.Empresa), "Endereco")]
        [MaxStringLocalizado(400, typeof(Resources.Empresa), "Endereco")]
        public string Endereco { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Localidade")]
        [MaxStringLocalizado(80, typeof(Resources.Empresa), "Localidade")]
        public string Localidade { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Cidade")]
        [RequiredLocalizado(typeof(Resources.Empresa), "Cidade")]
        [MaxStringLocalizado(80, typeof(Resources.Empresa), "Cidade")]
        public string Cidade { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "CodigoPostal")]
        [RequiredLocalizado(typeof(Resources.Empresa), "CodigoPostal")]
        [MaxStringLocalizado(20, typeof(Resources.Empresa), "CodigoPostal")]
        public string CodigoPostal { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Estado")]
        [MaxStringLocalizado(80, typeof(Resources.Empresa), "Estado")]
        public string Estado { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Pais")]
        [RequiredLocalizado(typeof(Resources.Empresa), "Pais")]
        public int? PaisID { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Latitude")]
        [MaxStringLocalizado(40, typeof(Resources.Empresa), "Latitude")]
        public string Latitude { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Longitude")]
        [MaxStringLocalizado(40, typeof(Resources.Empresa), "Longitude")]
        public string Longitude { get; set; }

        public bool Activo { get; set; }

        [RequiredLocalizado(typeof(Resources.Registo), "AceitarCondicoesGerais")]
        [BooleanMustBeTrueAttribute(typeof(Resources.Registo), "AceitarCondicoesGerais")]
        public bool AceitarCondicoesGerais { get; set; }


        public Regras.BD.Empresa ToBDModel() {
            return new Regras.BD.Empresa() {
                EmpresaID = (this.ID.HasValue ? Convert.ToInt64(this.ID) : 0),
                Nome = this.Nome,
                Contribuinte = this.Contribuinte,
                AvatarID = this.AvatarID,
                Endereco = this.Endereco,
                Localidade = this.Localidade,
                Cidade = this.Cidade,
                CodigoPostal = this.CodigoPostal,
                Estado = this.Estado,
                PaisID = (this.PaisID.HasValue ? this.PaisID.Value : 0),
                Latitude = Convert.ToDouble(this.Latitude, System.Globalization.CultureInfo.InvariantCulture),
                Longitude = Convert.ToDouble(this.Longitude, System.Globalization.CultureInfo.InvariantCulture),
                Activo = this.Activo
            };
        }
    }


    public class FornecedorRegisto {
        public FornecedorRegisto() {
            Activo = false;
            RaioAccao = 20;
        }

        public FornecedorRegisto(Regras.BD.Fornecedor obj) {
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
            if (obj.FornecedorCategoria.IsLoaded) {
                CategoriasID = String.Join(";", obj.FornecedorCategoria.Select(fc => fc.CategoriaID));
            }
        }

        public string IDCifrado { get; set; }
        public long? ID {
            get {
                if (!String.IsNullOrEmpty(IDCifrado)) {
                    return long.Parse(Regras.Util.Decifra(Regras.Util.UrlDecode(IDCifrado)));
                }
                return 0;
            }
            set { IDCifrado = CondoClub.Regras.Util.UrlEncode(CondoClub.Regras.Util.Cifra(value.ToString())); }
        }

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

        [DisplayLocalizado(typeof(Resources.Servico), "Descricao")]
        [RequiredLocalizado(typeof(Resources.Servico), "Descricao")]
        [MaxStringLocalizado(4000, typeof(Resources.Servico), "Descricao")]
        public string Descricao { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "Telefone")]
        [RequiredLocalizado(typeof(Resources.Servico), "Telefone")]
        [MaxStringLocalizado(20, typeof(Resources.Servico), "Telefone")]
        public string Telefone { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "Email")]
        [RequiredLocalizado(typeof(Resources.Servico), "Email")]
        [FormatoEmailLocalizado(typeof(Resources.Servico), "Email")]
        [MaxStringLocalizado(200, typeof(Resources.Servico), "Email")]
        public string Email { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "URL")]
        [MaxStringLocalizado(200, typeof(Resources.Servico), "URL")]
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
        [MaxStringLocalizado(40, typeof(Resources.Servico), "Latitude")]
        public string Latitude { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "Longitude")]
        [MaxStringLocalizado(40, typeof(Resources.Servico), "Longitude")]
        public string Longitude { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "RaioAccao")]
        //[RequiredLocalizado(typeof(Resources.Servico), "RaioAccao")]
        public int RaioAccao { get; set; }

        [DisplayLocalizado(typeof(Resources.Servico), "Activo")]
        [RequiredLocalizado(typeof(Resources.Servico), "Activo")]
        public bool Activo { get; set; }

        [RequiredLocalizado(typeof(Resources.Servico), "Categorias")]
        public string CategoriasID { get; set; }

        [RequiredLocalizado(typeof(Resources.Registo), "AceitarCondicoesGerais")]
        [BooleanMustBeTrueAttribute(typeof(Resources.Registo), "AceitarCondicoesGerais")]
        public bool AceitarCondicoesGerais { get; set; }


        public Regras.BD.Fornecedor ToBDModel() {
            Regras.BD.Fornecedor f = new Regras.BD.Fornecedor();
            f.FornecedorID = (this.ID != null ? Convert.ToInt64(this.ID) : 0);
            f.Nome = this.Nome;
            f.Contribuinte = this.Contribuinte;
            f.OpcaoPagamentoID = (this.OpcaoPagamentoID.HasValue ? this.OpcaoPagamentoID.Value : 0);
            f.FormaPagamentoID = (this.FormaPagamentoID.HasValue ? this.FormaPagamentoID.Value : 0);
            f.AvatarID = this.AvatarID;
            f.Descricao = this.Descricao;
            f.Telefone = this.Telefone;
            f.Email = this.Email;
            f.URL = this.URL;
            f.Endereco = this.Endereco;
            f.Localidade = this.Localidade;
            f.Cidade = this.Cidade;
            f.CodigoPostal = this.CodigoPostal;
            f.Estado = this.Estado;
            f.PaisID = (this.PaisID.HasValue ? this.PaisID.Value : 0);
            f.Latitude = Convert.ToDouble(this.Latitude, System.Globalization.CultureInfo.InvariantCulture);
            f.Longitude = Convert.ToDouble(this.Longitude, System.Globalization.CultureInfo.InvariantCulture);
            f.RaioAccao = this.RaioAccao;
            f.Activo = this.Activo;

            if (!string.IsNullOrEmpty(CategoriasID)) {
                string[] ids = CategoriasID.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string id in ids) {
                    if (f.FornecedorCategoria.FirstOrDefault(fc => fc.CategoriaID.ToString() == id) == null) {
                        f.FornecedorCategoria.Add(new Regras.BD.FornecedorCategoria { CategoriaID = int.Parse(id) });
                    }
                }
            }

            return f;
        }
    }

}
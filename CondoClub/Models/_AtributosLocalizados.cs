using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CondoClub.Web.Models {

    public class DisplayLocalizado : DisplayNameAttribute {

        private PropertyInfo propInfo;

        public DisplayLocalizado(Type resourceType, string resourceKey) 
            : base(resourceKey) {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        public override string DisplayName {
            get {
                return propInfo.GetValue(propInfo.DeclaringType, null).ToString();
            }
        }
    
    }


    public class RequiredLocalizado : RequiredAttribute, IClientValidatable {

        private PropertyInfo propInfo;

        public RequiredLocalizado(Type resourceType, string resourceKey)
            : base() {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context) {
            
            string nomeCampo = propInfo.GetValue(propInfo.DeclaringType, null).ToString();
            this.ErrorMessage = String.Format(Resources.Erro.CampoObrigatorio, nomeCampo);

            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = this.ErrorMessage;
            rule.ValidationType = "required";
            return new[] { rule };
        }
    }


    public class MaxStringLocalizado : StringLengthAttribute, IClientValidatable {

        private PropertyInfo propInfo;

        public MaxStringLocalizado(int maxLenght, Type resourceType, string resourceKey)
            : base(maxLenght) {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            if (value == null || value.ToString().Trim().Length <= this.MaximumLength) {
                return ValidationResult.Success;
            }
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context) {

            string nomeCampo = propInfo.GetValue(propInfo.DeclaringType, null).ToString();
            this.ErrorMessage = String.Format(Resources.Erro.TamanhoMaximo, nomeCampo, this.MaximumLength.ToString());

            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = this.ErrorMessage;
            rule.ValidationType = "length";
            rule.ValidationParameters.Add("max", this.MaximumLength);
            return new[] { rule };
        }

    }


    public class FormatoEmailLocalizado : RegularExpressionAttribute, IClientValidatable {

        private const string pattern = @"^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$"; 
        private PropertyInfo propInfo;

        public FormatoEmailLocalizado(Type resourceType, string resourceKey)
            : base(pattern) {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context) {

            string nomeCampo = propInfo.GetValue(propInfo.DeclaringType, null).ToString();
            this.ErrorMessage = String.Format(Resources.Erro.FormatoEmail, nomeCampo);

            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = this.ErrorMessage;
            rule.ValidationType = "regex";
            rule.ValidationParameters.Add("pattern", pattern);
            return new[] { rule };
        }
    }


    public class FormatoNumeroInteiroLocalizado : DataTypeAttribute, IClientValidatable {

        private const string pattern = @"^(\+|-)?\d+$";
        private PropertyInfo propInfo;

        public FormatoNumeroInteiroLocalizado(Type resourceType, string resourceKey)
            : base(pattern) {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context) {

            string nomeCampo = propInfo.GetValue(propInfo.DeclaringType, null).ToString();
            this.ErrorMessage = String.Format(Resources.Erro.FormatoNumeroInteiro, nomeCampo);
            
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = this.ErrorMessage;
            rule.ValidationType = "regex";
            rule.ValidationParameters.Add("pattern", pattern);
            return new[] { rule };
        }
    }

    public class FormatoDataLocalizado : DataTypeAttribute, IClientValidatable {

        private const string pattern = @"^([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$";
        private PropertyInfo propInfo;

        public FormatoDataLocalizado(Type resourceType, string resourceKey)
            : base(pattern) {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context) {

            string nomeCampo = propInfo.GetValue(propInfo.DeclaringType, null).ToString();
            this.ErrorMessage = String.Format(Resources.Erro.FormatoData, nomeCampo);

            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = this.ErrorMessage;
            rule.ValidationType = "regex";
            rule.ValidationParameters.Add("pattern", pattern);
            return new[] { rule };
        }
    }


    public class FormatoHoraLocalizado : DataTypeAttribute, IClientValidatable {

        private const string pattern = @"^(20|21|22|23|[01]\d|\d)(([:.][0-5]\d){1,2})$";
        private PropertyInfo propInfo;

        public FormatoHoraLocalizado(Type resourceType, string resourceKey)
            : base(pattern) {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context) {

            string nomeCampo = propInfo.GetValue(propInfo.DeclaringType, null).ToString();
            this.ErrorMessage = String.Format(Resources.Erro.FormatoHora, nomeCampo);

            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = this.ErrorMessage;
            rule.ValidationType = "regex";
            rule.ValidationParameters.Add("pattern", pattern);
            return new[] { rule };
        }
    }


    public class BooleanMustBeTrueAttribute : ValidationAttribute
    {
        private PropertyInfo propInfo;

        public BooleanMustBeTrueAttribute(Type resourceType, string resourceKey)
            : base() {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
            ErrorMessage = Resources.Erro.AceitarCondicoesGerais;
        }

        public override bool IsValid(object propertyValue)
        {
            return propertyValue != null && propertyValue is bool && (bool)propertyValue;
        }

    }


    public class RangeLocalizado : RangeAttribute, IClientValidatable {

        private PropertyInfo propInfo;

        public RangeLocalizado(int minValue, int maxValue, Type resourceType, string resourceKey)
            : base(minValue, maxValue) {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context) {

            string nomeCampo = propInfo.GetValue(propInfo.DeclaringType, null).ToString();
            if ((Int32)this.Minimum == Int32.MinValue && (Int32)this.Maximum < Int32.MaxValue)
                this.ErrorMessage = String.Format(Resources.Erro.NumeroMenorQue, nomeCampo, this.Maximum);
            else if ((Int32)this.Minimum > Int32.MinValue && (Int32)this.Maximum == Int32.MaxValue)
                this.ErrorMessage = String.Format(Resources.Erro.NumeroMaiorQue, nomeCampo, this.Minimum);
            else
                this.ErrorMessage = String.Format(Resources.Erro.NumeroEntre, nomeCampo, this.Minimum, this.Maximum);

            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = this.ErrorMessage;
            rule.ValidationType = "range";
            rule.ValidationParameters.Add("min", this.Minimum);
            rule.ValidationParameters.Add("max", this.Maximum);
            return new[] { rule };
        }
    }


    public class RangeDoubleLocalizado : RangeAttribute, IClientValidatable {
        private PropertyInfo propInfo;

        public RangeDoubleLocalizado(double minValue, double maxValue, Type resourceType, string resourceKey)
            : base(minValue, maxValue) {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context) {

            string nomeCampo = propInfo.GetValue(propInfo.DeclaringType, null).ToString();
            if ((Double)this.Minimum == Double.MinValue && (Double)this.Maximum < Double.MaxValue)
                this.ErrorMessage = String.Format(Resources.Erro.NumeroMenorQue, nomeCampo, this.Maximum);
            else if ((Double)this.Minimum > Double.MinValue && (Double)this.Maximum == Double.MaxValue)
                this.ErrorMessage = String.Format(Resources.Erro.NumeroMaiorQue, nomeCampo, this.Minimum);
            else
                this.ErrorMessage = String.Format(Resources.Erro.NumeroEntre, nomeCampo, this.Minimum, this.Maximum);

            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = this.ErrorMessage;
            rule.ValidationType = "rangedouble";
            rule.ValidationParameters.Add("min", this.Minimum);
            rule.ValidationParameters.Add("max", this.Maximum);
            return new[] { rule };
        }
    }


    
    public sealed class IsDateAfterLocalizado : ValidationAttribute, IClientValidatable {

        private readonly string dataMenorPropertyName;
        private readonly bool allowEqualDates;

        public IsDateAfterLocalizado(string dataMenorPropertyName, bool allowEqualDates, Type resourceType, string resourceKeyValorMenor, string resourceKeyValorMaior) {
            PropertyInfo propInfoResourceKeyValorMenor = resourceType.GetProperty(resourceKeyValorMenor, BindingFlags.Public | BindingFlags.Static);
            string nomeCampoMenor = propInfoResourceKeyValorMenor.GetValue(propInfoResourceKeyValorMenor.DeclaringType, null).ToString();

            PropertyInfo propInfoResourceKeyValorMaior = resourceType.GetProperty(resourceKeyValorMaior, BindingFlags.Public | BindingFlags.Static);
            string nomeCampoMaior = propInfoResourceKeyValorMaior.GetValue(propInfoResourceKeyValorMaior.DeclaringType, null).ToString();

            this.dataMenorPropertyName = dataMenorPropertyName;
            this.allowEqualDates = allowEqualDates;
            if(allowEqualDates){
                this.ErrorMessage = String.Format(Resources.Erro.DataMaiorIgualQue, nomeCampoMaior, nomeCampoMenor);
            }else{
                this.ErrorMessage = this.ErrorMessage = String.Format(Resources.Erro.DataMaiorQue, nomeCampoMaior, nomeCampoMenor);
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.dataMenorPropertyName);
            if (propertyTestedInfo == null) {
                return new ValidationResult(string.Format("unknown property {0}", this.dataMenorPropertyName));
            }

            var propertyTestedValue = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (value == null || !(value is DateTime)) {
                return ValidationResult.Success;
            }

            if (propertyTestedValue == null || !(propertyTestedValue is DateTime)) {
                return ValidationResult.Success;
            }

            // Compare values
            if ((DateTime)value >= (DateTime)propertyTestedValue) {
                if (this.allowEqualDates) {
                    return ValidationResult.Success;
                }
                if ((DateTime)value > (DateTime)propertyTestedValue) {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context) {
            var rule = new ModelClientValidationRule {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isdateafter"
            };
            rule.ValidationParameters["propertytested"] = this.dataMenorPropertyName;
            rule.ValidationParameters["allowequaldates"] = this.allowEqualDates;
            yield return rule;
        }
    }


    public sealed class IsDateAfterTodayLocalizado : ValidationAttribute, IClientValidatable {

        private readonly bool allowToday;

        public IsDateAfterTodayLocalizado(bool allowToday, Type resourceType, string resourceKey) {
            PropertyInfo propInfoResourceKey = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
            string nomeCampo = propInfoResourceKey.GetValue(propInfoResourceKey.DeclaringType, null).ToString();

            this.allowToday = allowToday;
            if (allowToday) {
                this.ErrorMessage = String.Format(Resources.Erro.DataMaiorIgualQueHoje, nomeCampo);
            } else {
                this.ErrorMessage = this.ErrorMessage = String.Format(Resources.Erro.DataMaiorQueHoje, nomeCampo);
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {

            if (value == null || !(value is DateTime)) {
                return ValidationResult.Success;
            }

            // Compare values
            if (((DateTime)value).Date >= DateTime.Now.Date) {
                if (this.allowToday) {
                    return ValidationResult.Success;
                }
                if ((DateTime)value > DateTime.Now.Date) {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context) {
            var rule = new ModelClientValidationRule {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isdateaftertoday"
            };
            rule.ValidationParameters["allowtoday"] = this.allowToday;
            yield return rule;
        }
    }

}
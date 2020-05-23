using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Extension
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UnlikeAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "The value of {0} cannot be the same as the value of the {1}.";

        public string OtherProperty { get; private set; }

        public UnlikeAttribute(string otherProperty)
            : base(DefaultErrorMessage)
        {
            if (string.IsNullOrEmpty(otherProperty))
            {
                throw new ArgumentNullException("otherProperty");
            }

            OtherProperty = otherProperty;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, OtherProperty);
        }

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            if (value != null)
            {
                var otherProperty = validationContext.ObjectInstance.GetType()
                    .GetProperty(OtherProperty);

                var otherPropertyValue = otherProperty
                    .GetValue(validationContext.ObjectInstance, null);
                if (otherPropertyValue.GetType() == typeof(bool))
                {
                    if (value.Equals(otherPropertyValue) && value.Equals(true))
                        return new ValidationResult(
                        FormatErrorMessage(validationContext.DisplayName));
                }
                else
                {
                    if (value.Equals(otherPropertyValue))
                    {
                        return new ValidationResult(
                            FormatErrorMessage(validationContext.DisplayName));
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}

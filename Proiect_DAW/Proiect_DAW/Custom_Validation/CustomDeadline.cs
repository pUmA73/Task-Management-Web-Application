

namespace Proiect_DAW.Custom_Validation
{
    using System.ComponentModel.DataAnnotations;
    using System;
    public class CustomDeadlineAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;

            DateTime enteredDate = (DateTime)value;
            enteredDate = enteredDate.AddHours(2);

            return enteredDate >= DateTime.Now;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} must be in the future.";
        }
    }


    public class CustomDateRangeAttribute : ValidationAttribute
    {
        private readonly string _startDatePropertyName;
        private readonly string _finishDatePropertyName;

        public CustomDateRangeAttribute(string startDatePropertyName, string finishDatePropertyName)
        {
            _startDatePropertyName = startDatePropertyName;
            _finishDatePropertyName = finishDatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);
            var finishDateProperty = validationContext.ObjectType.GetProperty(_finishDatePropertyName);

            var startDate = (DateTime?)startDateProperty.GetValue(validationContext.ObjectInstance, null);
            var finishDate = (DateTime?)finishDateProperty.GetValue(validationContext.ObjectInstance, null);

            if (startDate > finishDate)
            {
                return new ValidationResult("Finish date must be greater than the start date.");
            }

            return ValidationResult.Success;
        }
    }

}

namespace BeOrganized.Web.ViewModels.Home
{
    using System.ComponentModel.DataAnnotations;

    using BeOrganized.Common;
    using BeOrganized.Web.Common.ValidationAttribures;

    public class ContactViewModel
    {
        [MinLength(AttributesConstraints.ContactNameMinLength)]
        public string Name { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        [MaxLength(AttributesConstraints.ContactMessageMaxLength, ErrorMessage = AttributesErrorMessages.PasswordStringMaxLengthMessage)]
        public string Message { get; set; }

        [GoogleRecaptchaValidation]
        public string RecaptchaValue { get; set; }
    }
}

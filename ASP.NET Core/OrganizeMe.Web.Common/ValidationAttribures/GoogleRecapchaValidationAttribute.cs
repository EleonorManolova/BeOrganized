﻿namespace OrganizeMe.Web.Common.ValidationAttribures
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;

    using Microsoft.Extensions.Configuration;

    public class GoogleRecaptchaValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(
                    "Value of google recapcha is null or empty.",
                    new[] { validationContext.MemberName });
            }

            var httpClient = new HttpClient();
            var configuration = (IConfiguration)validationContext.GetService(typeof(IConfiguration));
            var content = new FormUrlEncodedContent(
                new[]
                    {
                        new KeyValuePair<string, string>("secret", configuration["GoogleRecaptcha:SecretKey"]),
                        new KeyValuePair<string, string>("response", value.ToString()),
                    });
            var httpResponse = httpClient.PostAsync($"https://www.google.com/recaptcha/api/siteverify", content)
                .GetAwaiter().GetResult();
            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                return new ValidationResult(
                    $"Recapcha validation failed. Status code: {httpResponse.StatusCode}.",
                    new[] { validationContext.MemberName });
            }

            var jsonResponse = httpResponse.Content.ReadAsStringAsync().Result;
            var siteVerifyResponse = JsonSerializer.Deserialize<RecaptchaResponse>(jsonResponse);
            return siteVerifyResponse.Success
                       ? ValidationResult.Success
                       : new ValidationResult(
                           "Google Recaptcha validation failed.",
                           new[] { validationContext.MemberName });
        }
    }
}
namespace BeOrganized.Web.Areas.Administration.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    public class BackUpController : AdministrationController
    {
        public IActionResult ReIndex()
        {
            return this.View();
        }

        public IActionResult BackUp()
        {
            return this.View();
        }
    }
}
namespace BeOrganized.Web.Areas.Administration.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualBasic;

    public class BackUpController : AdministrationController
    {
        private const string SuccessfullMessage = "Backup database successfully";
        private const string FileNotFound = "File Not Found";
        private IConfiguration configuration;

        public BackUpController(IConfiguration configuraiton)
        {
            this.configuration = configuraiton;
        }

        public IActionResult BackUp()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var currentDirectoryRoot = Directory.GetParent(currentDirectory).Parent.ToString();
            var backupDestination = Path.Combine(currentDirectoryRoot, "SQLBackUpFolder");
            var allbackups = new List<string>();
            if (!Directory.Exists(backupDestination))
            {
                return this.View(allbackups);
            }

            var fileNames = Directory.GetFiles(backupDestination).Select(x => Path.GetFileName(x)).ToList();
            return this.View(fileNames);
        }

        [HttpPost]
        [Route("/Administration/BackUp/BackUp")]
        public IActionResult BackUpPost()
        {
            var sqlconn = new SqlConnection(this.configuration.GetConnectionString("DefaultConnection"));
            var sqlcmd = new SqlCommand();

            var currentDirectory = Directory.GetCurrentDirectory();
            var currentDirectoryRoot = Directory.GetParent(currentDirectory).Parent.ToString();
            var backupDestination = Path.Combine(currentDirectoryRoot, "SQLBackUpFolder");

            if (!Directory.Exists(backupDestination))
            {
                Directory.CreateDirectory(backupDestination);
            }

            try
            {
                sqlconn.Open();
                var parsedPath = backupDestination.Replace(@"\\", @"\");
                sqlcmd = new SqlCommand("backup database BeOrganized to disk='" + parsedPath + @"\" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".bak'", sqlconn);
                sqlcmd.ExecuteNonQuery();
                sqlconn.Close();
                this.TempData["NotificationSuccess"] = SuccessfullMessage;
            }
            catch (Exception ex)
            {
                this.TempData["NotificationError"] = ex.Message;
            }

            return this.Redirect("/");
        }

        [HttpPost]
        [Route("Administration/BackUp/Download")]
        public async Task<IActionResult> Download(string fileName)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var currentDirectoryRoot = Directory.GetParent(currentDirectory).Parent.ToString();
            var backupDestination = Path.Combine(currentDirectoryRoot, "SQLBackUpFolder");
            var fileDestination = Path.Combine(backupDestination, fileName);
            if (string.IsNullOrEmpty(fileName))
            {
                this.TempData["NotificationError"] = FileNotFound;
                return this.Redirect("/");
            }

            if (!System.IO.File.Exists(fileDestination))
            {
                this.TempData["NotificationError"] = FileNotFound;
                return this.Redirect("/");
            }

            try
            {
                var memory = new MemoryStream();
                using (var stream = new FileStream(fileDestination, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0;
                var provider = new FileExtensionContentTypeProvider();
                string contentType;
                if (!provider.TryGetContentType(fileName, out contentType))
                {
                    contentType = "application/octet-stream";
                }

                var file = this.File(memory.ToArray(), contentType, fileName);
                this.TempData["NotificationSuccess"] = SuccessfullMessage;
                return file;
            }
            catch
            {
                this.TempData["NotificationError"] = FileNotFound;
            }

            return this.Redirect("/");
        }
    }
}

using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AzureDevOpsKats.Common.Configuration;
using AzureDevOpsKats.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AzureDevOpsKats.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/v{version:apiVersion}/values")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly SmtpConfiguration _smtpConfiguration;

        private readonly KeyVaultConfiguration _keyVaultConfiguration;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="vault"></param>
        public ValuesController(IOptionsSnapshot<SmtpConfiguration> settings, IOptionsSnapshot<KeyVaultConfiguration> vault)
        {
            _smtpConfiguration = settings.Value;
            _keyVaultConfiguration = vault.Value;
        }

        /// <summary>
        ///  AWS Parameter Store Configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SmtpConfiguration Get()
        {
            return _smtpConfiguration;
        }

        /// <summary>
        ///  Directory Items.
        /// </summary>
        /// <returns>File Items Model</returns>
        [HttpGet]
        [Route("directory/items")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<FileItemsModel> DirectoryItems()
        {
            var fileItemsModel = new FileItemsModel { DirectoryPath = @"/images", Status = false, FileCount = 0 };
            if (Directory.Exists(fileItemsModel.DirectoryPath))
            {
                fileItemsModel.Status = true;
                fileItemsModel.FileCount = Directory.GetFiles(fileItemsModel.DirectoryPath, "*", SearchOption.AllDirectories).Length;
                fileItemsModel.Files = Directory.GetFiles(fileItemsModel.DirectoryPath, "*", SearchOption.AllDirectories);
            }

            return Ok(fileItemsModel);
        }


        /// <summary>
        ///  KeyVault.
        /// </summary>
        /// <returns>File Items Model</returns>
        [HttpGet]
        [Route("keyvault/items")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> KeyVaultItems()
        {
            if(_keyVaultConfiguration.Enabled)
            {
                //TODO
            }

             var client = new SecretClient(new Uri(_keyVaultConfiguration.Uri), new DefaultAzureCredential());
             var secret = await client.GetSecretAsync("AzureDevopsConnectionString");

            return Ok(secret);
        }


    }
}

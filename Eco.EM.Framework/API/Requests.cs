using Eco.Core.Serialization;
using Eco.EM.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Eco.EM.Framework.API
{
    
    [AllowAnonymous, Route("elixr-mods/framework/api/v1")]
    public class Requests : Controller
    {
        [AllowAnonymous, HttpGet("get-recipes")]
        [Produces("application/json")]
        public IActionResult GetRecipes()
        {
            var result = JSONRecipeExporter.BuildExportData();

            if (result.Contains("Internal Server Error: 500"))
                return StatusCode(500);
            else
                return Ok(result);
        }

        [AllowAnonymous, HttpGet("")]
        public string GetPrices()
        {
            return "";
        }

        [AllowAnonymous, HttpGet("api-check")]
        public string Test()
        {
            return "We are working cheif!";
        }


    }
}

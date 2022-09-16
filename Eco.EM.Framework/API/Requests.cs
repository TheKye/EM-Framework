using Eco.Core.Serialization;
using Eco.EM.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Eco.EM.Framework.Plugins;

namespace Eco.EM.Framework.API
{
    
    [AllowAnonymous, Route("elixr-mods/framework/api/v1")]
    public class Requests : Controller
    {
        [AllowAnonymous, HttpGet("get-recipes")]
        [Produces("application/json")]
        public IActionResult GetRecipes()
        {
            if (BasePlugin.Obj.Config.EnableWebAPI)
            {
                var result = JSONRecipeExporter.BuildExportData();

                if (result.Contains("Internal Server Error: 500"))
                    return StatusCode(500);
                else
                    return Ok(result);
            }
            else
                return BadRequest(403);
        }

        [AllowAnonymous, HttpGet("get-prices")]
        [Produces("application/json")]
        public IActionResult GetPrices(bool includeOutOfStock)
        {
            if (BasePlugin.Obj.Config.EnableWebAPI)
            {
                var result = ShopUtils.GetAllItems(includeOutOfStock);
                if (result.Contains("Internal Server Error: 500"))
                    return StatusCode(500);
                else
                return Ok(result);
            }
            else
                return BadRequest(403);
        }

        [AllowAnonymous, HttpGet("api-check")]
        public IActionResult Test()
        {
            if (BasePlugin.Obj.Config.EnableWebAPI)
            {
                return Ok("We Are Working Chief! API System is enabled");
            }
            else
                return BadRequest(403);
        }
    }
}

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
using Eco.Gameplay.Players;
using Newtonsoft.Json;
using Eco.EM.Framework.Helpers;
using Newtonsoft.Json.Converters;
using Eco.EM.Framework.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

                if (result is null)
                    return StatusCode(500);
                else
                    return Ok(result);
            }
            else
                return BadRequest(403);
        }

        [AllowAnonymous, HttpGet("get-prices/{includeOutOfStock:bool?}")]
        [Produces("application/json")]
        public IActionResult GetPrices(bool includeOutOfStock = false)
        {
            List<OfferedItem> noResult = new();
            if (BasePlugin.Obj.Config.EnableWebAPI)
            {
                var result = ShopUtils.GetAllItems(includeOutOfStock).OrderBy(o => o.StoreName);
                if (result is null)
                    return Ok(noResult);
                else
                    return Ok(result);
            }
            else
                return BadRequest(403);
        }
        
        [AllowAnonymous, HttpGet("lookup-user/{username:string?}")]
        [Produces("application/json")]
        public IActionResult LookupUser(string username = "")
        {
            if (BasePlugin.Obj.Config.EnableWebAPI)
            {
                if (string.IsNullOrWhiteSpace(username))
                    return Ok("You must provide a user");
                var user = UserManager.FindUser(username);
                if (user is null)
                    return Ok("No User Found");

                Dictionary<string, string> UserDetails = new()
            {
                { "UserName", user.Name },
                { "SteamID", user.SteamId?.ToString()},
                { "SLGID", user.SlgId?.ToString()  },
                { "UserXp", user.UserXP.XP.ToString() },
                { "Position", user.Position.ToString() },
                { "Language", user.Language.ToString() },
                { "AccountLevel", user.AccountLevel.ToString() },
                { "Reputation", user.Reputation.ToString() },
                { "IsActive", user.IsActive.ToString() },
                { "IsAbandoned", user.IsAbandoned.ToString() },
                { "IsAdmin", user.IsAdmin.ToString() },
                { "SkillSet", user.Skillset.Skills.ToArray().ToString() },
                { "TalentSet", user.Talentset.Talents.ToArray().ToString() },
                { "TotalPlayTime", user.TotalPlayTime.ToString() },
                { "IsOnline", user.IsOnline.ToString() }
            };

                return Ok(UserDetails);
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
using Eco.Gameplay.Players;
using Eco.Gameplay.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eco.EM.Framework.Utils
{
    public static class SkillsUtil
    {
        /// <summary>
        /// Return true if the user has the level of the skillm return false if the user don't have the skill
        /// </summary>
        public static bool HasSkillLevel(User user, Type skillType, int level)
        {
            Skill[] skills = user.Skillset.Skills;
            return skills.Any(s => s.Type == skillType && s.Level >= level);
        }

        /// <summary>
        /// Return the level of the skillType for the user
        /// </summary>
        public static int GetSkillLevel(User user, Type skillType)
        {
            Skill[] skills = user.Skillset.Skills;

            foreach (Skill skill in skills)
            {
                if (skill.Type == skillType)
                {
                    return skill.Level;
                }
            }
            return 0;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace SonoGame
{
    [CreateAssetMenu(fileName = "SonogameSettings", menuName = "ScriptableObjects/Sonogame Settings", order = 1)]
    public class SonogameSettings : ScriptableObject
    {
        public AppConfig appConfig;
        //public List<CampaignDescription> campaignDescriptions;

        public static SonogameSettings Instance
        {
            get
            {
                return Resources.Load("SonogameSettings") as SonogameSettings;
            }
        }

        //public CampaignDescription GetCampaignDescriptionByName(string name)
        //{
        //    CampaignDescription result = campaignDescriptions.Find(x => CampaignName.Clean(x.name) == name);

        //    return result;
        //}

        //public List<string> CampaignNames
        //{
        //    get
        //    {
        //        List<string> campaignNames = new List<string>();

        //        campaignNames.Clear();
        //        foreach (var cd in campaignDescriptions)
        //        {
        //            campaignNames.Add(CampaignName.Clean(cd.name));
        //        }

        //        return campaignNames;
        //    }
        //}

    }

}
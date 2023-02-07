
namespace SonoGame
{
    [System.Serializable]
    public struct CampaignName
    {
        public string name;

        public CampaignName(string name)
        {
            this.name = name;
        }

        public static string Clean(string assetName)
        {
            return assetName.Replace("CampaignDescription (", "").Replace(")", "");
        }
    }
}

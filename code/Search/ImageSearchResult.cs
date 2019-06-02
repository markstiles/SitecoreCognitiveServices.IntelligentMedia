using System.Collections.Generic;
using Sitecore.ContentSearch;
using System.Web.Script.Serialization;
using Sitecore.ContentSearch.SearchTypes;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.Face;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.ComputerVision;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Search
{
    public class ImageSearchResult : SearchResultItem, IImageSearchResult {

        #region properties

        [IndexField("AdultScore")]
        public double AdultScore { get; set; }

        [IndexField("AgeMin")]
        public double AgeMin { get; set; }

        [IndexField("AgeMax")]
        public double AgeMax { get; set; }

        [IndexField("AllPeopleIdentified")]
        public bool AllPeopleIdentified { get; set; }

        [IndexField("AngerMin")]
        public double AngerMin { get; set; }

        [IndexField("AngerMax")]
        public double AngerMax { get; set; }

        [IndexField("Colors")]
        public string[] Colors { get; set; }

        [IndexField("ContemptMin")]
        public double ContemptMin { get; set; }

        [IndexField("ContemptMax")]
        public double ContemptMax { get; set; }

        [IndexField("DisgustMin")]
        public double DisgustMin { get; set; }

        [IndexField("DisgustMax")]
        public double DisgustMax { get; set; }

        [IndexField("FacialAnalysis")]
        public string FacialAnalysisValue { get; set; }

        public Face[] FacialAnalysis => SaturateValue<Face[]>(FacialAnalysisValue) ?? new Face[0];

        [IndexField("FearMin")]
        public double FearMin { get; set; }

        [IndexField("FearMax")]
        public double FearMax { get; set; }

        [IndexField("Gender")]
        public int Gender { get; set; }

        [IndexField("Glasses")]
        public List<int> Glasses { get; set; }

        [IndexField("HappinessMin")]
        public double HappinessMin { get; set; }

        [IndexField("HappinessMax")]
        public double HappinessMax { get; set; }

        [IndexField("IsImage")]
        public bool IsImage { get; set; }

        [IndexField("NeutralMin")]
        public double NeutralMin { get; set; }

        [IndexField("NeutralMax")]
        public double NeutralMax { get; set; }

        [IndexField("People")]
        public string[] People { get; set; }

        [IndexField("RacyScore")]
        public double RacyScore { get; set; }

        [IndexField("SadnessMin")]
        public double SadnessMin { get; set; }

        [IndexField("SadnessMax")]
        public double SadnessMax { get; set; }

        [IndexField("SurpriseMin")]
        public double SurpriseMin { get; set; }

        [IndexField("SurpriseMax")]
        public double SurpriseMax { get; set; }

        [IndexField("Height")]
        public int Height { get; set; }

        [IndexField("Width")]
        public int Width { get; set; }

        [IndexField("Tags")]
        public string[] Tags { get; set; }

        [IndexField("TextAnalysis")]
        public string TextAnalysisValue { get; set; }

        public OcrResults TextAnalysis => SaturateValue<OcrResults>(TextAnalysisValue) ?? new OcrResults();

        [IndexField("_uniqueid")]
        public string UniqueId { get; set; }

        [IndexField("VisionAnalysis")]
        public string VisionAnalysisValue { get; set; }

        public AnalysisResult VisionAnalysis => SaturateValue<AnalysisResult>(VisionAnalysisValue) ?? new AnalysisResult();

        #endregion properties

        private static T SaturateValue<T>(string value) {
            if (string.IsNullOrEmpty(value))
                return default(T);

            try {
                return new JavaScriptSerializer().Deserialize<T>(value);
            } catch { }

            return default(T);
        }
    }
}
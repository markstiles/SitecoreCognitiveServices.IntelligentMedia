using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.ComputerVision;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Search.ComputedFields.Image
{
    public class Tags : BaseComputedField
    {
        protected override object GetFieldValue(Item cognitiveIndexable)
        {
            //words in the image
            var words = GetImageWords();
    
            //tags on the image above a confidence threshold
            var tags = Visions?.Tags?.Where(a => a.Confidence > ConfidenceThreshold).Select(t => t.Name.Trim()).ToList() ?? new List<string>();
            if(tags.Any())
                words.AddRange(tags);

            //tags in the description
            var descTags = Visions?.Description?.Tags?.Select(t => t.Trim()).ToList() ?? new List<string>();
            if (descTags.Any())
                words.AddRange(descTags);

            //celebrity names
            var celebrities = GetCelebrityNames();
            if(celebrities.Any())
                words.AddRange(celebrities);

            //landmark names
            var landmarks = GetLandmarkNames();
            if(landmarks.Any())
                words.AddRange(landmarks);
            
            //people names
            if (People.Any())
                words.AddRange(People);

            //file name
            if(UseImageNameAsTag)
                words.Add(cognitiveIndexable.DisplayName);
            
            return words.Any()
                ? words.Select(a => a.ToLower()).Distinct()
                : null;
        }

        protected List<string> GetImageWords()
        {
            List<string> words = new List<string>();
            if (Text?.Regions == null)
                return words;
            
            foreach (var r in Text.Regions)
            {
                if (r.Lines == null || !r.Lines.Any())
                    continue;

                foreach (var l in r.Lines)
                {
                    if (l.Words == null || !l.Words.Any())
                        continue;

                    foreach (var w in l.Words)
                    {
                        var text = w?.Text.Trim();
                        if (string.IsNullOrWhiteSpace(text) || !Regex.IsMatch(text, @"^[a-zA-Z]+$"))
                            continue;

                        words.Add(text);
                    }
                }
            }

            return words;
        }

        protected List<string> GetCelebrityNames()
        {
            List<string> names = new List<string>();
            if(Visions?.Categories == null)
                return names;

            foreach (var c in Visions.Categories)
            {
                if (c.Detail?.Celebrities == null || !c.Detail.Celebrities.Any())
                    continue;

                foreach (var d in c.Detail.Celebrities)
                {
                    if (d.Confidence < ConfidenceThreshold)
                        continue;

                    names.Add(d.Name);
                }
            }

            return names;
        }

        protected List<string> GetLandmarkNames()
        {
            List<string> names = new List<string>();
            if (Visions?.Categories == null)
                return names;

            foreach (var c in Visions.Categories)
            {
                if (c.Detail?.Landmarks == null || !c.Detail.Landmarks.Any())
                    continue;

                foreach (var l in c.Detail.Landmarks)
                {
                    if (l.Confidence < ConfidenceThreshold)
                        continue;

                    names.Add(l.Name);
                }
            }

            return names;
        }
    }
}
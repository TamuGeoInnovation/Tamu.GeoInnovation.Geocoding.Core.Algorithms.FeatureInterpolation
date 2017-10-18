using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Tamu.GeoInnovation.Geocoding.Core.Algorithms.PenaltyScoring
{

    //this class is deprecated because we want to be able to return a 200, 1990, and 2010 census values which are hard coded in this class
    //use WebServiceGeocodeQueryResult instead


    public class PenaltyCodeResult
    {
        //These ints will be scores for each section - lower the score the less severe the penalty i.e. 0 indicates no penalty 9 is max penalty
        #region Penalty Variables
        public int matchScore { get; set; }     //penalty based on matchscore: 0(100),1(90-100),2(80-90),3(70-80)..etc
        public int inputType { get; set; } // full street, street without number, Zipcode only etc
        public int streetType { get; set; } // PO Box, RR, etc
        public int zip { get; set; } //0-zip matched,1-matched by ambiguous,2-didn't match
        public int city { get; set; } //0-city matched,1-city matched to alias,2-city soundex matched,3-city not matched at all
        public int soundexPenalty { get; set; } //soundex penalty - edit distance/length of word       
        public int zipPenalty { get; set; } //0-zip matched,1-1st digit from right different,2-2nd digit from right different etc       
        public int directionals { get; set; } //pre-post directionals - 0-no error,1-missing pre input,2-missing pre ref,3-missing post input,4-missing post ref
        public int qualifiers { get; set; } //pre-post qualifiers - 0-no error,1-missing pre input,2-missing pre ref,3-missing post input,4-missing post ref
        public int distance { get; set; }     //will assign penalty depending on average distance between all results      
        public int censusBlocks { get; set; }     //census blocks matched 0,didn't match 1
        public int censusTracts { get; set; }     //census Tracts matched 0,didn't match 1      
        public int county { get; set; }     //penalty based on how many counties are different 0-all counties match,9-all counties different       
               
        #endregion
        
        public PenaltyCodeResult()
        {
            matchScore = 0;
            inputType = 0;
            streetType = 0;
            zip = 0;
            city = 0;
            soundexPenalty = 0;
            zipPenalty = 0;
            directionals = 0;
            qualifiers = 0;
            distance = 0;
            censusBlocks = 0;
            censusTracts = 0;
            county = 0;
        }

        public void getPenalty(Dictionary<string, string> scoreResults)
        {
            var dicVal = string.Empty;
            string suffixPenalty = scoreResults["Suffix"];
            double zipPenaltyScore = Convert.ToDouble(Convert.ToDouble(scoreResults["Zip"]));
            assignZipPenalty(zipPenaltyScore);            
        }

        public void assignDirectionalPenalty(string inputStreet,string featureStreet)
        {
            List<string> directionalsList = new List<string>();
            directionalsList.AddRange(new string[] { "North", "NorthEast", "North East", "East", "South East", "South", "SouthWest", "South West", "West", "NorthWest", "North West", "N", "NE", "E", "SE", "S", "SW", "W", "NW" });
            bool inInput = false;
            bool inFeature = false;
            string[] inputParts = inputStreet.Split(' ');
            string[] featureParts = featureStreet.Split(' ');
            foreach (var dir in directionalsList)
            {
                foreach (var inPart in inputParts)
                {
                    if (inPart == dir)
                    {
                        inInput = true;
                    }
                }
                foreach (var featPart in featureParts)
                {
                    if (featPart == dir)
                    {
                        inFeature = true;
                    }
                }
            }
            if (inInput && !inFeature)
            {
                directionals = 2;
            }
            else if (inFeature && !inInput)
            {
                directionals = 1;
            }            
        }

        //public void assignDirectionalPenalty(string inputPre, string featurePre,string inputPost,string featurePost)
        //{
        //    bool missingPreDir = false;
        //    bool erroneousPreDir = false;
        //    bool missingPostDir = false;
        //    bool erroneousPostDir = false;

        //    if(inputPre != "" && featurePre == "")
        //    {
        //        erroneousPreDir = true;
        //    }
        //    if(inputPre == "" && featurePre != "")
        //    {
        //        missingPreDir = true;
        //    }
        //    if (inputPost == "" && featurePost != "")
        //    {
        //        missingPostDir = true;
        //    }
        //    if (inputPost != "" && featurePost == "")
        //    {
        //        erroneousPostDir = true;
        //    }            
        //    if (missingPreDir && !missingPostDir && !erroneousPreDir && !erroneousPostDir)
        //    {
        //        directionals = 1;
        //    }
        //    if (!missingPreDir && missingPostDir && !erroneousPreDir && !erroneousPostDir)
        //    {
        //        directionals = 2;
        //    }
        //    if (!missingPostDir && !missingPreDir && erroneousPreDir && !erroneousPostDir)
        //    {
        //        directionals = 3;
        //    }
        //    if (!missingPostDir && !missingPreDir && !erroneousPreDir && erroneousPostDir)
        //    {
        //        directionals = 4;
        //    }
        //    if (!missingPostDir && !missingPreDir && erroneousPreDir && erroneousPostDir)
        //    {
        //        directionals = 5;
        //    }
        //    if (missingPostDir && missingPreDir && !erroneousPreDir && !erroneousPostDir)
        //    {
        //        directionals = 6;
        //    }
        //    if (inputPre != "" && featurePre != "")
        //    {
        //        if (inputPre != featurePre)
        //            directionals = 7;
        //    }
        //    if (inputPost != "" && featurePost != "")
        //    {
        //        if (inputPost != featurePost)
        //            directionals = 8;
        //    }
        //    if (inputPost != "" && featurePost != "" && inputPre != "" && featurePre != "")
        //    {
        //        if (inputPost != featurePost && inputPre != featurePre)
        //            directionals = 9;
        //    }
        //    if (inputPre != "" && featurePost != "")
        //    {
        //        if (inputPre == featurePost)
        //            directionals = 0;
        //    }
        //    if (inputPost != "" && featurePre != "")
        //    {
        //        if (inputPost == featurePre)
        //            directionals = 0;
        //    }

        //}
        public void assignDirectionalPenalty(string inputPre, string featurePre, string inputPost, string featurePost)
        {
            
            if (inputPre != "" && inputPost == "" && featurePre == "" && featurePost != "" )
            {
                if (inputPre == featurePost)
                {
                    directionals = 4;
                }
                else
                {
                    directionals = 5;
                }
            }
            else if (inputPre == "" && inputPost != "" && featurePre != "" && featurePost == "")
            {
                if (inputPost == featurePre)
                {
                    directionals = 4;
                }
                else
                {
                    directionals = 5;
                }
            }
            else if (inputPre != "" && inputPost == "" && featurePre != "" && featurePost == "")
            {
                if (inputPre == featurePre)
                {
                    directionals = 0;
                }
                else
                {
                    directionals = 5;
                }
            }
            else if (inputPre == "" && inputPost != "" && featurePre == "" && featurePost != "")
            {
                if (inputPost == featurePost)
                {
                    directionals = 0;
                }
                else
                {
                    directionals = 5;
                }
            }
            else if (inputPre == "" && inputPost == "" && (featurePre != "" || featurePost != ""))
            {
                directionals = 1;
            }
            else if ((inputPre != "" || inputPost != "") && featurePre == "" && featurePost == "")
            {
                directionals = 3;
            }
            else if (inputPre == "" && inputPost != "" && featurePre == "" && featurePost == "")
            {
                directionals = 3;
            }

        }
        public void assignZipPenalty(double zipPenaltyScore)
        {
            if (zipPenaltyScore < 1 && zipPenaltyScore > 0)
            {
                zipPenalty = 1;
            }
            else if (zipPenaltyScore < 1.5 && zipPenaltyScore > 1)
            {
                zipPenalty = 2;
            }
            else if (zipPenaltyScore < 2 && zipPenaltyScore > 1.5)
            {
                zipPenalty = 3;
            }
            else if (zipPenaltyScore < 2.6 && zipPenaltyScore > 2.1)
            {
                zipPenalty = 4;
            }
            else if (zipPenaltyScore > 2.65)
            {
                zipPenalty = 5;
            }
            else
            {
                zipPenalty = 0;
            }
        }

        public void assignZipPenalty(string inputZip,string featureZip)
        {
            if (inputZip != "" && inputZip != null && featureZip != "" && featureZip != null)
            {
                double ret = 0;
                try
                {
                    double errorIndex = 0;

                    int inputLength = inputZip.Length;
                    int referenceLength = featureZip.Length;
                    double fullweight = 0.26627218934911245;
                    int minLength = Math.Min(inputLength, referenceLength);

                    for (int i = 0; i < minLength; i++)
                    {
                        if (inputZip[i] != featureZip[i])
                        {
                            errorIndex = Convert.ToDouble(i);
                            break;
                        }
                    }

                    double proportion = 1;
                    if(errorIndex>0)
                    {
                        proportion = (errorIndex / 5);
                    }
                    double normalized = Convert.ToDouble(1.0) - proportion;
                    double proportionalWeight = fullweight * normalized;

                    ret = proportionalWeight * 10;

                }
                catch (Exception e)
                {
                    throw new Exception("Exception in ComputePenaltyZipPositionProportional: " + e.Message, e);
                }
                assignZipPenalty(ret);
            }
            else
            {
                zipPenalty = 6;
            }
            
        }
        public string getPenaltyString()
        {
            StringBuilder ret = new StringBuilder();
            ret.Append(matchScore);
            ret.Append(inputType);
            ret.Append(streetType);
            ret.Append(zip);
            ret.Append(zipPenalty);
            ret.Append(city);
            ret.Append(soundexPenalty);           
            ret.Append(directionals);
            ret.Append(qualifiers);
            ret.Append(distance);
            ret.Append(censusBlocks);
            ret.Append(censusTracts);
            ret.Append(county);
            return ret.ToString();
        }

    }
}

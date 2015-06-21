using USC.GISResearchLab.Common.Core.Geocoders.FeatureMatching;
//using USC.GISResearchLab.Common.Core.Geocoders.ReferenceDatasets.Sources.Interfaces;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Interfaces;
using USC.GISResearchLab.Geocoding.Core.Queries.Parameters;
using System.Diagnostics;

namespace USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.AbstractClasses
{
    public abstract class AbstractFeatureInterpolationMethod : IInterpolationMethod
    {
        #region Properties
        
        public TraceSource TraceSource { get; set; }

        public int Quality { get; set; }
        public int Method { get; set; }
        public string Name { get; set; }

        public InterpolationType InterpolationType { get; set; }
        public InterpolationSubType InterpolationSubType { get; set; }

        //public IFeatureSource FeatureSource { get; set; }

        #endregion

        public abstract FeatureInterpolationResult DoFeatureInterpolation(ParameterSet parameterSet, MatchedFeature matchedFeature);


        
    }
}

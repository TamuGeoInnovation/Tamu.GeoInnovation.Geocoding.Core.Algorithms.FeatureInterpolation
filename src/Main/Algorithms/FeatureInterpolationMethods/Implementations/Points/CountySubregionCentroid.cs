using USC.GISResearchLab.Common.Core.Geocoders.ReferenceDatasets.Sources.Interfaces;
using USC.GISResearchLab.Geocoding.Core.Metadata.Qualities;

namespace USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Implementations
{
    public class CountySubregionCentroid : PointMethod
	{
        public CountySubregionCentroid(IFeatureSource featureSource)
            : base(featureSource)
        {
            Name = IneterpolationMethodNames.METHOD_NAME_COUNTY_SUBREGION_CENTROID;
            Method = IneterpolationMethodNames.METHOD_COUNTY_SUBREGION_CENTROID;
            Quality = (int)(GeocodeQualityType.CountySubdivisionCentroid);

            InterpolationType = InterpolationType.ArealInterpolation;
            InterpolationSubType = InterpolationSubType.ArealInterpolationGeometricCentroid;
        }
	}
}

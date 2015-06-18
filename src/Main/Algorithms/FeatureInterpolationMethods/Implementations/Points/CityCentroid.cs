using USC.GISResearchLab.Common.Core.Geocoders.ReferenceDatasets.Sources.Interfaces;
using USC.GISResearchLab.Geocoding.Core.Metadata.Qualities;

namespace USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Implementations
{
	
	public class CityCentroid : PointMethod
	{
        public CityCentroid(IFeatureSource featureSource)
            : base(featureSource)
        {
            Name = IneterpolationMethodNames.METHOD_NAME_CITY_CENTROID;
            Method = IneterpolationMethodNames.METHOD_CITY_CENTROID;
            Quality = (int)(GeocodeQualityType.CityCentroid);

            InterpolationType = InterpolationType.ArealInterpolation;
            InterpolationSubType = InterpolationSubType.ArealInterpolationGeometricCentroid;
        }
    }
}

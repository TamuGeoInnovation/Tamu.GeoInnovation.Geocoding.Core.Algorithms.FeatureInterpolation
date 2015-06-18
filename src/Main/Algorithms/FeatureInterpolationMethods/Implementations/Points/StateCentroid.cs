using USC.GISResearchLab.Common.Core.Geocoders.ReferenceDatasets.Sources.Interfaces;
using USC.GISResearchLab.Geocoding.Core.Metadata.Qualities;

namespace USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Implementations
{

    public class StateCentroid : PointMethod
    {
        public StateCentroid(IFeatureSource featureSource)
            : base(featureSource)
        {
            Name = IneterpolationMethodNames.METHOD_NAME_STATE_CENTROID;
            Method = IneterpolationMethodNames.METHOD_STATE_CENTROID;
            Quality = (int)(GeocodeQualityType.StateCentroid);

            InterpolationType = InterpolationType.ArealInterpolation;
            InterpolationSubType = InterpolationSubType.ArealInterpolationGeometricCentroid;
        }
    }
}

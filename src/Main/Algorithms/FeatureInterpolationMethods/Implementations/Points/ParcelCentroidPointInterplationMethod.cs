using USC.GISResearchLab.Common.Core.Geocoders.ReferenceDatasets.Sources.Interfaces;
using USC.GISResearchLab.Geocoding.Core.Metadata.Qualities;

namespace USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Implementations
{

    public class ParcelCentroidPointInterplationMethod : PointMethod
    {
        public ParcelCentroidPointInterplationMethod(IFeatureSource featureSource)
            : base(featureSource)
        {
            Name = IneterpolationMethodNames.METHOD_NAME_PARCEL_CENTROID_POINT;
            Method = IneterpolationMethodNames.METHOD_PARCEL_CENTROID_POINT;
            Quality = (int)(GeocodeQualityType.ExactParcelCentroidPoint);

            InterpolationType = InterpolationType.ArealInterpolation;
            InterpolationSubType = InterpolationSubType.ArealInterpolationGeometricCentroid;
        }
    }
}

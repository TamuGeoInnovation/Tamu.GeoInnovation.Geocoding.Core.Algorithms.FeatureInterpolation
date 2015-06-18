using USC.GISResearchLab.Common.Core.Geocoders.ReferenceDatasets.Sources.Interfaces;
using USC.GISResearchLab.Geocoding.Core.Metadata.Qualities;

namespace USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Implementations
{

    public class ZipCodeCentroid : PointMethod
	{
        public ZipCodeCentroid(IFeatureSource featureSource)
            : base(featureSource)
        {
            Name = IneterpolationMethodNames.METHOD_NAME_ZIP_CODE_TABULATION_AREA_CENTROID;
            Method = IneterpolationMethodNames.METHOD_ZIP_CODE_TABULATION_AREA_CENTROID;
            Quality = (int)(GeocodeQualityType.ZCTACentroid);

            InterpolationType = InterpolationType.ArealInterpolation;
            InterpolationSubType = InterpolationSubType.ArealInterpolationGeometricCentroid;
        }
	}
}

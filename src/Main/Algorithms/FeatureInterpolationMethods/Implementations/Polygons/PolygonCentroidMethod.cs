using USC.GISResearchLab.Common.Core.Geocoders.ReferenceDatasets.Sources.Interfaces;

namespace USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Implementations
{

    public class PolygonCentroidMethod : PointMethod
	{
        #region Properties
        
        
        #endregion

        public PolygonCentroidMethod()
        {
        }

        public PolygonCentroidMethod(IFeatureSource featureSource)
        {
            //TODO Check to see if still works - removed DG 2015-06-09
            //FeatureSource = featureSource;
            Name = IneterpolationMethodNames.METHOD_NAME_POLYGON_CENTROID;
            Method = IneterpolationMethodNames.METHOD_POLYGON_CENTROID;

            InterpolationType = InterpolationType.ArealInterpolation;
            InterpolationSubType = InterpolationSubType.ArealInterpolationGeometricCentroid;
        }
	}
}

using System;
using System.Reflection;
using USC.GISResearchLab.Common.Core.Geocoders.FeatureMatching;
using USC.GISResearchLab.Common.Core.Geocoders.ReferenceDatasets.Sources.Interfaces;
using USC.GISResearchLab.Common.Geographics.Units;
using USC.GISResearchLab.Common.Geometries.Points;
using USC.GISResearchLab.Common.Geometries.Polygons;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.AbstractClasses;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Interfaces;
using USC.GISResearchLab.Geocoding.Core.Metadata.FeatureInterpolationResults;
using USC.GISResearchLab.Geocoding.Core.Metadata.Qualities;
using USC.GISResearchLab.Geocoding.Core.OutputData.Error;
using USC.GISResearchLab.Geocoding.Core.Queries.Parameters;

namespace USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Implementations
{
    public class ActualGeometryMethod : AbstractFeatureInterpolationMethod, IPolygonInterpolationMethod
    {
        #region Properties
        
        #endregion

        #region Method Members

        public string GetName()
        {
            return IneterpolationMethodNames.METHOD_NAME_ACTUAL_GEOMETRY;
        }

        public int GetMethod()
        {
            return IneterpolationMethodNames.METHOD_ACTUAL_GEOMETRY;
        }

        #endregion

        public ActualGeometryMethod(IFeatureSource featureSource)
        {
            //TODO Check to see if still works - removed DG 2015-06-09
            //FeatureSource = featureSource;
            Name = IneterpolationMethodNames.METHOD_NAME_ACTUAL_GEOMETRY;
            Method = IneterpolationMethodNames.METHOD_ACTUAL_GEOMETRY;
            Quality = (int)(GeocodeQualityType.ExactParcelCentroid);

            InterpolationType = InterpolationType.ArealInterpolation;
            InterpolationSubType = InterpolationSubType.ArealInterpolationGeometricCentroid;
        }

        public override FeatureInterpolationResult DoFeatureInterpolation(ParameterSet parameterSet, MatchedFeature matchedFeature)
        {
            Serilog.Log.Verbose(this.GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " - entered");
            FeatureInterpolationResult ret = new FeatureInterpolationResult();

            try
            {
                Polygon polygon = (Polygon)matchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry;
                ret.Geometry = new Point(polygon.Centroid);

                if (matchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.Area != 0)
                {
                    ret.GeocodedError.ErrorBounds = UnitConverter.ConvertArea(matchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.AreaUnits, LinearUnitTypes.Meters, matchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.Area);
                    ret.GeocodedError.ErrorBoundsUnit = LinearUnitTypes.Meters;
                    ret.GeocodedError.ErrorCalculationType = GeocodedErrorCalculationType.featureGeometryArea;
                }
                else
                {
                    ret.GeocodedError.ErrorBounds = BoundingBoxUnitConverter.ConvertToNonLinearBoundingBoxToLinearArea(polygon.BoundingBox, LinearUnitTypes.Meters);
                    ret.GeocodedError.ErrorBoundsUnit = LinearUnitTypes.Meters;
                    ret.GeocodedError.ErrorCalculationType = GeocodedErrorCalculationType.featureBoundingBoxArea;
                }
                ret.FeatureInterpolationResultType = FeatureInterpolationResultType.Success;
            }
            catch (Exception e)
            {
            
                Serilog.Log.Error(e, this.GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " errored out");
               
            ret.FeatureInterpolationResultType = FeatureInterpolationResultType.ExceptionOccurred;
                ret.Error = "ActualGeometryMethod - Error getting reference feature: " + e.Message;
                ret.ExceptionOccurred = true;
                ret.Exception = e;
                ret.GeocodedError.GeoError = "Exception occured: " + e.Message;
            }
            return ret;
        }

    }
}
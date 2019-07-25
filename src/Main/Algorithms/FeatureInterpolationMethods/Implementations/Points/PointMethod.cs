using System;
using System.Reflection;
using USC.GISResearchLab.Common.Core.Geocoders.FeatureMatching;
using USC.GISResearchLab.Common.Core.Geocoders.ReferenceDatasets.Sources.Interfaces;
using USC.GISResearchLab.Common.Geographics.Units;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.AbstractClasses;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Interfaces;
using USC.GISResearchLab.Geocoding.Core.Metadata.FeatureInterpolationResults;
using USC.GISResearchLab.Geocoding.Core.OutputData.Error;
using USC.GISResearchLab.Geocoding.Core.Queries.Parameters;

namespace USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Implementations
{

    public abstract class PointMethod : AbstractFeatureInterpolationMethod, IPointInterpolationMethod
    {
        #region Properties


        #endregion

        public PointMethod()
        {
        }

        public PointMethod(IFeatureSource featureSource)
        {
            //TODO Check to see if still works - removed DG 2015-06-09
            //FeatureSource = featureSource;
            Name = IneterpolationMethodNames.METHOD_NAME_POINT;
            Method = IneterpolationMethodNames.METHOD_POINT;

            InterpolationType = InterpolationType.ArealInterpolation;
            InterpolationSubType = InterpolationSubType.ArealInterpolationGeometricCentroid;
        }

        public override FeatureInterpolationResult DoFeatureInterpolation(ParameterSet parameterSet, MatchedFeature matchedFeature)
        {
            Serilog.Log.Verbose(this.GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " - entered");
            FeatureInterpolationResult ret = new FeatureInterpolationResult();

            try
            {
                ret.Geometry = matchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry;
                ret.GeocodedError.ErrorBounds = UnitConverter.ConvertArea(matchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.AreaUnits, parameterSet.OutputUnits, matchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.Area);
                ret.GeocodedError.ErrorBoundsUnit = parameterSet.OutputUnits;
                ret.GeocodedError.ErrorCalculationType = GeocodedErrorCalculationType.featureGeometryArea;
                ret.FeatureInterpolationResultType = FeatureInterpolationResultType.Success;
            }
            catch (Exception e)
            {
            
                Serilog.Log.Error(e, this.GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " errored out");
                
            ret.FeatureInterpolationResultType = FeatureInterpolationResultType.ExceptionOccurred;
                ret.Error = "Point method - Error getting reference feature: " + e.Message;
                ret.ExceptionOccurred = true;
                ret.Exception = e;
                ret.GeocodedError.GeoError = "Exception occured: " + e.Message;
            }
            return ret;
        }
    }
}

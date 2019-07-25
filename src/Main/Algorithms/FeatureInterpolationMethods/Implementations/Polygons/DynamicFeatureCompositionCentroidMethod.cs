using System;
using System.Reflection;
using USC.GISResearchLab.Common.Core.Geocoders.FeatureMatching;
using USC.GISResearchLab.Common.Core.Geocoders.ReferenceDatasets.Sources.Interfaces;
using USC.GISResearchLab.Common.Geographics.Units;
using USC.GISResearchLab.Common.Geometries.BoundingBoxes;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.AbstractClasses;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Interfaces;
using USC.GISResearchLab.Geocoding.Core.Metadata.FeatureInterpolationResults;
using USC.GISResearchLab.Geocoding.Core.Metadata.Qualities;
using USC.GISResearchLab.Geocoding.Core.OutputData.Error;
using USC.GISResearchLab.Geocoding.Core.Queries.Parameters;

namespace USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Implementations
{

    public class DynamicFeatureCompositionCentroidMethod : AbstractFeatureInterpolationMethod, IPolygonInterpolationMethod
    {
        #region Properties

        #endregion

        public DynamicFeatureCompositionCentroidMethod(IFeatureSource featureSource)
        {
            //TODO Check to see if still works - removed DG 2015-06-09
            //FeatureSource = featureSource;
            Name = IneterpolationMethodNames.METHOD_NAME_DYNAMIC_FEATURE_COMPOSITION_CENTROID;
            Method = IneterpolationMethodNames.METHOD_DYNAMIC_FEATURE_COMPOSITION_CENTROID;
            Quality = (int)(GeocodeQualityType.DynamicFeatureCompositionCentroid);

            InterpolationType = InterpolationType.ArealInterpolation;
            InterpolationSubType = InterpolationSubType.ArealInterpolationConvexHullCentroid;
        }

        public override FeatureInterpolationResult DoFeatureInterpolation(ParameterSet parameterSet, MatchedFeature matchedFeature)
        {
            Serilog.Log.Verbose(this.GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " - entered");
            FeatureInterpolationResult ret = new FeatureInterpolationResult();

            try
            {
                CompositeMatchedFeature compositeMatchedFeature = (CompositeMatchedFeature)matchedFeature;

                ret.Geometry = ((BoundingBox)compositeMatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry).CentroidPoint;
                ret.GeocodedError.ErrorBounds = BoundingBoxUnitConverter.ConvertToNonLinearBoundingBoxToLinearArea((BoundingBox)compositeMatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry, LinearUnitTypes.Meters);
                ret.GeocodedError.ErrorBoundsUnit = LinearUnitTypes.Meters;
                ret.GeocodedError.ErrorCalculationType = GeocodedErrorCalculationType.featureBoundingBoxArea;
                ret.FeatureInterpolationResultType = FeatureInterpolationResultType.Success;

            }
            catch (Exception e)
            {
                Serilog.Log.Error(e, this.GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " errored out");
                ret.FeatureInterpolationResultType = FeatureInterpolationResultType.ExceptionOccurred;
                ret.Error = "DFC - Error getting reference feature: " + e.Message;
                ret.ExceptionOccurred = true;
                ret.Exception = e;
                ret.GeocodedError.GeoError = "Exception occured: " + e.Message;
            }
            return ret;
        }
    }
}

using System;
using USC.GISResearchLab.Common.Core.Geocoders.FeatureMatching;
using USC.GISResearchLab.Common.Core.Geocoders.ReferenceDatasets.Sources.Interfaces;
using USC.GISResearchLab.Common.Geographics.Units;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.AbstractClasses;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Interfaces;
using USC.GISResearchLab.Geocoding.Core.Metadata.FeatureInterpolationResults;
using USC.GISResearchLab.Geocoding.Core.Metadata.Qualities;
using USC.GISResearchLab.Geocoding.Core.Queries.Parameters;

namespace USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Implementations
{

    public class USPSZip4TigerLineCentroidMethod : AbstractLinearFeatureInterpolationMethod, ILinearInterpolationMethod
    {
        #region Properties
        
        #endregion

        public USPSZip4TigerLineCentroidMethod(IFeatureSource featureSource)
        {
            //TODO Check to see if still works - removed DG 2015-06-09
            //FeatureSource = featureSource;
            Name = IneterpolationMethodNames.METHOD_NAME_USPS_ZIP_CODE_PLUS_4_LINE_CENTROID;
            Method = IneterpolationMethodNames.METHOD_USPS_ZIP_CODE_PLUS_4_LINE_CENTROID;
            Quality = (int)(GeocodeQualityType.USPSZipPlus4LineCentroid);

            InterpolationType = InterpolationType.LinearInterpolation;
            InterpolationSubType = InterpolationSubType.LinearInterpolationMidPoint;
            
            DropbackValue = 10;
            DropbackUnits = LinearUnitTypes.Meters;
        }

        public override FeatureInterpolationResult DoFeatureInterpolation(ParameterSet parameterSet, MatchedFeature matchedFeature)
        {
            FeatureInterpolationResult ret = new FeatureInterpolationResult();

            try
            {
                return base.DoFeatureInterpolation(parameterSet, matchedFeature, 100, 50);
            }
            catch (Exception e)
            {
                ret.FeatureInterpolationResultType = FeatureInterpolationResultType.ExceptionOccurred;
                ret.Error = "Error performing interpolation: " + e.Message;
                ret.ExceptionOccurred = true;
                ret.Exception = e;
                ret.GeocodedError.GeoError = "Exception occured: " + e.Message;
            }
            return ret;
        }
    }
}

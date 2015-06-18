using USC.GISResearchLab.Common.Core.Geocoders.FeatureMatching;
//using USC.GISResearchLab.Common.Core.Geocoders.ReferenceDatasets.Sources.Interfaces;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Interfaces;
using USC.GISResearchLab.Geocoding.Core.Queries.Parameters;
using System.Diagnostics;

namespace USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.AbstractClasses
{
    public abstract class AbstractFeatureInterpolationMethod : IInterpolationMethod
    {
        #region Properties
        
        public TraceSource TraceSource { get; set; }

        public int Quality { get; set; }
        public int Method { get; set; }
        public string Name { get; set; }

        public InterpolationType InterpolationType { get; set; }
        public InterpolationSubType InterpolationSubType { get; set; }

        //public IFeatureSource FeatureSource { get; set; }

        #endregion

        public abstract FeatureInterpolationResult DoFeatureInterpolation(ParameterSet parameterSet, MatchedFeature matchedFeature);


        //public virtual List<IGeocode> DoFeatureInterpolation(ParameterSet parameterSet, FeatureMatchingResult featureMatchingResult)
        //{

        //    List <Geocode> ret = new List<IGeocode>();

        //    StreetAddress streetAddress = parameterSet.StreetAddress;
        //    //
        //    try
        //    {
        //        //FeatureMatchingResult featureMatchingResult = FeatureSource.DoFeatureMatching(parameterSet);


        //        foreach (MatchedFeature matchedFeature in featureMatchingResult.MatchedFeatures)
        //        {

        //            Geocode geocode = new Geocode(parameterSet.GeocoderConfiguration.Version);

        //            geocode.InputAddress = streetAddress;
        //            geocode.Geometry = new Point();
        //            geocode.MethodType = Name;
        //            geocode.ParsedAddress = parameterSet.StreetAddress;
        //            geocode.Attempted = true;


        //            //FeatureMatchingResult featureMatchingResult = FeatureSource.DoFeatureMatching(parameterSet);

        //            geocode.FM_Result = featureMatchingResult;
        //            geocode.FM_ResultType = featureMatchingResult.FeatureMatchingResultType;
        //            geocode.FM_Notes = featureMatchingResult.FeatureMatchingNotes;
        //            geocode.FM_TieNotes = featureMatchingResult.FeatureMatchingTieNotes;
        //            geocode.FM_TieStrategy = featureMatchingResult.TieHandlingStrategyType;
        //            geocode.FM_ResultCount = featureMatchingResult.FeatureMatchingResultCount;
        //            geocode.FM_GeographyType = featureMatchingResult.FeatureMatchingGeographyType;

        //            geocode.Statistics.MatchedLocationTypeStatistics.setMatchedLocationType((int)(featureMatchingResult.MatchedLocationTypes));
        //            geocode.Statistics.ReferenceDatasetsStatistics.AddReferenceDatasetStatistics(featureMatchingResult.ReferenceDatasetStatistics);
        //            geocode.Statistics.ReferenceDatasetsStatistics.AddReferenceDatasetStatistics(geocode.Statistics.ReferenceDatasetsStatistics.ReferenceDatasetStatistics);


        //            /////////
        //            ////////

        //            if (featureMatchingResult.FeatureMatchingResultType == FeatureMatchingResultType.Success || featureMatchingResult.FeatureMatchingResultType == FeatureMatchingResultType.BrokenTie)
        //            {

        //                MatchedFeature feature = featureMatchingResult.MatchedFeatures[0];

        //                Geocode geocode = new Geocode(parameterSet.GeocoderConfiguration.Version);
        //                geocode.InputAddress = streetAddress;
        //                geocode.Geometry = new Point();
        //                geocode.MethodType = Name;
        //                geocode.SourceType = FeatureSource.Name;
        //                geocode.SourceVintage = FeatureSource.Vintage;
        //                geocode.ParsedAddress = parameterSet.StreetAddress;
        //                geocode.Attempted = true;
        //                geocode.FM_Result = featureMatchingResult;
        //                geocode.FM_ResultType = featureMatchingResult.FeatureMatchingResultType;
        //                geocode.FM_Notes = featureMatchingResult.FeatureMatchingNotes;
        //                geocode.FM_TieNotes = featureMatchingResult.FeatureMatchingTieNotes;
        //                geocode.FM_TieStrategy = featureMatchingResult.TieHandlingStrategyType;
        //                geocode.FM_ResultCount = featureMatchingResult.FeatureMatchingResultCount;
        //                geocode.FM_GeographyType = featureMatchingResult.FeatureMatchingGeographyType;

        //                geocode.Statistics.MatchedLocationTypeStatistics.setMatchedLocationType((int)(featureMatchingResult.MatchedLocationTypes));
        //                geocode.Statistics.ReferenceDatasetsStatistics.AddReferenceDatasetStatistics(featureMatchingResult.ReferenceDatasetStatistics);



        //                geocode.InterpolationType = InterpolationType;
        //                geocode.InterpolationSubType = InterpolationSubType;

        //                geocode.MatchedFeature = feature;
        //                geocode.MatchedAddress = feature.MatchedAddress;
        //                geocode.MatchedFeatureAddress = feature.MatchedFeatureAddress;

        //                geocode.MatchedFeature.PrimaryIdField = FeatureSource.IdFieldNamePrimary;
        //                geocode.MatchedFeature.SecondaryIdField = FeatureSource.IdFieldNameSecondary;
        //                geocode.MatchedFeature.PrimaryIdValue = feature.MatchedReferenceFeature.PrimaryId;
        //                geocode.MatchedFeature.SecondaryIdValue = feature.MatchedReferenceFeature.SecondaryId;

        //                FeatureInterpolationResult featureInterpolationResult = DoFeatureInterpolation(parameterSet, feature);

        //                if (featureInterpolationResult.FeatureInterpolationResultType == FeatureInterpolationResultType.Success)
        //                {
        //                    geocode.Valid = true;
        //                    geocode.Geometry = featureInterpolationResult.Geometry;

        //                    if (featureInterpolationResult.GeocodedError.ErrorBounds > 0)
        //                    {
        //                        geocode.GeocodedError = featureInterpolationResult.GeocodedError;
        //                        geocode.MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.Area = geocode.GeocodedError.ErrorBounds;
        //                        geocode.MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.AreaUnits = geocode.GeocodedError.ErrorBoundsUnit;
        //                    }
        //                    else if (feature.GeocodedError.ErrorBounds > 0)
        //                    {
        //                        geocode.GeocodedError = feature.GeocodedError;
        //                    }


        //                    geocode.QueryStatusCodes = QueryStatusCodes.Success;
        //                    geocode.GeocodeQualityType = FeatureSource.geocodeQualityType;
        //                }
        //                else
        //                {
        //                    geocode.MethodError = featureInterpolationResult.Error;
        //                    geocode.QueryStatusCodes = QueryStatusCodes.InternalError;
        //                    geocode.Exception = featureInterpolationResult.Exception;
        //                    geocode.ErrorMessage = featureInterpolationResult.Error;
        //                    geocode.ExceptionOccurred = true;
        //                }

        //                ret.Add(geocode);

        //            }
        //            else if (featureMatchingResult.FeatureMatchingResultType == FeatureMatchingResultType.Nearby)
        //            {
        //                MatchedFeature feature = featureMatchingResult.MatchedFeature;

        //                ret.InterpolationType = InterpolationType;
        //                ret.InterpolationSubType = InterpolationSubType;

        //                ret.MatchedFeature = feature;
        //                ret.MatchedAddress = feature.MatchedAddress;
        //                ret.MatchedFeatureAddress = feature.MatchedFeatureAddress;

        //                ret.MatchedFeature.PrimaryIdField = FeatureSource.IdFieldNamePrimary;
        //                ret.MatchedFeature.SecondaryIdField = FeatureSource.IdFieldNameSecondary;
        //                ret.MatchedFeature.PrimaryIdValue = feature.MatchedReferenceFeature.PrimaryId;
        //                ret.MatchedFeature.SecondaryIdValue = feature.MatchedReferenceFeature.SecondaryId;

        //                FeatureInterpolationResult featureInterpolationResult = DoFeatureInterpolation(parameterSet, feature);

        //                if (featureInterpolationResult.FeatureInterpolationResultType == FeatureInterpolationResultType.Success)
        //                {

        //                    ret.Valid = true;
        //                    ret.Geometry = featureInterpolationResult.Geometry;
        //                    ret.GeocodedError.ErrorBounds = featureInterpolationResult.GeocodedError.ErrorBounds;
        //                    ret.GeocodedError.ErrorBoundsUnit = featureInterpolationResult.GeocodedError.ErrorBoundsUnit;
        //                    ret.GeocodedError.ErrorCalculationType = featureInterpolationResult.GeocodedError.ErrorCalculationType;

        //                    ret.MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.Area = ret.GeocodedError.ErrorBounds;
        //                    ret.MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.AreaUnits = ret.GeocodedError.ErrorBoundsUnit;

        //                    ret.QueryStatusCodes = QueryStatusCodes.Success;
        //                    ret.GeocodeQualityType = FeatureSource.geocodeQualityType;
        //                }
        //                else
        //                {
        //                    ret.MethodError = featureInterpolationResult.Error;
        //                    ret.QueryStatusCodes = QueryStatusCodes.InternalError;
        //                    ret.Exception = featureInterpolationResult.Exception;
        //                    ret.ErrorMessage = featureInterpolationResult.Error;
        //                    ret.ExceptionOccurred = true;
        //                }

        //            }
        //            else if (featureMatchingResult.FeatureMatchingResultType == FeatureMatchingResultType.Ambiguous)
        //            {
        //                MatchedFeature feature = featureMatchingResult.MatchedFeature;

        //                ret.Valid = false;
        //                ret.GeocodeQualityType = GeocodeQualityType.Unmatchable;
        //                ret.QueryStatusCodes = QueryStatusCodes.Success;
        //                ret.InterpolationType = InterpolationType.NotAttempted;
        //                ret.InterpolationSubType = InterpolationSubType.NotAttempted;


        //                //ret.MatchedFeature = compositeFeature;
        //                //ret.MatchedAddress = compositeFeature.MatchedAddress;
        //                //ret.MatchedFeatureAddress = compositeFeature.MatchedFeatureAddress;

        //                ret.MatchedFeature = feature;
        //                ret.MatchedFeature.PrimaryIdField = FeatureSource.IdFieldNamePrimary;
        //                ret.MatchedFeature.SecondaryIdField = FeatureSource.IdFieldNameSecondary;
        //                //ret.Matchedoutput.PrimaryIdValue = compositeFeature.MatchedReferenceFeature.Geometry.PrimaryId;
        //                //ret.Matchedoutput.SecondaryIdValue = compositeFeature.MatchedReferenceFeature.Geometry.SecondaryId;

        //            }
        //            else if (featureMatchingResult.FeatureMatchingResultType == FeatureMatchingResultType.Composite)
        //            {
        //                CompositeMatchedFeature compositeFeature = (CompositeMatchedFeature)featureMatchingResult.MatchedFeature;
        //                ret.Valid = true;

        //                ret.MatchedFeature = compositeFeature;
        //                ret.MatchedAddress = compositeFeature.MatchedAddress;
        //                ret.MatchedFeatureAddress = compositeFeature.MatchedFeatureAddress;

        //                ret.MatchedFeature.PrimaryIdField = FeatureSource.IdFieldNamePrimary;
        //                ret.MatchedFeature.SecondaryIdField = FeatureSource.IdFieldNameSecondary;
        //                ret.MatchedFeature.PrimaryIdValue = compositeFeature.MatchedReferenceFeature.PrimaryId;
        //                ret.MatchedFeature.SecondaryIdValue = compositeFeature.MatchedReferenceFeature.SecondaryId;

        //                ret.InterpolationType = InterpolationType.ArealInterpolation;
        //                ret.InterpolationSubType = InterpolationSubType.ArealInterpolationConvexHullCentroid;

        //                SqlGeometry centroid = compositeFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeometry.STCentroid();
        //                //if (String.Compare(compositeFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeometry.STGeometryType().Value, "linestring", true) == 0)
        //                //{
        //                //    double length = compositeFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeometry.STLength().Value;
        //                //    centroid = SQLSpatialTools.SQLSpatialToolsFunctions.LocateAlongGeom(compositeFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeometry, length/2.0);
        //                //}
        //                //else
        //                //{
        //                //    centroid = compositeFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeometry.STCentroid();
        //                //}

        //                ret.Geometry = new Point(centroid.STX.Value, centroid.STY.Value);
        //                ret.GeocodedError = compositeFeature.GeocodedError;
        //                ret.QueryStatusCodes = QueryStatusCodes.Success;
        //                ret.GeocodeQualityType = GeocodeQualityType.DynamicFeatureCompositionCentroid;

        //                ret.MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.Area = ret.GeocodedError.ErrorBounds;
        //                ret.MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.AreaUnits = ret.GeocodedError.ErrorBoundsUnit;
        //            }
        //            else if (featureMatchingResult.FeatureMatchingResultType == FeatureMatchingResultType.LessThanMinimumScore)
        //            {
        //                MatchedFeature feature = featureMatchingResult.MatchedFeature;
        //                ret.MatchedFeature = feature;
        //                ret.MatchedAddress = feature.MatchedAddress;
        //                ret.MatchedFeatureAddress = feature.MatchedFeatureAddress;

        //                ret.MatchedFeature.PrimaryIdField = FeatureSource.IdFieldNamePrimary;
        //                ret.MatchedFeature.SecondaryIdField = FeatureSource.IdFieldNameSecondary;
        //                ret.MatchedFeature.PrimaryIdValue = feature.MatchedReferenceFeature.PrimaryId;
        //                ret.MatchedFeature.SecondaryIdValue = feature.MatchedReferenceFeature.SecondaryId;

        //                ret.InterpolationType = InterpolationType.NotAttempted;
        //                ret.InterpolationSubType = InterpolationSubType.NotAttempted;
        //                ret.GeocodeQualityType = GeocodeQualityType.Unmatchable;
        //                ret.MethodError = "Interpolation not attempted. No matching feature available for interpolation.";
        //                ret.SourceError = featureMatchingResult.Error;
        //            }

        //            else
        //            {

        //                ret.InterpolationType = InterpolationType.NotAttempted;
        //                ret.InterpolationSubType = InterpolationSubType.NotAttempted;
        //                ret.GeocodeQualityType = GeocodeQualityType.Unmatchable;
        //                ret.MethodError = "Interpolation not attempted. No matching feature available for interpolation.";
        //                ret.SourceError = featureMatchingResult.Error;

        //                switch (featureMatchingResult.FeatureMatchingResultType)
        //                {
        //                    case FeatureMatchingResultType.InvalidFeature:
        //                    case FeatureMatchingResultType.NullFeature:
        //                        ret.QueryStatusCodes = QueryStatusCodes.InternalError;
        //                        break;
        //                    case FeatureMatchingResultType.Unmatchable:
        //                        ret.QueryStatusCodes = QueryStatusCodes.Success;
        //                        break;
        //                    case FeatureMatchingResultType.Unknown:
        //                        ret.QueryStatusCodes = QueryStatusCodes.QueryParameterMissing;
        //                        break;
        //                    case FeatureMatchingResultType.ExceptionOccurred:
        //                        ret.SourceError = "Exception occurred querying source: " + featureMatchingResult.Error;
        //                        ret.QueryStatusCodes = QueryStatusCodes.InternalError;
        //                        ret.Exception = featureMatchingResult.Exception;
        //                        ret.ExceptionOccurred = true;
        //                        ret.ErrorMessage = featureMatchingResult.Error;
        //                        break;
        //                    default:
        //                        throw new Exception("Unexpected or unimplmented FeatureMatchingResultType: " + featureMatchingResult.FeatureMatchingResultType);
        //                        break;
        //                }

        //            }

        //            ret.NAACCRCensusTractCertaintyType = NAACCRCensusTractCertainty.GetNAACCRCensusTractCertaintyTypeForGeocode(ret, parameterSet);
        //            ret.NAACCRCensusTractCertaintyCode = NAACCRCensusTractCertainty.GetNAACCRCensusTractCertaintyCode(ret.NAACCRCensusTractCertaintyType);
        //            ret.NAACCRCensusTractCertaintyName = NAACCRCensusTractCertainty.GetNAACCRCensusTractCertaintyName(ret.NAACCRCensusTractCertaintyType);

        //            ret.NAACCRGISCoordinateQualityType = NAACCRGISCoordinateQuality.GetNAACCRGISCoordinateQualityTypeForGeocode(ret);
        //            ret.NAACCRGISCoordinateQualityCode = NAACCRGISCoordinateQuality.GetNAACCRGISCoordinateQualityCode(ret.NAACCRGISCoordinateQualityType);
        //            ret.NAACCRGISCoordinateQualityName = NAACCRGISCoordinateQuality.GetNAACCRGISCoordinateQualityName(ret.NAACCRGISCoordinateQualityType);

        //            if (ret.GeocodedError != null)
        //            {
        //                ret.RegionSize = ret.GeocodedError.ErrorBounds.ToString();
        //                ret.RegionSizeUnits = ret.GeocodedError.ErrorBoundsUnit.ToString();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ret.SourceError = "Exception occured: " + e.Message;
        //        ret.Statistics.MatchedFeatureStatistics.StreetStatistics.Error = "Exception occured: " + e.Message;
        //        ret.GeocodedError.GeoError = "Exception occured: " + e.Message;
        //        ret.GeocodeQualityType = GeocodeQualityType.Unknown;
        //        ret.QueryStatusCodes = QueryStatusCodes.InternalError;
        //        ret.Exception = e;
        //        ret.ExceptionOccurred = true;
        //        ret.Valid = false;
        //        ret.ErrorMessage = e.Message;
        //    }

        //    return ret;
        //}

        //public virtual List<IGeocode> Geocode(ParameterSet parameterSet)
        //{

        //    StreetAddress streetAddress = parameterSet.StreetAddress;
        //    Geocode ret = new Geocode(parameterSet.GeocoderConfiguration.Version);
        //    try
        //    {
        //        ret.InputAddress = streetAddress;
        //        ret.Geometry = new Point();
        //        ret.MethodType = Name;
        //        ret.SourceType = FeatureSource.Name;
        //        ret.SourceVintage = FeatureSource.Vintage;
        //        ret.ParsedAddress = parameterSet.StreetAddress;
        //        ret.Attempted = true;


        //        FeatureMatchingResult featureMatchingResult = FeatureSource.DoFeatureMatching(parameterSet);

        //        ret.FM_Result = featureMatchingResult;
        //        ret.FM_ResultType = featureMatchingResult.FeatureMatchingResultType;
        //        ret.FM_Notes = featureMatchingResult.FeatureMatchingNotes;
        //        ret.FM_TieNotes = featureMatchingResult.FeatureMatchingTieNotes;
        //        ret.FM_TieStrategy = featureMatchingResult.TieHandlingStrategyType;
        //        ret.FM_ResultCount = featureMatchingResult.FeatureMatchingResultCount;
        //        ret.FM_GeographyType = featureMatchingResult.FeatureMatchingGeographyType;

        //        ret.Statistics.MatchedLocationTypeStatistics.setMatchedLocationType((int)(featureMatchingResult.MatchedLocationTypes));
        //        ret.Statistics.ReferenceDatasetsStatistics.AddReferenceDatasetStatistics(featureMatchingResult.ReferenceDatasetStatistics);

        //        if (featureMatchingResult.FeatureMatchingResultType == FeatureMatchingResultType.Success || featureMatchingResult.FeatureMatchingResultType == FeatureMatchingResultType.BrokenTie)
        //        {
        //            MatchedFeature feature = featureMatchingResult.MatchedFeatures[0];

        //            ret.InterpolationType = InterpolationType;
        //            ret.InterpolationSubType = InterpolationSubType;

        //            ret.MatchedFeature = feature;
        //            ret.MatchedAddress = feature.MatchedAddress;
        //            ret.MatchedFeatureAddress = feature.MatchedFeatureAddress;

        //            ret.MatchedFeature.PrimaryIdField = FeatureSource.IdFieldNamePrimary;
        //            ret.MatchedFeature.SecondaryIdField = FeatureSource.IdFieldNameSecondary;
        //            ret.MatchedFeature.PrimaryIdValue = feature.MatchedReferenceFeature.PrimaryId;
        //            ret.MatchedFeature.SecondaryIdValue = feature.MatchedReferenceFeature.SecondaryId;

        //            FeatureInterpolationResult featureInterpolationResult = DoFeatureInterpolation(parameterSet, feature);

        //            if (featureInterpolationResult.FeatureInterpolationResultType == FeatureInterpolationResultType.Success)
        //            {
        //                ret.Valid = true;
        //                ret.Geometry = featureInterpolationResult.Geometry;

        //                if (featureInterpolationResult.GeocodedError.ErrorBounds > 0)
        //                {
        //                    ret.GeocodedError = featureInterpolationResult.GeocodedError;
        //                    ret.MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.Area = ret.GeocodedError.ErrorBounds;
        //                    ret.MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.AreaUnits = ret.GeocodedError.ErrorBoundsUnit;
        //                }
        //                else if (feature.GeocodedError.ErrorBounds > 0)
        //                {
        //                    ret.GeocodedError = feature.GeocodedError;
        //                }


        //                ret.QueryStatusCodes = QueryStatusCodes.Success;
        //                ret.GeocodeQualityType = FeatureSource.geocodeQualityType;
        //            }
        //            else
        //            {
        //                ret.MethodError = featureInterpolationResult.Error;
        //                ret.QueryStatusCodes = QueryStatusCodes.InternalError;
        //                ret.Exception = featureInterpolationResult.Exception;
        //                ret.ErrorMessage = featureInterpolationResult.Error;
        //                ret.ExceptionOccurred = true;
        //            }

        //        }
        //        else if (featureMatchingResult.FeatureMatchingResultType == FeatureMatchingResultType.Nearby)
        //        {
        //            MatchedFeature feature = featureMatchingResult.MatchedFeature;

        //            ret.InterpolationType = InterpolationType;
        //            ret.InterpolationSubType = InterpolationSubType;

        //            ret.MatchedFeature = feature;
        //            ret.MatchedAddress = feature.MatchedAddress;
        //            ret.MatchedFeatureAddress = feature.MatchedFeatureAddress;

        //            ret.MatchedFeature.PrimaryIdField = FeatureSource.IdFieldNamePrimary;
        //            ret.MatchedFeature.SecondaryIdField = FeatureSource.IdFieldNameSecondary;
        //            ret.MatchedFeature.PrimaryIdValue = feature.MatchedReferenceFeature.PrimaryId;
        //            ret.MatchedFeature.SecondaryIdValue = feature.MatchedReferenceFeature.SecondaryId;

        //            FeatureInterpolationResult featureInterpolationResult = DoFeatureInterpolation(parameterSet, feature);

        //            if (featureInterpolationResult.FeatureInterpolationResultType == FeatureInterpolationResultType.Success)
        //            {

        //                ret.Valid = true;
        //                ret.Geometry = featureInterpolationResult.Geometry;
        //                ret.GeocodedError.ErrorBounds = featureInterpolationResult.GeocodedError.ErrorBounds;
        //                ret.GeocodedError.ErrorBoundsUnit = featureInterpolationResult.GeocodedError.ErrorBoundsUnit;
        //                ret.GeocodedError.ErrorCalculationType = featureInterpolationResult.GeocodedError.ErrorCalculationType;

        //                ret.MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.Area = ret.GeocodedError.ErrorBounds;
        //                ret.MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.AreaUnits = ret.GeocodedError.ErrorBoundsUnit;

        //                ret.QueryStatusCodes = QueryStatusCodes.Success;
        //                ret.GeocodeQualityType = FeatureSource.geocodeQualityType;
        //            }
        //            else
        //            {
        //                ret.MethodError = featureInterpolationResult.Error;
        //                ret.QueryStatusCodes = QueryStatusCodes.InternalError;
        //                ret.Exception = featureInterpolationResult.Exception;
        //                ret.ErrorMessage = featureInterpolationResult.Error;
        //                ret.ExceptionOccurred = true;
        //            }

        //        }
        //        else if (featureMatchingResult.FeatureMatchingResultType == FeatureMatchingResultType.Ambiguous)
        //        {
        //            MatchedFeature feature = featureMatchingResult.MatchedFeature;

        //            ret.Valid = false;
        //            ret.GeocodeQualityType = GeocodeQualityType.Unmatchable;
        //            ret.QueryStatusCodes = QueryStatusCodes.Success;
        //            ret.InterpolationType = InterpolationType.NotAttempted;
        //            ret.InterpolationSubType = InterpolationSubType.NotAttempted;


        //            //ret.MatchedFeature = compositeFeature;
        //            //ret.MatchedAddress = compositeFeature.MatchedAddress;
        //            //ret.MatchedFeatureAddress = compositeFeature.MatchedFeatureAddress;

        //            ret.MatchedFeature = feature;
        //            ret.MatchedFeature.PrimaryIdField = FeatureSource.IdFieldNamePrimary;
        //            ret.MatchedFeature.SecondaryIdField = FeatureSource.IdFieldNameSecondary;
        //            //ret.Matchedoutput.PrimaryIdValue = compositeFeature.MatchedReferenceFeature.Geometry.PrimaryId;
        //            //ret.Matchedoutput.SecondaryIdValue = compositeFeature.MatchedReferenceFeature.Geometry.SecondaryId;

        //        }
        //        else if (featureMatchingResult.FeatureMatchingResultType == FeatureMatchingResultType.Composite)
        //        {
        //            CompositeMatchedFeature compositeFeature = (CompositeMatchedFeature)featureMatchingResult.MatchedFeature;
        //            ret.Valid = true;

        //            ret.MatchedFeature = compositeFeature;
        //            ret.MatchedAddress = compositeFeature.MatchedAddress;
        //            ret.MatchedFeatureAddress = compositeFeature.MatchedFeatureAddress;

        //            ret.MatchedFeature.PrimaryIdField = FeatureSource.IdFieldNamePrimary;
        //            ret.MatchedFeature.SecondaryIdField = FeatureSource.IdFieldNameSecondary;
        //            ret.MatchedFeature.PrimaryIdValue = compositeFeature.MatchedReferenceFeature.PrimaryId;
        //            ret.MatchedFeature.SecondaryIdValue = compositeFeature.MatchedReferenceFeature.SecondaryId;

        //            ret.InterpolationType = InterpolationType.ArealInterpolation;
        //            ret.InterpolationSubType = InterpolationSubType.ArealInterpolationConvexHullCentroid;

        //            SqlGeometry centroid = compositeFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeometry.STCentroid();
        //            //if (String.Compare(compositeFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeometry.STGeometryType().Value, "linestring", true) == 0)
        //            //{
        //            //    double length = compositeFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeometry.STLength().Value;
        //            //    centroid = SQLSpatialTools.SQLSpatialToolsFunctions.LocateAlongGeom(compositeFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeometry, length/2.0);
        //            //}
        //            //else
        //            //{
        //            //    centroid = compositeFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.SqlGeometry.STCentroid();
        //            //}

        //            ret.Geometry = new Point(centroid.STX.Value, centroid.STY.Value);
        //            ret.GeocodedError = compositeFeature.GeocodedError;
        //            ret.QueryStatusCodes = QueryStatusCodes.Success;
        //            ret.GeocodeQualityType = GeocodeQualityType.DynamicFeatureCompositionCentroid;

        //            ret.MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.Area = ret.GeocodedError.ErrorBounds;
        //            ret.MatchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.AreaUnits = ret.GeocodedError.ErrorBoundsUnit;
        //        }
        //        else if (featureMatchingResult.FeatureMatchingResultType == FeatureMatchingResultType.LessThanMinimumScore)
        //        {
        //            MatchedFeature feature = featureMatchingResult.MatchedFeature;
        //            ret.MatchedFeature = feature;
        //            ret.MatchedAddress = feature.MatchedAddress;
        //            ret.MatchedFeatureAddress = feature.MatchedFeatureAddress;

        //            ret.MatchedFeature.PrimaryIdField = FeatureSource.IdFieldNamePrimary;
        //            ret.MatchedFeature.SecondaryIdField = FeatureSource.IdFieldNameSecondary;
        //            ret.MatchedFeature.PrimaryIdValue = feature.MatchedReferenceFeature.PrimaryId;
        //            ret.MatchedFeature.SecondaryIdValue = feature.MatchedReferenceFeature.SecondaryId;

        //            ret.InterpolationType = InterpolationType.NotAttempted;
        //            ret.InterpolationSubType = InterpolationSubType.NotAttempted;
        //            ret.GeocodeQualityType = GeocodeQualityType.Unmatchable;
        //            ret.MethodError = "Interpolation not attempted. No matching feature available for interpolation.";
        //            ret.SourceError = featureMatchingResult.Error;
        //        }

        //        else
        //        {

        //            ret.InterpolationType = InterpolationType.NotAttempted;
        //            ret.InterpolationSubType = InterpolationSubType.NotAttempted;
        //            ret.GeocodeQualityType = GeocodeQualityType.Unmatchable;
        //            ret.MethodError = "Interpolation not attempted. No matching feature available for interpolation.";
        //            ret.SourceError = featureMatchingResult.Error;

        //            switch (featureMatchingResult.FeatureMatchingResultType)
        //            {
        //                case FeatureMatchingResultType.InvalidFeature:
        //                case FeatureMatchingResultType.NullFeature:
        //                    ret.QueryStatusCodes = QueryStatusCodes.InternalError;
        //                    break;
        //                case FeatureMatchingResultType.Unmatchable:
        //                    ret.QueryStatusCodes = QueryStatusCodes.Success;
        //                    break;
        //                case FeatureMatchingResultType.Unknown:
        //                    ret.QueryStatusCodes = QueryStatusCodes.QueryParameterMissing;
        //                    break;
        //                case FeatureMatchingResultType.ExceptionOccurred:
        //                    ret.SourceError = "Exception occurred querying source: " + featureMatchingResult.Error;
        //                    ret.QueryStatusCodes = QueryStatusCodes.InternalError;
        //                    ret.Exception = featureMatchingResult.Exception;
        //                    ret.ExceptionOccurred = true;
        //                    ret.ErrorMessage = featureMatchingResult.Error;
        //                    break;
        //                default:
        //                    throw new Exception("Unexpected or unimplmented FeatureMatchingResultType: " + featureMatchingResult.FeatureMatchingResultType);
        //                    break;
        //            }

        //        }

        //        ret.NAACCRCensusTractCertaintyType = NAACCRCensusTractCertainty.GetNAACCRCensusTractCertaintyTypeForGeocode(ret, parameterSet);
        //        ret.NAACCRCensusTractCertaintyCode = NAACCRCensusTractCertainty.GetNAACCRCensusTractCertaintyCode(ret.NAACCRCensusTractCertaintyType);
        //        ret.NAACCRCensusTractCertaintyName = NAACCRCensusTractCertainty.GetNAACCRCensusTractCertaintyName(ret.NAACCRCensusTractCertaintyType);

        //        ret.NAACCRGISCoordinateQualityType = NAACCRGISCoordinateQuality.GetNAACCRGISCoordinateQualityTypeForGeocode(ret);
        //        ret.NAACCRGISCoordinateQualityCode = NAACCRGISCoordinateQuality.GetNAACCRGISCoordinateQualityCode(ret.NAACCRGISCoordinateQualityType);
        //        ret.NAACCRGISCoordinateQualityName = NAACCRGISCoordinateQuality.GetNAACCRGISCoordinateQualityName(ret.NAACCRGISCoordinateQualityType);

        //        if (ret.GeocodedError != null)
        //        {
        //            ret.RegionSize = ret.GeocodedError.ErrorBounds.ToString();
        //            ret.RegionSizeUnits = ret.GeocodedError.ErrorBoundsUnit.ToString();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ret.SourceError = "Exception occured: " + e.Message;
        //        ret.Statistics.MatchedFeatureStatistics.StreetStatistics.Error = "Exception occured: " + e.Message;
        //        ret.GeocodedError.GeoError = "Exception occured: " + e.Message;
        //        ret.GeocodeQualityType = GeocodeQualityType.Unknown;
        //        ret.QueryStatusCodes = QueryStatusCodes.InternalError;
        //        ret.Exception = e;
        //        ret.ExceptionOccurred = true;
        //        ret.Valid = false;
        //        ret.ErrorMessage = e.Message;
        //    }

        //    return ret;
        //}
    }
}

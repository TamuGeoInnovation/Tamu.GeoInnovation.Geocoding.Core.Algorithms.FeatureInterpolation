using System;
using USC.GISResearchLab.Common.Addresses;
using USC.GISResearchLab.Common.Core.Geocoders.FeatureMatching;
using USC.GISResearchLab.Common.Core.Geocoders.ReferenceDatasets.Sources.Interfaces;
using USC.GISResearchLab.Common.Geocoders.InterpolationAlgorithms.ParameterProviders;
using USC.GISResearchLab.Common.GeographicFeatures.Streets;
using USC.GISResearchLab.Common.Geographics.Units;
using USC.GISResearchLab.Common.Geometries.Lines;
using USC.GISResearchLab.Common.Geometries.Points;
using USC.GISResearchLab.Geocoding.Algorithms.FeatureInterpolationMethods.Interfaces;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.AbstractClasses;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Interfaces;
using USC.GISResearchLab.Geocoding.Core.Metadata.FeatureInterpolationResults;
using USC.GISResearchLab.Geocoding.Core.Metadata.Qualities;
using USC.GISResearchLab.Geocoding.Core.OutputData;
using USC.GISResearchLab.Geocoding.Core.Queries.Parameters;
using USC.GISResearchLab.Core.WebServices.ResultCodes;
using System.Reflection;

namespace USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Implementations
{

    public class UniformLot : AbstractLinearFeatureInterpolationMethod, IUniformLotLinearInterpolationMethod
    {
        #region Properties
        private ILotNumberProvider _LotNumberProvider;
        private INumberOfLotsProvider _NumberOfLotsProvider;

        public ILotNumberProvider LotNumberProvider
        {
            get { return _LotNumberProvider; }
            set { _LotNumberProvider = value; }
        }

        private IFeatureSource _FeatureSource;
        public IFeatureSource FeatureSource
        {
            get { return _FeatureSource; }
            set { _FeatureSource = value; }
        }

        public INumberOfLotsProvider NumberOfLotsProvider
        {
            get { return _NumberOfLotsProvider; }
            set { _NumberOfLotsProvider = value; }
        }

        #endregion

        public UniformLot(IFeatureSource featureSource, INumberOfLotsProvider numberOfLotsProvider, ILotNumberProvider lotNumberProvider)
        {
            FeatureSource = featureSource;
            NumberOfLotsProvider = numberOfLotsProvider;
            LotNumberProvider = lotNumberProvider;
            Name = IneterpolationMethodNames.METHOD_NAME_UNIFORM_LOT_SIZE;
            Method = IneterpolationMethodNames.METHOD_UNIFORM_LOT_SIZE;
            Quality = (int)(GeocodeQualityType.UniformLotInterpolation);

            InterpolationType = InterpolationType.LinearInterpolation;
            InterpolationSubType = InterpolationSubType.LinearInterpolationUniformLot;

            DropbackValue = 10;
            DropbackUnits = LinearUnitTypes.Meters;
        }

        private Geocode geocodeGivenSegmentAndNumberOfParcelsAndParcelNumber(string polyline, string fromAddressLeft, string toAddressLeft, string fromAddressRight, string toAddressRight, double numberOfParcelsLeft, double numberOfParcelsRight, int parcelNumber, double roadWidth, string addressNumber, string preDirectional, string name, string suffix, string postDirectional, string suite, string suiteNumberStr, string city, string state, string zipStr)
        {
            return getGeocodeGivenSegmentAndNumberOfParcelsAndParcelNumber(polyline, fromAddressLeft, toAddressLeft, fromAddressRight, toAddressRight, numberOfParcelsLeft, numberOfParcelsRight, parcelNumber, roadWidth, addressNumber, preDirectional, name, suffix, postDirectional, suite, suiteNumberStr, city, state, zipStr);
        }

        private Geocode getGeocodeGivenSegmentAndNumberOfParcelsAndParcelNumber(string polyLinestring, string fromAddressLeft, string toAddressLeft, string fromAddressRight, string toAddressRight, double numberOfParcelsLeft, double numberOfParcelsRight, int parcelNumber, double roadWidth, string numberStr, string preDirectional, string name, string suffix, string postDirectional, string suite, string suiteNumberStr, string city, string state, string zipStr)
        {
            Geocode ret = new Geocode(2.94);
            try
            {
                ret.Geometry = new Point();
                ret.MethodType = Name;
                ret.SourceType = FeatureSource.Name;
                ret.SourceVintage = FeatureSource.Vintage;


                ValidateableStreetAddress address = new ValidateableStreetAddress(numberStr, preDirectional, name, suffix, postDirectional, suite, suiteNumberStr, city, state, zipStr);

                PolyLine polyLine = PolyLine.FromCoordinateString(polyLinestring);
                Point startingPoint = polyLine.StartingPoint;
                Point endingPoint = polyLine.EndingPoint;

                Street street = new Street();
                street.Start = startingPoint;
                street.End = endingPoint;
                street.FromAddressHouseNumberMajorLeftStr = fromAddressLeft;
                street.FromAddressHouseNumberMajorRightStr = fromAddressRight;
                street.ToAddressHouseNumberMajorLeftStr = toAddressLeft;
                street.ToAddressHouseNumberMajorRightStr = toAddressRight;

                AddressRange addressRange = street.GetAddressRangeForAddress(address);


                // determine 
                //  1) which side of the street the address is on
                //  2) what the from and to addresses are on that side of the street
                if (addressRange.IsLeft)
                {
                    street.NumberOfLots = numberOfParcelsLeft;
                }
                else
                {
                    street.NumberOfLots = numberOfParcelsRight;
                }

                Point interpolatedPoint = street.InterpolateUniform(address.NumberInt);
                double dropbackValue = 0;
                try
                {
                    dropbackValue = Math.Abs(UnitConverter.MetersToDD(roadWidth, interpolatedPoint.Y));

                    double[] droppedBackValues = street.calculateDroppedBack(interpolatedPoint, dropbackValue, addressRange.IsLeft);

                    ((Point)ret.Geometry).X = droppedBackValues[0];
                    ((Point)ret.Geometry).Y = droppedBackValues[1];
                    ret.Valid = true;
                    ret.GeocodedError.ErrorBounds = street.BoundingBox.Area;
                    ret.QueryStatusCodes = QueryStatusCodes.Success;
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, this.GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " errored out - reference source: " + Name);
                    ret.GeocodedError.GeoError = "Error Calculating Dropback - Using Street Centerline Point: " + ex.Message;
                    ((Point)ret.Geometry).Y = interpolatedPoint.Y;
                    ((Point)ret.Geometry).X = interpolatedPoint.X;
                    ret.Valid = true;
                    ret.GeocodedError.ErrorBounds = street.BoundingBox.Area;
                    ret.QueryStatusCodes = QueryStatusCodes.Success;
                }

                //ret.GeocodedError.ErrorBounds = interpolatedPoint.GeocodedError.ErrorBounds;
                ret.Statistics.InterpolationStatistics.ParcelsOnBlock = street.NumberOfLots;
                ret.Statistics.InterpolationStatistics.DropbackValue = dropbackValue;
            }
            catch (Exception e)
            {
                Serilog.Log.Error(e, this.GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " errored out - reference source: " + Name);
                ret.ExceptionOccurred = true;
                ret.Exception = e;
                ret.ErrorMessage = e.Message;

                ret.SourceError = "Exception: " + e.Message;
                ret.Statistics.MatchedFeatureStatistics.StreetStatistics.Error = "Unable to get assessor Id for address, no assessor polygon can be obtained - Exception: " + e.Message;
                ret.GeocodedError.GeoError = "No assessor polygon can be obtained";
                ret.GeocodeQualityType = GeocodeQualityType.Unknown;
                ret.QueryStatusCodes = QueryStatusCodes.InternalError;

                if (TraceSource != null)
                {
                    TraceSource.TraceEvent(System.Diagnostics.TraceEventType.Error, 0, ret.ErrorMessage);
                }
            }
            return ret;
        }

        public override FeatureInterpolationResult DoFeatureInterpolation(ParameterSet parameterSet, MatchedFeature matchedFeature)
        {
            Serilog.Log.Verbose(this.GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " - entered");
            FeatureInterpolationResult ret = new FeatureInterpolationResult();

            try
            {

                StreetAddress streetAddress = parameterSet.StreetAddress;
                ValidateableStreetAddress address = ValidateableStreetAddress.FromStreetAddress(streetAddress);

                Street street = (Street)matchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry;
                street.Source = FeatureSource.Name;

                AddressRange addressRange = street.GetAddressRangeForAddress(address);

                double numberOfLots = NumberOfLotsProvider.GetNumberOfLots(address, street);
                if (street.NumberOfLots > 0)
                {

                    double lotNumber = LotNumberProvider.GetLotNumber(address, addressRange.FromAddress, addressRange.ToAddress);
                    return base.DoFeatureInterpolation(parameterSet, matchedFeature, numberOfLots, lotNumber);

                }
                else
                {
                    throw new Exception("Zero addresses on this block - invalid for unform lot");
                }
            }
            catch (Exception e)
            {
                Serilog.Log.Error(e, this.GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " errored out - reference source: " + Name);
                ret.FeatureInterpolationResultType = FeatureInterpolationResultType.ExceptionOccurred;
                ret.Error = "Uniform lot - Error getting reference feature: " + e.Message;
                ret.ExceptionOccurred = true;
                ret.Exception = e;
                ret.GeocodedError.GeoError = "Exception occured: " + e.Message;
            }
            return ret;
        }
    }
}

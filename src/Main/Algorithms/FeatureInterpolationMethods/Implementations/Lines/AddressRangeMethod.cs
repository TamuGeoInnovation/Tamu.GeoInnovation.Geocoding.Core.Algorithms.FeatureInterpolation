using System;
using System.Reflection;
using USC.GISResearchLab.Common.Addresses;
using USC.GISResearchLab.Common.Core.Geocoders.FeatureMatching;
using USC.GISResearchLab.Common.Core.Geocoders.ReferenceDatasets.Sources.Interfaces;
using USC.GISResearchLab.Common.GeographicFeatures.Streets;
using USC.GISResearchLab.Common.Geographics.Units;
using USC.GISResearchLab.Common.Geometries.Lines;
using USC.GISResearchLab.Common.Geometries.Points;
using USC.GISResearchLab.Common.Utils.Strings;
using USC.GISResearchLab.Core.WebServices.ResultCodes;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.AbstractClasses;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Interfaces;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureMatchScorers;
using USC.GISResearchLab.Geocoding.Core.Metadata.FeatureInterpolationResults;
using USC.GISResearchLab.Geocoding.Core.Metadata.Qualities;
using USC.GISResearchLab.Geocoding.Core.OutputData;
using USC.GISResearchLab.Geocoding.Core.Queries.Parameters;

namespace USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Implementations
{

    public class AddressRangeMethod : AbstractLinearFeatureInterpolationMethod, ILinearInterpolationMethod
    {
        #region Properties

        #endregion

        public AddressRangeMethod(IFeatureSource featureSource)
        {
            //TODO Check to see if still works - removed DG 2015-06-09
            //FeatureSource = featureSource;
            Name = IneterpolationMethodNames.METHOD_NAME_ADDRESS_RANGE;
            Method = IneterpolationMethodNames.METHOD_ADDRESS_RANGE;
            Quality = (int)(GeocodeQualityType.AddressRangeInterpolation);

            InterpolationType = InterpolationType.LinearInterpolation;
            InterpolationSubType = InterpolationSubType.LinearInterpolationAddressRange;

            DropbackValue = 10;
            DropbackUnits = LinearUnitTypes.Meters;
        }

        public Geocode geocodeGivenSegment(string polylinestring, string fromAddressLeft, string toAddressLeft, string fromAddressRight, string toAddressRight, double roadWidth, string addressNumber)
        {
            Geocode ret = new Geocode(2.94);
            try
            {
                ret.Geometry = new Point();

                ValidateableStreetAddress address = new ValidateableStreetAddress();
                address.Number = addressNumber;

                PolyLine polyLine = PolyLine.FromCoordinateString(polylinestring);
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
                addressRange.generateAddresses(true);

                street.NumberOfLots = addressRange.getNumberOfLots();
                double lotNumber = addressRange.getLotNumber(address.NumberInt);

                Point interpolatedPoint = street.InterpolateUniform(lotNumber);



                try
                {

                    double dropbackValue = Math.Abs(UnitConverter.MetersToDD(roadWidth, interpolatedPoint.Y)); ;

                    double[] droppedBackValues = street.calculateDroppedBack(interpolatedPoint, dropbackValue, street.StreetSide == StreetSide.Left);

                    ((Point)ret.Geometry).X = droppedBackValues[0];
                    ((Point)ret.Geometry).Y = droppedBackValues[1];
                    ret.Valid = true;
                    ret.QueryStatusCodes = QueryStatusCodes.Success;
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, this.GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " errored out - reference source: " + Name);
                    ret.GeocodedError.GeoError = "Error Calculating Dropback - Using Street Centerline Point: " + ex.Message;
                    ((Point)ret.Geometry).Y = interpolatedPoint.Y;
                    ((Point)ret.Geometry).X = interpolatedPoint.X;
                    ret.Valid = true;
                    ret.QueryStatusCodes = QueryStatusCodes.InternalError;
                }

                ret.GeocodedError.ErrorBounds = street.BoundingBox.Area;


                //geoPoint.GeocodedError.ErrorBounds = interpolatedPoint.GeocodedError.ErrorBounds;
                ret.Statistics.InterpolationStatistics.ParcelsOnBlock = street.NumberOfLots;
                ret.GeocodeQualityType = GeocodeQualityType.AddressRangeInterpolation;
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
                Line line = null;
                double numberOfLots = 0;
                double addressNumber = 0;
                double lotNumber = 0;

                if (matchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature != null)
                {
                    line = (Line)matchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry;
                    line.Source = matchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry.Source;

                    if (matchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.GetType() == typeof(NickleStreet))
                    {
                        NickleStreet nickleStreetAddress = (NickleStreet)matchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature;
                        AddressRange addressRange = null;
                        switch (matchedFeature.MatchScoreResult.PreferredAddressRangeResultType)
                        {
                            case FeatureMatchAddressRangePreferredAddressRangeResultType.AddressRange:

                                if (matchedFeature.MatchScoreResult.AddressNumberTypeUsed == AddressNumberType.Number)
                                {
                                    addressRange = nickleStreetAddress.AddressRangeMajor;
                                    if (StringUtils.IsInt(parameterSet.StreetAddress.Number))
                                    {
                                        addressNumber = Convert.ToDouble(parameterSet.StreetAddress.Number);
                                    }
                                }
                                else if (matchedFeature.MatchScoreResult.AddressNumberTypeUsed == AddressNumberType.Fractional)
                                {
                                    addressRange = nickleStreetAddress.AddressRangeMinor;
                                    if (StringUtils.IsInt(parameterSet.StreetAddress.NumberFractional))
                                    {
                                        addressNumber = Convert.ToDouble(parameterSet.StreetAddress.NumberFractional);
                                    }
                                }
                                else
                                {
                                    throw new Exception("Unexpected or unimplemented AddressNumberType: " + matchedFeature.MatchScoreResult.AddressNumberTypeUsed);
                                }

                                break;

                            case FeatureMatchAddressRangePreferredAddressRangeResultType.HouseNumber:

                                if (matchedFeature.MatchScoreResult.AddressNumberTypeUsed == AddressNumberType.Number)
                                {
                                    addressRange = nickleStreetAddress.AddressRangeHouseNumberRangeMajor;
                                    if (StringUtils.IsInt(parameterSet.StreetAddress.Number))
                                    {
                                        addressNumber = Convert.ToDouble(parameterSet.StreetAddress.Number);
                                    }
                                }
                                else if (matchedFeature.MatchScoreResult.AddressNumberTypeUsed == AddressNumberType.Fractional)
                                {
                                    addressRange = nickleStreetAddress.AddressRangeHouseNumberRangeMinor;
                                    if (StringUtils.IsInt(parameterSet.StreetAddress.NumberFractional))
                                    {
                                        addressNumber = Convert.ToDouble(parameterSet.StreetAddress.NumberFractional);
                                    }
                                }
                                else
                                {
                                    throw new Exception("Unexpected or unimplemented AddressNumberType: " + matchedFeature.MatchScoreResult.AddressNumberTypeUsed);
                                }

                                break;
                            case FeatureMatchAddressRangePreferredAddressRangeResultType.Super:
                                //addressRange = nickleStreetAddress.AddressRangeSuper;

                                throw new Exception("FeatureMatchAddressRangePreferredAddressRangeResultType.Super should no longer be used");

                                break;
                            default:
                                throw new Exception("Unexpected or unimplemented PreferredAddressRangeResultType: " + matchedFeature.MatchScoreResult.PreferredAddressRangeResultType);
                        }

                        if (matchedFeature.MatchScoreResult.AddressRangeResultType == FeatureMatchAddressRangeResultType.WithinRange)
                        {
                            numberOfLots = 1.0 + Convert.ToDouble(addressRange.Size) / 2.0;

                            if (addressRange.StreetNumberRangeOrderType == StreetNumberRangeOrderType.LowHi)
                            {
                                double offsetFromStart = .5 + Convert.ToDouble(addressNumber) - addressRange.FromAddress;


                                double lotNumberProportion = offsetFromStart / addressRange.Size;

                                if (lotNumberProportion > 1)
                                {
                                    lotNumberProportion = .95;
                                }
                                else if (lotNumberProportion < 0)
                                {
                                    lotNumberProportion = .05;
                                }

                                lotNumber = lotNumberProportion * numberOfLots;

                                if (lotNumber > numberOfLots)
                                {
                                    lotNumber = numberOfLots;
                                }

                            }
                            else if (addressRange.StreetNumberRangeOrderType == StreetNumberRangeOrderType.HiLow)
                            {
                                double offsetFromEnd = Convert.ToDouble(addressNumber) - addressRange.ToAddress - .5;
                                double lotNumberProportion = offsetFromEnd / addressRange.Size;

                                if (lotNumberProportion > 1)
                                {
                                    lotNumberProportion = .95;
                                }
                                else if (lotNumberProportion < 0)
                                {
                                    lotNumberProportion = .05;
                                }

                                lotNumber = lotNumberProportion * numberOfLots;

                                if (lotNumber > numberOfLots)
                                {
                                    lotNumber = numberOfLots;
                                }
                            }
                            else if (addressRange.StreetNumberRangeOrderType == StreetNumberRangeOrderType.SingleNumber)
                            {
                                lotNumber = numberOfLots / 2.0;
                            }

                        }
                        else if (matchedFeature.MatchScoreResult.AddressRangeResultType == FeatureMatchAddressRangeResultType.OutsideRange)
                        {
                            numberOfLots = 1.0 + Convert.ToDouble(addressRange.Size) / 2.0;

                            if (matchedFeature.MatchScoreResult.PreferredEndResultType == FeatureMatchAddressRangePreferredEndResultType.LoEnd)
                            {
                                lotNumber = numberOfLots / 2.0;
                            }
                            else if (matchedFeature.MatchScoreResult.PreferredEndResultType == FeatureMatchAddressRangePreferredEndResultType.HiEnd)
                            {
                                lotNumber = numberOfLots / 2.0;
                            }
                            else
                            {
                                throw new Exception("Unexpected or unimplemented FeatureMatchAddressRangePreferredEndResultType: " + matchedFeature.MatchScoreResult.PreferredEndResultType);
                            }
                        }
                        else
                        {
                            throw new Exception("Unexpected or unimplemented FeatureMatchAddressRangeResultType: " + matchedFeature.MatchScoreResult.AddressRangeResultType);
                        }
                    }
                }
                else
                {
                    Street street = (Street)matchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.Geometry;
                    ValidateableStreetAddress address = ValidateableStreetAddress.FromStreetAddress(parameterSet.StreetAddress);
                    AddressRange addressRange = street.GetAddressRangeForAddress(address);
                    addressRange.generateAddresses(true);

                    numberOfLots = addressRange.getNumberOfLots();
                    lotNumber = addressRange.getLotNumber(address.NumberInt);
                    line = street;
                }

                if (line != null)
                {
                    return base.DoFeatureInterpolation(parameterSet, matchedFeature, numberOfLots, lotNumber);
                }
                else
                {
                    throw new Exception("Line geometry is null");
                }

            }
            catch (Exception e)
            {
                Serilog.Log.Error(e, this.GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " errored out - reference source: " + Name);
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

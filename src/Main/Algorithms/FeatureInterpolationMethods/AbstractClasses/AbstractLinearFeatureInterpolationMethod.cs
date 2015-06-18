using System;
using Microsoft.SqlServer.Types;
using SQLSpatialTools;
using USC.GISResearchLab.Common.Addresses;
using USC.GISResearchLab.Common.Core.Geocoders.FeatureMatching;
using USC.GISResearchLab.Common.GeographicFeatures.Streets;
using USC.GISResearchLab.Common.Geographics.Units;
using USC.GISResearchLab.Common.Geometries.Lines;
using USC.GISResearchLab.Common.Geometries.Points;
using USC.GISResearchLab.Geocoding.Core.Metadata.FeatureInterpolationResults;
using USC.GISResearchLab.Geocoding.Core.OutputData.Error;
using USC.GISResearchLab.Geocoding.Core.Queries.Parameters;

namespace USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.AbstractClasses
{
    public abstract class AbstractLinearFeatureInterpolationMethod : AbstractFeatureInterpolationMethod
    {
        #region Properties

        public LinearUnitTypes DropbackUnits { get; set; }
        public double DropbackValue { get; set; }

        #endregion

        public virtual FeatureInterpolationResult DoFeatureInterpolation(ParameterSet parameterSet, MatchedFeature matchedFeature, double numberOfLots, double lotNumber)
        {
            FeatureInterpolationResult ret = new FeatureInterpolationResult();

            try
            {


                //Street street = (Street)matchedFeature.MatchedReferenceFeature.Geometry;

                if (matchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.GetType() != typeof(NickleStreet))
                {
                    throw new Exception("Linear interpolation requies a NickleStreet type: " + matchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature.GetType());
                }
                else
                {
                    NickleStreet nickleStreetAddress = (NickleStreet)matchedFeature.MatchedReferenceFeature.StreetAddressableGeographicFeature;

                    Point interpolatedPoint = ((Line)nickleStreetAddress.Geometry).Interpolate(lotNumber, numberOfLots);

                    double dropbackValueMetersParameter = parameterSet.DropbackValue;
                    double dropbackValueMetersMethodDefault = DropbackValue;
                    double dropbackValueDD = 0.0;
                    double[] droppedBackValues = null;

                    // first use the parameter value of the dropback
                    if (dropbackValueMetersParameter != 0)
                    {
                        if (parameterSet.DropbackUnits == LinearUnitTypes.Meters)
                        {
                            //dropbackValueDD = Math.Abs(UnitConverter.MetersToDD(dropbackValueMetersParameter, interpolatedPoint.Y));
                            double[] lengthAtLatitude = UnitConverter.MetersPerDDAlternative(interpolatedPoint.Y);
                            dropbackValueDD = dropbackValueMetersParameter / lengthAtLatitude[0];
                        }
                        else
                        {
                            throw new Exception("Not yet implemented dropback linear unit type: " + parameterSet.DropbackUnits + " - need to add in conversion to meters");
                        }
                    }
                    // then use the method default dropback
                    else if (dropbackValueMetersMethodDefault != 0)
                    {
                        if (DropbackUnits == LinearUnitTypes.Meters)
                        {
                            //dropbackValueDD = Math.Abs(UnitConverter.MetersToDD(dropbackValueMetersParameter, interpolatedPoint.Y));
                            double[] lengthAtLatitude = UnitConverter.MetersPerDDAlternative(interpolatedPoint.Y);
                            dropbackValueDD = dropbackValueMetersMethodDefault / lengthAtLatitude[0];
                        }
                        else
                        {
                            throw new Exception("Not yet implemented dropback linear unit type: " + DropbackUnits + " - need to add in conversion to meters");
                        }
                    }
                    // then use width of the street
                    else
                    {
                        dropbackValueDD = Math.Abs(UnitConverter.MetersToDD(nickleStreetAddress.NumberOfLanes, interpolatedPoint.Y));
                    }

                    if (dropbackValueDD > 0)
                    {
                        try
                        {
                            droppedBackValues = ((Line)nickleStreetAddress.Geometry).calculateDroppedBack(interpolatedPoint, dropbackValueDD, nickleStreetAddress.StreetSide == StreetSide.Left);
                            ret.Geometry = new Point(droppedBackValues[0], droppedBackValues[1]);
                        }
                        catch (Exception ex) // if there is an error calculating the drop back use the street center point
                        {
                            ret.Geometry = interpolatedPoint;
                            ret.FeatureInterpolationResultType = FeatureInterpolationResultType.Success;
                            //ret.Error = "Error performing interpolation: " + "Dropback error: Dropback calucation exception - using default interpolated point.";
                        }

                        //SqlGeography multiPoint = Geometry.BuildSqlGeographyMultiPoint(new Point[] { ((Line)nickleStreetAddress.Geometry).Start, ((Line)nickleStreetAddress.Geometry).End, (Point)ret.Geometry }, 4269);
                        SqlGeography buffered = nickleStreetAddress.Geometry.SqlGeography.STBuffer(10);
                        SqlGeography convexHull = SQLSpatialToolsFunctions.ConvexHullGeography(buffered);
                        double convexHullArea = convexHull.STArea().Value;


                        ret.GeocodedError.ErrorBounds = convexHullArea;
                        ret.GeocodedError.ErrorBoundsUnit = LinearUnitTypes.Meters;
                        ret.GeocodedError.ErrorCalculationType = GeocodedErrorCalculationType.featureConvexHullArea;

                        //BoundingBox streetWithDropBackBoundingBox = street.GetBoundingBoxFromBuffer(dropbackValueDD);
                        //ret.GeocodedError.ErrorBounds = UnitConverter.ConvertToNonLinearBoundingBoxToLinearArea(streetWithDropBackBoundingBox, LinearUnitTypes.Meters);
                        //ret.GeocodedError.ErrorBoundsUnit = LinearUnitTypes.Meters;
                        //ret.GeocodedError.ErrorCalculationType = GeocodedErrorCalculationType.featureBoundingBoxArea;

                        ret.FeatureInterpolationResultType = FeatureInterpolationResultType.Success;
                    }
                    else
                    {
                        ret.Geometry = interpolatedPoint;
                        ret.FeatureInterpolationResultType = FeatureInterpolationResultType.Success;
                        //ret.Error = "Error performing interpolation: " + "Dropback error: Dropback distance is 0.0 - using default interpolated point.";
                    }
                }

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
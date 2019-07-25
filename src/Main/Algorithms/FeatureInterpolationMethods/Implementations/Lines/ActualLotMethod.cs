using System;
using System.Reflection;
using Microsoft.SqlServer.Types;
using SQLSpatialTools;
using USC.GISResearchLab.Common.Addresses;
using USC.GISResearchLab.Common.Core.Geocoders.FeatureMatching;
using USC.GISResearchLab.Common.Core.Geocoders.ReferenceDatasets.Sources.Interfaces;
using USC.GISResearchLab.Common.Geocoders.InterpolationAlgorithms.ParameterProviders;
using USC.GISResearchLab.Common.GeographicFeatures.Streets;
using USC.GISResearchLab.Common.Geographics.Units;
using USC.GISResearchLab.Common.Geometries.Points;
using USC.GISResearchLab.Geocoding.Algorithms.FeatureInterpolationMethods.Interfaces;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.AbstractClasses;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Implementations.Blocks;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Interfaces;
using USC.GISResearchLab.Geocoding.Core.Metadata.FeatureInterpolationResults;
using USC.GISResearchLab.Geocoding.Core.Metadata.Qualities;
using USC.GISResearchLab.Geocoding.Core.OutputData.Error;
using USC.GISResearchLab.Geocoding.Core.Queries.Parameters;

namespace USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Implementations
{

    public class ActualLotMethod : AbstractLinearFeatureInterpolationMethod, ILinearInterpolationMethod
    {
        #region Properties

        private BlockSource _BlockSource;
        private INumberOfLotsProvider _NumberOfLotsProvider;
        private ILotNumberProvider _ILotNumberProvider;
        private IBlockProvider _BlockProvider;
        private IAddressRangeProvider _AddressRangeProvider;
        public IAddressRangeProvider AddressRangeProvider
        {
            get { return _AddressRangeProvider; }
            set { _AddressRangeProvider = value; }
        }

       
        public IBlockProvider BlockProvider
        {
            get { return _BlockProvider; }
            set { _BlockProvider = value; }
        }

        public ILotNumberProvider LotNumberProvider
        {
            get { return _ILotNumberProvider; }
            set { _ILotNumberProvider = value; }
        }
        
        public INumberOfLotsProvider NumberOfLotsProvider
        {
            get { return _NumberOfLotsProvider; }
            set { _NumberOfLotsProvider = value; }
        }
        public BlockSource BlockSource
        {
            get { return _BlockSource; }
            set { _BlockSource = value; }
        }
        

        #endregion

        public ActualLotMethod(IFeatureSource featureSource, INumberOfLotsProvider numberOfLotsProvider, ILotNumberProvider lotNumberProvider, IBlockProvider blockProvider, IAddressRangeProvider addressRangeProvider)
        {
            //TODO Check to see if still works - removed DG 2015-06-09
            //FeatureSource = featureSource;
            NumberOfLotsProvider = numberOfLotsProvider;
            LotNumberProvider = lotNumberProvider;
            BlockProvider = blockProvider;
            AddressRangeProvider = addressRangeProvider;
            Name = IneterpolationMethodNames.METHOD_NAME_ACTUAL_LOT_SIZE;
            Method = IneterpolationMethodNames.METHOD_ACTUAL_LOT_SIZE;
            Quality = (int)(GeocodeQualityType.ActualLotInterpolation);

            InterpolationType = InterpolationType.LinearInterpolation;
            InterpolationSubType = InterpolationSubType.LinearInterpolationActualLot;

            DropbackValue = 10;
            DropbackUnits = LinearUnitTypes.Meters;
        }

        public override FeatureInterpolationResult DoFeatureInterpolation(ParameterSet parameterSet, MatchedFeature matchedFeature)
        {
            Serilog.Log.Verbose(this.GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " - entered");
            FeatureInterpolationResult ret = new FeatureInterpolationResult();

            try
            {
                ParcelCombination bestCombo = BlockProvider.ComputeBestCombination();

                if (bestCombo != null)
                {
                    double numberOfLots = bestCombo.NumberOfLots;
                    Street bestStreet = bestCombo.Street;
                    if (bestStreet != null && bestStreet.Valid)
                    {

                        Point interpolatedPoint = bestStreet.InterpolateActual(parameterSet.StreetAddress.Number);

                        double dropbackValueMetersParameter = parameterSet.DropbackValue;
                        double dropbackValueMetersMethodDefault = DropbackValue;
                        double dropbackValueDD = 0.0;
                        double[] droppedBackValues = null;



                        // first use the parameter value of the dropback
                        if (dropbackValueMetersParameter != 0)
                        {
                            if (parameterSet.DropbackUnits == LinearUnitTypes.Meters)
                            {
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
                            dropbackValueDD = Math.Abs(UnitConverter.MetersToDD(bestStreet.NumberOfLanes, interpolatedPoint.Y));
                        }

                        if (dropbackValueDD > 0)
                        {
                            try
                            {
                                droppedBackValues = bestStreet.calculateDroppedBack(interpolatedPoint, dropbackValueDD, bestStreet.StreetSide == StreetSide.Left);
                                ret.Geometry = new Point(droppedBackValues[0], droppedBackValues[1]);
                                ret.FeatureInterpolationResultType = FeatureInterpolationResultType.Success;
                            }
                            catch (Exception ex) // if there is an error calculating the drop back use the street center point
                            {
                                Serilog.Log.Error(e, this.GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " errored out - reference source: " + Name);
                                ret.Geometry = interpolatedPoint;
                                ret.FeatureInterpolationResultType = FeatureInterpolationResultType.Success;
                                //ret.Error = "Error performing interpolation: " + "Dropback error: Dropback calucation exception - using default interpolated point.";
                            }

                            SqlGeography buffered = bestStreet.SqlGeography.STBuffer(10);
                            SqlGeography convexHull = SQLSpatialToolsFunctions.ConvexHullGeography(buffered);
                            double convexHullArea = convexHull.STArea().Value;

                            ret.GeocodedError.ErrorBounds = convexHullArea;
                            ret.GeocodedError.ErrorBoundsUnit = LinearUnitTypes.Meters;
                            ret.GeocodedError.ErrorCalculationType = GeocodedErrorCalculationType.featureConvexHullArea;

                        }
                        else
                        {
                            throw new Exception("Dropback error: Dropback distance is 0.0");
                        }
                    }
                    else
                    {
                        throw new Exception("Best street is null");
                    }
                }
                else
                {
                    throw new Exception("Unable to build block");

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
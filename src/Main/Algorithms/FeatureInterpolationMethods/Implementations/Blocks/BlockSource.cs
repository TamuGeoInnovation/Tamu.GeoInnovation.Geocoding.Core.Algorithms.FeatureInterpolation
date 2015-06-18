using System;
using USC.GISResearchLab.Common.Addresses;
using USC.GISResearchLab.Common.Core.Geocoders.ReferenceDatasets.Sources.Interfaces;
using USC.GISResearchLab.Common.Geocoders.InterpolationAlgorithms.ParameterProviders;
using USC.GISResearchLab.Common.Geocoders.ReferenceDatasets.Sources.Interfaces.Intersections;
using USC.GISResearchLab.Common.GeographicFeatures.Parcels;
using USC.GISResearchLab.Common.GeographicFeatures.Streets;
using USC.GISResearchLab.Common.Geometries.Bearings;
using USC.GISResearchLab.Common.Geometries.Directions;
using USC.GISResearchLab.Common.Geometries.Lines;
using USC.GISResearchLab.Common.Geometries.Points;
using USC.GISResearchLab.Geocoding.Algorithms.FeatureInterpolationMethods.Interfaces;

namespace USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureInterpolationMethods.Implementations.Blocks
{
    public class BlockSource : IBlockProvider
    {
        #region Properties
        
        private IAddressRangeProvider _AddressRangeProvider;
        public IAddressRangeProvider AddressRangeProvider
        {
            get { return _AddressRangeProvider; }
            set { _AddressRangeProvider = value; }
        }

        private IBlockProvider _BlockProvider;
        public IBlockProvider BlockProvider
        {
            get { return _BlockProvider; }
            set { _BlockProvider = value; }
        }

        private ILotNumberProvider _LotNumberProvider;
        public ILotNumberProvider LotNumberProvider
        {
            get { return _LotNumberProvider; }
            set { _LotNumberProvider = value; }
        }

        private ILineIntersectionSource _LineIntersectionSource;
        public ILineIntersectionSource LineIntersectionSource
        {
            get { return _LineIntersectionSource; }
            set { _LineIntersectionSource = value; }
        }

        private INumberOfLotsProvider _NumberOfLotsProvider;
        public INumberOfLotsProvider NumberOfLotsProvider
        {
            get { return _NumberOfLotsProvider; }
            set { _NumberOfLotsProvider = value; }
        }

        private IFeatureSource _FeatureSource;
        public IFeatureSource FeatureSource
        {
            get { return _FeatureSource; }
            set { _FeatureSource = value; }
        }


        #endregion

        public BlockSource(ILineIntersectionSource lineIntersectionSource, IBlockProvider blockProvider, IAddressRangeProvider addressRangeProvider)
        {
            LineIntersectionSource = lineIntersectionSource;
            BlockProvider = blockProvider;
            AddressRangeProvider = addressRangeProvider;
        }

        public ParcelCombination ComputeBestCombination()
        {
            return null;
        }

        public ParcelCombination ComputeBestCombination(string polyLinestring, string source, string sourceId, string fromAddressLeft, string toAddressLeft, string fromAddressRight, string toAddressRight, double roadWidth, string street1Info, string street1addressParcels, string street2Info, string street2addressParcels, string street3Info, string street3addressParcels, string street4Info, string street4addressParcels, ValidateableStreetAddress address)
        {

            ParcelCombination ret = null;

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

            Street street1 = Street.FromString(street1Info);
            Street street2 = Street.FromString(street2Info);
            Street street3 = Street.FromString(street3Info);
            Street street4 = Street.FromString(street4Info);

            street1.buildParcels(street1addressParcels);
            street2.buildParcels(street2addressParcels);
            street3.buildParcels(street3addressParcels);
            street4.buildParcels(street4addressParcels);

            ParcelCombination combo01 = CalculateError(1, street1, street2, street3, street4, 1, 1, 2, 3);
            ParcelCombination combo02 = CalculateError(2, street1, street2, street3, street4, 1, 2, 2, 3);
            ParcelCombination combo03 = CalculateError(3, street1, street2, street3, street4, 4, 1, 2, 3);
            ParcelCombination combo04 = CalculateError(4, street1, street2, street3, street4, 4, 2, 2, 3);

            ParcelCombination combo05 = CalculateError(5, street1, street2, street3, street4, 1, 1, 3, 3);
            ParcelCombination combo06 = CalculateError(6, street1, street2, street3, street4, 1, 2, 3, 3);
            ParcelCombination combo07 = CalculateError(7, street1, street2, street3, street4, 4, 1, 3, 3);
            ParcelCombination combo08 = CalculateError(8, street1, street2, street3, street4, 4, 2, 3, 3);

            ParcelCombination combo09 = CalculateError(9, street1, street2, street3, street4, 1, 1, 3, 4);
            ParcelCombination combo10 = CalculateError(10, street1, street2, street3, street4, 1, 2, 3, 4);
            ParcelCombination combo11 = CalculateError(11, street1, street2, street3, street4, 4, 1, 3, 4);
            ParcelCombination combo12 = CalculateError(12, street1, street2, street3, street4, 2, 2, 3, 4);

            ParcelCombination combo13 = CalculateError(13, street1, street2, street3, street4, 1, 1, 2, 4);
            ParcelCombination combo14 = CalculateError(14, street1, street2, street3, street4, 1, 2, 2, 4);
            ParcelCombination combo15 = CalculateError(15, street1, street2, street3, street4, 4, 1, 2, 4);
            ParcelCombination combo16 = CalculateError(16, street1, street2, street3, street4, 4, 2, 2, 4);

            ParcelCombination[] combos = { combo01, combo02, combo03, combo04, combo05, combo06, combo07, combo08, combo09, combo10, combo11, combo12, combo13, combo14, combo15, combo16 };

            ret = GetBestCombination(combos);
            return ret;

        }

        public bool IsRectangular(string polyline, int sourceId, string source, string fromAddressLeft, string toAddressLeft, string fromAddressRight, string toAddressRight, ValidateableStreetAddress address)
        {
            bool ret = false;
            Block block = GetBlock(polyline, sourceId, source, fromAddressLeft, toAddressLeft, fromAddressRight, toAddressRight, address);
            if (block != null)
            {
                ret = block.Valid;
            }
            return ret;
        }

        public ParcelCombination GetBestCombination(ParcelCombination[] parcelCombinations)
        {
            ParcelCombination ret = null;
            if (parcelCombinations != null && parcelCombinations.Length > 0)
            {
                for (int i = 0; i < parcelCombinations.Length; i++)
                {
                    ParcelCombination dimensionCombination = parcelCombinations[i];
                    if (ret == null)
                    {
                        ret = dimensionCombination;
                    }
                    else
                    {
                        if (dimensionCombination.Error < ret.Error)
                        {
                            ret = dimensionCombination;
                        }
                    }
                }
            }
            return ret;
        }

        public ParcelCombination CalculateError(int comboId, Street street1, Street street2, Street street3, Street street4, int corner1Facing, int corner2Facing, int corner3Facing, int corner4Facing)
        {
            ParcelCombination ret = new ParcelCombination();
            Street newStreet1 = street1.Clone();


            ret.NumberOfLots = street1.NumberOfLots;

            double street1WidthSummation = street1.GetWidthOfParcelsSummation();
            double street2WidthSummation = street2.GetWidthOfParcelsSummation();
            double street3WidthSummation = street3.GetWidthOfParcelsSummation();
            double street4WidthSummation = street4.GetWidthOfParcelsSummation();

            double street1Length = street1WidthSummation;
            double street2Length = street2WidthSummation;
            double street3Length = street3WidthSummation;
            double street4Length = street4WidthSummation;

            if (corner1Facing == 1)
            {
                street4Length += street1.GetFirstDepth();
            }
            else
            {
                street1Length += street4.GetLastDepth();
                newStreet1.insertFirstParcel(street4.GetLastParcel().invert());
                ret.NumberOfLots++;
            }

            if (corner2Facing == 2)
            {
                street1Length += street2.GetFirstDepth();
                newStreet1.insertLastParcel(street2.GetFirstParcel().invert());
                ret.NumberOfLots++;
            }
            else
            {
                street2Length += street1.GetLastDepth();
            }

            if (corner3Facing == 3)
            {
                street2Length += street3.GetFirstDepth();
            }
            else
            {
                street3Length += street2.GetLastDepth();
            }

            if (corner4Facing == 4)
            {
                street3Length += street4.GetFirstDepth();
            }
            else
            {
                street4Length += street3.GetLastDepth();
            }

            double street1Error = street1.Length - street1Length;
            double street2Error = street2.Length - street2Length;
            double street3Error = street3.Length - street3Length;
            double street4Error = street4.Length - street4Length;

            ret.Error = Math.Sqrt(
                (street1Error * street1Error) +
                (street2Error * street2Error) +
                (street3Error * street3Error) +
                (street4Error * street4Error)
                );

            ret.Street = newStreet1;
            ret.Id = comboId;

            return ret;
        }

        public Block GetBlock(string polyLinestring, int sourceId, string source, string fromAddressLeft, string toAddressLeft, string fromAddressRight, string toAddressRight, ValidateableStreetAddress address)
        {
            
            PolyLine polyLine = PolyLine.FromCoordinateString(polyLinestring);

            Street street = new Street(address, fromAddressLeft, toAddressLeft, fromAddressRight, toAddressRight);
            street.Start = polyLine.StartingPoint;
            street.End = polyLine.EndingPoint;

            // determine 
            //  1) which side of the street the address is on
            //  2) what the from and to addresses are on that side of the street

            AddressRange range = street.GetAddressRangeForAddress(address);
            int parcelDirection = CardinalDirection.calculateDropBackDirection(street, range.IsLeft);
            string parcelDirectionstring = CardinalDirection.getDirectionName(parcelDirection);

            return GetBlock(polyLinestring, sourceId, source, address, range, fromAddressLeft, toAddressLeft, fromAddressRight, toAddressRight, parcelDirectionstring); 
        }

        public Block GetBlock(string polyline, int sourceId, string source, ValidateableStreetAddress address, AddressRange addressRange, string fromAddressLeft, string toAddressLeft, string fromAddressRight, string toAddressRight, string parcelDirectionstring)
        {
            Block block = new Block();
            int parcelDirection = CardinalDirection.getDirectionValue(parcelDirectionstring);

            PolyLine startingPolyLine = PolyLine.FromCoordinateString(polyline);
            startingPolyLine.SourceId = sourceId;
            startingPolyLine.Source = source;


            if (!CardinalDirection.isAClockWiseCombination(startingPolyLine.PrimaryDirection, parcelDirection))
            {
                startingPolyLine = startingPolyLine.Reverse();
            }

            if (CardinalDirection.isAClockWiseCombination(startingPolyLine.PrimaryDirection, parcelDirection))
            {

                Street side1;
                Street side2 = null;
                Street side3 = null;
                Street side4;


                side1 = new Street(startingPolyLine.StartingPoint, startingPolyLine.EndingPoint);
                side1.PreDirectional = address.PreDirectional;
                side1.Name = address.StreetName;
                side1.Suffix = address.Suffix;
                side1.PostDirectional = address.PostDirectional;
                side1.Id = startingPolyLine.SourceId.ToString();
                side1.Source = startingPolyLine.Source;
                side1.FromAddressHouseNumberMajorLeftStr = fromAddressLeft;
                side1.ToAddressHouseNumberMajorLeftStr = toAddressLeft;
                side1.FromAddressHouseNumberMajorRightStr = fromAddressRight;
                side1.ToAddressHouseNumberMajorRightStr = toAddressRight;
                side1.IsReversed = startingPolyLine.IsReversed;
                side1.ZIPLeft = address.ZIP;
                side1.ZIPRight = address.ZIP;
                side1.State = address.State;


                AddressRange side1AR =
                    AddressRangeProvider.GetAddresses(address, addressRange.FromAddress, addressRange.ToAddress);

                side1AR.Id = sourceId.ToString();
                int[] side1AddressRange = side1AR.getAddresses();
                for (int i = 0; i < side1AddressRange.Length; i++)
                {
                    ValidateableStreetAddress tempAddress = side1.CreateAddress();
                    tempAddress.Number = side1AddressRange[i].ToString();
                    
                    // this line needs to be re-implemented
                    // string roughParcel = FeatureSource.GetFeature(new ParameterSet()).MatchedGeometry.CoordinateString
                    string roughParcel = null;
                    throw new Exception("GetBlock needs to be re-implemented");

                    double dropBackBearing = Bearing.getDropBackBearing(side1);
                    Parcel rectangleParcel =
                        Parcel.RectangeFromRough(roughParcel, dropBackBearing, side1.Bearing);
                    rectangleParcel.NumberStr = tempAddress.Number;
                    side1.addParcel(rectangleParcel);
                }

                side1.setInfoStrings();
                block.AddStreet(side1);

                Street[] corner1Segments = (Street[]) LineIntersectionSource.GetIntersectingLines(side1.End.Y, side1.End.X, address.State);
                if (corner1Segments != null && corner1Segments.Length > 0)
                {
                    Line nextLine = Line.getClockwiseCornerSegment(side1, corner1Segments);
                    if (nextLine != null)
                    {
                        side2 = new Street(nextLine.Start, nextLine.End);

                        // I think that all of the street attributes will be missing from nextLine and therefore side2, 
                        // and will need to be added back in otherwise the createAddress will not konw what street attributes to use
                        ValidateableStreetAddress tempAddress = side2.CreateAddress();

                        AddressRange side2AddressRange = side2.CreateAddressRange();
                        AddressRange side2AddressSegment =
                            AddressRangeProvider.GetAddresses(tempAddress, side2AddressRange.FromAddress, side2AddressRange.ToAddress);

                        int[] side2Addresses = side2AddressSegment.getAddresses();
                        for (int i = 0; i < side2Addresses.Length; i++)
                        {
                            tempAddress = side2.CreateAddress();
                            tempAddress.Number = side2Addresses[i].ToString();

                            // this line needs to be re-implemented
                            // string roughParcel = FeatureSource.GetFeature(new ParameterSet()).MatchedGeometry.CoordinateString
                            string roughParcel = null;
                            throw new Exception("GetBlock needs to be re-implemented");

                            double dropBackBearing = Bearing.getDropBackBearing(side2);
                            Parcel rectangleParcel =
                                Parcel.RectangeFromRough(roughParcel, dropBackBearing, side2.Bearing);
                            rectangleParcel.NumberStr = tempAddress.Number;
                            side2.addParcel(rectangleParcel);
                        }
                        side2.setInfoStrings();
                        block.AddStreet(side2);
                    }
                }

                if (side2 != null)
                {
                    Street[]corner2Segments = (Street[])LineIntersectionSource.GetIntersectingLines(side2.End.Y, side2.End.X, address.State);
                    if (corner2Segments != null && corner2Segments.Length > 0)
                    {
                        Line nextLine = Line.getClockwiseCornerSegment(side2, corner2Segments);
                        if (nextLine != null)
                        {
                            side3 = new Street(nextLine.Start, nextLine.End);
                            ValidateableStreetAddress tempAddress = side3.CreateAddress();
                            AddressRange side3AddressRange = side3.CreateAddressRange();
                            AddressRange side3AddressSegment =
                                AddressRangeProvider.GetAddresses(tempAddress, side3AddressRange.FromAddress, side3AddressRange.ToAddress);

                            int[] side3Addresses = side3AddressSegment.getAddresses();
                            for (int i = 0; i < side3Addresses.Length; i++)
                            {
                                tempAddress = side3.CreateAddress();
                                tempAddress.Number = side3Addresses[i].ToString();

                                // this line needs to be re-implemented
                                // string roughParcel = FeatureSource.GetFeature(new ParameterSet()).MatchedGeometry.CoordinateString
                                string roughParcel = null;
                                throw new Exception("GetBlock needs to be re-implemented");

                                double dropBackBearing = Bearing.getDropBackBearing(side3);
                                Parcel rectangleParcel =
                                    Parcel.RectangeFromRough(roughParcel, dropBackBearing, side3.Bearing);
                                rectangleParcel.NumberStr = tempAddress.Number;
                                side3.addParcel(rectangleParcel);
                            }
                            side3.setInfoStrings();
                            block.AddStreet(side3);
                        }
                    }

                    if (side3 != null)
                    {
                        Street[] corner3Segments = (Street[])LineIntersectionSource.GetIntersectingLines(side3.End.Y, side3.End.X, address.State);
                        if (corner3Segments != null && corner3Segments.Length > 0)
                        {
                            Line nextLine = Line.getClockwiseCornerSegment(side3, corner3Segments);
                            if (nextLine != null)
                            {
                                side4 = new Street(nextLine.Start, nextLine.End);
                                ValidateableStreetAddress tempAddress = side4.CreateAddress();
                                AddressRange side4AddressRange = side4.CreateAddressRange();
                                AddressRange side4AddressSegment =
                                    AddressRangeProvider.GetAddresses(tempAddress,side4AddressRange.FromAddress,side4AddressRange.ToAddress);

                                int[] side4Addresses = side4AddressSegment.getAddresses();
                                for (int i = 0; i < side4Addresses.Length; i++)
                                {
                                    tempAddress = side4.CreateAddress();
                                    tempAddress.Number = side4Addresses[i].ToString();

                                    // this line needs to be re-implemented
                                    // string roughParcel = FeatureSource.GetFeature(new ParameterSet()).MatchedGeometry.CoordinateString
                                    string roughParcel = null;
                                    throw new Exception("GetBlock needs to be re-implemented");

                                    double dropBackBearing = Bearing.getDropBackBearing(side4);
                                    Parcel rectangleParcel =
                                        Parcel.RectangeFromRough(roughParcel, dropBackBearing, side4.Bearing);
                                    rectangleParcel.NumberStr = tempAddress.Number;
                                    side4.addParcel(rectangleParcel);
                                }
                                side4.setInfoStrings();
                                block.AddStreet(side4);
                            }
                            else
                            {
                                block.BlockGroupError = "not able to determine side 4";
                            }
                        }
                    }
                    else
                    {
                        block.BlockGroupError = "not able to determine side 3";
                    }
                }
                else
                {
                    block.BlockGroupError = "not able to determine side 2";
                }
            }
            else
            {
                block.BlockGroupError = "The parcel direction relative to the street vector is an impossible match";
            }

            block.setInfoStrings();
            block.validate();
            return block;
        }
    }
}

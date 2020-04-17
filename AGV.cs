namespace WarehouseDesigner
{
    public class AGVBase
    {
        public double mSideLength { get; set; }
        public double mMaxSpeed { get; set; }
        public double mAcceleration { get; set; }
    }

    public class AGV : AGVBase
    {
        public (int, int) mStartPos { get; set; }
        public (int, int) mPickupPos { get; set; }
        public (int, int) mDropdownPos { get; set; }
        public (int, int) mEndPost { get; set; }
        public int mIndex { get; set; }
        public AGV()
        {
            mIndex = 0;
        }

        public int AddStartPos(int iX, int iY)
        {
            mStartPos = (iX, iY);
            return ConstDefine.ERR_NO_ERROR;
        }
        public int AddPickupPos(int iX, int iY)
        {
            mPickupPos = (iX, iY);
            return ConstDefine.ERR_NO_ERROR;
        }
        public int AddDropdownPos(int iX, int iY)
        {
            mDropdownPos = (iX, iY);
            return ConstDefine.ERR_NO_ERROR;
        }
        public int AddEndPost(int iX, int iY)
        {
            mEndPost = (iX, iY);
            return ConstDefine.ERR_NO_ERROR;
        }
    }
}

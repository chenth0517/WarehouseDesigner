using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace WarehouseDesigner
{
    public class Field : INotifyPropertyChanged
    {
        private int _rowCount;
        private int _columnCount;
        private string _fileName; //布局文件名
        public int mRowCount
        {
            get => _rowCount;
            set
            {
                if (_rowCount != value)
                {
                    _rowCount = value;
                    this.OnPropertyChanged();
                }
            }
        }
        public int mColumnCount
        {
            get => _columnCount;
            set
            {
                if (_columnCount != value)
                {
                    _columnCount = value;
                    this.OnPropertyChanged();
                }
            }
        }
        public string mFileName
        {
            get => _fileName;
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // 编辑阻塞方块：按坐标位置取反（没有就追加，已有就删除）
        public List<(int, int)> mBlockList = new List<(int, int)>();
        public int EditBlock(int iX, int iY)
        {
            (int, int) pos = (iX, iY);
            if (IsOccupied(mShelfList, ref pos))
            {
                Debug.WriteLine("You must remove the shelf first.");
                return ConstDefine.ERR_POSITION_OCCUPIED_BY_SHELF;
            }
            int oIndex = ConstDefine.INVALID_VALUE;
            if (IsOccupied(mAGVList, pos, out oIndex))
            {
                Debug.WriteLine("You must remove the AGV point first.");
                return ConstDefine.ERR_POSITION_OCCUPIED_BY_AGV;
            }
            return EditList(mBlockList, iX, iY);
        }
        // 编辑货架方块：按坐标位置取反（没有就追加，已有就删除）
        public List<(int, int)> mShelfList = new List<(int, int)>();
        public int EditShelf(int iX, int iY)
        {
            (int, int) pos = (iX, iY);
            if (IsOccupied(mBlockList, ref pos))
            {
                Debug.WriteLine("You must remove the block first.");
                return ConstDefine.ERR_POSITION_OCCUPIED_BY_BLOCK;
            }
            return EditList(mShelfList, iX, iY);
        }
        //占位判断：用于阻塞及货架
        public static bool IsOccupied(List<(int, int)> iTarget, ref (int, int) ioPos)
        {
            Contract.Requires(iTarget != null);
            foreach ((int, int) pos in iTarget)
            {
                if (pos.Equals(ioPos))
                {
                    ioPos = pos;
                    return true;
                }
            }
            return false;
        }
        //占位判断：用于AGV
        public static bool IsOccupied(List<AGV> iAGVTarger, (int, int) ioPos, out int oIndex)
        {
            Contract.Requires(iAGVTarger != null);
            oIndex = ConstDefine.INVALID_VALUE;
            foreach (AGV agv in iAGVTarger)
            {
                if (agv.mStartPos.Equals(ioPos))
                {
                    oIndex = agv.mIndex;
                    return true;
                }
                if (agv.mPickupPos.Equals(ioPos))
                {
                    oIndex = agv.mIndex;
                    return true;
                }
                if (agv.mDropdownPos.Equals(ioPos))
                {
                    oIndex = agv.mIndex;
                    return true;
                }
                if (agv.mEndPost.Equals(ioPos))
                {
                    oIndex = agv.mIndex;
                    return true;
                }
            }
            return false;
        }
        // 方块编辑：考察现有的列表iList，并按坐标位置取反（没有就追加，已有就删除）
        public int EditList(List<(int, int)> iList, int iX, int iY)
        {
            Contract.Requires(iList != null);
            if (iX >= mColumnCount || iY >= mRowCount)
            {
                Debug.WriteLine("EditBlock error: Index out of boundary");
                return ConstDefine.ERR_INDEX_OUT_OF_BOUNDARY;
            }
            (int, int) pos = (iX, iY);
            if (IsOccupied(iList, ref pos))
            {
                iList.Remove(pos);
            }
            else
            {
                iList.Add(pos);
            }
            return ConstDefine.ERR_NO_ERROR;
        }

        //AGV实例
        public int mAGVDeployStep { get; set; }
        public int NextAGVDeployStep()
        {
            switch (mAGVDeployStep)
            {
                case ConstDefine.DRAW_AGV_STEP_START:
                    return ConstDefine.DRAW_AGV_STEP_PICKUP;
                case ConstDefine.DRAW_AGV_STEP_PICKUP:
                    return ConstDefine.DRAW_AGV_STEP_DROPDOWN;
                case ConstDefine.DRAW_AGV_STEP_DROPDOWN:
                    return ConstDefine.DRAW_AGV_STEP_END;
                case ConstDefine.DRAW_AGV_STEP_END:
                    return ConstDefine.DRAW_AGV_STEP_START;
                default:
                    return ConstDefine.DRAW_AGV_STEP_START;
            }
        }
        AGV tAGV { get; set; }
        public List<AGV> mAGVList { get; set; }
        public int mMaxAgvListCount = 0;
        public int EditAGV(int iX, int iY, out int oRemoveIndex)
        {
            oRemoveIndex = ConstDefine.INVALID_VALUE;
            if (iX >= mColumnCount || iY >= mRowCount)
            {
                Debug.WriteLine("EditBlock error: Index out of boundary");
                return ConstDefine.ERR_INDEX_OUT_OF_BOUNDARY;
            }
            (int, int) pos = (iX, iY);
            if (IsOccupied(mBlockList, ref pos))
            {
                Debug.WriteLine("AGV cannot get through block.");
                return ConstDefine.ERR_POSITION_OCCUPIED_BY_BLOCK;
            }
            if (IsOccupied(mAGVList, pos, out oRemoveIndex))
            {
                foreach (AGV agv in mAGVList)
                {
                    if (agv.mIndex == oRemoveIndex)
                    {
                        mAGVList.Remove(agv);
                        break;
                    }
                }
            }
            switch (mAGVDeployStep)
            {
                case ConstDefine.DRAW_AGV_STEP_START:
                    tAGV = new AGV();
                    tAGV.mIndex = mMaxAgvListCount;
                    tAGV.AddStartPos(iX, iY);
                    break;
                case ConstDefine.DRAW_AGV_STEP_PICKUP:
                    tAGV.AddPickupPos(iX, iY);
                    break;
                case ConstDefine.DRAW_AGV_STEP_DROPDOWN:
                    tAGV.AddDropdownPos(iX, iY);
                    break;
                case ConstDefine.DRAW_AGV_STEP_END:
                    tAGV.AddEndPost(iX, iY);
                    mAGVList.Add(tAGV);
                    mMaxAgvListCount++;
                    break;
                default:
                    Debug.WriteLine($"Invalid AVG position {iX},{iY}");
                    break;
            }
            return ConstDefine.ERR_NO_ERROR;
        }
        //生成文件内容
        public int GenerateConfig(List<string> oConfig)
        {
            Contract.Requires(oConfig != null);
            oConfig.Add("RowCount=" + mRowCount);
            oConfig.Add("ColumnCount=" + mColumnCount);
            oConfig.Add("BlockList=" + JsonConvert.SerializeObject(mBlockList));
            oConfig.Add("ShelfList=" + JsonConvert.SerializeObject(mShelfList));
            oConfig.Add("AGVList=" + JsonConvert.SerializeObject(mAGVList));
            return ConstDefine.ERR_NO_ERROR;
        }
        //解析文件内容
        public int ParseConfig(List<string> iConfig)
        {
            Contract.Requires(iConfig != null);
            if (iConfig.Count < ConstDefine.LAYOUT_CONFIG_FILE_ROW)
            {
                return ConstDefine.ERR_INVALID_FILE_CONTENT;
            }
            mRowCount = int.Parse(iConfig[0].Split("=")[1]);
            mColumnCount = int.Parse(iConfig[1].Split("=")[1]);
            mBlockList = JsonConvert.DeserializeObject<List<(int, int)>>(iConfig[2].Split("=")[1]);
            mShelfList = JsonConvert.DeserializeObject<List<(int, int)>>(iConfig[3].Split("=")[1]);
            mAGVList = JsonConvert.DeserializeObject<List<AGV>>(iConfig[4].Split("=")[1]);
            mMaxAgvListCount = mAGVList[mAGVList.Count-1].mIndex + 1; //mAGVList中的mIndex是增序排列的，所以取最大值+1
            return ConstDefine.ERR_NO_ERROR;
        }
    }
}

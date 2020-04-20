namespace WarehouseDesigner
{
    class ConstDefine
    {
        // 错误编吗
        public const int ERR_NO_ERROR = 0;                      // 没有错误
        public const int ERR_POSITION_OCCUPIED_BY_SHELF = -1;   // 位置已被货架占用
        public const int ERR_POSITION_OCCUPIED_BY_BLOCK = -2;   // 位置已被阻塞占用
        public const int ERR_POSITION_OCCUPIED_BY_AGV = -3;     // 位置已被AGV占用
        public const int ERR_INDEX_OUT_OF_BOUNDARY = -4;        // 下表越界
        public const int ERR_INVALID_FILE_CONTENT = -5;         // 无效文件内容

        // 缺省参数
        public const int DEFAULT_COLUMN_COUNT = 12;             // 缺省列数量
        public const int DEFAULT_ROW_COUNT = 9;                 // 缺省行数量
        public const double DEFAULT_SHELF_SIDE_LENGTH = 0.8;    // 缺省货架边长（按网格边长比例计算）
        public const double DEFAULT_AVG_SIDE_LENGTH = 0.9;      // 缺省AGV边长（按网格边长比例计算）
        public const int DEFAULT_MAX_SPEED = 1;                 // 缺省AGV最大速度
        public const int DEFAULT_ACCELERATION = 1;              // 缺省AGV加速度
        public const int DEFAULT_LINE_THICKNESS = 2;            // 缺省AGV网格线宽

        // 存储参数
        public const int LAYOUT_CONFIG_FILE_ROW = 5;            // 布局配置文件内容行数量

        // 绘制AGV的不同步骤
        public const int DRAW_AGV_STEP_START = 0;               // AGV初始位置
        public const int DRAW_AGV_STEP_PICKUP = 1;              // AGV装货位置
        public const int DRAW_AGV_STEP_DROPDOWN = 2;            // AGV卸货位置
        public const int DRAW_AGV_STEP_END = 3;                 // AGV停止位置
        public const int DRAW_AGV_STEP_FINISH = 4;              // AGV结束位置（暂未使用）

        // 其他参数
        public const int INVALID_VALUE = -1;                    // 无效值
    }
}

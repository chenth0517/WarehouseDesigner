namespace WarehouseDesigner
{
    class ConstDefine
    {
        public const int ERR_NO_ERROR = 0;
        public const int ERR_POSITION_OCCUPIED_BY_SHELF = -1;
        public const int ERR_POSITION_OCCUPIED_BY_BLOCK = -2;
        public const int ERR_POSITION_OCCUPIED_BY_AGV = -3;
        public const int ERR_INDEX_OUT_OF_BOUNDARY = -4;
        public const int ERR_INVALID_FILE_CONTENT = -5;

        public const int DEFAULT_COLUMN_COUNT = 12;
        public const int DEFAULT_ROW_COUNT = 9;
        public const double DEFAULT_SHELF_SIDE_LENGTH = 0.8;
        public const double DEFAULT_AVG_SIDE_LENGTH = 0.9;
        public const int DEFAULT_MAX_SPEED = 1;
        public const int DEFAULT_ACCELERATION = 1;
        public const int DEFAULT_LINE_THICKNESS = 2;

        public const int LAYOUT_CONFIG_FILE_ROW = 5;

        public const int DRAW_AGV_STEP_START = 0;
        public const int DRAW_AGV_STEP_PICKUP = 1;
        public const int DRAW_AGV_STEP_DROPDOWN = 2;
        public const int DRAW_AGV_STEP_END = 3;
        public const int DRAW_AGV_STEP_FINISH = 4;

        public const int INVALID_VALUE = -1;
    }
}

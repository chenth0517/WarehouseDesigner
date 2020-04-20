using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WarehouseDesigner
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        //布局信息
        private Field mField = new Field
        {
            mColumnCount = ConstDefine.DEFAULT_COLUMN_COUNT,
            mRowCount = ConstDefine.DEFAULT_ROW_COUNT,
            mAGVList = new List<AGV>()
        };
        //AGV属性
        private AGVBase mAGVBase = new AGVBase
        {
            mSideLength = ConstDefine.DEFAULT_AVG_SIDE_LENGTH,
            mMaxSpeed = ConstDefine.DEFAULT_MAX_SPEED,
            mAcceleration = ConstDefine.DEFAULT_ACCELERATION
        };
        //AGV画布
        private List<DrawAGV> mDrawAGVList = new List<DrawAGV>();
        //不同的画布
        private Canvas mCanvasGrid;     //网格画布
        private Canvas mCanvasBlock;    //阻碍画布
        private Canvas mCanvasShelf;    //货架画布
        private Canvas mCanvasAGV;      //AGV根画布
        //交互消息
        private string _displayMessage;
        public string mDisplayMessage
        {
            get => _displayMessage;
            set
            {
                if (_displayMessage != value)
                {
                    _displayMessage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public MainPage()
        {
            this.InitializeComponent();
            mCanvasGrid = new Canvas();
            mCanvasBlock = new Canvas();
            mCanvasShelf = new Canvas();
            mCanvasAGV = new Canvas();
            MainCanvas.Children.Add(mCanvasGrid);
            MainCanvas.Children.Add(mCanvasBlock);
            MainCanvas.Children.Add(mCanvasShelf);
            MainCanvas.Children.Add(mCanvasAGV);
            Test();
        }

        private void Test()
        {
            
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            mField.mBlockList.Clear();
            mField.mShelfList.Clear();
            mField.mAGVList.Clear();
            mField.mMaxAgvListCount = 0;
            mField.mFileName = "New file";
            mDisplayMessage = mField.mFileName;
            ReDrawAll();
            ReDrawAGVList(mField.mAGVList);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private int ReDrawAll()
        {
            int ret = ReDrawGrid();
            if (ret < 0) return ret;
            ret = ReDrawBlockList();
            if (ret < 0) return ret;
            ret = ReDrawShelfList();
            return ret;
        }

        private int ReDrawGrid()
        {
            mCanvasGrid.Children.Clear();
            double gridWidth = MainCanvas.ActualWidth / mField.mColumnCount;
            double gridHeight = MainCanvas.ActualHeight / mField.mRowCount;
            for(int i = 0; i <= mField.mRowCount; i++)
            {
                var lineHorizontal = new Line();
                lineHorizontal.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
                lineHorizontal.StrokeThickness = ConstDefine.DEFAULT_LINE_THICKNESS;
                lineHorizontal.X1 = 0;
                lineHorizontal.Y1 = i * gridHeight;
                lineHorizontal.X2 = MainCanvas.ActualWidth;
                lineHorizontal.Y2 = lineHorizontal.Y1;

                mCanvasGrid.Children.Add(lineHorizontal);
            }
            for (int i = 0; i <= mField.mColumnCount; i++)
            {
                var lineVertical = new Line();
                lineVertical.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
                lineVertical.StrokeThickness = ConstDefine.DEFAULT_LINE_THICKNESS;
                lineVertical.X1 = i * gridWidth;
                lineVertical.Y1 = 0;
                lineVertical.X2 = lineVertical.X1;
                lineVertical.Y2 = MainCanvas.ActualHeight;

                //lineVertical.RenderTransform = new TranslateTransform
                //{
                //    X = 0,
                //    Y = 50
                //};
                mCanvasGrid.Children.Add(lineVertical);
            }
            return ConstDefine.ERR_NO_ERROR;
        }

        private int ReDrawBlockList()
        {
            mCanvasBlock.Children.Clear();
            return ReDrawRectangleList(mField.mBlockList, Windows.UI.Colors.Black);
        }
        private int ReDrawShelfList()
        {
            mCanvasShelf.Children.Clear();
            return ReDrawRectangleList(mField.mShelfList, Windows.UI.Colors.LightGoldenrodYellow, ConstDefine.DEFAULT_SHELF_SIDE_LENGTH);
        }
        private int ReDrawRectangleList(List<(int, int)> iRectList, Windows.UI.Color iColor, double iShrink = 1)
        {
            double gridWidth = MainCanvas.ActualWidth / mField.mColumnCount;
            double gridHeight = MainCanvas.ActualHeight / mField.mRowCount;
            foreach ((int, int) pos in iRectList)
            {
                var rectangle = new Rectangle();
                rectangle.Fill = new SolidColorBrush(iColor);
                rectangle.Width = gridWidth * iShrink;
                rectangle.Height = gridHeight * iShrink;
                rectangle.Stroke = new SolidColorBrush(iColor);
                rectangle.StrokeThickness = ConstDefine.DEFAULT_LINE_THICKNESS;
                rectangle.RadiusX = 0;
                rectangle.RadiusY = 0;
                rectangle.RenderTransform = new TranslateTransform
                {
                    X = pos.Item1 * gridWidth + gridWidth * (1 - iShrink) / 2,
                    Y = pos.Item2 * gridHeight + gridHeight * (1 - iShrink) / 2,
                };
                if (iRectList == mField.mBlockList)
                {
                    mCanvasBlock.Children.Add(rectangle);
                }
                else if (iRectList == mField.mShelfList)
                {
                    mCanvasShelf.Children.Add(rectangle);
                }
            }
            return ConstDefine.ERR_NO_ERROR;
        }

        DrawAGV drawAGV;
        private int ReDrawAGVList(List<AGV> iAGVList)
        {
            mCanvasAGV.Children.Clear();
            foreach (AGV agv in iAGVList)
            {
                ReDrawAGV(ConstDefine.DRAW_AGV_STEP_START, agv.mStartPos.Item1, agv.mStartPos.Item2, agv.mIndex);
                ReDrawAGV(ConstDefine.DRAW_AGV_STEP_PICKUP, agv.mPickupPos.Item1, agv.mPickupPos.Item2, agv.mIndex);
                ReDrawAGV(ConstDefine.DRAW_AGV_STEP_DROPDOWN, agv.mDropdownPos.Item1, agv.mDropdownPos.Item2, agv.mIndex);
                ReDrawAGV(ConstDefine.DRAW_AGV_STEP_END, agv.mEndPost.Item1, agv.mEndPost.Item2, agv.mIndex);
            }
            return ConstDefine.ERR_NO_ERROR;
        }
        private int ReDrawAGV(int iStep, int iX, int iY, int? iIndex=null)
        {
            double gridWidth = MainCanvas.ActualWidth / mField.mColumnCount;
            double gridHeight = MainCanvas.ActualHeight / mField.mRowCount;
            Windows.UI.Color color;
            TextBlock textBlock = new TextBlock();
            textBlock.FontFamily = new FontFamily("Segoe MDL2 Assets");
            switch(iStep)
            {
                case ConstDefine.DRAW_AGV_STEP_START:
                    drawAGV = new DrawAGV();
                    drawAGV.mIndex = ((iIndex == null) ? mField.mMaxAgvListCount : (int)iIndex);
                    color = Windows.UI.Colors.Yellow;
                    textBlock.Text = ((char)0xE707).ToString();
                    break;
                case ConstDefine.DRAW_AGV_STEP_PICKUP:
                    color = Windows.UI.Colors.Orange;
                    textBlock.Text = ((char)0xE898).ToString();
                    break;
                case ConstDefine.DRAW_AGV_STEP_DROPDOWN:
                    color = Windows.UI.Colors.DarkOrange;
                    textBlock.Text = ((char)0xE896).ToString();
                    break;
                case ConstDefine.DRAW_AGV_STEP_END:
                    color = Windows.UI.Colors.OrangeRed;
                    textBlock.Text = ((char)0xE7C1).ToString();
                    break;
                default:
                    Debug.WriteLine($"Invalid AVG position {iX},{iY}");
                    break;
            }
            var points = new PointCollection();
            points.Add(new Windows.Foundation.Point(0, 0));
            points.Add(new Windows.Foundation.Point(32, 0));
            points.Add(new Windows.Foundation.Point(0, 32));
            //points.Add(new Windows.Foundation.Point(gridWidth * ConstDefine.DEFAULT_AVG_SIDE_LENGTH, 0));
            //points.Add(new Windows.Foundation.Point(0, gridHeight * ConstDefine.DEFAULT_AVG_SIDE_LENGTH));
            var polygon = new Polygon();
            polygon.Fill = new SolidColorBrush(color);
            polygon.Stroke = new SolidColorBrush(color);
            polygon.StrokeThickness = ConstDefine.DEFAULT_LINE_THICKNESS;
            polygon.Points = points;
            polygon.RenderTransform = new TranslateTransform
            {
                X = iX * gridWidth + ConstDefine.DEFAULT_LINE_THICKNESS,
                Y = iY * gridHeight + ConstDefine.DEFAULT_LINE_THICKNESS,
            };
            textBlock.RenderTransform = new TranslateTransform
            {
                X = iX * gridWidth + ConstDefine.DEFAULT_LINE_THICKNESS,
                Y = iY * gridHeight + ConstDefine.DEFAULT_LINE_THICKNESS,
            };

            TextBlock textBlockIdx = new TextBlock();
            textBlockIdx.Text = " " + drawAGV.mIndex.ToString();
            textBlockIdx.RenderTransform = new TranslateTransform
            {
                X = iX * gridWidth + ConstDefine.DEFAULT_LINE_THICKNESS + 10,
                Y = iY * gridHeight + ConstDefine.DEFAULT_LINE_THICKNESS - 4,
            };

            drawAGV.mCanvas.Children.Add(polygon);
            drawAGV.mCanvas.Children.Add(textBlock);
            drawAGV.mCanvas.Children.Add(textBlockIdx);

            if (iStep == ConstDefine.DRAW_AGV_STEP_START)
            {
                mDrawAGVList.Add(drawAGV);
                mCanvasAGV.Children.Add(drawAGV.mCanvas);
            }
            return ConstDefine.ERR_NO_ERROR;
        }

        private void MainCanvas_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            Debug.WriteLine("MainCanvas_KeyDown: "+e.ToString());
        }

        private void MainCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            double mouseX = e.GetPosition(MainCanvas).X;
            double mouseY = e.GetPosition(MainCanvas).Y;
            Debug.WriteLine("MainCanvas_Tapped: Position({0},{1})", mouseX, mouseY);

            double gridWidth = MainCanvas.ActualWidth / mField.mColumnCount;
            double gridHeight = MainCanvas.ActualHeight / mField.mRowCount;

            int positionX = (int)(mouseX / gridWidth);
            int positionY = (int)(mouseY / gridHeight);
            //编辑Block
            if (editBlock.IsChecked == true)
            {
                int ret = mField.EditBlock(positionX, positionY);
                if (ret < 0)
                {
                    ShowErrorMessage(ret);
                    return;
                }
            }
            //编辑shelf
            else if (editShelf.IsChecked == true)
            {
                int ret = mField.EditShelf(positionX, positionY);
                if (ret < 0)
                {
                    ShowErrorMessage(ret);
                    return;
                }
            }
            ReDrawAll();
            //编辑AGV
            if (editAGV.IsChecked == true)
            {
                int oRemoveIndex = ConstDefine.INVALID_VALUE;
                int ret = mField.EditAGV(positionX, positionY, out oRemoveIndex);
                if (oRemoveIndex != ConstDefine.INVALID_VALUE)
                {
                    foreach (DrawAGV drawAGV in mDrawAGVList)
                    {
                        if (drawAGV.mIndex == oRemoveIndex)
                        {
                            mCanvasAGV.Children.Remove(drawAGV.mCanvas);
                            mDrawAGVList.Remove(drawAGV);
                            break;
                        }
                    }
                }
                if (ret < 0)
                {
                    ShowErrorMessage(ret);
                    return;
                }
                ReDrawAGV(mField.mAGVDeployStep, positionX, positionY);
                mField.mAGVDeployStep = mField.NextAGVDeployStep();
            }
        }

        private void EditBlock_Checked(object sender, RoutedEventArgs e)
        {
            editShelf.IsChecked = false;
            editAGV.IsChecked = false;
            mDisplayMessage = $"{mField.mFileName}: Edit Block ...";
        }
        private void EditBlock_Unchecked(object sender, RoutedEventArgs e)
        {
            mDisplayMessage = mField.mFileName;
        }

        private void EditShelf_Checked(object sender, RoutedEventArgs e)
        {
            editBlock.IsChecked = false;
            editAGV.IsChecked = false;
            mDisplayMessage = $"{mField.mFileName}: Edit Shelf ...";
        }
        private void EditShelf_Unchecked(object sender, RoutedEventArgs e)
        {
            mDisplayMessage = mField.mFileName;
        }

        private void EditAGV_Checked(object sender, RoutedEventArgs e)
        {
            editBlock.IsChecked = false;
            editShelf.IsChecked = false;
            mField.mAGVDeployStep = ConstDefine.DRAW_AGV_STEP_START;
            mDisplayMessage = $"{mField.mFileName}: Edit AGV : Start -> Pickup -> Dropdown -> End";
        }
        private void EditAGV_Unchecked(object sender, RoutedEventArgs e)
        {
            mField.mAGVDeployStep = ConstDefine.DRAW_AGV_STEP_START;
            mDisplayMessage = mField.mFileName;
        }

        public static async void ShowMessage(string iMessage)
        {
            var msgDialog = new Windows.UI.Popups.MessageDialog(iMessage);
            await msgDialog.ShowAsync();
        }
        public static async void ShowErrorMessage(int iErrorCode)
        {
            string message = "NO_ERROR";
            switch (iErrorCode)
            {
                case ConstDefine.ERR_NO_ERROR:
                    message = "ERR_NO_ERROR";
                    break;
                case ConstDefine.ERR_POSITION_OCCUPIED_BY_SHELF:
                    message = "ERR_POSITION_OCCUPIED_BY_SHELF";
                    break;
                case ConstDefine.ERR_POSITION_OCCUPIED_BY_BLOCK:
                    message = "ERR_POSITION_OCCUPIED_BY_BLOCK";
                    break;
                case ConstDefine.ERR_POSITION_OCCUPIED_BY_AGV:
                    message = "ERR_POSITION_OCCUPIED_BY_AGV";
                    break;
                case ConstDefine.ERR_INDEX_OUT_OF_BOUNDARY:
                    message = "ERR_INDEX_OUT_OF_BOUNDARY";
                    break;
                case ConstDefine.ERR_INVALID_FILE_CONTENT:
                    message = "ERR_INVALID_FILE_CONTENT";
                    break;
                default:
                    break;
            }
            var msgDialog = new Windows.UI.Popups.MessageDialog(message);
            await msgDialog.ShowAsync();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFile(sender, e, "Properties File", ".properties");
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(sender, e, ".properties");
        }

        private async void SaveFile(object sender, RoutedEventArgs e, string iDecription, string iFilter)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add(iDecription, new List<string>() { iFilter });
            savePicker.SuggestedFileName = "NewFile";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                List<string> content = new List<string>();
                mField.GenerateConfig(content);
                await FileIO.WriteLinesAsync(file, content);
                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    Debug.WriteLine("File " + file.Name + " was saved.");
                }
                else
                {
                    Debug.WriteLine("File " + file.Name + " couldn't be saved.");
                }
            }
            else
            {
                Debug.WriteLine("Operation canceled.");
            }
        }

        private async void OpenFile(object sender, RoutedEventArgs e, string iFilter)
        {
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            openPicker.FileTypeFilter.Add(iFilter);

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                Debug.WriteLine("1 StorageFile is {0}", file.Path);
                Debug.WriteLine("2 StorageFile is " + file.Path);
                List<string> content = (await FileIO.ReadLinesAsync(file)).ToList<string>();
                for (int i = 0; i < content.Count; i++)
                {
                    Debug.WriteLine(content[i]);
                }
                if (mField.ParseConfig(content) == ConstDefine.ERR_NO_ERROR)
                {
                    mField.mFileName = file.Name;
                    mDisplayMessage = mField.mFileName;
                    ReDrawAll();
                    ReDrawAGVList(mField.mAGVList);
                }
            }
            else
            {
                Debug.WriteLine("StorageFile is NULL");
            }
        }
    }

    public class MainViewModel
    {

    }
}

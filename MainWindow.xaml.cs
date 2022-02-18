using pz3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Point = System.Windows.Point;

namespace pz3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        
        private Dictionary<GeometryModel3D, string> models;
        private GeographicModel geographicModel;
        private Dictionary<Point, int> allModels;
        private Dictionary<ulong, List<GeometryModel3D>> lineEntityModels;
        private List<ulong> iDsOfLinesThatStartFromSwitch;
        private Dictionary<ulong, float> lineEntityResistances;
        private Dictionary<ulong, int> numberOfLines;
        private Dictionary<ulong, EntityModel> modelClasses;
        private Dictionary<ulong, string> switchEntityStatuses;
        private ToolTipHelper currToolTip;
        private int zoomCurrent;
        #region Points
        private Point start;
        private Point diffOffset;
        private Point startPosition;
        private Point currentMouseHit;
        #endregion
        #region Constants
        private const double mapSquare = -1;
        private const double step = 0.007;
        private const double cubeSize = 0.005;
        private const int zoomMax = 70;
        #endregion
        #region ButtonsClicked
        private bool hsbuttonClicked;
        private bool hsbutton2Clicked;
        private bool hsbutton3Clicked;

        private bool isClicked;
        private bool isClicked2;
        private bool isClicked3;

        private bool isActiveClicked;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            InitializeFields();
        }
        private void InitializeFields()
        {
            geographicModel = new GeographicModel();
            allModels = new Dictionary<Point, int>();
            models = new Dictionary<GeometryModel3D, string>();
            lineEntityModels = new Dictionary<ulong, List<GeometryModel3D>>();
            iDsOfLinesThatStartFromSwitch = new List<ulong>();
            lineEntityResistances = new Dictionary<ulong, float>();
            switchEntityStatuses = new Dictionary<ulong, string>();
            currToolTip = new ToolTipHelper("Test");
            zoomCurrent = 1;
            hsbuttonClicked = hsbutton2Clicked = hsbutton3Clicked = false;
            isClicked = isClicked2 = isClicked3 = false;
            isActiveClicked = false;
            start = new Point();
            diffOffset = new Point();
            startPosition = new Point(0, 0);
            currentMouseHit = new Point();
            modelClasses = new Dictionary<ulong, EntityModel>();
            numberOfLines = new Dictionary<ulong, int>();
        }

        #region DrawMethods
        private void DrawSubstations()
        {
            foreach (var item in geographicModel.Substations)
            {
                double offsetX = mapSquare + (((item.X - geographicModel.mapLeft) / geographicModel.intervalX) * geographicModel.mapEdgeSize);
                double offsetY = mapSquare + (((item.Y - geographicModel.mapBottom) / geographicModel.intervalY) * geographicModel.mapEdgeSize);
                double offsetZ = 0;
                var cube = GetCube(Brushes.Red);
                if (CheckSameSpot(item.X, item.Y, out Point point))
                {
                    offsetX = mapSquare + (((point.X - geographicModel.mapLeft) / geographicModel.intervalX) * geographicModel.mapEdgeSize);
                    offsetY = mapSquare + (((point.Y - geographicModel.mapBottom) / geographicModel.intervalY) * geographicModel.mapEdgeSize);
                    offsetZ = step + allModels[point] * step;
                    allModels[point]++;
                }
                else
                {
                    offsetZ = step;
                    allModels.Add(new Point(item.X, item.Y), 1);
                }
                cube.Transform = new TranslateTransform3D(offsetX - (cubeSize / 2), offsetY - (cubeSize / 2), offsetZ);
                models.Add(cube, $"Substation:\nID: {item.Id}\nName: {item.Name}");

                if (!modelClasses.ContainsKey(item.Id))
                {
                    modelClasses.Add(item.Id, new EntityModel(EntityType.Substations, cube));
                }
                if (!numberOfLines.ContainsKey(item.Id))
                {
                    numberOfLines.Add(item.Id, 0);
                }

                AllModelsGroup.Children.Add(cube);
            }
        }
        private void DrawNodes()
        {
            foreach (var item in geographicModel.Nodes)
            {
                double offsetX = mapSquare + (((item.X - geographicModel.mapLeft) / geographicModel.intervalX) * geographicModel.mapEdgeSize);
                double offsetY = mapSquare + (((item.Y - geographicModel.mapBottom) / geographicModel.intervalY) * geographicModel.mapEdgeSize);
                double offsetZ = 0;
                var cube = GetCube(Brushes.Blue);
                if (CheckSameSpot(item.X, item.Y, out Point point))
                {
                    offsetX = mapSquare + (((point.X - geographicModel.mapLeft) / geographicModel.intervalX) * geographicModel.mapEdgeSize);
                    offsetY = mapSquare + (((point.Y - geographicModel.mapBottom) / geographicModel.intervalY) * geographicModel.mapEdgeSize);
                    offsetZ = step + allModels[point] * step;
                    allModels[point]++;
                }
                else
                {
                    offsetZ = step;
                    allModels.Add(new Point(item.X, item.Y), 1);
                }
                cube.Transform = new TranslateTransform3D(offsetX - (cubeSize / 2), offsetY - (cubeSize / 2), offsetZ);
                models.Add(cube, $"Node:\nID: {item.Id}\nName: {item.Name}");

                if (!modelClasses.ContainsKey(item.Id))
                {
                    modelClasses.Add(item.Id, new EntityModel(EntityType.Nodes, cube));
                }
                if (!numberOfLines.ContainsKey(item.Id))
                {
                    numberOfLines.Add(item.Id, 0);
                }

                AllModelsGroup.Children.Add(cube);
            }

        }
        private void DrawSwitches()
        {
            foreach (var item in geographicModel.Switches)
            {
                double offsetX = mapSquare + (((item.X - geographicModel.mapLeft) / geographicModel.intervalX) * geographicModel.mapEdgeSize);
                double offsetY = mapSquare + (((item.Y - geographicModel.mapBottom) / geographicModel.intervalY) * geographicModel.mapEdgeSize);
                double offsetZ = 0;
                var cube = GetCube(Brushes.Black);
                if (CheckSameSpot(item.X, item.Y, out Point point))
                {
                    offsetX = mapSquare + (((point.X - geographicModel.mapLeft) / geographicModel.intervalX) * geographicModel.mapEdgeSize);
                    offsetY = mapSquare + (((point.Y - geographicModel.mapBottom) / geographicModel.intervalY) * geographicModel.mapEdgeSize);
                    offsetZ = step + allModels[point] * step;
                    allModels[point]++;
                }
                else
                {
                    offsetZ = step;
                    allModels.Add(new Point(item.X, item.Y), 1);
                }
                cube.Transform = new TranslateTransform3D(offsetX - (cubeSize / 2), offsetY - (cubeSize / 2), offsetZ);
                models.Add(cube, $"Switch:\nID: {item.Id}\nName: {item.Name}");

                if (!modelClasses.ContainsKey(item.Id))
                {
                    modelClasses.Add(item.Id, new EntityModel(EntityType.Switches, cube));
                }
                if (!numberOfLines.ContainsKey(item.Id))
                {
                    numberOfLines.Add(item.Id, 0);
                }
                switchEntityStatuses.Add(item.Id, item.Status);
                AllModelsGroup.Children.Add(cube);
            }

        }
        private void DrawLines()
        {
            Point p1 = new Point(0, 0);
            Point p2 = new Point(0, 0);

            int[] triangleIndices = new int[] { 0, 1, 2, 0, 2, 3 };
            foreach (var line in geographicModel.Lines)
            {
                bool next = false;
                foreach (var item in line.Vertices)
                {
                    if (!geographicModel.IsValidPowerEntity(item.X, item.Y))
                    {
                        next = true;
                    }
                }
                if (next == false)
                {
                    lineEntityModels.Add(line.Id, new List<GeometryModel3D>());
                    lineEntityResistances.Add(line.Id, line.R);
                    for (int i = 0; i < line.Vertices.Count - 1; i++)
                    {
                        double firstVertexLong = line.Vertices[i].X;
                        double firstVertexLat = line.Vertices[i].Y;


                        p1.X = mapSquare + (((firstVertexLong - geographicModel.mapLeft) / geographicModel.intervalX) * geographicModel.mapEdgeSize);
                        p1.Y = mapSquare + (((firstVertexLat - geographicModel.mapBottom) / geographicModel.intervalY) * geographicModel.mapEdgeSize);

                        double secondVertexLong = line.Vertices[i + 1].X;
                        double secondVertexLat = line.Vertices[i + 1].Y;


                        p2.X = mapSquare + (((secondVertexLong - geographicModel.mapLeft) / geographicModel.intervalX) * geographicModel.mapEdgeSize);
                        p2.Y = mapSquare + (((secondVertexLat - geographicModel.mapBottom) / geographicModel.intervalY) * geographicModel.mapEdgeSize);

                        Point3D a1 = new Point3D(p1.X, p1.Y, 0.00005);
                        Point3D b1 = new Point3D(p2.X, p2.Y, 0.00005);



                        Vector3D diffVector = b1 - a1;
                        Vector3D nVector = Vector3D.CrossProduct(diffVector, new Vector3D(0, 0, 1));
                        nVector = Vector3D.Divide(nVector, nVector.Length);
                        nVector = Vector3D.Multiply(nVector, 0.0005);

                        Point3D firstPoint = a1 + nVector;
                        Point3D secondPoint = a1 - nVector;
                        Point3D thirdPoint = b1 + nVector;
                        Point3D fourthPoint = b1 - nVector;

                        Point3D[] positions = new Point3D[] { secondPoint, firstPoint, thirdPoint, fourthPoint };

                        GeometryModel3D lineGeometry = new GeometryModel3D();
                        MeshGeometry3D meshGeometry = new MeshGeometry3D();
                        DiffuseMaterial material = new DiffuseMaterial();
                        // oboj vodove
                        if (line.ConductorMaterial == "Acsr") // aluminijum
                        {
                            material.Brush = Brushes.Silver;
                        }
                        else if (line.ConductorMaterial == "Copper") // bakar
                        {
                            material.Brush = Brushes.DarkRed;
                        }
                        else if (line.ConductorMaterial == "Steel") // celik
                        {
                            material.Brush = Brushes.Purple;
                        }
                        else if (line.ConductorMaterial == "Other") // ostale
                        {
                            material.Brush = Brushes.Blue;
                        }
                        else                                        // default
                        {
                            material.Brush = Brushes.Green;
                        }


                        //lineGeometry = MakePartOfLine(p1, p2, 0,  0.5, material);

                        lineGeometry.Material = material;
                        meshGeometry.Positions = new Point3DCollection(positions);
                        meshGeometry.TriangleIndices = new Int32Collection(triangleIndices);
                        lineGeometry.Geometry = meshGeometry;
                        lineGeometry.Transform = new TranslateTransform3D(0, 0, step);
                        models.Add(lineGeometry, $"Line\nId:{line.Id}\nName:{line.Name}\nFirstEnd:{line.FirstEnd}\nSecondEnd:{line.SecondEnd}");
                        lineEntityModels[line.Id].Add(lineGeometry);
                        AllModelsGroup.Children.Add(lineGeometry);

                        foreach (var item in switchEntityStatuses)
                        {
                            if (item.Value == "Open")
                            {
                                if (item.Key == line.FirstEnd || item.Key == line.SecondEnd)
                                {

                                    iDsOfLinesThatStartFromSwitch.Add(line.Id);
                                }
                            }
                        }
                    }
                    if (numberOfLines.ContainsKey(line.FirstEnd))
                    {
                        numberOfLines[line.FirstEnd]++;
                    }
                    else
                    {
                        numberOfLines.Add(line.FirstEnd, 1);
                    }
                    if (numberOfLines.ContainsKey(line.SecondEnd))
                    {
                        numberOfLines[line.SecondEnd]++;
                    }
                    else
                    {
                        numberOfLines.Add(line.SecondEnd, 1);
                    }
                }
            }
        }
        private void DrawAll()
        {
            DrawSubstations();
            DrawNodes();
            DrawSwitches();
            DrawLines();
        }
        #endregion

        #region ViewPortMethods
        private void ViewPort_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            currToolTip.turnOff();
            viewPort.CaptureMouse();
            start = e.GetPosition(this);
            diffOffset.X = translation.OffsetX;
            diffOffset.Y = translation.OffsetY;

            System.Windows.Point mousePosition = e.GetPosition(viewPort);
            currentMouseHit = mousePosition;
            Point3D testpoint3D = new Point3D(mousePosition.X, mousePosition.Y, 0);
            Vector3D testdirection = new Vector3D(mousePosition.X, mousePosition.Y, 10);

            PointHitTestParameters pointparams = new PointHitTestParameters(currentMouseHit);
            RayHitTestParameters rayparams = new RayHitTestParameters(testpoint3D, testdirection);
            VisualTreeHelper.HitTest(viewPort, null, HTResult, pointparams);

        }
        private void ViewPort_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            viewPort.ReleaseMouseCapture();
        }
        private void ViewPort_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point currentPosition = e.GetPosition(this);
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                currToolTip.turnOff();
                double offsetX = currentPosition.X - startPosition.X;
                double offsetY = currentPosition.Y - startPosition.Y;
                double step = 0.2;
                if ((axisAngleRotationX.Angle + (step * offsetY)) < 70 && (axisAngleRotationX.Angle + (step * offsetY)) > -70)
                    axisAngleRotationX.Angle += step * offsetY;
                if ((axisAngleRotationY.Angle + (step * offsetX)) < 70 && (axisAngleRotationY.Angle + (step * offsetX)) > -70)
                    axisAngleRotationY.Angle += step * offsetX;
            }

            startPosition = currentPosition;

            if (viewPort.IsMouseCaptured)
            {
                System.Windows.Point end = e.GetPosition(this);
                double offsetX = end.X - start.X;
                double offsetY = end.Y - start.Y;
                double w = this.Width;
                double h = this.Height;
                double translateX = (offsetX * 100) / w;
                double translateY = -(offsetY * 100) / h;
                translation.OffsetX = diffOffset.X + (translateX / (100 * scaling.ScaleX));
                translation.OffsetY = diffOffset.Y + (translateY / (100 * scaling.ScaleX));
            }
        }
        private void ViewPort_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0 && zoomCurrent < zoomMax)
            {
                zoomCurrent++;
                Camera.FieldOfView--;
            }
            else if (e.Delta <= 0 && zoomCurrent > -zoomMax)
            {
                zoomCurrent--;
                Camera.FieldOfView++;
            }
        }
        private void ViewPort_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                startPosition = e.GetPosition(this);
            }
        }
        #endregion

        #region ButtonClickedMethods
        private void hsbutton_Click(object sender, RoutedEventArgs e)
        {
            if (hsbuttonClicked == false)
            {
                foreach (var item in lineEntityResistances)
                {
                    if (item.Value >= 0 && item.Value < 1)
                    {
                        foreach (var model in lineEntityModels[item.Key])
                        {
                            AllModelsGroup.Children.Remove(model);
                        }
                    }
                }
                hsbuttonClicked = true;
                HideEntities();
            }
            else
            {
                foreach (var item in lineEntityResistances)
                {
                    if (item.Value >= 0 && item.Value < 1)
                    {
                        foreach (var model in lineEntityModels[item.Key])
                        {
                            AllModelsGroup.Children.Add(model);
                        }
                    }
                }

                hsbuttonClicked = false;
                HideEntities();
            }
        }
        private void hsbutton2_Click(object sender, RoutedEventArgs e)
        {
            if (hsbutton2Clicked == false)
            {
                foreach (var item in lineEntityResistances)
                {
                    if (item.Value >= 1 && item.Value <= 2)
                    {

                        foreach (var model in lineEntityModels[item.Key])
                        {
                            AllModelsGroup.Children.Remove(model);
                        }
                    }
                }
                hsbutton2Clicked = true;
                HideEntities();

            }
            else
            {
                foreach (var item in lineEntityResistances)
                {
                    if (item.Value >= 1 && item.Value <= 2)
                    {
                        foreach (var model in lineEntityModels[item.Key])
                        {
                            AllModelsGroup.Children.Add(model);
                        }
                    }
                }
                hsbutton2Clicked = false;
                HideEntities();
            }
        }
        private void hsbutton3_Click(object sender, RoutedEventArgs e)
        {
            if (hsbutton3Clicked == false)
            {
                foreach (var item in lineEntityResistances)
                {
                    if (item.Value > 2)
                    {
                        foreach (var model in lineEntityModels[item.Key])
                        {
                            AllModelsGroup.Children.Remove(model);
                        }
                    }
                }
                hsbutton3Clicked = true;
                HideEntities();
            }
            else
            {
                foreach (var item in lineEntityResistances)
                {
                    if (item.Value > 2)
                    {
                        foreach (var model in lineEntityModels[item.Key])
                        {
                            AllModelsGroup.Children.Add(model);
                        }
                    }
                }
                hsbutton3Clicked = false;
                HideEntities();
            }
        }
        private void ClickHide_0_3(object sender, RoutedEventArgs e)
        {
            if (isClicked == false)
            {
                foreach (var item in numberOfLines)
                {
                    if (item.Value >= 0 && item.Value < 3)
                    {
                        AllModelsGroup.Children.Remove(modelClasses[item.Key].Model);
                    }
                }
                isClicked = true;
                HideEntities();
            }
            else
            {
                foreach (var item in numberOfLines)
                {
                    if (item.Value >= 0 && item.Value < 3)
                    {
                        AllModelsGroup.Children.Add(modelClasses[item.Key].Model);
                    }
                }

                isClicked = false;
                HideEntities();
            }
        }
        private void ClickHide_3_5(object sender, RoutedEventArgs e)
        {
            if (isClicked2 == false)
            {
                foreach (var item in numberOfLines)
                {
                    if (item.Value >= 3 && item.Value <= 5)
                    {
                        AllModelsGroup.Children.Remove(modelClasses[item.Key].Model);
                    }
                }
                isClicked2 = true;
                HideEntities();

            }
            else
            {
                foreach (var item in numberOfLines)
                {
                    if (item.Value >= 3 && item.Value <= 5)
                    {
                        AllModelsGroup.Children.Add(modelClasses[item.Key].Model);
                    }
                }
                isClicked2 = false;
                HideEntities();
            }
        }
        private void ClickHide_5(object sender, RoutedEventArgs e)
        {
            if (isClicked3 == false)
            {
                foreach (var item in numberOfLines)
                {
                    if (item.Value > 5)
                    {
                        AllModelsGroup.Children.Remove(modelClasses[item.Key].Model);
                    }
                }
                isClicked3 = true;
                HideEntities();
            }
            else
            {
                foreach (var item in numberOfLines)
                {
                    if (item.Value > 5)
                    {
                        AllModelsGroup.Children.Add(modelClasses[item.Key].Model);
                    }
                }
                isClicked3 = false;
                HideEntities();
            }
        }
        private void ClickActive(object sender, RoutedEventArgs e)
        {
            if (isActiveClicked == false)
            {

                foreach (var line in iDsOfLinesThatStartFromSwitch)
                {
                    foreach (var model in lineEntityModels[line])
                    {
                        AllModelsGroup.Children.Remove(model);
                    }
                }

                isActiveClicked = true;
                HideEntities();
            }
            else
            {

                foreach (var line in iDsOfLinesThatStartFromSwitch)
                {
                    foreach (var model in lineEntityModels[line])
                    {
                        AllModelsGroup.Children.Add(model);
                    }
                }

                isActiveClicked = false;
                HideEntities();
            }
        }
        private void lmbutton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in models)
            {
                AllModelsGroup.Children.Remove(item.Key);
            }
            InitializeFields();
            HideEntities();
            DrawAll();
        }
        private void HideEntities()
        {
            if (!hsbuttonClicked)
            {
                hsbutton.Content = "Hide[0-1]";
            }
            else
            {
                hsbutton.Content = "Show[0-1]";
            }
            if (!hsbutton2Clicked)
            {
                hsbutton2.Content = "Hide[1-2]";
            }
            else
            {
                hsbutton2.Content = "Show[1-2]";
            }
            if (!hsbutton3Clicked)
            {
                hsbutton3.Content = "Hide[> 2]";
            }
            else
            {
                hsbutton3.Content = "Show[> 2]";
            }
            if (!isClicked)
            {
                hide03.Content = "Hide 0-3";
            }
            else
            {
                hide03.Content = "Show 0-3";
            }
            if (!isClicked2)
            {
                hide35.Content = "Hide 3-5";
            }
            else
            {
                hide35.Content = "Show 3-5";
            }
            if (!isClicked3)
            {
                hide5.Content = "Hide >5";
            }
            else
            {
                hide5.Content = "Show >5";
            }
            if (!isActiveClicked)
            {
                hideActive.Content = "Hide active";
            }
            else
            {
                hideActive.Content = "Show active";
            }
        }
        private void resetView_Click(object sender, RoutedEventArgs e)
        {
            //postavljanje kamere
            Camera.FieldOfView = 45;
            //zumiranje
            zoomCurrent = 0;
            //transliranje
            translation.OffsetX = translation.OffsetY = translation.OffsetZ = 0;
            //skaliranje
            scaling.ScaleX = scaling.ScaleY = scaling.ScaleZ = 1;
            //rotiranje
            rotationX.CenterX = rotationX.CenterY = rotationX.CenterZ = 0;
            rotationY.CenterX = rotationY.CenterY = rotationY.CenterZ = 0;
            axisAngleRotationX.Angle = -55;
            axisAngleRotationY.Angle = 0;
        }
        #endregion

        #region OtherFunctions
        private bool CheckSameSpot(double x, double y, out Point point)
        {
            if (allModels.ContainsKey(new Point(x, y)))
            {
                Console.WriteLine("");
            }

            point = new Point();
            foreach (var item in allModels)
            {
                if (CalculateDistance(y, x, item.Key.Y, item.Key.X) <= 0.02)
                {
                    point = item.Key;
                    return true;
                }
            }
            return false;
        }
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double theta = lon1 - lon2;

            double radTheta = theta * Math.PI / 180.0;
            double radLat1 = lat1 * Math.PI / 180.0;
            double radLat2 = lat2 * Math.PI / 180.0;

            double dist = Math.Sin(radLat1) * Math.Sin(radLat2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Cos(radTheta);
            dist = Math.Acos(dist);
            dist = (dist / Math.PI * 180.0);
            dist = dist * 60 * 1.1515;
            dist = dist * 1.609344;
            return dist;
        }
        private HitTestResultBehavior HTResult(HitTestResult result)
        {
            RayHitTestResult rayResult = result as RayHitTestResult;

            if (rayResult != null)
            {
                foreach (KeyValuePair<GeometryModel3D, string> pair in models)
                {
                    if (pair.Key == rayResult.ModelHit)
                    {

                        currToolTip.turnOn(pair.Value);
                        if (pair.Value.ToLower().Contains("line"))
                        {
                            var id = ulong.Parse(pair.Value.Split(':', '\n')[2]);
                            ColorLinesEntities(id);
                        }
                    }
                }
            }
            return HitTestResultBehavior.Stop;
        }
        private void ColorLinesEntities(ulong id)
        {
            var dto = geographicModel.Lines.SingleOrDefault(x => x.Id == id);
            if (dto != null)
            {
                resetColors();
                var cube1 = modelClasses[dto.FirstEnd];
                DiffuseMaterial diffMat1 = new DiffuseMaterial();
                diffMat1.Brush = Brushes.DarkGoldenrod;
                cube1.Model.Material = diffMat1;
                var cube2 = modelClasses[dto.SecondEnd]; ;
                DiffuseMaterial diffMat2 = new DiffuseMaterial();
                diffMat2.Brush = Brushes.DarkGoldenrod;
                cube2.Model.Material = diffMat2;
            }
        }
        private void resetColors()
        {
            foreach (var item in modelClasses)
            {
                var diffMat = new DiffuseMaterial();
                if (item.Value.Type == EntityType.Switches)
                {
                    diffMat.Brush = Brushes.Black;
                }
                else if (item.Value.Type == EntityType.Nodes)
                {
                    diffMat.Brush = Brushes.Blue;
                }
                else if (item.Value.Type == EntityType.Substations)
                {
                    diffMat.Brush = Brushes.Red;
                }

                item.Value.Model.Material = diffMat;
            }
        }
        public GeometryModel3D GetCube(Brush brush)
        {
            var geoMod3d = new GeometryModel3D();
            var meshGeo3d = new MeshGeometry3D();
            meshGeo3d.Positions.Add(new Point3D(0, 0, 0));
            meshGeo3d.Positions.Add(new Point3D(cubeSize, 0, 0));
            meshGeo3d.Positions.Add(new Point3D(0, cubeSize, 0));
            meshGeo3d.Positions.Add(new Point3D(cubeSize, cubeSize, 0));
            meshGeo3d.Positions.Add(new Point3D(0, 0, cubeSize));
            meshGeo3d.Positions.Add(new Point3D(cubeSize, 0, cubeSize));
            meshGeo3d.Positions.Add(new Point3D(0, cubeSize, cubeSize));
            meshGeo3d.Positions.Add(new Point3D(cubeSize, cubeSize, cubeSize));

            meshGeo3d.TriangleIndices.Add(2);
            meshGeo3d.TriangleIndices.Add(3);
            meshGeo3d.TriangleIndices.Add(1);

            meshGeo3d.TriangleIndices.Add(2);
            meshGeo3d.TriangleIndices.Add(1);
            meshGeo3d.TriangleIndices.Add(0);

            meshGeo3d.TriangleIndices.Add(7);
            meshGeo3d.TriangleIndices.Add(1);
            meshGeo3d.TriangleIndices.Add(3);

            meshGeo3d.TriangleIndices.Add(7);
            meshGeo3d.TriangleIndices.Add(5);
            meshGeo3d.TriangleIndices.Add(1);

            meshGeo3d.TriangleIndices.Add(6);
            meshGeo3d.TriangleIndices.Add(5);
            meshGeo3d.TriangleIndices.Add(7);

            meshGeo3d.TriangleIndices.Add(6);
            meshGeo3d.TriangleIndices.Add(4);
            meshGeo3d.TriangleIndices.Add(5);

            meshGeo3d.TriangleIndices.Add(6);
            meshGeo3d.TriangleIndices.Add(2);
            meshGeo3d.TriangleIndices.Add(4);

            meshGeo3d.TriangleIndices.Add(2);
            meshGeo3d.TriangleIndices.Add(0);
            meshGeo3d.TriangleIndices.Add(4);

            meshGeo3d.TriangleIndices.Add(2);
            meshGeo3d.TriangleIndices.Add(7);
            meshGeo3d.TriangleIndices.Add(3);

            meshGeo3d.TriangleIndices.Add(2);
            meshGeo3d.TriangleIndices.Add(6);
            meshGeo3d.TriangleIndices.Add(7);

            meshGeo3d.TriangleIndices.Add(0);
            meshGeo3d.TriangleIndices.Add(1);
            meshGeo3d.TriangleIndices.Add(5);

            meshGeo3d.TriangleIndices.Add(0);
            meshGeo3d.TriangleIndices.Add(5);
            meshGeo3d.TriangleIndices.Add(4);
            geoMod3d.Geometry = meshGeo3d;

            var diffMat = new DiffuseMaterial(brush);
            geoMod3d.Material = diffMat;
            return geoMod3d;
        }

        #endregion

    }
}

//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BodyBasics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Radius of drawn hand circles
        /// </summary>
        private const double HandSize = 30;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 5;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Constant for clamping Z values of camera space points from being negative
        /// </summary>
        private const float InferredZPositionClamp = 0.1f;

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as closed
        /// </summary>
        private readonly Brush handClosedBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as opened
        /// </summary>
        private readonly Brush handOpenBrush = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as in lasso (pointer) position
        /// </summary>
        private readonly Brush handLassoBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Drawing group for body rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor = null;

        /// <summary>
        /// Coordinate mapper to map one type of point to another
        /// </summary>
        private CoordinateMapper coordinateMapper = null;

        /// <summary>
        /// Reader for body frames
        /// </summary>
        private BodyFrameReader bodyFrameReader = null;

        /// <summary>
        /// Array for the bodies
        /// </summary>
        private Body[] bodies = null;

        /// <summary>
        /// definition of bones
        /// </summary>
        private List<Tuple<JointType, JointType>> bones;

        /// <summary>
        /// Width of display (depth space)
        /// </summary>
        private int displayWidth;

        /// <summary>
        /// Height of display (depth space)
        /// </summary>
        private int displayHeight;

        /// <summary>
        /// List of colors for each body tracked
        /// </summary>
        private List<Pen> bodyColors;

        /// <summary>
        /// Current status text to display
        /// </summary>
        private string statusText = null;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            // one sensor is currently supported
            this.kinectSensor = KinectSensor.GetDefault();

            // get the coordinate mapper
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            // get the depth (display) extents
            FrameDescription frameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;

            // get size of joint space
            this.displayWidth = frameDescription.Width;
            this.displayHeight = frameDescription.Height;

            // open the reader for the body frames
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            // a bone defined as a line between two joints
            this.bones = new List<Tuple<JointType, JointType>>();

            // Torso
            //this.bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
            //this.bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
            //this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
            //this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));

            // Right Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
            //this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
            //this.bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
            //this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));

            // Left Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
            //this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
            //this.bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
            //this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));
            /*
            // Right Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));

            // Left Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft));
            */
            // populate body colors, one for each BodyIndex
            this.bodyColors = new List<Pen>();

            /*
            this.bodyColors.Add(new Pen(Brushes.Red, 6));
            this.bodyColors.Add(new Pen(Brushes.Orange, 6));
            this.bodyColors.Add(new Pen(Brushes.Green, 6));
            this.bodyColors.Add(new Pen(Brushes.Blue, 6));
            this.bodyColors.Add(new Pen(Brushes.Indigo, 6));*/
            this.bodyColors.Add(new Pen(Brushes.Violet, 6));

            // set IsAvailableChanged event notifier
            this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;

            // open the sensor
            this.kinectSensor.Open();

            // set the status text
            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.NoSensorStatusText;

            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // use the window object as the view model in this simple example
            this.DataContext = this;

            // initialize the components (controls) of the window
            this.InitializeComponent();

            startTime = DateTime.Now.Ticks;
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                return this.imageSource;
            }
        }

        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;

                    // notify any bound elements that the text has changed
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }

        /// <summary>
        /// Execute start up tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.FrameArrived += this.Reader_FrameArrived;
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        CameraSpacePoint s2eRP = new CameraSpacePoint();
        CameraSpacePoint eRVP = new CameraSpacePoint();
        CameraSpacePoint e2wRP = new CameraSpacePoint();
        CameraSpacePoint wRVP = new CameraSpacePoint();

        CameraSpacePoint s2eLP = new CameraSpacePoint();
        CameraSpacePoint eLVP = new CameraSpacePoint();
        CameraSpacePoint e2wLP = new CameraSpacePoint();
        CameraSpacePoint wLVP = new CameraSpacePoint();

        CameraSpacePoint eRVRA = new CameraSpacePoint();
        CameraSpacePoint wRVRA = new CameraSpacePoint();
        CameraSpacePoint eLVRA = new CameraSpacePoint();
        CameraSpacePoint wLVRA = new CameraSpacePoint();

        CameraSpacePoint eRARA = new CameraSpacePoint();
        CameraSpacePoint wRARA = new CameraSpacePoint();
        CameraSpacePoint eLARA = new CameraSpacePoint();
        CameraSpacePoint wLARA = new CameraSpacePoint();

        List<string> log = new List<string>();

        float runningAvgCoef = 4f;

        long startTime = 0;

        /// <summary>
        /// Handles the body frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                using (DrawingContext dc = this.drawingGroup.Open())
                {
                    // Draw a transparent background to set the render size
                    dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));

                    //int penIndex = 0;
                    foreach (Body body in this.bodies)
                    {
                        Pen drawPen = this.bodyColors[0];

                        if (body.IsTracked)
                        {
                            //this.DrawClippedEdges(body, dc);

                            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                            // convert the joint points to depth (display) space
                            Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                            foreach (JointType jointType in joints.Keys)
                            {
                                // sometimes the depth(Z) of an inferred joint may show as negative
                                // clamp down to 0.1f to prevent coordinatemapper from returning (-Infinity, -Infinity)
                                CameraSpacePoint position = joints[jointType].Position;
                                if (position.Z < 0)
                                {
                                    position.Z = InferredZPositionClamp;
                                }

                                DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(position);
                                jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                            }

                            this.DrawBody(joints, jointPoints, dc, drawPen);

                            Pen redPen = new Pen(Brushes.Red, 6);
                            Pen bluePen = new Pen(Brushes.Blue, 6);
                            Pen greenPen = new Pen(Brushes.Green, 6);

                            CameraSpacePoint wristR = joints[JointType.WristRight].Position;
                            CameraSpacePoint elbowR = joints[JointType.ElbowRight].Position;
                            CameraSpacePoint shoulderR = joints[JointType.ShoulderRight].Position;
                            CameraSpacePoint s2eR = SubVec(elbowR, shoulderR);
                            CameraSpacePoint e2wR = SubVec(wristR, elbowR);

                            CameraSpacePoint eRV = SubVec(s2eR, s2eRP);
                            CameraSpacePoint wRV = SubVec(e2wR, e2wRP);
                            
                            eRVRA = MulVec(AddVec(MulVec(eRVRA, runningAvgCoef - 1f), eRV), 1f/runningAvgCoef);
                            wRVRA = MulVec(AddVec(MulVec(wRVRA, runningAvgCoef - 1f), wRV), 1f/runningAvgCoef);

                            CameraSpacePoint eRA = SubVec(eRVRA, eRVP);
                            CameraSpacePoint wRA = SubVec(wRVRA, wRVP);

                            s2eRP = s2eR;
                            eRVP = eRVRA;
                            e2wRP = e2wR;
                            wRVP = wRVRA;

                            eRARA = MulVec(AddVec(MulVec(eRARA, runningAvgCoef - 1f), eRA), 1f / runningAvgCoef);
                            wRARA = MulVec(AddVec(MulVec(wRARA, runningAvgCoef - 1f), wRA), 1f / runningAvgCoef);


                            CameraSpacePoint wristL = joints[JointType.WristLeft].Position;
                            CameraSpacePoint elbowL = joints[JointType.ElbowLeft].Position;
                            CameraSpacePoint shoulderL = joints[JointType.ShoulderLeft].Position;
                            CameraSpacePoint s2eL = SubVec(elbowL, shoulderL);
                            CameraSpacePoint e2wL = SubVec(wristL, elbowL);

                            CameraSpacePoint eLV = SubVec(s2eL, s2eLP);
                            CameraSpacePoint wLV = SubVec(e2wL, e2wLP);
                            

                            eLVRA = MulVec(AddVec(MulVec(eLVRA, runningAvgCoef - 1f), eLV), 1f/runningAvgCoef);
                            wLVRA = MulVec(AddVec(MulVec(wLVRA, runningAvgCoef - 1f), wLV), 1f/runningAvgCoef);

                            CameraSpacePoint eLA = SubVec(eLVRA, eLVP);
                            CameraSpacePoint wLA = SubVec(wLVRA, wLVP);

                            s2eLP = s2eL;
                            eLVP = eLVRA;
                            e2wLP = e2wL;
                            wLVP = wLVRA;

                            eLARA = MulVec(AddVec(MulVec(eLARA, runningAvgCoef - 1f), eRA), 1f / runningAvgCoef);
                            wLARA = MulVec(AddVec(MulVec(wLARA, runningAvgCoef - 1f), wRA), 1f / runningAvgCoef);

                            CameraSpacePoint spineBase = joints[JointType.SpineShoulder].Position;
                            CameraSpacePoint spineEnd = joints[JointType.SpineBase].Position;
                            CameraSpacePoint b2sL = SubVec(shoulderL, spineBase);
                            CameraSpacePoint b2sR = SubVec(shoulderR, spineBase);
                            CameraSpacePoint b2e = SubVec(spineEnd, spineBase);


                            this.DrawVector(convertPoint(shoulderR), convertPoint(AddVec(shoulderR, s2eR)), dc, redPen);
                            this.DrawVector(convertPoint(elbowR), convertPoint(AddVec(elbowR, MulVec(eRVRA, 10))), dc, greenPen);
                            this.DrawVector(convertPoint(elbowR), convertPoint(AddVec(elbowR, MulVec(eRARA, 50))), dc, bluePen);

                            this.DrawVector(convertPoint(elbowR), convertPoint(AddVec(elbowR, e2wR)), dc, redPen);
                            this.DrawVector(convertPoint(wristR), convertPoint(AddVec(wristR, MulVec(wRVRA, 10))), dc, greenPen);
                            this.DrawVector(convertPoint(wristR), convertPoint(AddVec(wristR, MulVec(wRARA, 50))), dc, bluePen);


                            this.DrawVector(convertPoint(shoulderL), convertPoint(AddVec(shoulderL, s2eL)), dc, redPen);
                            this.DrawVector(convertPoint(elbowL), convertPoint(AddVec(elbowL, MulVec(eLVRA, 10))), dc, greenPen);
                            this.DrawVector(convertPoint(elbowL), convertPoint(AddVec(elbowL, MulVec(eLARA, 50))), dc, bluePen);

                            this.DrawVector(convertPoint(elbowL), convertPoint(AddVec(elbowL, e2wL)), dc, redPen);
                            this.DrawVector(convertPoint(wristL), convertPoint(AddVec(wristL, MulVec(wLVRA, 10))), dc, greenPen);
                            this.DrawVector(convertPoint(wristL), convertPoint(AddVec(wristL, MulVec(wLARA, 50))), dc, bluePen);

                            long ellapsedTime = (DateTime.Now.Ticks - startTime) / 100000;

                            if (RecordCheckBox.IsChecked.Value)
                            {
                                log.Add(ellapsedTime.ToString() + flattenPoints(b2sR, b2sL, b2e) + flattenPoints(s2eR, eRVRA, eRARA) + flattenPoints(e2wR, wRVRA, wRARA) + flattenPoints(s2eL, eLVRA, eLARA) + flattenPoints(e2wL, wLVRA, wLARA));
                            }

                            

                            //this.DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
                            //this.DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);
                        }
                    }

                    // prevent drawing outside of our render area
                    this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                }
            }
        }

        string flattenPoints(CameraSpacePoint a, CameraSpacePoint b, CameraSpacePoint c)
        {
            return string.Format("\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}", a.X, a.Y, a.Z, b.X, b.Y, b.Z, c.X, c.Y, c.Z);
        }

        Point convertPoint(CameraSpacePoint p)
        {
            
            if (p.Z < 0)
            {
                p.Z = InferredZPositionClamp;
            }

            DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(p);
            return new Point(depthSpacePoint.X, depthSpacePoint.Y);
        }

        CameraSpacePoint SubVec(CameraSpacePoint a, CameraSpacePoint b)
        {
            CameraSpacePoint rel = new CameraSpacePoint();
            rel.X = a.X - b.X;
            rel.Y = a.Y - b.Y;
            rel.Z = a.Z - b.Z;
            return rel;
        }
        CameraSpacePoint AddVec(CameraSpacePoint a, CameraSpacePoint b)
        {
            CameraSpacePoint rel = new CameraSpacePoint();
            rel.X = a.X + b.X;
            rel.Y = a.Y + b.Y;
            rel.Z = a.Z + b.Z;
            return rel;
        }
        CameraSpacePoint MulVec(CameraSpacePoint a, float b)
        {
            CameraSpacePoint rel = new CameraSpacePoint();
            rel.X = a.X * b;
            rel.Y = a.Y * b;
            rel.Z = a.Z * b;
            return rel;
        }

        /// <summary>
        /// Draws a body
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="drawingPen">specifies color to draw a specific body</param>
        private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext, Pen drawingPen)
        {
            // Draw the bones
            foreach (var bone in this.bones)
            {
                this.DrawBone(joints, jointPoints, bone.Item1, bone.Item2, drawingContext, drawingPen);
            }

            JointType[] trackedJoints = new JointType[]{
                JointType.WristLeft, JointType.WristRight, JointType.ElbowLeft, JointType.ElbowRight, JointType.ShoulderLeft, JointType.ShoulderRight, JointType.SpineShoulder
            };

            // Draw the joints
            foreach (JointType jointType in trackedJoints)
            {
                Brush drawBrush = null;

                TrackingState trackingState = joints[jointType].TrackingState;

                if (trackingState == TrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
                }
            }
        }

        /// <summary>
        /// Draws one bone of a body (joint to joint)
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="jointType0">first joint of bone to draw</param>
        /// <param name="jointType1">second joint of bone to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// /// <param name="drawingPen">specifies color to draw a specific bone</param>
        private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, Pen drawingPen)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
            {
                drawPen = drawingPen;
            }

            drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
        }

        private void DrawVector(Point a, Point b, DrawingContext drawingContext, Pen drawingPen)
        {
            drawingContext.DrawLine(drawingPen, a, b);
        }

        /// <summary>
        /// Draws a hand symbol if the hand is tracked: red circle = closed, green circle = opened; blue circle = lasso
        /// </summary>
        /// <param name="handState">state of the hand</param>
        /// <param name="handPosition">position of the hand</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawHand(HandState handState, Point handPosition, DrawingContext drawingContext)
        {
            switch (handState)
            {
                case HandState.Closed:
                    drawingContext.DrawEllipse(this.handClosedBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Open:
                    drawingContext.DrawEllipse(this.handOpenBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Lasso:
                    drawingContext.DrawEllipse(this.handLassoBrush, null, handPosition, HandSize, HandSize);
                    break;
            }
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping body data
        /// </summary>
        /// <param name="body">body to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawClippedEdges(Body body, DrawingContext drawingContext)
        {
            FrameEdges clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, this.displayHeight - ClipBoundsThickness, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, this.displayHeight));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(this.displayWidth - ClipBoundsThickness, 0, ClipBoundsThickness, this.displayHeight));
            }
        }

        /// <summary>
        /// Handles the event which the sensor becomes unavailable (E.g. paused, closed, unplugged).
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // on failure, set the status text
            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.SensorNotAvailableStatusText;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string filename = FileNameBox.Text + ".txt";
            File.WriteAllLines(filename, log.ToArray());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            log = new List<string>();
            startTime = DateTime.Now.Ticks;   
        }

        private void FileNameBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}

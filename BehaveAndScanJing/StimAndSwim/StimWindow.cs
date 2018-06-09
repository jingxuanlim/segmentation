using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.IO;
using System.Threading;

namespace BehaveAndScanSPIM
{
    public partial class StimWindow : Form
    {

        float ww = 4f;//1f;//4f;  // size of landscape in x,y
        float hh = 3f;//2f;
        int repx = 3;   // periodicity in x,y  ((if any))
        int repy = 3;
        float h_ = 1f;//1f*1f;         // size of visible area
        float w_ = 4f/3f;//1f*4f / 3f;
        float yPosition, xPosition, orientation;
        StimEphysOscilloscopeControl stimSender = null;
        public StimCheckWindow SCWindow;

        private Microsoft.DirectX.Direct3D.Device deviceP = null;
        private Microsoft.DirectX.Direct3D.Device deviceW = null;
        private VertexBuffer vertices1 = null;       // Vertex buffer for our drawing
        private VertexBuffer vertices2 = null;       // Vertex buffer for our drawing

        // Background texture management
        private Texture backgroundT1 = null;         // Background texture map
        private Texture backgroundW1 = null;         // Background texture map
        private Texture backgroundT2 = null;         // Background texture map
        private Texture backgroundW2 = null;         // Background texture map
        private VertexBuffer backgroundV1 = null;
        private VertexBuffer backgroundV2 = null;    // Background vertex buffer

        byte backgroundcolor;

        public StimWindow()
        {
            InitializeComponent();
            if (!InitializeDirect3D())
                return;

            vertices1 = new VertexBuffer(typeof(CustomVertex.PositionColored), 20, deviceP, 0, CustomVertex.PositionColored.Format, Pool.Managed);
            vertices2   = new VertexBuffer(typeof(CustomVertex.PositionColored), 20, deviceW, 0, CustomVertex.PositionColored.Format, Pool.Managed);
            backgroundV1 = new VertexBuffer(typeof(CustomVertex.PositionColoredTextured), 4, deviceP, 0, CustomVertex.PositionColored.Format, Pool.Managed);
            backgroundV2 = new VertexBuffer(typeof(CustomVertex.PositionColoredTextured), 4, deviceW, 0, CustomVertex.PositionColored.Format, Pool.Managed);

            backgroundT1 = TextureLoader.FromFile(deviceP, "../../StimAndSwim/background_images/texture.bmp");
            backgroundT2 = TextureLoader.FromFile(deviceW, "../../StimAndSwim/background_images/texture.bmp");
            backgroundW1 = TextureLoader.FromFile(deviceP, "../../StimAndSwim/background_images/white.bmp");
            backgroundW2 = TextureLoader.FromFile(deviceW, "../../StimAndSwim/background_images/white.bmp");

            GraphicsStream stm1 = backgroundV1.Lock(0, 0, 0);     // Lock the background vertex list
            int clr1 = System.Drawing.Color.Transparent.ToArgb();
            stm1.Write(new CustomVertex.PositionColoredTextured(-ww / 3f, -hh / 3f, 0, clr1, 0, 1));   // here the size of the background
            stm1.Write(new CustomVertex.PositionColoredTextured(-ww / 3f, hh * 2f / 3f, 0, clr1, 0, 0));    // bmp is set, also the shape
            stm1.Write(new CustomVertex.PositionColoredTextured(ww * 2f / 3f, hh * 2f / 3f, 0, clr1, 1, 0));     // so needs to match with the bitmap file
            stm1.Write(new CustomVertex.PositionColoredTextured(ww * 2f / 3f, -hh / 3f, 0, clr1, 1, 1));

            backgroundV1.Unlock();

            GraphicsStream stm2 = backgroundV2.Lock(0, 0, 0);     // Lock the background vertex list
            int clr2 = System.Drawing.Color.Transparent.ToArgb();
            stm2.Write(new CustomVertex.PositionColoredTextured(-ww / 3f, -hh / 3f, 0, clr2, 0, 1));   // here the size of the background
            stm2.Write(new CustomVertex.PositionColoredTextured(-ww / 3f, hh * 2f / 3f, 0, clr2, 0, 0));    // bmp is set, also the shape
            stm2.Write(new CustomVertex.PositionColoredTextured(ww * 2f / 3f, hh * 2f / 3f, 0, clr2, 1, 0));     // so needs to match with the bitmap file
            stm2.Write(new CustomVertex.PositionColoredTextured(ww * 2f / 3f, -hh / 3f, 0, clr2, 1, 1));

            backgroundV2.Unlock();

        }

        private bool InitializeDirect3D()
        {
            SCWindow = new StimCheckWindow();
            SCWindow.Show();

            try
            {                
                PresentParameters presentParams = new PresentParameters();
                presentParams.Windowed = true;
                presentParams.SwapEffect = SwapEffect.Discard;
                deviceP = new Microsoft.DirectX.Direct3D.Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);
                deviceW = new Microsoft.DirectX.Direct3D.Device(0, DeviceType.Hardware, SCWindow, CreateFlags.SoftwareVertexProcessing, presentParams);      
            }
            catch (DirectXException)
            {
                return false;
            }
            return true;
        }

        public void Render(object sender)
        {
               stimSender = (StimEphysOscilloscopeControl)sender;
               DisplayClear(deviceP);
               DisplayClear(deviceW);
               SetDrawWorld(stimSender.stimState, stimSender.senderWindow.InstStimParams.moveobject, deviceP, backgroundV1, backgroundT1, backgroundW1);
               SetDrawWorld(stimSender.stimState, stimSender.senderWindow.InstStimParams.moveobject, deviceW, backgroundV2, backgroundT2, backgroundW2);

              
                
               if (stimSender.senderWindow.InstStimParams.moveobject == 1)   // display grating - more response probably
               {
                   DrawGrating((float)stimSender.displayAngle, stimSender.stripesY, (byte)stimSender.stim1DclosedLoopContrast, stimSender.blevel, stimSender.dH, deviceP, vertices1);
                   DrawGrating((float)stimSender.displayAngle, stimSender.stripesY, (byte)stimSender.stim1DclosedLoopContrast, stimSender.blevel, stimSender.dH, deviceW, vertices2); 
               }

               if (stimSender.senderWindow.InstStimParams.moveobject == 3)   // display grating - more response probably
               {
                   DrawCrosses((float)stimSender.displayAngle, stimSender.stripesY, (byte)stimSender.stim1DclosedLoopContrast, stimSender.blevel, stimSender.dH, deviceP, vertices1);
                   DrawCrosses((float)stimSender.displayAngle, stimSender.stripesY, (byte)stimSender.stim1DclosedLoopContrast, stimSender.blevel, stimSender.dH, deviceW, vertices2);
               }

               if (stimSender.senderWindow.InstStimParams.moveobject == 4)   // display grating - more response probably
               {
                   DrawBox((byte)stimSender.stim1DclosedLoopContrast, stimSender.blevel, stimSender.boxpos, stimSender.dH, deviceP, vertices1);
                   DrawBox((byte)stimSender.stim1DclosedLoopContrast, stimSender.blevel, stimSender.boxpos, stimSender.dH, deviceW, vertices2);
               }

               if (stimSender.senderWindow.InstStimParams.moveobject == 7)   // display grating + loom 
               {
                   DrawGrating((float)stimSender.displayAngle, stimSender.stripesY, (byte)stimSender.stim1DclosedLoopContrast, stimSender.blevel, stimSender.dH, deviceP, vertices1);
                   DrawGrating((float)stimSender.displayAngle, stimSender.stripesY, (byte)stimSender.stim1DclosedLoopContrast, stimSender.blevel, stimSender.dH, deviceW, vertices2);

                   float dot_sizeX = (float)(stimSender.senderWindow.InstStimParams.dotSizeX / 100);
                   float dot_sizeY = (float)(stimSender.senderWindow.InstStimParams.dotSizeY / 100);

                   float[] xx = { stimSender.xx_ - dot_sizeX, stimSender.xx_ + dot_sizeX, stimSender.xx_ + dot_sizeX, stimSender.xx_ - dot_sizeX };
                   float[] yy = { stimSender.yy_ - dot_sizeY, stimSender.yy_ - dot_sizeY, stimSender.yy_ + dot_sizeY, stimSender.yy_ + dot_sizeY };

                   stimSender.dotContrast = 0;

                   drawPoly(vertices1, deviceP, xx, yy, stimSender.dotContrast);
                   drawPoly(vertices2, deviceW, xx, yy, stimSender.dotContrast);
               }

               else if (stimSender.senderWindow.InstStimParams.moveobject == 6)   // swarm of enemies
               {
                   //                   DrawWhite((byte)stimSender.stim1DclosedLoopContrast, stimSender.senderWindow.switchParamsAuto.stopmode, deviceP, vertices1);
                   //                   DrawWhite((byte)stimSender.stim1DclosedLoopContrast, stimSender.senderWindow.switchParamsAuto.stopmode, deviceW, vertices2);

                   backgroundcolor = 255;// (byte)(255 - stimSender.senderWindow.InstStimParams.dotcolor * 255);



                   //                   DrawWhite(stimSender.blevel, deviceP, vertices1);
                   //                   DrawWhite(stimSender.blevel, deviceW, vertices2);
                   DrawWhite(stimSender.blevel, deviceP, vertices1);
                   DrawWhite(stimSender.blevel, deviceW, vertices2);


                   float dot_sizeX = (float)(stimSender.senderWindow.InstStimParams.dotSizeX / 100);
                   float dot_sizeY = (float)(stimSender.senderWindow.InstStimParams.dotSizeY / 100);

                   float[] xx = { stimSender.xx_ - dot_sizeX, stimSender.xx_ + dot_sizeX, stimSender.xx_ + dot_sizeX, stimSender.xx_ - dot_sizeX };
                   float[] yy = { stimSender.yy_ - dot_sizeY, stimSender.yy_ - dot_sizeY, stimSender.yy_ + dot_sizeY, stimSender.yy_ + dot_sizeY };
                   // float[] xx = { .66f + stimSender.stripesY * ini_distance * (float)Math.Cos(ang) + wobble_size * (float)Math.Cos(wobble_speed * stimSender.stripesY + 99*ang), .66f + stimSender.stripesY * ini_distance * (float)Math.Cos(ang) + dot_size + wobble_size * (float)Math.Cos(wobble_speed * stimSender.stripesY + 99*ang), .66f + stimSender.stripesY * ini_distance * (float)Math.Cos(ang) + dot_size + wobble_size * (float)Math.Cos(wobble_speed * stimSender.stripesY + 99*ang), .66f + stimSender.stripesY * ini_distance * (float)Math.Cos(ang) + wobble_size * (float)Math.Cos(wobble_speed * stimSender.stripesY + 99*ang) };
                   //{.7f, .7f, .8f, .8f};
                   //float[] yy = { .5f + stimSender.stripesY * ini_distance * (float)Math.Sin(ang) + dot_size + wobble_size * (float)Math.Sin(wobble_speed * stimSender.stripesY + 99*ang), .5f + stimSender.stripesY * ini_distance * (float)Math.Sin(ang) + dot_size + wobble_size * (float)Math.Sin(wobble_speed * stimSender.stripesY + 99*ang), .5f + stimSender.stripesY * ini_distance * (float)Math.Sin(ang) + wobble_size * (float)Math.Sin(wobble_speed * stimSender.stripesY + 99*ang), .5f + stimSender.stripesY * ini_distance * (float)Math.Sin(ang) + wobble_size * (float)Math.Sin(wobble_speed * stimSender.stripesY + 99*ang) };
                   //                   float[] yy = {.7f, .8f, .8f, .7f};

                   stimSender.dotContrast = 0;


                   drawPoly(vertices1, deviceP, xx, yy, stimSender.dotContrast);
                   drawPoly(vertices2, deviceW, xx, yy, stimSender.dotContrast);

               }

                   /*
               else
               {
                   DrawWhite((byte)stimSender.blevel, deviceP, vertices1);
                   DrawWhite((byte)stimSender.blevel, deviceW, vertices2);
               }
                 * */

               Flip(deviceP);
               Flip(deviceW);         
        }

        public void DisplayClear(Microsoft.DirectX.Direct3D.Device device)
        {
            if (device == null)
                return;

            device.Clear(ClearFlags.Target, System.Drawing.Color.Black, 1.0f, 0);
            device.RenderState.ZBufferEnable = false;   // We'll not use this feature
            device.RenderState.Lighting = false;        // Or this one...
            device.RenderState.CullMode = Cull.None;    // Or this one...

            //Begin the scene
            device.BeginScene();
        }

        public void SetDrawWorld(StimEphysOscilloscopeControl.StimState stimState_, int moveobject, Microsoft.DirectX.Direct3D.Device device, VertexBuffer backgroundV, Texture backgroundT, Texture backgroundW) //(IAsyncResult ar)
        {
            yPosition = stimState_.yPosition;
            xPosition = stimState_.xPosition;
            orientation = stimState_.orientation;

            if (stimState_.wrap)
            {
                xPosition = (float)(((xPosition % (ww / repx)) + (ww / repx)) % (ww / repx));
                yPosition = (float)(((yPosition % (hh / repy)) + (hh / repy)) % (hh / repy));
            }
            
            if (moveobject==4 )
            {
                xPosition = 0; yPosition = 0;
            }



            device.Transform.World = Matrix.Translation(xPosition, yPosition, 0) *     // translation
                                     Matrix.Translation(-(w_ / 2), -(h_ / 2), 0) *     // rotation about center of screen
                                     Matrix.RotationZ(orientation) *                   // c'd
                                     Matrix.Translation((w_ / 2), (h_ / 2), 0);        // c'd

            if (moveobject == 1 || moveobject == 3 || moveobject==4)
                device.SetTexture(0, backgroundW);
            else
                device.SetTexture(0, backgroundT);

            device.SetStreamSource(0, backgroundV, 0);
            device.VertexFormat = CustomVertex.PositionColoredTextured.Format;
            device.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
            device.SetTexture(0, null);

        }

        
        public void DrawGrating(float displayAngle, float yPos, byte contrast, int blevel, float dH, Microsoft.DirectX.Direct3D.Device device, VertexBuffer vertice)
        {
            float W = 3f;// /1.6f since bigger screen

            yPos = yPos % (2 * dH);
            
 
            if (contrast > 0)
            {
                for (float H = -1.5f + yPos - 1f; H < 1.5 + yPos; H += 2 * dH)
                {
                    float[] xx = {
                          (float)(Math.Cos(displayAngle)*(-W) + Math.Sin(displayAngle)*H),
                          (float)(Math.Cos(displayAngle)*W + Math.Sin(displayAngle)*H),
                          (float)(Math.Cos(displayAngle)*W + Math.Sin(displayAngle)*(H+dH)),
                          (float)(Math.Cos(displayAngle)*(-W) + Math.Sin(displayAngle)*(H+dH)),
                          };

                    float[] yy = {
                          (float)(-Math.Sin(displayAngle)*(-W) + Math.Cos(displayAngle)*H),
                          (float)(-Math.Sin(displayAngle)*W + Math.Cos(displayAngle)*H),
                          (float)(-Math.Sin(displayAngle)*W + Math.Cos(displayAngle)*(H+dH)),
                          (float)(-Math.Sin(displayAngle)*(-W) + Math.Cos(displayAngle)*(H+dH)),
                          };

                    if (contrast < 50)
                    {
                        //drawPoly(vertice, device, xx, yy, 50 - contrast);
                        drawPoly(vertice, device, xx, yy, 100);
                    }
                    else
                    {
                        //drawPoly(vertice, device, xx, yy, 0);
                        drawPoly(vertice, device, xx, yy, 100);
                    }

                    float[] xx1 = {
                          (float)(Math.Cos(displayAngle)*(-W) + Math.Sin(displayAngle)*(H+dH)),
                          (float)(Math.Cos(displayAngle)*W + Math.Sin(displayAngle)*(H+dH)),
                          (float)(Math.Cos(displayAngle)*W + Math.Sin(displayAngle)*(H+2*dH)),
                          (float)(Math.Cos(displayAngle)*(-W) + Math.Sin(displayAngle)*(H+2*dH)),
                          };

                    float[] yy1 = {
                          (float)(-Math.Sin(displayAngle)*(-W) + Math.Cos(displayAngle)*(H+dH)),
                          (float)(-Math.Sin(displayAngle)*W + Math.Cos(displayAngle)*(H+dH)),
                          (float)(-Math.Sin(displayAngle)*W + Math.Cos(displayAngle)*(H+2*dH)),
                          (float)(-Math.Sin(displayAngle)*(-W) + Math.Cos(displayAngle)*(H+2*dH)),
                          };

                    drawPoly(vertice, device, xx1, yy1, 50 + contrast);

                }
            }
            else
            {
                DrawWhite(blevel, device, vertice);
            }

        }

        public void DrawBox(byte contrast, int blevel, int boxpos, float dH, Microsoft.DirectX.Direct3D.Device device, VertexBuffer vertice)
        {

            DrawWhite(128, device, vertice);
            if (boxpos == 1)
            {
                float[] xx = { 1.0f, 1.1f, 1.1f, 1.0f }; 
                float[] yy = { 0.8f, 0.8f, 0.7f, 0.7f }; 
                drawPoly(vertice, device, xx, yy, 255);
            }
            if (boxpos == 2)
            {
                float[] xx = { 1.0f, 1.1f, 1.1f, 1.0f }; 
                float[] yy = { 0.65f , 0.65f , 0.55f , 0.55f  };
                drawPoly(vertice, device, xx, yy, 255); 
            }
            if (boxpos == 3)
            {
                float[] xx = { 0.7f, 0.8f, 0.8f, 0.7f };
                float[] yy = { 0.8f, 0.8f, 0.7f, 0.7f }; 
                drawPoly(vertice, device, xx, yy, 255); 
            }
            if (boxpos == 4)
            {
                float[] xx = { 0.7f, 0.8f, 0.8f, 0.7f };
                float[] yy = { 0.65f, 0.65f , 0.55f , 0.55f };
                drawPoly(vertice, device, xx, yy, 255); 
            }  
        }

        public void DrawCrosses(float displayAngle, float yPos, byte contrast, int blevel, float dH, Microsoft.DirectX.Direct3D.Device device, VertexBuffer vertice)
        {
            float W = 3f;// /1.6f since bigger screen

            yPos = yPos % (2 * dH);

            if (contrast > 0)
            {   
                DrawWhite(0, device, vertice);

                for (float H = -1.5f + yPos - 1f; H < 1.5 + yPos; H += 3 * dH)
                {
                    float[] xx = {
                          (float)(Math.Cos(displayAngle)*(-W) + Math.Sin(displayAngle)*H),
                          (float)(Math.Cos(displayAngle)*W + Math.Sin(displayAngle)*H),
                          (float)(Math.Cos(displayAngle)*W + Math.Sin(displayAngle)*(H+dH)),
                          (float)(Math.Cos(displayAngle)*(-W) + Math.Sin(displayAngle)*(H+dH)),
                          };

                    float[] yy = {
                          (float)(-Math.Sin(displayAngle)*(-W) + Math.Cos(displayAngle)*H),
                          (float)(-Math.Sin(displayAngle)*W + Math.Cos(displayAngle)*H),
                          (float)(-Math.Sin(displayAngle)*W + Math.Cos(displayAngle)*(H+dH)),
                          (float)(-Math.Sin(displayAngle)*(-W) + Math.Cos(displayAngle)*(H+dH)),
                          };

                    drawPoly(vertice, device, xx, yy, 255);

                    float[] xx1 = {
                          (float)(Math.Cos(displayAngle+(Math.PI/2))*(-W) + Math.Sin(displayAngle+(Math.PI/2))*H),
                          (float)(Math.Cos(displayAngle+(Math.PI/2))*W + Math.Sin(displayAngle+(Math.PI/2))*H),
                          (float)(Math.Cos(displayAngle+(Math.PI/2))*W + Math.Sin(displayAngle+(Math.PI/2))*(H+dH)),
                          (float)(Math.Cos(displayAngle+(Math.PI/2))*(-W) + Math.Sin(displayAngle+(Math.PI/2))*(H+dH)),
                          };

                    float[] yy1 = {
                          (float)(-Math.Sin(displayAngle+(Math.PI/2))*(-W) + Math.Cos(displayAngle+(Math.PI/2))*H),
                          (float)(-Math.Sin(displayAngle+(Math.PI/2))*W + Math.Cos(displayAngle+(Math.PI/2))*H),
                          (float)(-Math.Sin(displayAngle+(Math.PI/2))*W + Math.Cos(displayAngle+(Math.PI/2))*(H+dH)),
                          (float)(-Math.Sin(displayAngle+(Math.PI/2))*(-W) + Math.Cos(displayAngle+(Math.PI/2))*(H+dH)),
                          };


                    drawPoly(vertice, device, xx1, yy1, 255);

                }

            }
            else
            {
                DrawWhite(blevel, device, vertice);
            }

        }

        public void DrawWhite(int blevel, Microsoft.DirectX.Direct3D.Device device, VertexBuffer vertice)
        {
            float[] xx = { -5, 5, 5, -5 };
            float[] yy = { 5, 5, -5, -5 };
            drawPoly(vertice, device, xx, yy, blevel);
        }



        public void Flip(Microsoft.DirectX.Direct3D.Device device)
        {
            device.Transform.Projection = Matrix.OrthoOffCenterLH(0, w_, 0, h_, 0, 1);
            device.EndScene();
            device.Present();
        }

        public void drawPoly(VertexBuffer vertice, Microsoft.DirectX.Direct3D.Device device, float[] x, float[] y, int grayCol)
        {
            GraphicsStream gs2 = vertice.Lock(0, 0, 0);     // Lock the vertex list
            int clr = Color.FromArgb(grayCol, 0, 0).ToArgb();

            for (int i = 0; i < x.Length; i++)
            {
                gs2.Write(new CustomVertex.PositionColored(x[i], y[i], 0, clr));
            }

            vertice.Unlock();
            device.SetStreamSource(0, vertice, 0);
            device.VertexFormat = CustomVertex.PositionColored.Format;
            device.DrawPrimitives(PrimitiveType.TriangleFan, 0, x.Length - 2);
        }

        private void StimWindow_Load(object sender, EventArgs e)
        {

        }

        public void CloseWindow()
        {
            SCWindow.Dispose();
            SCWindow.Close();
            SCWindow = null;

        }

    }
}

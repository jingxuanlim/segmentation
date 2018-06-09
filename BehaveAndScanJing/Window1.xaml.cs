using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;  // for ZedGrap
using System.Windows.Threading;
using System.Drawing;
using System.Threading;
using System.Windows.Forms.Integration;
using System.ComponentModel;

namespace BehaveAndScanSPIM
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        public Window BaseWindow;

        public System.Windows.Forms.Panel panelProjector;
        public bool writeOnEP    = false;
        public bool triggerStart = false;

        public bool centerOnPoint = false;
        public int centerOnPointNumber = 0;
        public bool StimulusON = false;

        public bool DurationStopE = false;
        public double CommonDuration ;
        public int recStackSPS;
        public double actSPSv;
        public bool openprobe = false;
        public bool onePulse = false;
        public float stim3Test;

        public StimParams InstStimParams = new StimParams();
        public switchParams switchParamsAuto = new switchParams();
        public oscParams oscParam = new oscParams();
        public oscULValues oscULValue = new oscULValues();

        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        double[] switchParamsPreset;

        public string fishName;
        public string directory;
        public string fileNameEP;       

        public StimEphysOscilloscopeControl stimSession;
        
        public Window1()
        {
            InitializeComponent();

        }

        private void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            BaseWindow = this;
            InstStimParams.clmode = 4;
            InstStimParams.stimtype = 2;
            InstStimParams.moveobject = 1;
            InstStimParams.autoTime1 = 20;
            InstStimParams.autoTime2 = 20;
            InstStimParams.autoTimeSum = InstStimParams.autoTime1 + InstStimParams.autoTime2;
            InstStimParams.autoTimeSumDouble = InstStimParams.autoTime1 + InstStimParams.autoTime2 + InstStimParams.autoTime1 + InstStimParams.autoTime2;
            InstStimParams.gwidth= 1;
            InstStimParams.sps = 30;
            InstStimParams.replayfcycle = 1800;
            InstStimParams.recordPlayback = false;
            InstStimParams.recordPlayback_special = false;
            InstStimParams.stimSequence = new float[70];

            InstStimParams.autoVel1 = 0.5;
            InstStimParams.autoVel2 = 0.5;
            InstStimParams.autoGain1 = 0.006;
            InstStimParams.autoGain2 = 0.018;

            for (int k = 0; k < 70; k++)
            {
                InstStimParams.stimSequence[k] = 1;
            }


            init_struct_arrays();
            TotalTime.Content = switchParamsAuto.phasedurinc[5].ToString();
            CycleNum.Text = switchParamsAuto.cycles.ToString();

            directory = "E:\\Jing\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            commonFileName.Text = directory;
            FishName.Text = fishName;

            fileNameEP = directory + fishName;

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(33);
            dispatcherTimer.Start();
            
        }

        private void init_struct_arrays()
        {
            switchParamsAuto = new switchParams();
            switchParamsAuto.phasedur = new double[6];
            switchParamsAuto.phasedurinc = new double[6];
            switchParamsAuto.test_params = new double[3];
            switchParamsAuto.test_durinc = new double[3];
            switchParamsAuto.stopmode = 1;
            switchParamsAuto.cycles = 20;

            switchParamsPreset = new double[6];
            switchParamsPreset[0] = 30;
            switchParamsPreset[1] = 10;
            switchParamsPreset[2] = 5;
            switchParamsPreset[3] = 30;
            switchParamsPreset[4] = 10;
            switchParamsPreset[5] = 5;

            switchParamsAuto.phasedur[0] = switchParamsPreset[0];
            switchParamsAuto.phasedur[1] = switchParamsPreset[1];
            switchParamsAuto.phasedur[2] = switchParamsPreset[2];
            switchParamsAuto.phasedur[3] = switchParamsPreset[3];
            switchParamsAuto.phasedur[4] = switchParamsPreset[4];
            switchParamsAuto.phasedur[5] = switchParamsPreset[5];

            switchParamsAuto.test_params[0] = 7;
            switchParamsAuto.test_params[1] = 15;
            switchParamsAuto.test_params[2] = 30;
            switchParamsAuto.test_durinc[0] = 0;
            switchParamsAuto.test_durinc[1] = 0;
            switchParamsAuto.test_durinc[2] = 0;

            switchParamsAuto = update_phasedur1(switchParamsAuto);
            CommonDuration = (switchParamsAuto.cycles * switchParamsAuto.phasedurinc[5])+1;

            switchParamsAuto.stopmode = 1;
            
            oscParam.oscMode = 1;
            oscParam.threshScale = new double[3];
            oscParam.threshScale[0] = 2.5;
            oscParam.threshScale[1] = 2.5;
            oscParam.threshScale[2] = 0.5;

            oscULValue.ch1UpLim = 0;
            oscULValue.ch1LoLim = 0;
            oscULValue.ch2UpLim = 0;
            oscULValue.ch2LoLim = 0;
        }


         void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            gain1Check.IsChecked = StimEphysOscilloscopeControl.switchDisp.gain1check;
            gain2Check.IsChecked = StimEphysOscilloscopeControl.switchDisp.gain2check;
            Phase1Time.IsEnabled = StimEphysOscilloscopeControl.switchDisp.Phase1Time;
            Phase2Time.IsEnabled = StimEphysOscilloscopeControl.switchDisp.Phase2Time;
            Phase3Time.IsEnabled = StimEphysOscilloscopeControl.switchDisp.Phase3Time;
            Phase4Time.IsEnabled = StimEphysOscilloscopeControl.switchDisp.Phase4Time;
            Phase5Time.IsEnabled = StimEphysOscilloscopeControl.switchDisp.Phase5Time;
            Phase6Time.IsEnabled = StimEphysOscilloscopeControl.switchDisp.Phase6Time;
            playBackOnBox.IsChecked = StimEphysOscilloscopeControl.switchDisp.velReplay;
            FrameNumDisp.Content = StimEphysOscilloscopeControl.switchDisp.frameNum.ToString("0");
            StackNumDisp.Content = StimEphysOscilloscopeControl.switchDisp.stackNum.ToString("0");
            //GainLabel.Content = StimEphysOscilloscopeControl.stimParam3;
            TrigLabel.Content = StimEphysOscilloscopeControl.gainTriggerSwitch;
            commonFileName.IsEnabled = StimEphysOscilloscopeControl.FileNameEnabled;
            FishName.IsEnabled = StimEphysOscilloscopeControl.FishNameEnabled;
            RecTime.Content = StimEphysOscilloscopeControl.recEPtime.ToString("0");
            RecTimeSecLabel.Content = "Seconds";
            RecSec.Content = "Seconds";
        

            if ( StimEphysOscilloscopeControl.triggerGo )
            {                
                actSPSv = (double)StimEphysOscilloscopeControl.switchDisp.stackNum / (double) StimEphysOscilloscopeControl.recEPtime;
                ActSPS.Content =actSPSv.ToString("0.0");
            }
            

            ch1UpLim.Content = oscULValue.ch1UpLim.ToString("N5");
            ch1LoLim.Content = oscULValue.ch1LoLim.ToString("N5");
            ch2UpLim.Content = oscULValue.ch2UpLim.ToString("N5");
            ch2LoLim.Content = oscULValue.ch2LoLim.ToString("N5");

            //if (InstStimParams.clmode != 111) { InstStimParams.recordPlayback_special = false; }
            
        }    



             

        private void buttonStartStimEphys_Click(object sender, RoutedEventArgs e)
        {
            if (!StimulusON)//stimulus == null)
            {
                stimSession = new StimEphysOscilloscopeControl();

                Thread scroll = new Thread(new ParameterizedThreadStart(stimSession.StartEphys));
                scroll.Start(this);

                commonFileName.IsEnabled = false;
                FishName.IsEnabled = false;
                buttonStartStimEphys.IsEnabled = false;
                buttonStopStimEphys.IsEnabled = true;
                sps_box.IsEnabled = false;
                StimulusON = true;
            }
        }

        private void buttonStopStimEphys_Click(object sender, RoutedEventArgs e)
        {
            InstStimParams.stopStimEphys = true;
            FishName.IsEnabled = true;
            buttonStartStimEphys.IsEnabled = true;
            buttonStopStimEphys.IsEnabled = false;
            Triggered.IsEnabled = true; 
            sps_box.IsEnabled = true;
            StimulusON = false;
        }  
       
        private void writeFileBox_Changed(object sender, RoutedEventArgs e)
        {
            writeOnEP = (writeFileBox.IsChecked == true);
            Triggered.IsEnabled = !(writeFileBox.IsChecked == true);
            
        }

        private void Triggered_Click(object sender, RoutedEventArgs e)
        {
            triggerStart = (Triggered.IsChecked == true);
        }


        private void nostimradio_Checked(object sender, RoutedEventArgs e)
        {
            InstStimParams.stimtype = 1;
        }


        private void d1radio_Checked(object sender, RoutedEventArgs e)
        {
            InstStimParams.stimtype = 2;
        }


        private void autoGain1Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            double.TryParse(autoGain1Box.Text, out InstStimParams.autoGain1);
        }


        private void autoVel1Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            double.TryParse(autoVel1Box.Text, out InstStimParams.autoVel1);
        }


        private void autoGain2Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            double.TryParse(autoGain2Box.Text, out InstStimParams.autoGain2);
        }


        private void autoVel2Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            double.TryParse(autoVel2Box.Text, out InstStimParams.autoVel2);
        }

        
        private void velZeroBox_Changed(object sender, RoutedEventArgs e)
        {
            InstStimParams.closedLoop1DzeroVel = (velZeroBox.IsChecked == true);
        }


        private void memTestRadio_Changed(object sender, RoutedEventArgs e)
        {
            if (memTestRadio.IsChecked == true)
            {
                InstStimParams.clmode = 1;
                CommonDuration = (switchParamsAuto.phasedurinc[5] * switchParamsAuto.cycles)+1;
                DurationEBox.Text = CommonDuration.ToString("0");
            }
        }



        private void raphe_training_test2_Click(object sender, RoutedEventArgs e)
        {
            if (raphe_training_test2.IsChecked == true)
            {
                InstStimParams.clmode = 54;
                CommonDuration = (200 * switchParamsAuto.cycles) + 1;
                DurationEBox.Text = CommonDuration.ToString("0");
            }

        }


        private void raphe_training_test_Click(object sender, RoutedEventArgs e)
        {
            if (raphe_training_test.IsChecked == true)
            {
                InstStimParams.clmode = 53;
                CommonDuration = (157 * 5) + 1;
                DurationEBox.Text = CommonDuration.ToString("0");
            }
        }

        private void low_training_test_Click(object sender, RoutedEventArgs e)
        {
            if (low_training_test.IsChecked == true)
            {
                InstStimParams.clmode = 56;
                CommonDuration = (157 * switchParamsAuto.cycles) + 1;
                DurationEBox.Text = CommonDuration.ToString("0");
            }

        }

        private void high_training_test_Click(object sender, RoutedEventArgs e)
        {
            if (high_training_test.IsChecked == true)
            {
                InstStimParams.clmode = 156;
                CommonDuration = (157 * switchParamsAuto.cycles) + 1;
                DurationEBox.Text = CommonDuration.ToString("0");
            }

        }

        private void autoGainSwitchSimpleRadio_Changed(object sender, RoutedEventArgs e)
        {
            if (autoGainSwitchSimpleRadio.IsChecked == true)
            {
                InstStimParams.clmode = 4;
                CommonDuration = 640 + 1;
                DurationEBox.Text = CommonDuration.ToString("0");
            }
        }
        

        private void SpeedTuneRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (SpeedTuneRadio.IsChecked == true)
                InstStimParams.clmode = 5;
                stopmode3.IsChecked = true;
                switchParamsAuto.stopmode = 3;
        }


        private void Oritune_Click(object sender, RoutedEventArgs e)
        {
            if (Oritune.IsChecked == true)
                InstStimParams.clmode = 6;
                switchParamsAuto.stopmode = 3;
                CommonDuration = (720) + 1;
                DurationEBox.Text = CommonDuration.ToString("0");

        }

        private void SPFtuning_Click(object sender, RoutedEventArgs e)
        {
            if (SPFtuning.IsChecked == true)
                InstStimParams.clmode = 7;
                stopmode3.IsChecked = true;
                 switchParamsAuto.stopmode = 3;
        }
              
        private void recordPlaybackBox_Changed(object sender, RoutedEventArgs e)
        {
            InstStimParams.recordPlayback = (recordPlaybackBox.IsChecked == true);
        }

      


        private void DurationStopECheck_Click(object sender, RoutedEventArgs e)
        {
            DurationStopE = (DurationStopECheck.IsChecked == true);
            if (DurationStopE)
            {
                DurationEBox.IsEnabled = true;
            }
            else
            {
                DurationEBox.IsEnabled = false;
            }
        }       

     

        private void DurationE_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DurationStopE)
            {
                double.TryParse(DurationEBox.Text, out CommonDuration);
            }
        }
        

        
        public void update_phasedurpanel(switchParams switchParam)
        {
            if (switchParam.phasedur != null && switchParam.phasedurinc != null)
            {
                Phase1Time.Text = switchParam.phasedur[0].ToString();
                Phase2Time.Text = switchParam.phasedur[1].ToString();
                Phase3Time.Text = switchParam.phasedur[2].ToString();
                Phase4Time.Text = switchParam.phasedur[3].ToString();
                Phase5Time.Text = switchParam.phasedur[4].ToString();
                Phase6Time.Text = switchParam.phasedur[5].ToString();
            }
        }



        public switchParams update_phasedur1(switchParams switchParam)
        {
            if (switchParam.phasedur != null && switchParam.phasedurinc != null)
            {
                switchParam.phasedurinc[0] = switchParam.phasedur[0];
                switchParam.phasedurinc[1] = switchParam.phasedurinc[0] + switchParam.phasedur[1];
                switchParam.phasedurinc[2] = switchParam.phasedurinc[1] + switchParam.phasedur[2];
                switchParam.phasedurinc[3] = switchParam.phasedurinc[2] + switchParam.phasedur[3];
                switchParam.phasedurinc[4] = switchParam.phasedurinc[3] + switchParam.phasedur[4];
                switchParam.phasedurinc[5] = switchParam.phasedurinc[4] + switchParam.phasedur[5];

                TotalTime.Content = switchParam.phasedurinc[5].ToString();
                CycleNum.Text = switchParam.cycles.ToString();           

                if (InstStimParams.clmode==1)
                {     
                    CommonDuration = (switchParam.cycles * switchParam.phasedurinc[5])+1;
                    DurationEBox.Text = CommonDuration.ToString("0");
                }
                else if (InstStimParams.clmode==2)
                {
                    create_testdurinc(switchParam, "resting");
                }
                else if (InstStimParams.clmode == 3)
                {
                    create_testdurinc(switchParam, "training");
                }
            } 

            return switchParam;
        }

        public switchParams update_phasedur2(switchParams switchParam, string mode, int testcycle)
        {
           
            if (switchParam.phasedur != null && switchParam.phasedurinc != null)
            {
                if (mode == "resting")
                {
                    switchParam.phasedur[1] = switchParam.test_params[testcycle];
                    switchParam.phasedur[4] = switchParam.test_params[testcycle];

                    switchParam.phasedurinc[0] = switchParam.phasedur[0];
                    switchParam.phasedurinc[1] = switchParam.phasedurinc[0] + switchParam.phasedur[1];
                    switchParam.phasedurinc[2] = switchParam.phasedurinc[1] + switchParam.phasedur[2];
                    switchParam.phasedurinc[3] = switchParam.phasedurinc[2] + switchParam.phasedur[3];
                    switchParam.phasedurinc[4] = switchParam.phasedurinc[3] + switchParam.phasedur[4];
                    switchParam.phasedurinc[5] = switchParam.phasedurinc[4] + switchParam.phasedur[5];

                    TotalTime.Content = switchParam.phasedurinc[5].ToString();
                    update_phasedurpanel(switchParam); ;

                }
                
                if (mode == "training")
                {
                    switchParam.phasedur[0] = switchParam.test_params[testcycle];
                    switchParam.phasedur[3] = switchParam.test_params[testcycle];

                    switchParam.phasedurinc[0] = switchParam.phasedur[0];
                    switchParam.phasedurinc[1] = switchParam.phasedurinc[0] + switchParam.phasedur[1];
                    switchParam.phasedurinc[2] = switchParam.phasedurinc[1] + switchParam.phasedur[2];
                    switchParam.phasedurinc[3] = switchParam.phasedurinc[2] + switchParam.phasedur[3];
                    switchParam.phasedurinc[4] = switchParam.phasedurinc[3] + switchParam.phasedur[4];
                    switchParam.phasedurinc[5] = switchParam.phasedurinc[4] + switchParam.phasedur[5];

                    TotalTime.Content = switchParamsAuto.phasedurinc[5].ToString();
                    update_phasedurpanel(switchParam); ;
                }
            }
            
            return switchParam;
        }

        public switchParams create_testdurinc(switchParams switchParam, string mode)
        {

            if (switchParam.phasedur != null && switchParam.phasedurinc != null)
            {
                double base1,base2;
                if (mode == "resting")
                {
                    base1=switchParam.phasedur[0]+switchParam.phasedur[2]+switchParam.phasedur[3]+switchParam.phasedur[5];

                    base2 = base1 + 2 * switchParam.test_params[0];
                    switchParam.test_durinc[0] = base2;
                    base2 = base1 + 2 * switchParam.test_params[1];
                    switchParam.test_durinc[1] = switchParam.test_durinc[0] + base2;
                    base2 = base1 + 2 * switchParam.test_params[2];
                    switchParam.test_durinc[2] = switchParam.test_durinc[1] + base2;
                }

                if (mode == "training")
                {
                    base1=switchParam.phasedur[1]+switchParam.phasedur[2]+switchParam.phasedur[4]+switchParam.phasedur[5];

                    base2 = base1 + 2 * switchParam.test_params[0];
                    switchParam.test_durinc[0] = base2;
                    base2 = base1 + 2 * switchParam.test_params[1];
                    switchParam.test_durinc[1] = switchParam.test_durinc[0] + base2;
                    base2 = base1 + 2 * switchParam.test_params[2];
                    switchParam.test_durinc[2] = switchParam.test_durinc[1] + base2;
                }


                TotalTimeTest.Content = switchParam.test_durinc[2].ToString();

                CommonDuration = (switchParam.cycles * switchParam.test_durinc[2])+1;
                DurationEBox.Text = CommonDuration.ToString("0");
            }

            return switchParam;
        }

        private void stopmode1_Click(object sender, RoutedEventArgs e)
        {
            if(stopmode1.IsChecked==true)
                switchParamsAuto.stopmode=1;            
        }

        private void stopmode2_Click(object sender, RoutedEventArgs e)
        {
            if (stopmode2.IsChecked == true)
                switchParamsAuto.stopmode = 2;
        }

        private void stopmode3_Click(object sender, RoutedEventArgs e)
        {
            if (stopmode3.IsChecked == true)
                switchParamsAuto.stopmode = 3;
        }

        private void stopmode4_Click(object sender, RoutedEventArgs e)
        {
            if (stopmode4.IsChecked == true)
                switchParamsAuto.stopmode = 4;        }


        private void Phase1Time_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Phase1Time.Text != "" && switchParamsAuto.phasedur != null)
            {
                double.TryParse(Phase1Time.Text, out switchParamsAuto.phasedur[0]);
                switchParamsAuto = update_phasedur1(switchParamsAuto);
            }
        }

        private void Phase2Time_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Phase2Time.Text != "" && switchParamsAuto.phasedur != null)
            {
                double.TryParse(Phase2Time.Text, out switchParamsAuto.phasedur[1]);
                switchParamsAuto = update_phasedur1(switchParamsAuto);
            }
            
        }

        private void Phase3Time_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Phase3Time.Text != "" && switchParamsAuto.phasedur != null)
            {
                double.TryParse(Phase3Time.Text, out switchParamsAuto.phasedur[2]);
                switchParamsAuto = update_phasedur1(switchParamsAuto);
            }
        }


        private void Phase4Time_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Phase4Time.Text != "" && switchParamsAuto.phasedur != null)
            {
                double.TryParse(Phase4Time.Text, out switchParamsAuto.phasedur[3]);
                switchParamsAuto = update_phasedur1(switchParamsAuto);
            }
        }


        private void Phase5Time_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Phase5Time.Text != "" && switchParamsAuto.phasedur != null)
            {
                double.TryParse(Phase5Time.Text, out switchParamsAuto.phasedur[4]);
                switchParamsAuto = update_phasedur1(switchParamsAuto);
            }
        }


        private void Phase6Time_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Phase6Time.Text != "" && switchParamsAuto.phasedur != null)
            {
                double.TryParse(Phase6Time.Text, out switchParamsAuto.phasedur[5]);
                switchParamsAuto = update_phasedur1(switchParamsAuto);
            }
        }

        private void CycleNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CycleNum.Text != "" && switchParamsAuto.test_durinc!=null && switchParamsAuto.phasedurinc!=null)
            {
                double.TryParse(CycleNum.Text, out switchParamsAuto.cycles);
                if (InstStimParams.clmode == 2 || InstStimParams.clmode == 3)
                {
                    CommonDuration = (switchParamsAuto.test_durinc[2] * switchParamsAuto.cycles)+1;
                    DurationEBox.Text = CommonDuration.ToString("0");
                }
                else
                {
                    CommonDuration = (switchParamsAuto.phasedurinc[5]* switchParamsAuto.cycles)+1; 
                    DurationEBox.Text = CommonDuration.ToString("0");
                }
            }

        }

        private void testdur1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (testdur1.Text != "" && switchParamsAuto.test_params != null)
            {
                double.TryParse(testdur1.Text, out switchParamsAuto.test_params[0]);
                if (InstStimParams.clmode == 2)
                {
                    switchParamsAuto = create_testdurinc(switchParamsAuto, "resting");
                }
                else if (InstStimParams.clmode == 3)
                {
                    switchParamsAuto = create_testdurinc(switchParamsAuto, "training");
                }
            }
        }

        private void testdur2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (testdur2.Text != "" && switchParamsAuto.test_params != null)
            {
                double.TryParse(testdur2.Text, out switchParamsAuto.test_params[1]);
                if (InstStimParams.clmode == 2)
                {
                    switchParamsAuto = create_testdurinc(switchParamsAuto, "resting");
                }
                else if (InstStimParams.clmode == 3)
                {
                    switchParamsAuto = create_testdurinc(switchParamsAuto, "training");
                }
            }
        }

        private void testdur3_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (testdur3.Text != "" && switchParamsAuto.test_params != null)
            {
                double.TryParse(testdur3.Text, out switchParamsAuto.test_params[2]);
                if (InstStimParams.clmode == 2)
                {
                    switchParamsAuto = create_testdurinc(switchParamsAuto, "resting");
                }
                else if (InstStimParams.clmode == 3)
                {
                    switchParamsAuto = create_testdurinc(switchParamsAuto, "training");
                }
            }
        }

        private void memForget_Click(object sender, RoutedEventArgs e)
        {
            if (memForget.IsChecked == true)
                InstStimParams.clmode = 2;
                switchParamsAuto = create_testdurinc(switchParamsAuto, "resting");
        }

        private void memTrain_Click(object sender, RoutedEventArgs e)
        {
            if (memTrain.IsChecked == true)
                InstStimParams.clmode = 3;
                switchParamsAuto = create_testdurinc(switchParamsAuto, "training");
        }


        private void Grating_Click(object sender, RoutedEventArgs e)
        {
            InstStimParams.moveobject = 1;
        }

        private void Texture_Click(object sender, RoutedEventArgs e)
        {
            InstStimParams.moveobject = 2;
        }

        private void oscMode1Butt_Click(object sender, RoutedEventArgs e)
        {
            if (oscMode1Butt.IsChecked == true)
                oscParam.oscMode = 1;
        }

        private void oscMode2Butt_Click(object sender, RoutedEventArgs e)
        {
            if (oscMode2Butt.IsChecked == true)
                oscParam.oscMode = 2;
        }

        private void oscMode3Butt_Click(object sender, RoutedEventArgs e)
        {
            if (oscMode3Butt.IsChecked == true)
                oscParam.oscMode = 3;
        }

        private void threshCh1Scale_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (oscParam.threshScale != null) 
                double.TryParse(threshCh1Scale.Text, out oscParam.threshScale[0]);
        }

        private void threshCh2Scale_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (oscParam.threshScale != null)
                double.TryParse(threshCh2Scale.Text, out oscParam.threshScale[1]);
        }
                

        private void commonFileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            directory = commonFileName.Text;
        }

  

        private void Gain1Dur_TextChanged(object sender, TextChangedEventArgs e)
        {
                double.TryParse(Gain1Dur.Text, out InstStimParams.autoTime1);  // parse input and assign value to autoTime1
                InstStimParams.autoTimeSum = InstStimParams.autoTime1 + InstStimParams.autoTime2;
                InstStimParams.autoTimeSumDouble = InstStimParams.autoTime1 + InstStimParams.autoTime2 + InstStimParams.autoTime1 + InstStimParams.autoTime2;
        }

        private void Gain2Dur_TextChanged(object sender, TextChangedEventArgs e)
        {
                double.TryParse(Gain2Dur.Text, out InstStimParams.autoTime2);  // parse input and assign value to autoTime2
                InstStimParams.autoTimeSum = InstStimParams.autoTime1 + InstStimParams.autoTime2;
                InstStimParams.autoTimeSumDouble = InstStimParams.autoTime1 + InstStimParams.autoTime2 + InstStimParams.autoTime1 + InstStimParams.autoTime2;
        }

        private void flashFreq_TextChanged(object sender, TextChangedEventArgs e)
        { 
                double.TryParse(flashFreq.Text, out InstStimParams.flash_freq);
        }

        private void flashDur_TextChanged(object sender, TextChangedEventArgs e)
        { 
                double.TryParse(flashDur.Text, out InstStimParams.flash_dur);
        }

        private void flashITI_TextChanged(object sender, TextChangedEventArgs e)
        { 
                double.TryParse(flashITI.Text, out InstStimParams.flash_iti);
        }

        private void flashCol1_TextChanged(object sender, TextChangedEventArgs e)
        { 
                double.TryParse(flashCol1.Text, out InstStimParams.flash_col1);
        }

        private void flashCol2_TextChanged(object sender, TextChangedEventArgs e)
        { 
                double.TryParse(flashCol2.Text, out InstStimParams.flash_col2);
        }

        private void sps_box_TextChanged(object sender, TextChangedEventArgs e)
        {
                int.TryParse(sps_box.Text, out InstStimParams.sps);

        }


        private void OpenProbeBox_Click(object sender, RoutedEventArgs e)
        {
            openprobe = (OpenProbeBox.IsChecked == true);

        }

        private void checkBox1_Click(object sender, RoutedEventArgs e)
        {
            onePulse = (OnePulseBox.IsChecked == true);
        }

        private void GratingWidthBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double.TryParse(GratingWidthBox.Text, out InstStimParams.gwidth);
        }

      

        private void GainRandSwitch_Click(object sender, RoutedEventArgs e)
        {
            OnePulseBox.IsEnabled = false;
            if (GainRandSwitch.IsChecked == true)
            {
                InstStimParams.clmode = 55;
                CommonDuration = 1950 + 1;
                DurationEBox.Text = CommonDuration.ToString("0");


                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Text file (*.txt)|*.txt";
                ofd.RestoreDirectory = true;
                DialogResult dr = ofd.ShowDialog();

                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    string[] lines = System.IO.File.ReadAllLines(ofd.FileName);

                    for (int i = 0; i < 65; i++)
                    {
                        InstStimParams.stimSequence[i] = Convert.ToSingle(lines[i]);
                    }

                }

            }
        }


        private void GainRandSwitch2_Click(object sender, RoutedEventArgs e)
        {
            OnePulseBox.IsEnabled = false;
            if (GainRandSwitch2.IsChecked == true)
            {
                InstStimParams.clmode = 555;
                CommonDuration = 2070 + 1;
                DurationEBox.Text = CommonDuration.ToString("0");


                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Text file (*.txt)|*.txt";
                ofd.RestoreDirectory = true;
                DialogResult dr = ofd.ShowDialog();

                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    string[] lines = System.IO.File.ReadAllLines(ofd.FileName);

                    for (int i = 0; i < 69; i++)
                    {
                        InstStimParams.stimSequence[i] = Convert.ToSingle(lines[i]);
                    }

                }

            }
        }

                

        



        private void ReplayFrameCycle_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(ReplayFrameCycle.Text, out InstStimParams.replayfcycle);            
        }

        private void movGain_Checked(object sender, RoutedEventArgs e)
        {
            OnePulseBox.IsEnabled = false;
            if (movGain.IsChecked == true)
            {
                InstStimParams.clmode = 123;
                CommonDuration = 840 + 1;
                DurationEBox.Text = CommonDuration.ToString("0");

            }

        }

        /*private void GainOMR_Rplay_Click(object sender, RoutedEventArgs e)
        {
            OnePulseBox.IsEnabled = false;
            if (GainOMR_Rplay.IsChecked == true)
            {
                InstStimParams.clmode = 124;
                CommonDuration = 800 + 1;
                DurationEBox.Text = CommonDuration.ToString("0");

                InstStimParams.recordPlayback = true;
                InstStimParams.replayfcycle = 40;
                recordPlaybackBox.IsChecked = true;
                ReplayFrameCycle.Text = InstStimParams.replayfcycle.ToString(); 
            }
        }*/

        private void StopGain_Replay_Click(object sender, RoutedEventArgs e)
        {
            OnePulseBox.IsEnabled = false;
            if (StopGain_Replay.IsChecked == true)
            {
                InstStimParams.clmode = 125;
                CommonDuration = 840 + 1;
                DurationEBox.Text = CommonDuration.ToString("0");

                InstStimParams.recordPlayback = true;
                InstStimParams.replayfcycle = 40;
                recordPlaybackBox.IsChecked = true;
                ReplayFrameCycle.Text = InstStimParams.replayfcycle.ToString();
            }

        }

        private void VisualClamp_Click(object sender, RoutedEventArgs e)
        {
            OnePulseBox.IsEnabled = false;
            if (VisualClamp.IsChecked == true)
            {
                InstStimParams.clmode = 126;
                CommonDuration = 600 + 1;
                DurationEBox.Text = CommonDuration.ToString("0");
            }

        }


        private void GainSwitch2_Click(object sender, RoutedEventArgs e)
        {
            OnePulseBox.IsEnabled = false;
            if (GainSwitch2.IsChecked == true)
            {
                InstStimParams.clmode = 127;
                CommonDuration = 720 + 1;
                DurationEBox.Text = CommonDuration.ToString("0");
            }

        }

        private void GainOMRReplay2_Click(object sender, RoutedEventArgs e)
        {
            OnePulseBox.IsEnabled = false;
            if (GainOMRReplay2.IsChecked == true)
            {
                InstStimParams.clmode = 111;
                CommonDuration = 800 + 1;
                InstStimParams.replayfcycle = 60;
                DurationEBox.Text = CommonDuration.ToString("0");

                InstStimParams.recordPlayback_special = true;
            }

        }

        private void AutoswitchStop_Click(object sender, RoutedEventArgs e)
        {
            OnePulseBox.IsEnabled = false;
            if (AutoswitchStop.IsChecked == true)
            {
                InstStimParams.clmode = 314;
                DurationEBox.Text = CommonDuration.ToString("0");
            }
        }


        
        private void AutoswitchStopFast_Click(object sender, RoutedEventArgs e)
        {
            if (AutoswitchStopFast.IsChecked == true)
            {
                OnePulseBox.IsEnabled = true;
                
                InstStimParams.clmode = 315;
                DurationEBox.Text = CommonDuration.ToString("0");
                
                
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(directory);
        }

        private void FishName_TextChanged(object sender, TextChangedEventArgs e)
        {
            fishName = FishName.Text;
        }

        private void LowTrainingShort_Click(object sender, RoutedEventArgs e)
        {
            OnePulseBox.IsEnabled = false;
            if (LowTrainingShort.IsChecked == true)
            {
                InstStimParams.clmode = 1056;
                CommonDuration = (157 * switchParamsAuto.cycles) + 1;
                DurationEBox.Text = CommonDuration.ToString("0");
            }
        }

        private void OMRLoom_Click(object sender, RoutedEventArgs e)
        {
            if (OMRLoom.IsChecked == true)
            {
                InstStimParams.clmode = 999999;
                CommonDuration = (40 * switchParamsAuto.cycles) + 1;
                DurationEBox.Text = CommonDuration.ToString("0");
            }
        }

        private void flash_Click(object sender, RoutedEventArgs e)
        {
            //if (flash.IsChecked == true)
           InstStimParams.clmode = 1234567;
           CommonDuration = (40 * switchParamsAuto.cycles) + 1;
           DurationEBox.Text = CommonDuration.ToString("0");
         
        }








        



        





    }
}

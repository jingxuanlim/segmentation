using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows;
using System.Diagnostics;


namespace BehaveAndScanSPIM
{
    public partial class StimEphysOscilloscopeControl
    {
        double t_old1 = 0;
        double tc;

        public void swimModeSwitch()
        {
            replayGo = true;

            if (triggerGo)
            {
                tt = (int)((switchDisp.stackNum - 1) / sps);
                tt2 = (int)((switchDisp.stackNum - 1) / (sps / 10.0));
                DTT = (int)(stopwatch.ElapsedMilliseconds);
            }
            else
            {
                tt = (int)(stopwatch.ElapsedMilliseconds / 1000.0);  // (int)(RecTime);
                tt2 = (int)(stopwatch.ElapsedMilliseconds / 100.0);
                DTT = (int)(stopwatch.ElapsedMilliseconds);
            
            }

            if (senderWindow.InstStimParams.clmode==1)  //////
            {
                resetGratingParams();  
                double ta;
                ta = tt % (int)senderWindow.switchParamsAuto.phasedurinc[5];
                memorytest_switch(ta);
            }

            if (senderWindow.InstStimParams.clmode == 51)
            {
                resetGratingParams();
                double ta;
                ta = tt % (int)senderWindow.switchParamsAuto.phasedurinc[5];
                memorytest_switch2(ta);
            }


            if (senderWindow.InstStimParams.clmode == 53) // raphe training  ///////////////
            {
                resetGratingParams();
                double ta = tt % (int)157;
                int aa = (int)tt / (int)157;
                stimParam5 = (float)aa + 1;

                if (ta < 42)
                {
                    stimParam4 = 1;
                    if (ta < 20)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = autoGain1;
                        velMultip = velMultip1;
                        stimParam3 = 1;
                        replayGo = false;
                    }
                    else if (ta < 27)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip2;
                        stimParam3 = 2;
                        replayGo = true;
                    }
                    else if (ta < 37)
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = false;
                        }
                        /*
                        stim1DclosedLoopContrast = 0;
                        blevel = 180;
                        closedLoop1Dgain = 0;
                        velMultip = 0;
                        stimParam3 = 3;
                         */
                        
                        
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = 0;
                        velMultip = -0.2;
                        stimParam3 = 3;
                        
                        replayGo = false;

                    }
                    else
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = (autoGain1 + autoGain2) / 2;
                        velMultip = velMultip1;
                        stimParam3 = 4;
                        replayGo = false;
                    }


                }
                else if (ta < 92)
                {
                    stimParam4 = 2;
                    if (ta < 62)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = autoGain1;
                        velMultip = velMultip1;
                        stimParam3 = 1;
                        replayGo = false;
                    }
                    else if (ta < 77)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip2;
                        stimParam3 = 2;
                        replayGo = true;
                    }
                    else if (ta < 87)
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = false;
                        } 
                        /*
                        stim1DclosedLoopContrast = 0;
                        blevel = 180;
                        closedLoop1Dgain = 0;
                        velMultip = 0;
                        stimParam3 = 3;
                         * */

                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = 0;
                        velMultip = -0.2;
                        stimParam3 = 3;

                        replayGo = false;
                        /*
                        closedLoop1Dgain = 0;
                        velMultip = -0.3;
                        stimParam3 = 3;
                        replayGo = false;
                        */

                    }
                    else
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = (autoGain1 + autoGain2) / 2;
                        velMultip = velMultip1;
                        stimParam3 = 4;
                        replayGo = false;
                    }

                }
                else
                {
                    stimParam4 = 3;
                    if (ta < 112)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = autoGain1;
                        velMultip = velMultip1;
                        stimParam3 = 1;
                        replayGo = false;
                    }
                    else if (ta < 142)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip2;
                        stimParam3 = 2;
                        replayGo = true;
                    }
                    else if (ta < 152)
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = false;
                        }

                        /*
                        stim1DclosedLoopContrast = 0;
                        blevel = 180;
                        closedLoop1Dgain = 0;
                        velMultip = 0;
                        stimParam3 = 3;
                         */


                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = 0;
                        velMultip = -0.2;
                        stimParam3 = 3;
                        /*
                        closedLoop1Dgain = 0;
                        velMultip = -0.3;
                        stimParam3 = 3;
                        replayGo = false;
                        */


                    }
                    else
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = (autoGain1 + autoGain2) / 2;
                        velMultip = velMultip1;
                        stimParam3 = 4;
                        replayGo = false;
                    }
                }
               

            }


            tchange1 = (int)(stopwatch.ElapsedMilliseconds);   
            if (senderWindow.InstStimParams.clmode == 123) // change gain with bouts  //////////////
            {
                resetGratingParams();
                stim1DclosedLoopContrast = 200;
                closedLoop1Dgain = autoGain2;
                velMultip = velMultip2;
                if (gainTriggerSwitch == true && (swimPow[0] > 0 || swimPow[1] > 0) && tchange1-tchange2>400)
                {
                    tchange2 = tchange1;
                    shuffler(shaul_ori2);
                }

                stimParam3 = shaul_ori2[1];

                if (stimParam3==1)
                {
                    closedLoop1Dgain=autoGain1;
                }
                if (stimParam3==2)
                {
                    closedLoop1Dgain=(autoGain1+autoGain2)/2;
                }
                if (stimParam3==3)
                {
                    closedLoop1Dgain=autoGain2;
                }

                gain = stimParam3;


            }

            if (senderWindow.InstStimParams.clmode == 1058) // simple switch stop fast movement ////////////////
            {
                resetGratingParams();

                int t = (int)tt % (int)senderWindow.InstStimParams.autoTimeSumDouble;
                //int t = (int)tt % 40;
                int t2 = (int)tt2 % (int)senderWindow.InstStimParams.autoTimeSum;
                //int t2 = (int)tt2 % 20;


                if (t < senderWindow.InstStimParams.autoTime1)
                //if (t < 10)
                {
                    if (closedLoop1Dgain != autoGain2)
                    {
                        reset_switchDisp();
                        switchDisp.gain1check = false;
                        switchDisp.gain2check = true;
                    }
                    closedLoop1Dgain = autoGain2;
                    velMultip = velMultip2;
                    stimParam3 = 1;
                }

                else if (t < senderWindow.InstStimParams.autoTimeSum)
                //else if (t < 20)
                {
                    if (senderWindow.switchParamsAuto.stopmode == 2 || senderWindow.switchParamsAuto.stopmode == 3)
                    {
                        stim1DclosedLoopContrast = 0;
                    }
                    closedLoop1Dgain = 0;
                    velMultip = 0;
                    stimParam3 = 2;

                }

                else if (t < senderWindow.InstStimParams.autoTimeSum + senderWindow.InstStimParams.autoTime1)
                //if (t < 30)
                {
                    if (closedLoop1Dgain != autoGain2)
                    {
                        reset_switchDisp();
                        switchDisp.gain1check = false;
                        switchDisp.gain2check = true;
                    }
                    closedLoop1Dgain = autoGain2;
                    velMultip = velMultip2;
                    stimParam3 = 3;
                }

                else if (t < senderWindow.InstStimParams.autoTimeSum + senderWindow.InstStimParams.autoTime1 + 2)
                //else if (t < 30)
                {
                    if (closedLoop1Dgain != autoGain1)
                    {
                        reset_switchDisp();
                        switchDisp.gain1check = true;
                        switchDisp.gain2check = false;
                    }
                    closedLoop1Dgain = 0;
                    velMultip = 0;
                    stimParam3 = 4;
                }
                else if (t < senderWindow.InstStimParams.autoTimeSum + senderWindow.InstStimParams.autoTime1 + 3) // move stimulus for .1 * autoTime1
                {
                    closedLoop1Dgain = 0;
                    velMultip = velMultip1;
                    stimParam3 = 5;

                }

                else
                {
                    if (closedLoop1Dgain != autoGain1)
                    {
                        reset_switchDisp();
                        switchDisp.gain1check = true;
                        switchDisp.gain2check = false;
                    }
                    closedLoop1Dgain = 0;
                    velMultip = 0;
                    stimParam3 = 6;
                }

            }


            if (senderWindow.InstStimParams.clmode == 126) // Visual Clamp assay /////////////
            {
                resetGratingParams();
                stim1DclosedLoopContrast = 180;
                t = (int)tt / 60;
                aa = (int)tt % 60;
                stimParam5 = (float)t + 1;
                stimParam5 = (float)t + 1;
                

                if (aa < 15)
                {
                    stimParam3 = 1;
                    closedLoop1Dgain = (autoGain1+autoGain2)/2;
                    velMultip = velMultip1;

                    bout_replay_play = false;
                    bout_replay = false;

                    if (gainTriggerSwitch == true && (swimPow[0] > 0 || swimPow[1] > 0) && tchange1 - tchange2 > 400)
                    {
                        bout_clock = 0;
                        bout_replay_rec = true;
                        numevents++;
                        tchange2 = tchange1;
                    }

                    if (bout_clock < 60 & bout_replay_rec)
                    {
                        replay_matrix[bout_clock] = stimParam1;
                        bout_clock++;
                    }
                    else
                    {
                        bout_replay_rec = false;
                    }

                }
                else
                {
                    bout_replay_rec = false;

                    stimParam3 = 2;
                    closedLoop1Dgain = (autoGain1 + autoGain2) / 2;
                    velMultip = velMultip1;

                    if (!bout_replay)
                    {
                        bout_replay = true;
                    }

                    if (gainTriggerSwitch == true && (swimPow[0] > 0 || swimPow[1] > 0) && tchange1 - tchange2 > 400)
                    {
                        bout_clock = 0;
                        bout_replay_play = true;
                        tchange2 = tchange1;
                    }

                    if (bout_clock < 60 & bout_replay_play)
                    {
                        bout_vel = replay_matrix[bout_clock];
                        bout_clock++;
                    }
                    else
                    {
                        bout_replay_play = false;
                    }
                }

            }

            if (senderWindow.InstStimParams.clmode ==124) // Gain OMR2  /////////////////
            {
                resetGratingParams();
                stim1DclosedLoopContrast = 180;


                int aa_old = aa;

                t  = (int)tt % 80;
                aa = (int)tt / 80;
                bb = (int)t / 40;
                cc = (int)(t % 40) / 20;

                stimParam3 = (float)cc + 1;
                stimParam4 = (float)bb + 1;
                stimParam5 = (float)aa + 1;


                if (stimParam3 == 1)
                {
                    closedLoop1Dgain = 0;
                    velMultip = 0;
                }
                else if (stimParam3 == 2)
                {
                    closedLoop1Dgain = autoGain2;
                    velMultip = velMultip2;
                }

            }

            if (senderWindow.InstStimParams.clmode == 111) // Gain OMR+Openloop+replay  ////////
            {
                resetGratingParams();
                stim1DclosedLoopContrast = 200;


                int aa_old = aa;

                t = (int)tt % 100;
                aa = (int)tt /100;

                stimParam3 = 0;
                stimParam4 = 0;
                stimParam5 = (float)aa + 1;


                if (t < 10)
                {
                    stimParam3 = 1;
                    closedLoop1Dgain = 0;
                    velMultip = 0;
                }
                else if (t < 40)
                {
                    stimParam3 = 2;
                    velMultip = velMultip2;
                    if (gainTriggerSwitch == true && (swimPow[0] > 0 || swimPow[1] > 0) && tchange1 - tchange2 > 400)
                    {
                        tchange2 = tchange1;
                        shuffler(shaul_ori2);
                    }

                    stimParam4 = shaul_ori2[1];

                    if (stimParam4 == 1)
                    {
                        closedLoop1Dgain = autoGain1;
                    }
                    if (stimParam4 == 2)
                    {
                        closedLoop1Dgain = (autoGain1 + autoGain2) / 2;
                    }
                    if (stimParam4 == 3)
                    {
                        closedLoop1Dgain = autoGain2;
                    }

                    gain = stimParam4;
                }
                else if (t < 60)
                {
                    stimParam3 = 3;
                    closedLoop1Dgain = 0;
                    velMultip = velMultip2;
                }
                else if (t < 70)
                {
                    stimParam3 = 4;
                    closedLoop1Dgain = 0;
                    velMultip = 0;
                }
                else if (t<100)
                {
                    stimParam3 = 5;
                    closedLoop1Dgain = autoGain2;
                    velMultip = velMultip2;
                }

            }

            if (senderWindow.InstStimParams.clmode == 125) // Gain OMR2  /////////////////
            {
                resetGratingParams();
                stim1DclosedLoopContrast = 180;


                int aa_old = aa;

                t = (int)tt % 80;
                aa = (int)tt / 80;
                bb = (int)t / 40;
                cc = (int)(t % 40) / 20;

                stimParam3 = (float)cc + 1;
                stimParam4 = (float)bb + 1;
                stimParam5 = (float)aa + 1;


                if (stimParam3 == 1)
                {
                    closedLoop1Dgain = 0;
                    velMultip = 0;
                }
                else if (stimParam3 == 2)
                {
                    closedLoop1Dgain = autoGain2;
                    velMultip = 0;
                }

            }

            if (senderWindow.InstStimParams.clmode == 127) // Gain Switch 2  ////////////////
            {
                resetGratingParams();
                stim1DclosedLoopContrast = 180;


                int aa_old = aa;

                t = (int)tt % 90;
                aa = (int)tt / 90;
                bb = (int)t / 30;
                ;
                stimParam5 = (float)aa + 1;
                stimParam3 = (float)bb + 1;


                if (stimParam3 == 1)
                {
                    closedLoop1Dgain = 0;
                    velMultip = velMultip2;
                }
                else if (stimParam3 == 2)
                {
                    closedLoop1Dgain = autoGain2;
                    velMultip = velMultip2;
                }

                else if (stimParam3 == 3)
                {
                    closedLoop1Dgain = 0;
                    velMultip = -0.2;
                }

            }


            if (senderWindow.InstStimParams.clmode == 54) // forgetting paradim  ///////////////
            {
                resetGratingParams();
                double ta = tt % (int)200;
                int aa = (int)tt / (int)200;
                stimParam5 = (float)aa + 1;

                if (ta < 60)
                {
                    stimParam4 = 1;
                    if (ta < 20)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = autoGain1;
                        velMultip = velMultip1;
                        stimParam3 = 1;
                    }
                    else if (ta < 50)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip2;
                        stimParam3 = 2;
                    }
                    else if (ta < 55)
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = false;
                        }
                        stim1DclosedLoopContrast = 0;
                        blevel = 180;
                        closedLoop1Dgain = 0;
                        velMultip = 0;
                        stimParam3 = 3;

                        /*
                        closedLoop1Dgain = 0;
                        velMultip = -0.3;
                        stimParam3 = 3;
                         * */


                    }
                    else
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = (autoGain1 + autoGain2) / 2;
                        velMultip = velMultip1;
                        stimParam3 = 4;
                    }


                }
                else if (ta < 125)
                {
                    stimParam4 = 2;
                    if (ta < 80)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = autoGain1;
                        velMultip = velMultip1;
                        stimParam3 = 1;
                    }
                    else if (ta < 110)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip2;
                        stimParam3 = 2;
                    }
                    else if (ta < 120)
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = false;
                        }
                        stim1DclosedLoopContrast = 0;
                        blevel = 180;
                        closedLoop1Dgain = 0;
                        velMultip = 0;
                        stimParam3 = 3;
                        /*
                        closedLoop1Dgain = 0;
                        velMultip = -0.3;
                        stimParam3 = 3;
                        */


                    }
                    else
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = (autoGain1 + autoGain2) / 2;
                        velMultip = velMultip1;
                        stimParam3 = 4;
                    }

                }
                else
                {
                    stimParam4 = 3;
                    if (ta < 145)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = autoGain1;
                        velMultip = velMultip1;
                        stimParam3 = 1;
                    }
                    else if (ta < 175)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip2;
                        stimParam3 = 2;
                    }
                    else if (ta < 195)
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = false;
                        }
                        stim1DclosedLoopContrast = 0;
                        blevel = 180;
                        closedLoop1Dgain = 0;
                        velMultip = 0;
                        stimParam3 = 3;
                        /*
                        closedLoop1Dgain = 0;
                        velMultip = -0.3;
                        stimParam3 = 3;
                        */

                    }
                    else
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        stim1DclosedLoopContrast = 200;
                        closedLoop1Dgain = (autoGain1 + autoGain2) / 2;
                        velMultip = velMultip1;
                        stimParam3 = 4;
                    }
                }
               

            }
            
            if (senderWindow.InstStimParams.clmode == 156) // high training  ////////////////
            {
                resetGratingParams();
                double ta = tt % (int)157;
                int aa = (int)tt / (int)157;
                stimParam5 = (float)aa + 1;
                stim1DclosedLoopContrast = 200;

                if (ta < 42)
                {
                    stimParam4 = 1;
                    if (ta < 20)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        closedLoop1Dgain = autoGain1;
                        velMultip = velMultip1;
                        stimParam3 = 1;
                    }
                    else if (ta < 27)
                    {
                        if (closedLoop1Dgain != autoGain2)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip1;
                        stimParam3 = 2;
                    }
                    else if (ta < 37)
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = false;
                        }
                        stim1DclosedLoopContrast = 0;
                        // blevel = 180;
                        // closedLoop1Dgain = 0;
                        velMultip = -0.2;
                        stimParam3 = 3;

                    }
                    else
                    {
                        if (closedLoop1Dgain != ((autoGain1 + autoGain2) / 2))
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = (autoGain1 + autoGain2) / 2;
                        velMultip = velMultip1;
                        stimParam3 = 4;
                    }


                }
                else if (ta < 92)
                {
                    stimParam4 = 2;
                    if (ta < 62)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        closedLoop1Dgain = autoGain1;
                        velMultip = velMultip1;
                        stimParam3 = 1;
                    }
                    else if (ta < 77)
                    {
                        if (closedLoop1Dgain != autoGain2)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip1;
                        stimParam3 = 2;
                    }
                    else if (ta < 87)
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = false;
                        }
                        closedLoop1Dgain = 0;
                        velMultip = -0.2;
                        stimParam3 = 3;
                        stim1DclosedLoopContrast = 0;
                        //blevel = 180;

                    }
                    else
                    {
                        if (closedLoop1Dgain != ((autoGain1 + autoGain2) / 2))
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = (autoGain1 + autoGain2) / 2;
                        velMultip = velMultip1;
                        stimParam3 = 4;
                    }

                }
                else
                {
                    stimParam4 = 3;
                    if (ta < 112)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        closedLoop1Dgain = autoGain1;
                        velMultip = velMultip1;
                        stimParam3 = 1;
                    }
                    else if (ta < 142)
                    {
                        if (closedLoop1Dgain != autoGain2)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip1;
                        stimParam3 = 2;
                    }
                    else if (ta < 152)
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = false;
                        }
                        closedLoop1Dgain = 0;
                        velMultip = -0.2;
                        stimParam3 = 3;
                        stim1DclosedLoopContrast = 0;
                        //blevel = 180;

                    }
                    else
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = (autoGain1 + autoGain2) / 2;
                        velMultip = velMultip1;
                        stimParam3 = 4;
                    }
                }
            }
            if (senderWindow.InstStimParams.clmode == 56) // low training  ////////////////
            {
                resetGratingParams();
                double ta = tt % (int)157;
                int aa = (int)tt / (int)157;
                stimParam5 = (float)aa + 1;
                stim1DclosedLoopContrast = 200;

                if (ta < 42)
                {
                    stimParam4 = 1;
                    if (ta < 20)
                    {
                        if (closedLoop1Dgain != autoGain2)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip1;
                        stimParam3 = 1;
                    }
                    else if (ta < 27)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        closedLoop1Dgain = autoGain1;
                        velMultip = velMultip1;
                        stimParam3 = 2;
                    }
                    else if (ta < 37)
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = false;
                        }
                        //stim1DclosedLoopContrast = 0;
                        // blevel = 180;
                        // closedLoop1Dgain = 0;
                        velMultip = 0;
                        stimParam3 = 3;

                    }
                    else
                    {
                        if (closedLoop1Dgain != ((autoGain1 + autoGain2) / 2))
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = (autoGain1 + autoGain2) / 2;
                        velMultip = velMultip1;
                        stimParam3 = 4;
                    }


                }
                else if (ta < 92)
                {
                    stimParam4 = 2;
                    if (ta < 62)
                    {
                        if (closedLoop1Dgain != autoGain2)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip1;
                        stimParam3 = 1;
                    }
                    else if (ta < 77)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        closedLoop1Dgain = autoGain1;
                        velMultip = velMultip1;
                        stimParam3 = 2;
                    }
                    else if (ta < 87)
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = false;
                        }
                        closedLoop1Dgain = 0;
                        velMultip = 0;
                        stimParam3 = 3;
                        //stim1DclosedLoopContrast = 0;
                        //blevel = 180;

                    }
                    else
                    {
                        if (closedLoop1Dgain != ((autoGain1 + autoGain2) / 2))
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = (autoGain1 + autoGain2) / 2;
                        velMultip = velMultip1;
                        stimParam3 = 4;
                    }

                }
                else
                {
                    stimParam4 = 3;
                    if (ta < 112)
                    {
                        if (closedLoop1Dgain != autoGain2)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip1;
                        stimParam3 = 1;
                    }
                    else if (ta < 142)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        closedLoop1Dgain = autoGain1;
                        velMultip = velMultip1;
                        stimParam3 = 2;
                    }
                    else if (ta < 152)
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = false;
                        }
                        closedLoop1Dgain = 0;
                        velMultip = 0;
                        stimParam3 = 3;
                        //stim1DclosedLoopContrast = 0;
                        //blevel = 180;

                    }
                    else
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = (autoGain1 + autoGain2) / 2;
                        velMultip = velMultip1;
                        stimParam3 = 4;
                    }
                }
            }

            if (senderWindow.InstStimParams.clmode == 1056) // low training shot delay ////////////////
            {
                resetGratingParams();
                double ta = tt % (int)157;
                int aa = (int)tt / (int)157;
                stimParam5 = (float)aa + 1;
                stim1DclosedLoopContrast = 200;

                if (ta < 42)
                {
                    stimParam4 = 1;
                    if (ta < 20)
                    {
                        if (closedLoop1Dgain != autoGain2)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip1;
                        stimParam3 = 1;
                    }
                    else if (ta < 27)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        closedLoop1Dgain = autoGain1;
                        velMultip = velMultip1;
                        stimParam3 = 2;
                    }
                    else if (ta < 32)
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = false;
                        }
                        //stim1DclosedLoopContrast = 0;
                        // blevel = 180;
                        // closedLoop1Dgain = 0;
                        velMultip = 0;
                        stimParam3 = 3;

                    }
                    else
                    {
                        if (closedLoop1Dgain != ((autoGain1 + autoGain2) / 2))
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = (autoGain1 + autoGain2) / 2;
                        velMultip = velMultip1;
                        stimParam3 = 4;
                    }


                }
                else if (ta < 92)
                {
                    stimParam4 = 2;
                    if (ta < 62)
                    {
                        if (closedLoop1Dgain != autoGain2)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip1;
                        stimParam3 = 1;
                    }
                    else if (ta < 77)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        closedLoop1Dgain = autoGain1;
                        velMultip = velMultip1;
                        stimParam3 = 2;
                    }
                    else if (ta < 82)
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = false;
                        }
                        closedLoop1Dgain = 0;
                        velMultip = 0;
                        stimParam3 = 3;
                        //stim1DclosedLoopContrast = 0;
                        //blevel = 180;

                    }
                    else
                    {
                        if (closedLoop1Dgain != ((autoGain1 + autoGain2) / 2))
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = (autoGain1 + autoGain2) / 2;
                        velMultip = velMultip1;
                        stimParam3 = 4;
                    }

                }
                else
                {
                    stimParam4 = 3;
                    if (ta < 112)
                    {
                        if (closedLoop1Dgain != autoGain2)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip1;
                        stimParam3 = 1;
                    }
                    else if (ta < 142)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        closedLoop1Dgain = autoGain1;
                        velMultip = velMultip1;
                        stimParam3 = 2;
                    }
                    else if (ta < 147)
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = false;
                        }
                        closedLoop1Dgain = 0;
                        velMultip = 0;
                        stimParam3 = 3;
                        //stim1DclosedLoopContrast = 0;
                        //blevel = 180;

                    }
                    else
                    {
                        if (closedLoop1Dgain != 0)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = (autoGain1 + autoGain2) / 2;
                        velMultip = velMultip1;
                        stimParam3 = 4;
                    }
                }
            }

            if (senderWindow.InstStimParams.clmode == 55) // Gain OMR2  ///////////////////
            {
                resetGratingParams();
                stim1DclosedLoopContrast = 180;


                int aa_old = aa;

                t = (int)tt % 150;
                aa = (int)tt / 150;
                bb = (int)tt / 30;
                stimParam5 = (float)aa + 1;

                int ee = (int)stimSequence[bb];
                stimParam3 = (float)ee;


                if (ee == 1)
                {
                    stim1DclosedLoopContrast = 180;
                    closedLoop1Dgain = 0;
                    velMultip = velMultip1;
                }
                else if (ee == 2)
                {
                    stim1DclosedLoopContrast = 180;
                    closedLoop1Dgain = autoGain1;
                    velMultip = velMultip1;
                }
                else if (ee == 3)
                {
                    stim1DclosedLoopContrast = 180;
                    closedLoop1Dgain = autoGain2;
                    velMultip = velMultip1;
                }
                else if (ee == 4)
                {
                    stim1DclosedLoopContrast = 180;
                    closedLoop1Dgain = 0;
                    velMultip = -1;
                }
                else if (ee == 5)
                {
                    stim1DclosedLoopContrast = 0;
                    closedLoop1Dgain = 0;
                    velMultip = 0;
                    blevel = 180;

                }


            }

            if (senderWindow.InstStimParams.clmode == 555) // Gain OMR2  ///////////////
            {
                resetGratingParams();
                stim1DclosedLoopContrast = 180;


                int aa_old = aa;

                t = (int)tt % 90;
                aa = (int)tt / 90;
                bb = (int)tt / 30;
                stimParam5 = (float)aa + 1;

                int ee = (int)stimSequence[bb];
                stimParam3 = (float)ee;


                if (ee == 1)
                {
                    closedLoop1Dgain = 0;
                    velMultip = velMultip1;
                }
                else if (ee == 2)
                {
                    closedLoop1Dgain = autoGain2;
                    velMultip = velMultip1;
                }
                else if (ee == 3)
                {
                    closedLoop1Dgain = 0;
                    velMultip = 0;
                }

            }



            if (senderWindow.InstStimParams.clmode == 2 || senderWindow.InstStimParams.clmode == 3)  //////////////
            {
                resetGratingParams();
                double ta2;
                stimParam4old = stimParam4;
                ta2 = tt % (int)senderWindow.switchParamsAuto.test_durinc[2];

                if (ta2 < senderWindow.switchParamsAuto.test_durinc[0])
                {
                    stimParam4 = 1;
                    ta = ta2;
                }
                else if (ta2 < senderWindow.switchParamsAuto.test_durinc[1])
                {
                    stimParam4 = 2;
                    ta = ta2 - senderWindow.switchParamsAuto.test_durinc[0];
                }
                else
                {
                    stimParam4 = 3;
                    ta = ta2 - senderWindow.switchParamsAuto.test_durinc[1];
                }
                

                if (stimParam4 != stimParam4old)
                {
                    senderWindow.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        if (senderWindow.InstStimParams.clmode==2)
                            senderWindow.switchParamsAuto = senderWindow.update_phasedur2(senderWindow.switchParamsAuto, "resting", (int) stimParam4 - 1);
                        if (senderWindow.InstStimParams.clmode == 3)
                            senderWindow.switchParamsAuto = senderWindow.update_phasedur2(senderWindow.switchParamsAuto, "training", (int) stimParam4 - 1);
                    }));
                }

                memorytest_switch(ta);

            }
            
            if (senderWindow.InstStimParams.clmode==4) // simple switch  ////////////////
            {
                resetGratingParams();

                t = (int)tt % (int)senderWindow.InstStimParams.autoTimeSum;

                if (t < senderWindow.InstStimParams.autoTime1)
                {
                    if (closedLoop1Dgain != autoGain1)
                    {
                        reset_switchDisp();
                        switchDisp.gain1check = true;
                        switchDisp.gain2check = false;
                    }
                    closedLoop1Dgain = autoGain1;
                    velMultip = velMultip1;
                    stimParam3 = 1;
                }
                else 
                {
                    if (closedLoop1Dgain != autoGain2)
                    {
                        reset_switchDisp();
                        switchDisp.gain1check = false; 
                        switchDisp.gain2check = true;
                    }
                    closedLoop1Dgain = autoGain2;
                    velMultip = velMultip2;
                    stimParam3 = 2;
                }


            }

            if (senderWindow.InstStimParams.clmode == 1234567)  // Jing's flashing trial structure
            {
                double t = (stopwatch.ElapsedMilliseconds/1000.0) % 
                    (int)(senderWindow.InstStimParams.flash_iti 
                    + senderWindow.InstStimParams.flash_dur);  // trial time

                resetGratingParams();  // reset grating angle, motion and gain

                /*           *\
                  CLOSED LOOP
                \*           */

                if (t < senderWindow.InstStimParams.flash_iti)  // start with closed loop first ("ITI")
                {
                    int t_iti = (int)tt % (int)senderWindow.InstStimParams.autoTimeSum;  // ITI time
                    stim1DclosedLoopContrast = 200;   // add contrast back

                    if (t_iti < senderWindow.InstStimParams.autoTime1)
                    {
                        if (closedLoop1Dgain != autoGain1)  // set 1 
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        closedLoop1Dgain = autoGain1;
                        velMultip = velMultip1;
                        stimParam3 = 1;
                    }

                    else                                    // set 2
                    {
                        if (closedLoop1Dgain != autoGain2)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip2;
                        stimParam3 = 2;
                    }
                    stimParam4 = 0;  // do not record blevel in close loop
                }

                /*        *\
                  FLASHING
                \*        */

                else
                {

                    t_flash =  t - senderWindow.InstStimParams.flash_iti;  // flash time
                    // Console.WriteLine("flash time: {0}", t_flash);

                    stim1DclosedLoopContrast = 0; // remove constrast to remove gratings

                    if (senderWindow.switchParamsAuto.stopmode == 2
                        || senderWindow.switchParamsAuto.stopmode == 3)
                    {
                        stim1DclosedLoopContrast = 0;
                    }

                    // change background color at different times so it flashes

                    int numFlashes = (int)senderWindow.InstStimParams.flash_freq 
                        * (int)senderWindow.InstStimParams.flash_dur;
                    Console.WriteLine("Brief? {0}", senderWindow.InstStimParams.brief);

                    /* BRIEF STIMULUS */

                    if (senderWindow.InstStimParams.brief == true)
                    {

                        stimParam4 = blevel;
                        try
                        {
                            double isi = 1 / senderWindow.InstStimParams.flash_freq;

                            if (senderWindow.InstStimParams.jitter == true)
                            {
                                Random rnd = new Random();
                                jit = rnd.Next((int)-senderWindow.InstStimParams.flash_jitter * 1000, (int)senderWindow.InstStimParams.flash_jitter * 1000);
                                jit = jit/1000.0;
                            }
                            else
                            { jit = 0;}

                            double cycle_remain = t_flash % (isi+jit);
                            cur_remain = cycle_remain;
                            // Console.WriteLine(cur_remain);

                            if (cur_remain < pre_remain)
                            {
                                Console.WriteLine("Brief Triggered [{0} s]", t_flash);
                                blevel = senderWindow.InstStimParams.flash_col2;  // set col2 as stimulus
                                stimParam4 = blevel;
                                stim_t = cur_remain;  // save time
                                off_bool = true;
                                // Console.WriteLine("stim_t: {0}", stim_t);
                                // Console.WriteLine("off_t: {0}", stim_t + 0.015);
                            }

                            if (cur_remain >= stim_t + (senderWindow.InstStimParams.flash_brief/1000) &&  off_bool)  // switch off stim after 15 ms
                            {
                                Console.WriteLine("Off Triggered [{0} s]", t_flash);
                                blevel = senderWindow.InstStimParams.flash_col1;  // set col1 as stimulus
                                stimParam4 = blevel;
                                off_bool = false;
                            }
                            pre_remain = cycle_remain;
                        }
                        catch { }
                    }

                    /* ALTERNATING STIMULUS */

                    else{
                        try
                        {
                            double isi = 1 / (2 * senderWindow.InstStimParams.flash_freq);

                            if (senderWindow.InstStimParams.jitter == true)
                            {
                                Random rnd = new Random();
                                jit = rnd.Next((int)-senderWindow.InstStimParams.flash_jitter * 1000, (int)senderWindow.InstStimParams.flash_jitter * 1000);
                                jit = jit / 1000.0;
                            }
                            else
                            { jit = 0; }

                            double cycle_rem = t_flash % (isi+jit);
                            cur_rem = cycle_rem;

                            if (cur_rem < pre_rem)
                            {
                                Console.WriteLine("Triggered [{0} s]", t_flash);

                                if (blevel == senderWindow.InstStimParams.flash_col1 || blevel == 128)
                                {
                                    blevel = senderWindow.InstStimParams.flash_col2;
                                    stimParam4 = blevel;
                                }
                                else if (blevel == senderWindow.InstStimParams.flash_col2 || blevel == 128)
                                {
                                    blevel = senderWindow.InstStimParams.flash_col1;
                                    stimParam4 = blevel;
                                }

                            }
                            pre_rem = cycle_rem;
                        }
                        catch
                        { }
                    }


                    closedLoop1Dgain = 0;
                    velMultip = 0;
                    stimParam3 = 3;             // set 3

                }

            }

            if (senderWindow.InstStimParams.clmode==5) // speed tuning;////////
            {
                double[] speedlist = { -3, -2, -1, 0, 1, 2, 3, 0 };

                int t2_old = t2;     


                resetGratingParams();

                t1 = (int)tt % 20;
                t2 = (int)(tt % 160) / 20;

                if (t2 == 0 && t2_old!= t2 )
                {
                    for (int ss=0;ss<shufflelist1.Length; ss++)
                    {
                        shufflelist2[ss]=shufflelist1[ss];
                    }
                    shuffler(shufflelist2);
                }

                if (t1 < 10)
                {
                    if (senderWindow.switchParamsAuto.stopmode == 2 || senderWindow.switchParamsAuto.stopmode == 3)
                    {
                        stim1DclosedLoopContrast = 0;
                    }
                    velMultip = 0;
                }
                else
                {
                    stim1DclosedLoopContrast = 200;
                    velMultip = speedlist[shufflelist2[t2]-1];
                }

                stimParam3 = shufflelist2[t2];
            }

            if (senderWindow.InstStimParams.clmode == 314) // simple switch + stop ////////////////
            {
                resetGratingParams();

                t = (int)tt % (int)senderWindow.InstStimParams.autoTimeSum;

                if (t < senderWindow.InstStimParams.autoTime1)
                {
                    if (closedLoop1Dgain != autoGain1)
                    {
                        reset_switchDisp();
                        switchDisp.gain1check = true;
                        switchDisp.gain2check = false;
                    }
                    closedLoop1Dgain = autoGain1;
                    velMultip = velMultip1;
                    stimParam3 = 1;
                }
                else
                {
                    if (senderWindow.switchParamsAuto.stopmode == 2 || senderWindow.switchParamsAuto.stopmode == 3)
                    {
                        stim1DclosedLoopContrast = 0;
                    }
                    closedLoop1Dgain = 0;
                    velMultip = 0;
                    stimParam3 = 2;
                }


            }

            if (senderWindow.InstStimParams.clmode == 999999) // OMR_loom  ///////
            {
                resetGratingParams();
                double ta = tt % (int)(2 * senderWindow.InstStimParams.autoTimeSum);
                stimParam4 = 1;
                loom = true;
                if (ta < senderWindow.InstStimParams.autoTime1)
                {
                    senderWindow.InstStimParams.moveobject = 1;

                    if (closedLoop1Dgain != autoGain1)
                    {
                        reset_switchDisp();
                        switchDisp.gain1check = true;
                        switchDisp.gain2check = false;
                    }
                    stim1DclosedLoopContrast = 200;
                    blevel = 180;
                    closedLoop1Dgain = autoGain1;
                    velMultip = velMultip1;
                    stimParam3 = 1;
                }
                else if (ta < senderWindow.InstStimParams.autoTimeSum)
                {
                    if (closedLoop1Dgain != autoGain1)
                    {
                        reset_switchDisp();
                        switchDisp.gain1check = true;
                        switchDisp.gain2check = false;
                    }

                    closedLoop1Dgain = 0;
                    velMultip = 0;
                    stimParam3 = 2;
                }
                else if (ta < senderWindow.InstStimParams.autoTimeSum + senderWindow.InstStimParams.autoTime1)
                {
                    if (closedLoop1Dgain != autoGain2)
                    {
                        reset_switchDisp();
                        switchDisp.gain1check = false;
                        switchDisp.gain2check = true;
                    }

                    stim1DclosedLoopContrast = 200;
                    closedLoop1Dgain = autoGain2;
                    velMultip = velMultip1;
                    stimParam3 = 3;


                }
                else if (ta < senderWindow.InstStimParams.autoTimeSum + senderWindow.InstStimParams.autoTime1 + 2)
                {
                    if (closedLoop1Dgain != 0)
                    {
                        reset_switchDisp();
                        switchDisp.gain1check = false;
                        switchDisp.gain2check = false;
                    }
                    velMultip = 0;
                    stim1DclosedLoopContrast = 200;
                    closedLoop1Dgain = 0;
                    stimParam3 = 4;


                    t_old1 = 0;
                    shuffler(beepShuf);
                }
                else if (ta < senderWindow.InstStimParams.autoTimeSum
                    + senderWindow.InstStimParams.autoTime1 + 4)
                {
                    if (closedLoop1Dgain != 0)
                    {
                        reset_switchDisp();
                        switchDisp.gain1check = false;
                        switchDisp.gain2check = false;
                    }


                    if (loom == true)
                    {
                        if (t_old1 == 0)
                        {
                            t_old1 = DTT / 1000.0;
                        }

                        if (beepShuf[1] == 1)
                        {

                            tc = DTT / 1000.0 - t_old1;
                            senderWindow.InstStimParams.moveobject = 7;

                            senderWindow.InstStimParams.LeftSizeX = 20;
                            senderWindow.InstStimParams.LeftSizeY = 20;
                            senderWindow.InstStimParams.LeftSpeed = 1;

                            xx_ = 0.5f;
                            yy_ = 0.5f;

                            senderWindow.InstStimParams.dotSizeX = tc * senderWindow.InstStimParams.LeftSizeX * senderWindow.InstStimParams.LeftSpeed;
                            senderWindow.InstStimParams.dotSizeY = tc * senderWindow.InstStimParams.LeftSizeY * senderWindow.InstStimParams.LeftSpeed;

                            stimParam3 = (float)(senderWindow.InstStimParams.dotSizeX);
                            stimParam4 = 1;

                            if (senderWindow.InstStimParams.dotSizeX > 40)
                            {
                                loom = false;
                                senderWindow.InstStimParams.dotSizeX = 40;
                                senderWindow.InstStimParams.dotSizeY = 40;
                            }
                        }
                    }
                }
                else
                {
                    if (closedLoop1Dgain != 0)
                    {
                        reset_switchDisp();
                        switchDisp.gain1check = false;
                        switchDisp.gain2check = false;
                    }
                    senderWindow.InstStimParams.moveobject = 1;
                    velMultip = 0;
                    stim1DclosedLoopContrast = 200;
                    closedLoop1Dgain = 0;
                    stimParam3 = 4;
                }
            }

            if (senderWindow.InstStimParams.clmode == 315) // simple switch stop fast movement ////////////////
            {
                resetGratingParams();

                int t = (int)tt % (int)senderWindow.InstStimParams.autoTimeSumDouble;
                //int t = (int)tt % 40;
                int t2 = (int)tt2 % (int)senderWindow.InstStimParams.autoTimeSum
                    ;
                //int t2 = (int)tt2 % 20;

                
                
                    if (t < senderWindow.InstStimParams.autoTime1)
                    //if (t < 10)
                    {
                        if (closedLoop1Dgain != autoGain2)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip2;
                        stimParam3 = 1;
                    }

                    else if (t < senderWindow.InstStimParams.autoTimeSum)
                    //else if (t < 20)
                    {
                        if (senderWindow.switchParamsAuto.stopmode == 2 || senderWindow.switchParamsAuto.stopmode == 3)
                        {
                            stim1DclosedLoopContrast = 0;
                        }
                        closedLoop1Dgain = 0;
                        velMultip = 0;
                        stimParam3 = 2;

                    }

                    else if (t < senderWindow.InstStimParams.autoTimeSum + senderWindow.InstStimParams.autoTime1)
                    //if (t < 30)
                    {
                        if (closedLoop1Dgain != autoGain2)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip2;
                        stimParam3 = 3;
                    }

                    else if (t < senderWindow.InstStimParams.autoTimeSum + senderWindow.InstStimParams.autoTime1 + 2)
                    //else if (t < 30)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        closedLoop1Dgain = 0;
                        velMultip = 0;
                        stimParam3 = 4;
                    }
                    else if (t < senderWindow.InstStimParams.autoTimeSum + senderWindow.InstStimParams.autoTime1 + 3) // move stimulus for .1 * autoTime1
                    {
                        closedLoop1Dgain = 0;
                        velMultip = velMultip1;
                        stimParam3 = 5;

                    }

                    else
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        closedLoop1Dgain = 0;
                        velMultip = 0;
                        stimParam3 = 6;
                    }
                

                /*else
                {
                    if (t < senderWindow.InstStimParams.autoTime1)
                    //if (t < 10)
                    {
                        if (closedLoop1Dgain != autoGain2)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = false;
                            switchDisp.gain2check = true;
                        }
                        closedLoop1Dgain = autoGain2;
                        velMultip = velMultip2;
                        stimParam3 = 1;
                    
                    }

                    else if (t < senderWindow.InstStimParams.autoTimeSum)
                    //else if (t < 20)
                    {
                        if (senderWindow.switchParamsAuto.stopmode == 2 || senderWindow.switchParamsAuto.stopmode == 3)
                        {
                            stim1DclosedLoopContrast = 0;
                        }
                        closedLoop1Dgain = 0;
                        velMultip = 0;
                        stimParam3 = 2; 
                    
                    
                    }

                    else if (t < senderWindow.InstStimParams.autoTimeSum + senderWindow.InstStimParams.autoTime1)
                    //else if (t < 30)
                    {
                        if (closedLoop1Dgain != autoGain1)
                        {
                            reset_switchDisp();
                            switchDisp.gain1check = true;
                            switchDisp.gain2check = false;
                        }
                        if (t2 < (senderWindow.InstStimParams.autoTime1 / 2)) // move stimulus for .05 * autoTime1
                        //if (t2 < (senderWindow.InstStimParams.autoTime1)) // move stimulus for .1 * autoTime1
                        {
                            closedLoop1Dgain = 0;
                            velMultip = velMultip1;
                            stimParam3 = 3;
                        }
                        else if (t2 < senderWindow.InstStimParams.autoTimeSum) // stop stimulus for remainder of .1 * autoTimeSum
                        {
                            if (senderWindow.switchParamsAuto.stopmode == 2 || senderWindow.switchParamsAuto.stopmode == 3)
                            {
                                stim1DclosedLoopContrast = 0;
                            }
                            closedLoop1Dgain = 0;
                            velMultip = 0;
                            stimParam3 = 4;
                        }
                    }
                    else
                    {
                        if (senderWindow.switchParamsAuto.stopmode == 2 || senderWindow.switchParamsAuto.stopmode == 3)
                        {
                            stim1DclosedLoopContrast = 0;
                        }
                        closedLoop1Dgain = 0;
                        velMultip = 0;
                        stimParam3 = 5;
                    
                    }
                }*/
                
            }

            /*
            if (senderWindow.InstStimParams.clmode == 6) // Orientation tuning
            {
                resetGratingParams();
                int t1 = (int)tt % 15;
                int t2 = (int)(tt % 180) / 15;

                if (t1 < 10)
                {
                    if (senderWindow.switchParamsAuto.stopmode == 2 || senderWindow.switchParamsAuto.stopmode == 3)
                    {
                        stim1DclosedLoopContrast = 0;
                    }
                    velMultip = 0;
                }
                else
                {
                    velMultip = 2;
                    stim1DclosedLoopContrast = 200;

                    double an= (double)t2/6;
                    displayAngle = an*Math.PI;
                    moveAngle    = an*Math.PI;
                }
            } 
             */
            
            if (senderWindow.InstStimParams.clmode == 6) // Orientation tuning  //////////////
            {
                resetGratingParams();
                int t1 = (int)tt % 20;
                int t2 = (int)(tt % 80) / 20;
                stimParam4 = (float)t2 + 1;
                stimParam5 = (float)(tt / 80) + 1;

                if (t1 < 15)
                {
                    if (senderWindow.switchParamsAuto.stopmode == 2 || senderWindow.switchParamsAuto.stopmode == 3)
                    {
                        stim1DclosedLoopContrast = 0;
                    }
                    velMultip = 0;
                    stimParam3 = 1;
                }
                else
                {
                    velMultip = 2;
                    stim1DclosedLoopContrast = 200;

                    double an = (double)t2 / 4;
                    displayAngle = an * 2  *Math.PI;
                    moveAngle = an * 2* Math.PI;
                    stimParam3 = 2;
                }
            }


            if (senderWindow.InstStimParams.clmode == 7) // spatial frequency  ////////////////////
            {
                resetGratingParams();
                int t1 = (int)tt % 20;
                int t2 = (int)(tt % 240) / 20;

                if (t1 < 10)
                {
                    if (senderWindow.switchParamsAuto.stopmode == 2 || senderWindow.switchParamsAuto.stopmode == 3)
                    {
                        stim1DclosedLoopContrast = 0;
                    }
                    velMultip = 0;
                }
                else
                {
                    stim1DclosedLoopContrast = 200;
                    if (t2 < 6)
                    {
                        velMultip = 2;
                        double an = ((double)t2 + 1) / 3;
                        dH = (float)an * 0.15f / 2.2f;
                        velMultip = 2 * an; ;
                    }
                    else
                    {
                        double an = ((double)t2 -5) / 3;
                        dH = (float)an * 0.15f / 2.2f;
                        velMultip = -2 * an; ;
                    }
                }
            }

          

            

              
        }

        public void disp_shaul_stim(int mode)
        {
            velMultip = 0;
            stim1DclosedLoopContrast = 200;
            stimParam3 = 1;

            if (mode == 1)
            {
                senderWindow.InstStimParams.moveobject = 1;
                displayAngle = 0.5 * Math.PI;
                moveAngle = 0.5 * Math.PI;

            }
            if (mode == 2)
            {
                senderWindow.InstStimParams.moveobject = 1;
                displayAngle = Math.PI;
                moveAngle = Math.PI;
            }

            if (mode == 3)
            {
                displayAngle = Math.PI;
                moveAngle = Math.PI;
                senderWindow.InstStimParams.moveobject = 3;

            }
            if (mode == 4)
            {
                displayAngle = Math.PI;
                moveAngle = Math.PI;
                senderWindow.InstStimParams.moveobject = 2;

            }


        }



        public void memorytest_switch(double ta)
        {
            int aa = (int)tt / (int)senderWindow.switchParamsAuto.phasedurinc[5];
            stimParam5 = (float)aa + 1;
            if (ta < senderWindow.switchParamsAuto.phasedurinc[0])
            {
                if (stimParam3 != 1)
                {
                    reset_switchDisp();
                    switchDisp.gain1check = true;
                    switchDisp.Phase1Time = false;
                }

                stim1DclosedLoopContrast = 200;
                closedLoop1Dgain = autoGain1;
                velMultip = velMultip1;
                stimParam3 = 1;
                if (senderWindow.switchParamsAuto.stopmode == 3 && (ta % 5 == 1) && aa % 2 == 0)
                {
                    closedLoop1Dgain = 0;
                }


            }
            else if (ta < senderWindow.switchParamsAuto.phasedurinc[1])
            {

                if (stimParam3 != 2)
                {
                    reset_switchDisp();
                    switchDisp.Phase2Time = false;
                }

                closedLoop1Dgain = 0;
                velMultip = 0;// 1;
                stimParam3 = 2;

                if (senderWindow.switchParamsAuto.stopmode == 2 || senderWindow.switchParamsAuto.stopmode == 3)
                {
                    stim1DclosedLoopContrast = 0;
                    blevel = 180;
                }

                /*
                if (senderWindow.switchParamsAuto.stopmode == 3)
                {
                    aa = (int)tt / (int)senderWindow.switchParamsAuto.phasedurinc[5];

                    if (aa % 2 == 0)
                    {
                        if (ta < senderWindow.switchParamsAuto.phasedurinc[0] + 1)
                        {
                            stim1DclosedLoopContrast = 0;
                            blevel = 200;
                        }
                        else if (ta < senderWindow.switchParamsAuto.phasedurinc[0] + 1.5)
                        {
                            stim1DclosedLoopContrast = 200;
                            closedLoop1Dgain = 0;
                            velMultip = 1;
                        }
                        else
                        {
                            stim1DclosedLoopContrast = 0;
                            blevel = 200;
                        }
                    }
                    else
                    {
                        stim1DclosedLoopContrast = 0;
                        blevel = 200;
                    }
                }
                 * */

                if (senderWindow.switchParamsAuto.stopmode == 4)
                {
                    velMultip = -1;
                }

            }
            else if (ta <= senderWindow.switchParamsAuto.phasedurinc[2])
            {
                if (stimParam3 != 3)
                {
                    reset_switchDisp();
                    switchDisp.Phase3Time = false;
                }
                stimParam3 = 3;

                if (senderWindow.openprobe)
                {
                    if ((ta - senderWindow.switchParamsAuto.phasedurinc[1]) < 1.5)
                    {
                        stim1DclosedLoopContrast = 180;
                        closedLoop1Dgain = 0; // 0;
                        velMultip = (velMultip1 + velMultip2) / 2;
                    }
                    else
                    {
                        velMultip = 0;// 1;
                        closedLoop1Dgain = 0;
                        if (senderWindow.switchParamsAuto.stopmode == 2)
                        {
                            stim1DclosedLoopContrast = 0;
                            blevel = 200;
                        }

                        if (senderWindow.switchParamsAuto.stopmode == 3)
                        {
                            stim1DclosedLoopContrast = 0;
                            blevel = 128;
                        }

                        if (senderWindow.switchParamsAuto.stopmode == 4)
                        {
                            velMultip = -1;
                        }
                    }
                }
                else
                {
                    stim1DclosedLoopContrast = 200;
                    closedLoop1Dgain = (autoGain1 + autoGain2) / 2; // 0;
                    velMultip = (velMultip1 + velMultip2) / 2;
                }
                 
            }
            else if (ta <= senderWindow.switchParamsAuto.phasedurinc[3])
            {
                if (stimParam3 != 4)
                {
                    reset_switchDisp();
                    switchDisp.gain2check = true;
                    switchDisp.Phase4Time = false;
                 }

                stim1DclosedLoopContrast = 200;
                closedLoop1Dgain = autoGain2;
                velMultip = velMultip2;
                stimParam3 = 4;
                if (senderWindow.switchParamsAuto.stopmode == 3 && (ta % 5 == 1) && aa % 2 == 0)
                {
                    closedLoop1Dgain = 0;
                }

            }
            else if (ta < senderWindow.switchParamsAuto.phasedurinc[4])
            {
                if (stimParam3 != 5)
                {
                    reset_switchDisp();
                    switchDisp.Phase5Time = false;
                }

                closedLoop1Dgain = 0;
                velMultip = 0;// 1;
                stimParam3 = 5;

                if (senderWindow.switchParamsAuto.stopmode == 2 || senderWindow.switchParamsAuto.stopmode == 3)
                {
                    stim1DclosedLoopContrast = 0;
                    blevel = 180;
                }
                
                /*
                if (senderWindow.switchParamsAuto.stopmode == 3)
                {

                    aa = (int)tt / (int)senderWindow.switchParamsAuto.phasedurinc[5];

                    if (aa % 2 == 0)
                    {

                        if (ta < senderWindow.switchParamsAuto.phasedurinc[3] + 1)
                        {
                            stim1DclosedLoopContrast = 0;
                            blevel = 200;
                        }
                        else if (ta < senderWindow.switchParamsAuto.phasedurinc[3] + 1.5)
                        {
                            stim1DclosedLoopContrast = 200;
                            closedLoop1Dgain = 0;
                            velMultip = 1;
                        }
                        else
                        {
                            stim1DclosedLoopContrast = 0;
                            blevel = 200;
                        }
                    }
                    else
                    {
                        stim1DclosedLoopContrast = 0;
                        blevel = 200;
                    }
                }
                */

                if (senderWindow.switchParamsAuto.stopmode == 4)
                {
                    velMultip = -1;
                }
            }
            else if (ta < senderWindow.switchParamsAuto.phasedurinc[5])
            {
                if (stimParam3 != 6)
                {
                    reset_switchDisp();
                    switchDisp.Phase6Time = false;
                }
                stimParam3 = 6;



                if (senderWindow.openprobe)
                {
                    if ((ta - senderWindow.switchParamsAuto.phasedurinc[4]) < 1.5)
                    {
                        stim1DclosedLoopContrast = 180;
                        closedLoop1Dgain = 0; // 0;
                        velMultip = (velMultip1 + velMultip2) / 2;
                    }
                    else
                    {
                        velMultip = 0;// 1;
                        closedLoop1Dgain = 0;
                        if (senderWindow.switchParamsAuto.stopmode == 2)
                        {
                            stim1DclosedLoopContrast = 0;
                            blevel = 200;
                        }

                        if (senderWindow.switchParamsAuto.stopmode == 3)
                        {
                            stim1DclosedLoopContrast = 0;
                            blevel = 128;
                        }

                        if (senderWindow.switchParamsAuto.stopmode == 4)
                        {
                            velMultip = -1;
                        }
                    }
                }
                else
                {
                    stim1DclosedLoopContrast = 200;
                    closedLoop1Dgain = (autoGain1 + autoGain2) / 2; // 0;
                    velMultip = (velMultip1 + velMultip2) / 2;
                }
                 
            }

        }
        public void memorytest_switch2(double ta)
        {
            int aa = (int)tt / (int)senderWindow.switchParamsAuto.phasedurinc[5];
            stimParam5 = (float)aa+1;

            if ((aa % 2) == 0)
                stimParam4 = 1;
            else
                stimParam4 = 2;

            if (ta <= senderWindow.switchParamsAuto.phasedurinc[0])
            {
                if (stimParam3 != 1)
                {
                    reset_switchDisp();
                    switchDisp.gain1check = true;
                    switchDisp.Phase1Time = false;
                }

                stim1DclosedLoopContrast = 200;
                closedLoop1Dgain = autoGain1;
                velMultip = velMultip1;
                stimParam3 = 1;
                if (aa % 2 == 1 && ta > senderWindow.switchParamsAuto.phasedurinc[0] - 15)
                {
                    velMultip = 0;// 1;
                    closedLoop1Dgain = 0;
                    if (senderWindow.switchParamsAuto.stopmode == 2)
                    {
                        stim1DclosedLoopContrast = 0;
                        blevel = 180;
                    }
                }


            }
            else if (ta <= senderWindow.switchParamsAuto.phasedurinc[1])
            {

                if (stimParam3 != 2)
                {
                    reset_switchDisp();
                    switchDisp.Phase2Time = false;
                }

                closedLoop1Dgain = 0;
                velMultip = 0;// 1;
                stimParam3 = 2;

                if (senderWindow.switchParamsAuto.stopmode == 2 || senderWindow.switchParamsAuto.stopmode == 3)
                {
                    stim1DclosedLoopContrast = 0;
                    blevel = 180;
                }

             

                if (senderWindow.switchParamsAuto.stopmode == 4)
                {
                    velMultip = -1;
                }

            }
            else if (ta <= senderWindow.switchParamsAuto.phasedurinc[2])
            {
                if (stimParam3 != 3)
                {
                    reset_switchDisp();
                    switchDisp.Phase3Time = false;
                }
                stimParam3 = 3;

                if (senderWindow.openprobe)
                {
                    if ((ta - senderWindow.switchParamsAuto.phasedurinc[1]) < 1.5)
                    {
                        stim1DclosedLoopContrast = 180;
                        closedLoop1Dgain = 0; // 0;
                        velMultip = (velMultip1 + velMultip2) / 2;
                    }
                    else
                    {
                        velMultip = 0;// 1;
                        closedLoop1Dgain = 0;
                        if (senderWindow.switchParamsAuto.stopmode == 2)
                        {
                            stim1DclosedLoopContrast = 0;
                            blevel = 200;
                        }

                        if (senderWindow.switchParamsAuto.stopmode == 3)
                        {
                            stim1DclosedLoopContrast = 0;
                            blevel = 128;
                        }

                        if (senderWindow.switchParamsAuto.stopmode == 4)
                        {
                            velMultip = -1;
                        }
                    }
                }
                else
                {
                    stim1DclosedLoopContrast = 200;
                    closedLoop1Dgain = (autoGain1 + autoGain2) / 2; // 0;
                    velMultip = (velMultip1 + velMultip2) / 2;
                }

            }
            else if (ta <= senderWindow.switchParamsAuto.phasedurinc[3])
            {
                if (stimParam3 != 4)
                {
                    reset_switchDisp();
                    switchDisp.gain2check = true;
                    switchDisp.Phase4Time = false;
                }

                stim1DclosedLoopContrast = 200;
                closedLoop1Dgain = autoGain2;
                velMultip = velMultip2;
                stimParam3 = 4;
              
                if (aa % 2 == 1 && ta > senderWindow.switchParamsAuto.phasedurinc[3] - 15)
                {
                    velMultip = 0;// 1;
                    closedLoop1Dgain = 0;
                    if (senderWindow.switchParamsAuto.stopmode == 2)
                    {
                        stim1DclosedLoopContrast = 0;
                        blevel = 180;
                    }
                }

            }
            else if (ta <= senderWindow.switchParamsAuto.phasedurinc[4])
            {
                if (stimParam3 != 5)
                {
                    reset_switchDisp();
                    switchDisp.Phase5Time = false;
                }

                closedLoop1Dgain = 0;
                velMultip = 0;// 1;
                stimParam3 = 5;

                if (senderWindow.switchParamsAuto.stopmode == 2 || senderWindow.switchParamsAuto.stopmode == 3)
                {
                    stim1DclosedLoopContrast = 0;
                    blevel = 180;
                }

               

                if (senderWindow.switchParamsAuto.stopmode == 4)
                {
                    velMultip = -1;
                }
            }
            else if (ta <= senderWindow.switchParamsAuto.phasedurinc[5])
            {
                if (stimParam3 != 6)
                {
                    reset_switchDisp();
                    switchDisp.Phase6Time = false;
                }
                stimParam3 = 6;



                if (senderWindow.openprobe)
                {
                    if ((ta - senderWindow.switchParamsAuto.phasedurinc[4]) < 1.5)
                    {
                        stim1DclosedLoopContrast = 180;
                        closedLoop1Dgain = 0; // 0;
                        velMultip = (velMultip1 + velMultip2) / 2;
                    }
                    else
                    {
                        velMultip = 0;// 1;
                        closedLoop1Dgain = 0;
                        if (senderWindow.switchParamsAuto.stopmode == 2)
                        {
                            stim1DclosedLoopContrast = 0;
                            blevel = 200;
                        }

                        if (senderWindow.switchParamsAuto.stopmode == 3)
                        {
                            stim1DclosedLoopContrast = 0;
                            blevel = 128;
                        }

                        if (senderWindow.switchParamsAuto.stopmode == 4)
                        {
                            velMultip = -1;
                        }
                    }
                }
                else
                {
                    stim1DclosedLoopContrast = 200;
                    closedLoop1Dgain = (autoGain1 + autoGain2) / 2; // 0;
                    velMultip = (velMultip1 + velMultip2) / 2;
                }

            }

        }

        public void resetGratingParams()
        {
           dH = (float)(senderWindow.InstStimParams.gwidth*0.32) / 2.2f;
           displayAngle = Math.PI;
           moveAngle = Math.PI;
           closedLoop1Dgain = 0; 
        }

        public void reset_switchDisp()
        {
            switchDisp.gain1check = false;
            switchDisp.gain2check = false;
            switchDisp.Phase1Time = true;
            switchDisp.Phase2Time = true;
            switchDisp.Phase3Time = true;
            switchDisp.Phase4Time = true;
            switchDisp.Phase5Time = true;
            switchDisp.Phase6Time = true;
            switchDisp.velReplay = false;
        }

        public void shuffler<T>(T[] array)
        {
            Random random = new Random();
            for (int i = array.Length; i > 1; i--)
            {
                // Pick random element to swap.
                int j = random.Next(i); // 0 <= j <= i-1
                // Swap.
                T tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
            }
        }

    }
}

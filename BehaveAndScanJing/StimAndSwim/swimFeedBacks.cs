using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BehaveAndScanSPIM
{
    public partial class StimEphysOscilloscopeControl
    {

        public void swimFeedBacks()
        {
         
            if (senderWindow.InstStimParams.stimtype==2) //1DClosedLoop
            {
                if ((swimPow[0] > 0 || swimPow[1] > 0) && (senderWindow.InstStimParams.closedLoop1DzeroVel == false))
                {
                    vel = 0.3f * vel + 0.6f * (float)(-0.0025 / 3 * velMultip + closedLoop1Dgain * (swimPow[0] + swimPow[1]) / 2)*1;  // /2 since bigger screen
                    if (vel > 0.0025 / 3f * 10)//0.0025 * 10)
                        vel = (float)(0.0025 / 3f * 10);
                }
                else    // no swimming
                {
                    vel = vel - dVel / 3f;
                    if (vel < -0.0025f / 3f * velMultip)
                        vel = -0.0025f / 3f * (float)velMultip;

                    if (senderWindow.InstStimParams.closedLoop1DzeroVel)
                    {
                        vel = 0f;
                    }
                }

                if (vel > 0 && gainTriggerSwitch)
                {
                    gainTriggerSwitch = false;
                }
                if (vel < 0 && gainTriggerSwitch == false)
                {
                    gainTriggerSwitch = true;
                }



                if (senderWindow.InstStimParams.recordPlayback)
                {   
                    switchDisp.velReplay = (!VRecordOn);
                    
                    if (velBufferI == 0 && velReplayI == 0)
                        VRecordOn = true;

                    int smod = (switchDisp.stackNum - 1) % (senderWindow.InstStimParams.replayfcycle * 2);

                    if (smod < senderWindow.InstStimParams.replayfcycle && VRecordOn==false)
                    {
                        VRecordOn = true;
                        velBufferI = 0;

                    }
                    else if (smod >= senderWindow.InstStimParams.replayfcycle && VRecordOn == true)
                    {
                        VRecordOn = false;
                        velReplayI = 0;
                    }


                    if (triggerGo)
                    {   
                        if (VRecordOn)
                        {
                            velBufferFL[velBufferI] = vel;
                            if (switchDisp.stackNum > stacknum_old)
                            {
                                velBufferInds[smod] = velBufferI;
                            }
                            velBufferI++;
                            stimParam6 = 1;

                        }
                        else
                        {
                            if (switchDisp.stackNum > stacknum_old)
                            {
                                velReplayI = velBufferInds[smod-senderWindow.InstStimParams.replayfcycle];
                            }
                            if (replayGo)
                            {
                                vel = velBufferFL[velReplayI];
                            }
                            velReplayI++;
                            stimParam6 = 2;

                        }

                        stacknum_old = switchDisp.stackNum;

                    }
                    else
                    {

                        if (VRecordOn)
                        {
                            velBuffer[velBufferI] = vel;
                            velBufferI++;
                            if (velBufferI > velBuffer.Length - 1)
                            {
                                VRecordOn = false;
                                velReplayI = 0;
                            }
                        }
                        else
                        {
                            vel = velBuffer[velReplayI];
                            velReplayI++;
                            if (velReplayI > velBuffer.Length - 1)
                            {
                                VRecordOn = true;
                                velBufferI = 0;
                            }
                        }
                    }


                }
                else if (senderWindow.InstStimParams.recordPlayback_special)
                {
                    switchDisp.velReplay = (!VRecordOn);

                    if (velBufferI == 0 && velReplayI == 0)
                        VRecordOn = true;
                    int smod;
                    if (triggerGo)
                    {
                        smod = tt % 100;
                    }
                    else
                    {
                        smod = tt % 100;
                    }

                    if (smod < 60 && VRecordOn == false)
                    {
                        VRecordOn = true;
                        velBufferI = 0;

                    }
                    else if (smod >= 60 && VRecordOn == true)
                    {
                        VRecordOn = false;
                        velReplayI = 0;
                    }


                    if (triggerGo)
                    {
                        if (VRecordOn)
                        {
                            velBufferFL[velBufferI] = vel;
                            if (switchDisp.stackNum > stacknum_old)
                            {
                                velBufferInds[smod] = velBufferI;
                            }
                            velBufferI++;
                            stimParam6 = 1;

                        }
                        else
                        {
                            if (switchDisp.stackNum > stacknum_old)
                            {
                                velReplayI = velBufferInds[smod - senderWindow.InstStimParams.replayfcycle];
                            }
                            if (replayGo)
                            {
                                vel = velBufferFL[velReplayI];
                            }
                            velReplayI++;
                            stimParam6 = 2;

                        }

                        stacknum_old = switchDisp.stackNum;

                    }
                    else
                    {

                        if (VRecordOn)
                        {
                            velBuffer[velBufferI] = vel;
                            velBufferI++;
                            if (velBufferI > velBuffer.Length - 1)
                            {
                                VRecordOn = false;
                                velReplayI = 0;
                            }
                            stimParam6 =   1 ;
                        }
                        else
                        {
                            vel = velBuffer[velReplayI];
                            velReplayI++;
                            if (velReplayI > velBuffer.Length - 1)
                            {
                                VRecordOn = true;
                                velBufferI = 0;
                            }
                            stimParam6 = 2;
                        }
                    }


                }


                if (bout_replay && bout_replay_play)
                {
                    vel = bout_vel;
                }


                stimID = 10;
                stimParam1 = (float)vel;
                stimParam2 = (float)closedLoop1Dgain;

                if (senderWindow.InstStimParams.moveobject==1)
                    stripesY            += (float)vel * (float)(DT * (90.0 / 1000.0));
                else
                    stimState.yPosition -= (float)vel * (float)(DT * (90.0 / 1000.0));

            }

            else
            {
                if (stimID != 0)
                {
                    stimID = 0;
                    stimParam1 = 0;
                    stimParam2 = 0;
                }

                velBufferI = 0; // reset closed loop velocity recording buffer
                velReplayI = 0; // also reset this index
                stimState.orientation = 0f;
                stimState.xPosition = 0f;
                stimState.yPosition = 0f;
                stimID = 0;
                stimState.xPosition = 0;
                stimState.yPosition = 0;
            }

        }            
        
    }
}

# -*- coding: utf-8 -*-
"""
Created on Wed Jan 25 21:16:25 2017

@author: kawashimat
"""

plt.close("all")
clear_all()

import ep
import os
import array as ar
import struct as st
import scipy.io as sio
from scipy.fftpack import fft


pathname=r"D:\Takashi\2P\06302017";
fname="Trace10-withlaser"


full_fname=pathname+"\\"+fname+".10chFlt"
print(fname)

fileContent = np.fromfile(full_fname, np.float32)

rootdir=pathname+"\\"+fname
if not os.path.exists(rootdir):
    os.mkdir(rootdir);
    
outdir=pathname+"\\"+fname+"\\swim\\"
if not os.path.exists(outdir):
    os.mkdir(outdir);


nlen=fileContent.size
npoint=nlen/10
ds=20

rawdata=dict()


rawdata['ch1'] = fileContent[0:nlen:10];
rawdata['ch2'] = fileContent[1:nlen:10];
rawdata['ch3'] = fileContent[2:nlen:10];
rawdata['stimParam4'] = fileContent[3:nlen:10];
rawdata['stimParam6'] = fileContent[4:nlen:10];
rawdata['stimParam5'] = fileContent[5:nlen:10];
rawdata['stimParam3'] = fileContent[6:nlen:10];
rawdata['stimID'] = fileContent[7:nlen:10];
rawdata['stimParam1'] = fileContent[8:nlen:10];
rawdata['stimParam2'] = fileContent[9:nlen:10];





tt1=fft(rawdata['ch1'],2048)
ppy1=tt1*np.conj(tt1)

tt2=fft(rawdata['ch2'],2048)
ppy2=tt2*np.conj(tt2)

f6000=6000*np.arange(2048)/4096

plt.figure(4)
plt.subplot(211).plot(f6000[1:2048],ppy1[1:2048])
plt.xlim(0,300)
plt.ylim((0,20))
plt.subplot(212).plot(f6000[1:2048],ppy2[1:2048])
plt.xlim(0,300)
plt.ylim((0,20))



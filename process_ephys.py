# -*- coding: utf-8 -*-

plt.close("all")
clear_all()

import ep
import os
import array as ar
import struct as st
import scipy.io as sio
from scipy.fftpack import fft


pathname=r"D:\Takashi\SPIM\05242017";
fname="Fish1-6"


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


swimdata=ep.kickAssSwimDetect01(rawdata['ch1'],rawdata['ch2'],2.5);

#blockpowers=0
       


#tt=fft(rawdata['ch2'],2048)
#ppy=tt*np.conj(tt)
#f=6000*np.arange(513)/2048
#                
#plt.plot(f[1:400],ppy[1:400])
#bbb

np.save(outdir+"rawdata",rawdata);
np.save(outdir+"swimdata",swimdata);

plt.figure(figsize=(16,5))
tvec=np.arange(1,npoint)/6000;

plt.subplot(211).plot(tvec[::ds],rawdata['ch1'][::ds],'r')
plt.subplot(211).plot(tvec[::ds],rawdata['ch2'][::ds],'b')
plt.axis('tight')
plt.ylim([-0.4, 0.4]);

plt.subplot(212).plot(tvec[::ds],swimdata['fltCh1'][::ds],'r')
plt.subplot(212).plot(tvec[::ds],swimdata['fltCh2'][::ds],'b')
plt.axis('tight')
plt.ylim([-0.001, 0.005]);
plt.savefig("swim.png")
plt.savefig("swim.png")


blocklist, rawdata['stimParam5']=ep.create_stim_blocks(rawdata['stimParam5']);
blockpowers=ep.calc_blockpowers(swimdata,blocklist)
np.save(outdir+"blocklist",blocklist);
np.save(outdir+"blockpowers",blockpowers);


totblock=(blocklist.max()).astype('i4');

plt.figure(figsize=(16,5))
plt.subplot(131)
plt.bar(np.arange(0.5,totblock+0.5,1),blockpowers[0,]);

plt.subplot(132)
plt.bar(np.arange(0.5,totblock+0.5,1),blockpowers[1,]);

plt.subplot(133)
plt.bar(np.arange(0.5,totblock+0.5,1),blockpowers[2,]);
       
       
plt.figure(figsize=(16,5))
tvec=np.arange(1,npoint)/6000;

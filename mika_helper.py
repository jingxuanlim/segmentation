### mika_helper.py --- 
## 
## Filename: mika_helper.py
## Description: 
## Author: Jing Xuan Lim
## Email: jingx.lim@outlook.com
######################################################################
## 
### Code:

import numpy as np
import matplotlib.pyplot as plt

class segmented(object):

    def __init__(self, im_dir, cell_file, cleaned_file, component_file, \
                        parameters_file, mask_file, ephys_file=None):

        from datetime import datetime
        import h5py

        self.path = im_dir
        self.ephys_file = ephys_file
        self.expt_date = datetime.strptime(im_dir.split('/')[6], '%Y%m%d')
        self.expt_name = '_'.join(im_dir.split('/')[7].split('_')[:-2])

        try:
        
            f = h5py.File(im_dir+mask_file,'r')

            self.background = f['background'][()]
            self.blok_lidx = f['blok_lidx'][()]
            self.blok_nmbr = f['blok_nmbr'][()]
            self.brain_mask = f['brain_mask'][()]
            self.image_mean = f['image_mean'][()]
            self.image_peak = f['image_peak'][()]
            self.image_peak_fine = f['image_peak_fine'][()]
            self.thr_mask = f['thr_mask'][()]
            self.thr_prob = f['thr_prob'][()]

            f.close()
            
        except: print("Problems loading %s" % mask_file)

        try: 
            f = h5py.File(im_dir+cell_file,'r')

            self.Cmpn_position = f['Cmpn_position'][()]
            self.Cmpn_spcesers = f['Cmpn_spcesers'][()]
            self.Cmpn_timesers = f['Cmpn_timesers'][()]
            self.dims = f['dims'][()]
            self.freq = int(f['freq'][()])
            self.resn = f['resn'][()]

            f.close()

        except: print("Problems loading %s" % cell_file)

        try: 
            f = h5py.File(im_dir+cleaned_file,'r')

            self.Cell_X = f['Cell_X'][()] - 1
            self.Cell_Y = f['Cell_Y'][()] - 1
            self.Cell_Z = f['Cell_Z'][()] - 1
            self.Cell_baseline1 = f['Cell_baseline1'][()]
            self.Cell_spcesers = f['Cell_spcesers'][()]
            self.Cell_timesers0 = f['Cell_timesers0'][()]
            self.Cell_timesers1 = f['Cell_timesers1'][()]
            self.Labels = f['Labels'][()]
            self.Volume = f['Volume'][()]
            self.background = f['background'][()]
            self.freq = int(f['freq'][()])
            self.n = int(f['n'][()])
            self.x = int(f['x'][()])
            self.y = int(f['y'][()])
            self.z = int(f['z'][()])

            f.close()
            
        except: print("Problems loading %s" % cleaned_file)

        try: 
            f = h5py.File(im_dir+component_file,'r')
            
            self.H0 = f['H0'][()]
            self.W0 = f['W0'][()].transpose()

            try: 
                self.H1 = f['H1'][()]
                self.W1 = f['W1'][()].transpose()
            except:
                pass

            f.close()

        except: print("Problems loading %s" % component_file)

        
        try: del f
        except: pass

        print('Segmented imaging data loaded!')

        self.image_starts = None
        self.image_starttimes = None
        self.nstacks = None

        if ephys_file:

            from ephys_helper import ephys

            self.ep = ephys(ephys_file)

            self.compute_imagingtimes()

            self.check_align()

            print("Electrophysiological data loaded!")

        self.cleanup_artefacts()

        self.ephys_rate = self.ep.rate
        self.duration = self.ep.t / self.ephys_rate
        self.im_rate = self.nstacks / self.duration

        ## the following variables are set interactively

        # derived ephys
        self.swim = None
        self.swim_power = None
        self.swim_starts = None
        self.swim_stops = None

        self.downsample_factor = None
        
        # chopped ephys
        self.chopped_swim = None
        self.chopped_swim_power = None
        self.chopped_swim_starts = None
        self.chopped_swim_stops = None
        self.chopped_set = None

        # downsampled ephys
        self.ds_swim = None
        self.ds_swimpower = None
        self.ds_swimstarts = None
        self.ds_swimstops = None
        self.ds_set = None  # not set yet

        # indexed in the downsampled domain
        self.swimstart_index = None
        self.swimstop_index = None
        # self.flash_index = None
        

    def compute_imagingtimes(self):

        self.nstacks = self.Cell_timesers1.shape[1]
        
        if self.z == 1:
            print("Detected high speed single plane data")
            self.image_starts = self.ep.im_starts
        else:
            print("Detected multi-plane data")
            self.image_starts = self.ep.stack_starts

        self.image_starttimes = np.where(self.image_starts)[0]            

    def check_align(self):
        
        import warnings
        
        if self.image_starts.sum() != self.Cell_timesers1.shape[1]:
            warnings.warn("Number of images recorded after \
            alignment (%i) and number of images actually taken (%i) \
            don't match. Please verify!" % \
                          (self.image_starts.sum(),self.Cell_timesers1.shape[1]))
        else:
            print('Datasets are aligned!')
            

    @classmethod
    def dataset_keys(cls, out_file):

        import h5py

        h5py_file = h5py.File(out_file,'r')
        print(list(h5py_file.keys()))
        h5py_file.close()


    def cleanup_artefacts(self):
        
        # find where the signal stops to be different
        artificialstop_imidx = np.where(np.around(self.H0[0,:],decimals=3) \
                                        != np.round(self.H0[0,0],decimals=3))[0][0]
        if artificialstop_imidx != 1:

            print("Artificial initial constant value detected. \
            Truncating first %i data points" % artificialstop_imidx)

        # truncate imaging data
        self.H0 = self.H0[:,artificialstop_imidx:-artificialstop_imidx]
        try: self.H1 = self.H1[:,artificialstop_imidx:-artificialstop_imidx]
        except: pass

        self.Cell_timesers0 = self.Cell_timesers0[:,artificialstop_imidx:-artificialstop_imidx]
        self.Cell_baseline1 = self.Cell_baseline1[:,artificialstop_imidx:-artificialstop_imidx]
        self.Cell_timesers1 = self.Cell_timesers1[:,artificialstop_imidx:-artificialstop_imidx]

        if self.ephys_file:
            # truncate ephys data
            artificialstop_ephysidx = self.image_starttimes[artificialstop_imidx]
            self.ep.replace_ephys(self.ep.ep[:,artificialstop_ephysidx:-artificialstop_ephysidx])
            self.compute_imagingtimes()

        self.check_align()

    def set_swims(self, channel):

        from fish.ephys.ephys import load, windowed_variance, estimate_swims

        self.swim = channel
        self.swim_power = windowed_variance(channel)[0]
        self.swim_starts = estimate_swims(self.swim_power, scaling=2.1)[0]
        self.swim_stops = estimate_swims(self.swim_power, scaling=2.1)[1]

        
    def compute_max_imintvl(self, show_stats=False):

        im_intervals = np.diff(self.image_starttimes)
        max_interval = np.max(im_intervals)

        self.downsample_factor = max_interval

        if show_stats:

            from utils import unique

            print("Imaging intervals: %s" % str(unique(im_intervals)))
            print("Bins of Imaging intervals: %s" % np.histogram(im_intervals)[0])
            print("Imaging histo counts: %s" % np.histogram(im_intervals)[1])
            print('Largest interval: %i' % max_interval)

            imtint_fig, imtint_ax= plt.subplots(2, 1, figsize=(9,6), facecolor='w',gridspec_kw={'height_ratios':[1,3]})
            imtint_ax[0].plot(self.image_starttimes[0:-1]/self.ephys_rate, im_intervals)
            imtint_ax[0].set_ylabel('Intervals')
            imtint_ax[0].set_xlabel('Time (s)')
            imtint_ax[0].set_xlim([0,10])

            imtint_ax[1].hist(im_intervals,align='left')
            imtint_ax[1].set_yscale('log', nonposy='clip')
            imtint_ax[1].set_ylabel('log(count)')
            imtint_ax[1].set_xlabel('Intervals')

            imtint_fig.tight_layout()

    def chop_ephys(self, timeseries):

        chopped_series = np.zeros([self.nstacks,self.downsample_factor])
        
        for i in range(0, self.nstacks):  # range from 0 to nstack-1

            start = self.image_starttimes[i]
    
            final_frame = self.ep.ep.shape[1] - 1
    
            if i >= len(self.image_starttimes)-1:  # if second last frame
                
                if final_frame - start <= self.downsample_factor:
                    stop = final_frame
                else:
                    stop = start + self.downsample_factor
            
            else: stop = self.image_starttimes[i+1]

            chopped_series[i,:stop-start] = timeseries[start:stop]

        return chopped_series


    def downsample_ephys(self):

        if self.downsample_factor is None: self.compute_max_imintvl()

        self.chopped_swim = self.chop_ephys(self.swim)
        self.chopped_swim_power = self.chop_ephys(self.swim_power)
        self.chopped_swim_starts = self.chop_ephys(self.swim_starts)
        self.chopped_swim_stops = self.chop_ephys(self.swim_stops)
        self.chopped_set = self.chop_ephys(self.ep.channel6)

        self.ds_swimstarts = np.sum(self.chopped_swim_starts,axis=1)
        self.ds_swimstops = np.sum(self.chopped_swim_stops,axis=1)
        self.ds_swimpower = np.max(self.chopped_swim_power,axis=1)
        self.ds_swim = np.max(self.chopped_swim,axis=1)

        self.swimstart_index = self.index_onsets(self.chopped_swim_starts)
        self.swimstop_index = self.index_onsets(self.chopped_swim_stops)

        self.check_mismatched_timings(self.swimstart_index,self.swimstop_index)

    def check_mismatched_timings(cls, swimstart_index, swimstop_index):
        
        import warnings        
        
        if (swimstop_index > swimstart_index).sum() > 0:
            warnings.warn("Some swim stops happen before their \
            corresponding swim starts. You might want to check...")

    def correct_mismatched_timings(self, swimstart_index, swimstop_index):

        swimstart_list = list(self.swimstart_index)
        swimstop_list = list(self.swimstop_index)

        while len(swimstart_list) - len(swimstop_list) != 0:
            
            print('Starts: %i Stops: %i' % (len(swimstart_list), len(swimstop_list)))
            
            for i, (start, stop) in enumerate(zip(swimstart_list,swimstop_list)):

                if start > stop:
                    
                    print(i,start,stop)
            
                    if len(swimstop_list) > len(swimstart_list): del swimstp_list[i]
                    else: del swimstart_list[i]
 
                    print('Restarting... ')
                    break
            
                if i == min(len(swimstart_list),len(swimstop_list))-1:
            
                    if len(swimstop_list) > len(swimstart_list):
                        swimstop_list = swimstop_list[:len(swimstart_list) - len(swimstop_list)]
                    else:
                        swimstart_list = swimstart_list[:len(swimstop_list) - len(swimstart_list)]

                    print('Done! ')

            self.check_mismatched_timings(np.array(swimstart_list), np.array(swimstop_list))
            
        else:
            print('Same number of starts and stops!')

        return np.array(swimstart_list), np.array(swimstop_list)


    def index_onsets(self, chopped):

        chopped_index = []

        for i, row in enumerate(chopped):
            n = int(row.sum())
    
            for swims_idx in range(n):
                chopped_index.append(i)

        assert np.sum(chopped) == len(chopped_index)

        return np.array(chopped_index)    
    
        
    def find_cell(self, cell_num, mask=False):

        import numpy as np
        
        cell_volume = np.zeros((self.z, self.y, self.x))
    
        for j in range(np.count_nonzero(self.Cell_X[cell_num, :] > 0)):
        
            if mask:
                cell_volume[int(self.Cell_Z[cell_num, j]),
                            int(self.Cell_Y[cell_num, j]),
                            int(self.Cell_X[cell_num, j])] = 1
            
            else:
                cell_volume[int(self.Cell_Z[cell_num, j]),
                            int(self.Cell_Y[cell_num, j]),
                            int(self.Cell_X[cell_num, j])] = \
                            self.Cell_spcesers[cell_num, j]
            
        return cell_volume
    

    def plot_volume(self, save_name=None):

        from utils import get_transparent_cm

        trans_inferno = get_transparent_cm('hot',tvmax=1,gradient=False)

        nplanes = self.Volume.shape[2]
        vol_fig, vol_ax = plt.subplots(nplanes,1,figsize=(8,nplanes*3),
                                       squeeze=False)

        for nplane in range(nplanes):
    
            vol_ax[nplane,0].imshow(self.image_mean[nplane,:,:], cmap='gray',
                                  vmin=np.percentile(np.ravel(self.image_mean[nplane,:,:]),1),
                                  vmax=np.percentile(np.ravel(self.image_mean[nplane,:,:]),99.9))


            vax = vol_ax[nplane,0].imshow(self.Volume[:,:,nplane].transpose(),
                                        vmax=np.percentile(np.ravel(self.Volume[:,:,:]),99.9), cmap=trans_inferno)
            vol_fig.colorbar(vax,ax=vol_ax[nplane,0])
    
            vol_ax[nplane,0].set_title('Plane %i' % nplane)
        
            vol_fig.tight_layout()

            if save_name:
                vol_fig.savefig(save_name)

        return vol_fig, vol_ax

    def plot_cells(self, num_cells=10, mask=False, zoom_pad=25, save_name=None):

        from utils import get_transparent_cm
        import random

        trans_inferno = get_transparent_cm('hot',tvmax=1,gradient=False)

        ts_fig, ts_ax = plt.subplots(num_cells,2,figsize=(20,num_cells*3),
                                     gridspec_kw = {'width_ratios':[1,3]})

        for neuron in range(num_cells):
    
            randcell = random.randint(0,self.n-1)
    
            try:
                ts_ax[neuron,0].imshow(self.image_mean.max(0),
                                       cmap='gray',
                                       vmax=np.percentile(np.ravel(self.image_mean),99.9))
            except:
                pass    
    
            cell_volume = self.find_cell(randcell, mask=mask)
    
            cell_im = ts_ax[neuron,0].imshow(cell_volume.max(0),cmap=trans_inferno)
            ts_ax[neuron,0].set_title('Maximum projection')

            if zoom_pad:
                max_X = (self.Cell_X[randcell][self.Cell_X[randcell] > 0]).max()
                min_X = (self.Cell_X[randcell][self.Cell_X[randcell] > 0]).min()
                max_Y = (self.Cell_Y[randcell][self.Cell_Y[randcell] > 0]).max()
                min_Y = (self.Cell_Y[randcell][self.Cell_Y[randcell] > 0]).min()

                ts_ax[neuron,0].set_xlim([min_X-zoom_pad,max_X+zoom_pad])
                ts_ax[neuron,0].set_ylim([min_Y-zoom_pad,max_Y+zoom_pad])
            
            ts_fig.colorbar(cell_im,ax=ts_ax[neuron,0])
    
            dff = (self.Cell_timesers1[randcell,:] - self.Cell_baseline1[randcell,:])/ \
                   (self.Cell_baseline1[randcell,:] - self.background)
    
            t = len(dff)
            ts_ax[neuron,1].plot(np.linspace(0,t/self.im_rate,num=t),
                                 self.Cell_timesers0[randcell,:],
                                 alpha=0.5,label='F')
            ts_ax[neuron,1].plot(np.linspace(0,t/self.im_rate,num=t),
                                 self.Cell_timesers1[randcell,:],
                                 alpha=0.5,label='detrended F')
            ts_ax[neuron,1].plot(np.linspace(0,t/self.im_rate,num=t),
                                 self.Cell_baseline1[randcell,:],
                                 alpha=0.5,label='baseline')

            ts_ax[neuron,1].set_ylim([np.percentile(self.Cell_timesers0[randcell,:],0.1),
                                      np.percentile(self.Cell_timesers0[randcell,:],99.9)])
    
            ts_dff_ax = ts_ax[neuron,1].twinx()
            ts_dff_ax.plot(np.linspace(0,t/self.im_rate,num=t),dff,
                           alpha=0.5,label='$\Delta F / F$',
                           color='#17becf')
            ts_dff_ax.set_ylim([np.percentile(dff,0.1),
                                np.percentile(dff,99.9)])
    
            ts_ax[neuron,1].legend(loc='lower left',mode='expand',
                                   bbox_to_anchor=(0,1.02,1,0.2),
                                   ncol=3)
    
            xlim_win = 500  # seconds
            randslice = random.randint(0,int(t-xlim_win*self.im_rate))
            ts_ax[neuron,1].set_xlim(randslice/self.im_rate,
                                     (randslice+xlim_win*self.im_rate)/ \
                                     self.im_rate)
            ts_ax[neuron,1].set_ylabel('$F$')
    
            ts_ax[neuron,1].set_xlabel('t [s]')
    
            ts_dff_ax.set_ylabel('$\Delta F / F$')
            ts_dff_ax.legend(loc='lower right')
    
            ts_fig.tight_layout()

            if save_name:
                ts_fig.savefig(save_name)

        return ts_fig, ts_ax


    def find_component(self, comp_spcesers, comp_num, mask=False):

        cell_volume = np.zeros((self.z, self.y, self.x))
    
        for cell in range(np.count_nonzero(np.nonzero(comp_spcesers[comp_num, :]))):
    
            for j in range(np.count_nonzero(self.Cell_X[cell, :] > 0)):

                if mask:
                    cell_volume[int(self.Cell_Z[cell, j]), int(self.Cell_Y[cell, j]),
                                int(self.Cell_X[cell, j])] = 1

                else:
                    cell_volume[int(self.Cell_Z[cell, j]), int(self.Cell_Y[cell, j]),
                                int(self.Cell_X[cell, j])] = comp_spcesers[comp_num,cell]
            
        return cell_volume


    def mika_visualize_components(self,comp_spcesers, comp_timesers, \
                                  save_name=None):

        # loop over components
        for h in range(comp_spcesers.shape[0]):

            # construct component volumes
            S = np.zeros((self.x, self.y, self.z))
            for i in range(self.n):
                for j in range(np.count_nonzero(np.isfinite(self.Cell_X[i]))):
                    xij, yij, zij = int(self.Cell_X[i, j]), \
                                    int(self.Cell_Y[i, j]), \
                                    int(self.Cell_Z[i, j])
                    S[xij, yij, zij] = np.maximum(S[xij, yij, zij],
                                                  comp_spcesers[h,i])

            # visualize component maximal projections
            clust_fig, clust_ax = plt.subplots(2,1)
            clust_ax[0].imshow(S.max(2).T)

            # visualize component timeseries
            clust_ax[1].plot(comp_timesers[h])

            if save_name:
                from datetime import datetime
                save_names = save_name.split('.')
                clust_fig.savefig(save_names[0] + '.'+ str(h) + '.'+ \
                                  datetime.now().strftime("%Y-%m-%d_%H-%M") + \
                                  '.' + save_names[-1])

    def visualize_component(self, comp_num, comp_timesers, comp_spcesers, \
                            save_name=None, close_fig=False):

        from utils import get_transparent_cm
        trans_inferno = get_transparent_cm('hot',tvmax=1,gradient=False)

        clust_volume = self.find_component(comp_spcesers, comp_num)
        nplanes = clust_volume.shape[0]
        clust_fig, clust_ax = plt.subplots(nplanes+2,1,
                                           figsize=(8,2*2 + nplanes*3))

        clust_ax[0].plot(np.linspace(0,len(comp_timesers[comp_num])/self.im_rate,
                                     num=len(comp_timesers[comp_num])),
                         comp_timesers[comp_num])

        clust_ax[0].set_ylabel('$\Delta F / F$')
        clust_ax[0].set_xlabel('Time (s)')
        clust_ax[0].set_title('Calcium dynamics')

        clust_ax[1].plot(np.linspace(0,len(comp_timesers[comp_num])/self.im_rate,
                                     num=len(comp_timesers[comp_num])),
                         comp_timesers[comp_num])

        clust_ax[1].set_ylabel('$\Delta F / F$')
        clust_ax[1].set_xlabel('Time (s)')
        clust_ax[1].set_title('Calcium dynamics')
        clust_ax[1].set_ylim(top=np.percentile(comp_timesers[comp_num],99.9))

        slice_win = 10  # in seconds
        rand_slice = np.random.randint(len(comp_timesers[comp_num])/self.im_rate - slice_win)
        clust_ax[1].set_xlim([rand_slice, rand_slice+slice_win])   

        for nplane in range(nplanes):

            clust_ax[nplane+2].imshow(self.image_mean[nplane,:,:],cmap='gray')
            cax = clust_ax[nplane+2].imshow(clust_volume[nplane,:,:],
                                            vmax=np.percentile(np.ravel(clust_volume),99.9),
                                            cmap=trans_inferno)
            clust_fig.colorbar(cax,ax=clust_ax[nplane+2])

        clust_fig.suptitle(self.expt_name)
        clust_fig.tight_layout()
        clust_fig.subplots_adjust(top = 0.9)

        if save_name:
            from datetime import datetime
            
            save_names = save_name.split('.')
            clust_fig.savefig(save_names[0]+'-'+ \
                              datetime.now().strftime("%Y-%m-%d_%H-%M")+ \
                              '.'+save_names[1])

        if close_fig: plt.close(clust_fig)

        return clust_fig, clust_ax

    

######################################################################
### mika_helper.py ends here

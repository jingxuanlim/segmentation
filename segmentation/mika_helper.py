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
import h5py

from analysis_toolbox.spim_helper import spim

def load_segmented_data(
                        # spim attributes
                        im_dir, impro_dir=[], ephys_file='',
                        channel_labels=[], debug=False,

                        # segmented attributes
                        cell_file='', cleaned_file='', component_file='',
                        parameters_file='', mask_file=''):
 
    """
    Keyword Arguments:
    im_dir         -- 
    impro_dir      -- (default [])
    ephys_file     -- (default None)
    channel_labels -- (default [])
    debug          --         
    """
    
    spim_dset = segmented(
                          # spim attributes
                          im_dir, impro_dir=impro_dir, ephys_file=ephys_file,
                          channel_labels=channel_labels, debug=debug,

                          # segmented attributes
                          cell_file=cell_file,
                          cleaned_file=cleaned_file,
                          component_file=component_file,
                          parameters_file=parameters_file,
                          mask_file=mask_file)

    spim_dset.setup()  ## already done from the imaging class
    
    try: spim_dset.open_raw_images()  ## from the spim subclass
    except: pass
    
    spim_dset.load_segmented_files()  ## from the segmented subclass

    ## if processed image file path is provided, load processed images
    if len(impro_dir) != 0: spim_dset.open_processed_images() ## from the spim subclass

    ## if ephys file is provided, load ephys file
    if len(ephys_file) != 0:
        spim_dset.load_and_match_ephys_data() ## function of the same name as the spim subclass, but redefined here

    return spim_dset

class segmented(spim):

    def __init__(self,
                 
                 # spim attributes
                 im_dir, impro_dir=[], ephys_file='',
                 channel_labels=[], debug=False,

                 # segmented attributes
                 cell_file='', cleaned_file='', component_file='',
                 parameters_file='', mask_file=''):

        ######################
        # imaging attributes #
        ######################
        super().__init__(self) ## set path, ephys_file and debug

        ## Initialize imaging class
        self.path = im_dir
        self.ephys_file = ephys_file
        self.debug = debug

        self.setup()  ## set savepath, expt_id, expt_date, expt_name

        ###################
        # spim attributes #
        ###################
        self.ppaths = impro_dir
        self.channel_labels = channel_labels

        ########################
        # segmented attributes #
        ########################
        self.cell_file = cell_file
        self.cleaned_file = cleaned_file
        self.component_file = component_file
        self.parameters_file = parameters_file
        self.mask_file = mask_file
        

    ###################################################################
    #                            Data I/O                             #
    ###################################################################

    def load_segmented_files(self, debug=False):

        self.load_mask_file()
        self.load_cell_file()
        self.load_cleaned_file()
        self.load_component_file()

        print('Segmented imaging data loaded!')


    def load_mask_file(self, debug=False):

        try:
        
            f = h5py.File(self.mask_file,'r')

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
            
        except: print("Problems loading %s" % self.mask_file)


    def load_cell_file(self, debug=False):

        try: 
            f = h5py.File(self.cell_file,'r')

            self.Cmpn_position = f['Cmpn_position'][()]
            self.Cmpn_spcesers = f['Cmpn_spcesers'][()]
            self.Cmpn_timesers = f['Cmpn_timesers'][()]
            self.dims = f['dims'][()]
            self.freq = int(f['freq'][()])
            self.resn = f['resn'][()]

            f.close()

        except: print("Problems loading %s" % self.cell_file)


    def load_cleaned_file(self, debug=False):

        try: 
            f = h5py.File(self.cleaned_file,'r')

            self.Cell_X = f['Cell_X'][()]
            self.Cell_Y = f['Cell_Y'][()]
            self.Cell_Z = f['Cell_Z'][()]
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
            
        except: print("Problems loading %s" % self.cleaned_file)


    def load_component_file(self, debug=False):

        try: 
            f = h5py.File(self.component_file,'r')
            
            self.H0 = f['H0'][()]
            self.W0 = f['W0'][()].transpose()

            try: 
                self.H1 = f['H1'][()]
                self.W1 = f['W1'][()].transpose()
            except:
                pass

            f.close()

        except: print("Problems loading %s" % self.component_file)

    ###################################################################
    #                         Preprocessing                           #
    ###################################################################        


    def check_congruence(self, debug=False):
        """
        Determine if the segmented data is accurately derived from the
        raw data. Since we commonly downsample in x and y, check for
        coherence in the number of planes (i.e. z) and the number of 
        stacks (i.e. t).

        If segmented data is congruent, rely on that data to make
        calculations for imaging times and for aligning images to ephys
        data, applying the same treatment to the raw images and
        processed images.
        
        """

        nstacks = self.Cell_timesers1.shape[1] == self.im.shape[0]
        z = self.z == self.im.shape[1]

        if np.logical_and(nstacks,z): return True
        else: return False
        

    def load_and_match_ephys_data(self, debug=False):

        """
        _WARNING: Head_
        Redefined method. Many parts copied from segmented.load_and_match_ephys_data().
        Remember to update that method whenever you update this one.
        (Not the best way; think of a better way eventually.)
        """

        print("Aligning ephys and im data")
        print("==========================")        

        self.open_ephys_data()  ## from spim class
        self.apply_main_image()  ## from spim class

        if self.check_congruence:
            
            self.match_ephys_im_data()  ## WARNING 1
            self.calculate_DFF()


    def match_ephys_im_data(self, debug=False):
        
        """
        _WARNING: 1_
        Redefined method. Many parts copied from segmented.match_ephys_im_data().
        Remember to update that method whenever you update this one.
        (Not the best way; think of a better way eventually.)
        """

        print("Aligning ephys and im data")
        print("==========================")        

        self.compute_imagingtimes_segments(debug=self.debug)

        ## remove first segment of experiment (that mika clipped because
        ## it helps with segmentation)
        ## Remove values from the START ##
        if len(self.cell_file) != 0: self.remove_clipped_values()

        self.aligned = self.check_align_segments()

        ## Remove values from the END ##
        while np.logical_not(self.aligned):
            self.cleanup_artefacts_segments(debug=self.debug)

        ## Remove values from the END ##
        if self.aligned:
            self.remove_lastframe(debug=self.debug)

        ## print out final shapes for sainty check
        print('Main image: %s' % (self.im.shape,))
        print('Raw image: %s' % (self.im_raw.shape,))

        if len(self.im_pro) != 0:
            for i,im_pro in enumerate(self.im_pro):
                print('Processed image %i: %s' % (i,im_pro.shape,))


        print("Computed imaging rate: %f" % self.im_rate)
        

    def compute_imagingtimes_segments(self, debug=False):

        t = self.Cell_timesers1.shape[1]
        z = self.z

        self.compute_imagingtimes(t,z,debug=debug)


    def check_align_segments(self, debug=False):

        t = self.Cell_timesers1.shape[1]

        return self.check_align(t)
            

    @classmethod
    def dataset_keys(cls, out_file):

        import h5py

        h5py_file = h5py.File(out_file,'r')
        print(list(h5py_file.keys()))
        h5py_file.close()


    def remove_clipped_values(self, debug=False):
        """
        [Mika segmentation specific]
        Mika: Many recordings have big events at the beginning. These 
        events are hard to correct and can also cause problems with
        downstream component detection. To overcome this problem, I
        have now set the signal at the onset of each recording (equal 
        in length to baseline_tau) to a constant value.
        """

        # find where the signal stops to be different
        artificialstop_imidx = np.where(np.around(self.H0[0,:],decimals=3) \
                                        != np.around(self.H0[0,0],decimals=3))[0][0]
        if artificialstop_imidx != 1:

            print("Artificial initial constant value detected. "
                  "Truncating first %i data points" % artificialstop_imidx)

        else: artificialstop_imidx = 0

        if debug:

            self.ep.plot_stackstarts(xlim_pos='end')

            overlay_fig, overlay_ax = self.overlay_im_ephys()
            
            ## zoom into end of artefact
            overlay_ax.axvline(x=artificialstop_imidx/self.im_rate,color='k',ls='--')
            overlay_ax.set_xlim([artificialstop_imidx/self.im_rate-5,artificialstop_imidx/self.im_rate+5])

            overlay_ax.set_xlabel('Time [s]')        
            

        # truncate imaging data
        ## !! is this correct? I'm removing stuff from the start !!
        self.im = self.im[:artificialstop_imidx,:,:,:]
        self.im_raw = self.im_raw[:artificialstop_imidx,:,:,:]
        if self.im_eq:  # processed images can be treated the same way
            for i,im_pro in enumerate(self.im_pro):
                self.im_pro[i] = self.im_pro[i][:artificialstop_imidx,:,:,:]

        # truncate cell data
        self.Cell_timesers0 = self.Cell_timesers0[:,artificialstop_imidx:]
        self.Cell_baseline1 = self.Cell_baseline1[:,artificialstop_imidx:]
        self.Cell_timesers1 = self.Cell_timesers1[:,artificialstop_imidx:]                
        
        # truncate component data (not necessarily included in all analysis)
        try: self.H0 = self.H0[:,artificialstop_imidx:]
        except: pass
        try: self.H1 = self.H1[:,artificialstop_imidx:]
        except: pass

        # truncate ephys data
        artificialstop_ephysidx = self.image_starttimes[artificialstop_imidx]
        self.ep.replace_ephys(self.ep.ep[:,artificialstop_ephysidx:])

        # recalculate imaging times and check for alignment
        self.compute_imagingtimes(debug=self.debug)
        self.aligned = self.check_align()


    def cleanup_artefacts_segments(self, debug=False):

        t = self.Cell_timesers1.shape[1]

        self.cleanup_artefacts(t)

    
    def cleanup_artefacts(self, t, debug=False):
        """
        _WARNING: 1A_
        Redefined method. Many parts copied from spim.cleanup_artefacts().
        Remember to update that method whenever you update this one.
        (Not the best way; think of a better way eventually.)
        """

        num_lsim = t
        num_epim = self.image_starts.sum()

        n_imdiff = num_epim - num_lsim
        print(f'Number of light sheet images: {num_lsim}')
        print(f'Number of ephys images: {num_epim}')

        if n_imdiff > 0:

            print('More images in ephys. Truncating ephys...')

            diff_idx  = self.image_starttimes[-n_imdiff]
            self.ep.replace_ephys(self.ep.ep[:,:diff_idx])
            self.compute_imagingtimes_segments(debug=self.debug)
        

        elif n_imdiff < 0:

            print('More images in imaging. Truncating imaging...')
            
            # truncate imaging data
            self.im = self.im[:n_imdiff,:,:,:]

            if self.im_raw is not None:
                self.im_raw = self.im_raw[:n_imdiff,:,:,:]
                
            if self.im_eq:
                for i,im_pro in enumerate(self.im_pro):
                    self.im_pro[i] = self.im_pro[i][:n_imdiff,:,:,:]            

            # truncate cell data
            self.Cell_timesers0 = self.Cell_timesers0[:,:n_imdiff]
            self.Cell_baseline1 = self.Cell_baseline1[:,:n_imdiff]
            self.Cell_timesers1 = self.Cell_timesers1[:,:n_imdiff]

            # truncate component data (not necessarily included in all analysis)
            try: self.H0 = self.H0[:,:n_imdiff]
            except: pass
            try: self.H1 = self.H1[:,:n_imdiff]
            except: pass

        self.aligned = self.check_align()       


    def remove_lastframe(self, debug=False):
        """
        There could be the same number of images in both ephys and imaging but
        the ends are not aligned.

        _WARNING: 1B_
        This method is not inherited but redefined in segmented.remove_lastframe().
        Remember to update that method whenever you update this one.
        (Not the best way; think of a better way eventually.)
        """
         
        print('Ephys and imaging aligned; remove last frame from both...')

        # truncate images
        self.im = self.im[:-1,:,:,:]
        
        if self.im_raw is not None:
            self.im_raw = self.im_raw[:-1,:,:,:]
            
        if self.im_eq:
            for i,im_pro in enumerate(self.im_pro):
                self.im_pro[i] = self.im_pro[i][:-1,:,:,:]

        # truncate components
        try: self.H0 = self.H0[:,:-1]
        except: pass
        try: self.H1 = self.H1[:,:-1]
        except: pass
        
        # truncate cells
        self.Cell_timesers0 = self.Cell_timesers0[:,:-1]
        self.Cell_baseline1 = self.Cell_baseline1[:,:-1]
        self.Cell_timesers1 = self.Cell_timesers1[:,:-1]                

        # truncate ephys
        diff_idx  = self.image_starttimes[-1]
        self.ep.replace_ephys(self.ep.ep[:,:diff_idx])
        self.compute_imagingtimes_segments(debug=self.debug)

        self.aligned = self.check_align_segments()


    ###################################################################
    #                            Analysis                             #
    ###################################################################        

    def overlay_im_ephys(self):

        overlay_fig, overlay_ax = plt.subplots(figsize=(9,3))
        overlay_ax.plot(np.linspace(0, self.H0[0].shape[0]/self.im_rate, num=self.H0[0].shape[0]),
                        self.H0[0])
        overlay_ax.plot(np.linspace(0, self.H0[0].shape[0]/self.im_rate, num=self.H0[0].shape[0]),
                        self.H0[0],'.')
        overlay_ax.plot(np.linspace(0, self.image_starts.shape[0]/self.ephys_rate, num=self.image_starts.shape[0]),
                        self.image_starts)

        return overlay_fig, overlay_ax        
    
        
    def find_cell(self, cell_num, mask=1):

        cell_volume = np.zeros((self.z, self.y, self.x))
    
        for j in range(np.count_nonzero(self.Cell_X[cell_num, :] > 0)):
        
            if mask:
                cell_volume[int(self.Cell_Z[cell_num, j]),
                            int(self.Cell_Y[cell_num, j]),
                            int(self.Cell_X[cell_num, j])] = mask
                
            else:
                cell_volume[int(self.Cell_Z[cell_num, j]),
                            int(self.Cell_Y[cell_num, j]),
                            int(self.Cell_X[cell_num, j])] = \
                            self.Cell_spcesers[cell_num, j]
            
        return cell_volume
    

    def plot_volume(self, nrows, ncols, save_name=None):
        
        """
        Plot all cells segmented using self.Volume.

        """

        from analysis_toolbox.utils import get_transparent_cm

        trans_inferno = get_transparent_cm('hot',tvmax=1,gradient=False)

        nplanes = self.Volume.shape[2]

        assert nrows*ncols >= nplanes
        
        vol_fig, vol_ax = plt.subplots(nrows,ncols,figsize=(ncols*4,nrows*3),
                                       squeeze=False)

        vol_ax = vol_ax.flatten()

        for nplane in range(nplanes):
    
            vol_ax[nplane].imshow(self.image_mean[nplane,:,:], cmap='gray',
                                  vmin=np.percentile(np.ravel(self.image_mean[nplane,:,:]),1),
                                  vmax=np.percentile(np.ravel(self.image_mean[nplane,:,:]),99.9))


            vax = vol_ax[nplane].imshow(self.Volume[:,:,nplane].transpose(),
                                        vmax=np.percentile(np.ravel(self.Volume[:,:,:]),99.9),
                                        cmap=trans_inferno)
            
            vol_fig.colorbar(vax,ax=vol_ax[nplane])
    
            vol_ax[nplane].set_title('Plane %i' % nplane)
        
            vol_fig.tight_layout()

        if save_name:
            vol_fig.savefig(save_name)

        return vol_fig, vol_ax

    

    def plot_allcells_map(self, label=None, cmap=None, save_name=None, parallelize=False, show_plot=False, alpha=1):

        cells = np.arange(self.n)

        cell_volume, vol_fig, vol_ax = self.plot_cell_map(cells,label=label,
                                                          cmap=cmap, save_name=save_name,
                                                          parallelize=parallelize,
                                                          show_plot=show_plot, alpha=alpha)

        return  cell_volume, vol_fig, vol_ax
        


    def plot_cell_map(self, cells, nrows, ncols, label=None, cmap=None,
                      save_name=None, parallelize=False,
                      show_plot=False, alpha=1):
        
        """

        """
        from tqdm import tqdm

        if parallelize:

            import multiprocessing as mp

            num_processes = min(mp.cpu_count(), self.n)

            # divide clusters into all processes
            cells_list = np.array_split(cells,num_processes)
            label_list = np.array_split(label,num_processes)

            output=mp.Queue()

            processes = [mp.Process(target=self.collapse_multiple_cells,
                                    args=(cells_list[proc],label_list[proc]),
                                    kwargs={"save_name": save_name,
                                            "output": output}) \
                         for proc in range(num_processes)]



            print("Starting %i processes..." % num_processes)
            for p in processes: p.start()
            for p in processes: p.join()
            result = [output.get() for p in processes]
            
            cell_volume = result  ## TODO: has to be some combination of result

        else:

            cell_volume = self.collapse_multiple_cells(cells,label,save_name=save_name)

        if show_plot:
            
            vol_fig, vol_ax = self.overlay_volume(cell_volume, nrows, ncols, cmap=cmap, alpha=alpha, save_name=save_name)

            return cell_volume, vol_fig, vol_ax 

        else:
            return cell_volume

    def overlay_volume(self, volume, nrows, ncols, cmap=None, alpha=1, save_name=None):

        nplanes = self.z
        assert nrows*ncols >= nplanes
        
        vol_fig, vol_ax = plt.subplots(nrows, ncols, figsize=(ncols*4,nrows*3),
                                       squeeze=False)

        vol_ax = vol_ax.flatten()

        for nplane in range(nplanes):

            vol_ax[nplane].imshow(self.image_mean[nplane,:,:], cmap='gray',
                                  vmin=np.percentile(np.ravel(self.image_mean[nplane,:,:]),1),
                                  vmax=np.percentile(np.ravel(self.image_mean[nplane,:,:]),99.9))


            vax = vol_ax[nplane].imshow(volume[nplane,:,:], cmap=cmap, alpha=alpha)

            vol_fig.colorbar(vax,ax=vol_ax[nplane])

            vol_ax[nplane].set_title('Plane %i' % nplane)

            vol_fig.tight_layout()

            if save_name:
                vol_fig.savefig(save_name)

        return vol_fig, vol_ax


    def collapse_multiple_cells(self, cell_list, label_list, save_name=None, output=None):

        from tqdm import tqdm
        from analysis_toolbox.utils import now_str

        # create empty volume to fill
        cell_volume = np.zeros(self.Volume.shape).T

        for cell, label in tqdm(zip(cell_list, label_list),total=len(cell_list)):
            

            volume = self.find_cell(cell, mask=label)
            
            zloc, yloc, xloc = np.where(volume != 0)
            cell_volume[zloc,yloc,xloc] = volume[zloc,yloc,xloc]

        if save_name: np.save(save_name+now_str(), cell_volume)

        if output: output.put(cell_volume)
        else: return cell_volume

    

    def plot_cells(self, num_cells=10, mask=0, zoom_pad=25, save_name=None):

        from analysis_toolbox.utils import get_transparent_cm
        import random

        trans_inferno = get_transparent_cm('hot',tvmax=1,gradient=False)

        ts_fig, ts_ax = plt.subplots(num_cells,2,figsize=(8.5,num_cells*3),
                                     gridspec_kw = {'width_ratios':[1,3]})

        for neuron in range(num_cells):
    
            randcell = random.randint(0,self.n-1)
    
            cell_volume = self.find_cell(randcell, mask=mask)
            cell_z = np.where(np.any(cell_volume,axis=(1,2)))[0][0]

            try:
                ts_ax[neuron,0].imshow(self.image_mean[cell_z],
                                       cmap='gray',
                                       vmax=np.percentile(np.ravel(self.image_mean),99.0))
            except:
                pass            
    
            cell_im = ts_ax[neuron,0].imshow(cell_volume[cell_z],cmap=trans_inferno)
            ts_ax[neuron,0].set_title(f'Plane {cell_z}')

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

        import datetime

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

        from analysis_toolbox.utils import get_transparent_cm
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

    def visualize_multiple_components(self, component_list, comp_spcesers, comp_timesers,
                                      save_name=False, close_fig=False):

        for i in component_list:

            self.visualize_component(i, comp_timesers, comp_spcesers, 
                                     save_name=save_name, close_fig=close_fig)

            
    def visualize_components(self, component_list, comp_spcesers, comp_timesers,
                             save_name='visualize_cluster', close_fig=False, parallelize=False):

        if parallelize:

            import multiprocessing as mp

            num_processes = min(mp.cpu_count(),len(component_list))
            
            # divide clusters into all processes
            components_list = np.array_split(component_list,num_processes)

            processes = [mp.Process(target=self.visualize_multiple_components,
                                    args=(components_list[proc],comp_spcesers, comp_timesers),
                                    kwargs={"save_name": self.savepath+'visualize_cluster-'+str(components_list[proc])+'.png',
                                            "close_fig": True}) \
                         for proc in range(num_processes)]

            print("Starting %i processes..." % num_processes)
            for p in processes: p.start()
            for p in processes: p.join()
            print("Done!")

        else:

            self.visualize_multiple_components(component_list, comp_spcesers, comp_timesers,
                                               save_name=save_name, close_fig=close_fig)

    def compute_triggers(self, triggers, time_window, trigger_savename=False):

        # how many stacks before and after the trigger are you interested to see?
        window = np.arange(round(-time_window*self.im_rate),round(time_window*self.im_rate))

        triggers_arr = triggers.reshape(-1,1) + window

        triggers_around = triggers_arr[(triggers_arr < self.nstacks).all(axis=1),:]

        if trigger_savename:
            np.save(self.savepath + trigger_savename + '.npy', triggers_around)

        return window, triggers_around


    def compute_triggered(self, triggers_around, comp_timesers, statistic='mean'):

        triggered = comp_timesers[:,triggers_around]

        if statistic == 'mean':
            triggered_center = triggered.mean(axis=1)

        elif statistic == 'median':
            triggered_center = np.median(triggered, axis=1)

        elif statistic == 'both':
            triggered_center = (triggered.mean(axis=1), np.median(triggered, axis=1))

        return triggered, triggered_center

    def visualize_triggered(self, comp_num, window, triggered, triggered_mean, triggered_median,
                            comp_spcesers, comp_timesers, plot_trials=False):

        from scipy.stats import sem
        from math_helper import compute_fft

        roi_fig, roi_ax = plt.subplots(1, 6, figsize=(16, 3))

        clust_volume = self.find_component(comp_spcesers, comp_num)

        vmax = np.max(np.array([np.max(clust_volume.max(0)),np.max(clust_volume.max(1))]))
        vmin = np.min(np.array([np.min(clust_volume.max(0)),np.min(clust_volume.max(1))]))
        
        # Plot brain and ROI (xy projection)
        roi_ax[0].imshow(self.image_mean.max(0).T,cmap='gray')
        roi_ax[0].imshow(clust_volume.max(0).T,alpha=0.5,vmin=vmin,vmax=vmax)
        roi_ax[0].axis('off')

        # Plot brain and ROI (yz projection)
        roi_ax[1].imshow(self.image_mean.max(1).T,cmap='gray',aspect='auto')
        clust_imshow = roi_ax[1].imshow(clust_volume.max(1).T,alpha=0.5,aspect='auto',vmin=vmin,vmax=vmax)
        roi_ax[1].axis('off')

        roi_fig.colorbar(clust_imshow, ax=roi_ax[1])

        if plot_trials:
            ntriggers = triggered[comp_num].shape[0]
            t_axis = np.tile(window/self.im_rate,(ntriggers,1)).transpose()
            roi_ax[2].plot(t_axis,triggered[comp_num].transpose(), color='#1f77b4', alpha=0.05)
            roi_ax[3].plot(t_axis,triggered[comp_num].transpose(), color='#1f77b4', alpha=0.05)

        roi_ax[2].plot(window/self.im_rate, triggered_mean[comp_num], color='#d62728', zorder=1e6)
        roi_ax[3].plot(window/self.im_rate, triggered_mean[comp_num], color='#d62728', zorder=1e6)

        roi_ax[2].plot(window/self.im_rate, triggered_median[comp_num], color='#E377C2', zorder=1e6)
        roi_ax[3].plot(window/self.im_rate, triggered_median[comp_num], color='#E377C2', zorder=1e6)    


        # Plot error bars
        if plot_trials:
            error = sem(triggered[comp_num].transpose(),axis=1)
            roi_ax[2].fill_between(window/self.im_rate, triggered_mean[comp_num]+error, triggered_mean[comp_num]-error,
                                              color='#d62728',alpha=0.5,zorder=1e6-1)
            roi_ax[3].fill_between(window/self.im_rate, triggered_mean[comp_num]+error, triggered_mean[comp_num]-error,
                                              color='#d62728',alpha=0.5,zorder=1e6-1)

        roi_ax[2].axvline(x=0,color='k',ls='--')
        roi_ax[2].set_ylabel(r'$\Delta F/ F$')
        roi_ax[2].set_xlabel('Time (s)')
        roi_ax[2].set_xlim([window.min()/self.im_rate,window.max()/self.im_rate])

        roi_ax[3].axvline(x=0,color='k',ls='--')
        roi_ax[3].set_ylabel(r'$\Delta F/ F$')
        roi_ax[3].set_xlabel('Time (s)')
        roi_ax[3].set_xlim([window.min()/self.im_rate,window.max()/self.im_rate])

        if plot_trials:
            roi_ax[3].set_ylim([np.min(np.array([(triggered_mean[comp_num]-error).min(),triggered_median[comp_num].min()])),
                                np.max(np.array([(triggered_mean[comp_num]+error).max(),triggered_median[comp_num].max()]))])

        # Plot raw calcium trace
        roi_ax[4].plot(np.linspace(0,self.nstacks/self.im_rate,self.nstacks), comp_timesers[comp_num])
        roi_ax[4].set_ylabel(r'$\Delta F/ F$')
        roi_ax[4].set_xlabel('Time (s)')

        slice_win = 10  # in seconds
        rand_slice = np.random.randint(self.nstacks/self.im_rate - 10)
        roi_ax[4].set_xlim([rand_slice, rand_slice+10])
        # roi_ax[3].set_ylim([np.percentile(timeseries[clust],0.1), np.percentile(timeseries[clust],99.8)])


        # Overlay swim power
        roi_ax2 = roi_ax[4].twinx()
        roi_ax2.plot(np.linspace(0,self.swim_power.shape[0]/self.ephys_rate,num=self.swim_power.shape[0]),
                     self.swim_power,color='#ff7f0e')
        # roi_ax2.set_xlim([swim_power[0]*rand_slice/ephys_rate, swim_power[0]*rand_slice/ephys_rate])
        roi_ax2.axis('off')

        # Overlay flashes
        roi_ax3 = roi_ax[4].twinx()
        roi_ax3.plot(np.linspace(0, self.ep.channel4.shape[0]/self.ephys_rate,num= self.ep.channel4.shape[0]),
                     self.ep.channel4,color='#17becf')
        roi_ax3.axis('off')

        Y, angle, frq = compute_fft(comp_timesers[comp_num], self.im_rate)

        roi_ax[5].plot(frq[1:],abs(Y)[1:])
        roi_ax[5].set_xlabel('Freq (Hz)')
        roi_ax[5].set_ylabel(r'|$\gamma$(freq)|')
        # roi_ax[5].set_xlim([-0.001,0.5])

        roi_fig.suptitle(str(self.expt_date.date())+'_'+self.expt_name)    
        roi_fig.tight_layout()
        roi_fig.subplots_adjust(top = 0.8)

        return roi_fig


    def visualize_multiple_triggered(self, component_list, window, triggered, triggered_mean, triggered_median,
                                     comp_spcesers, comp_timesers,
                                     plot_trials=False, save_name=False, close_fig=True, output=None):

        import datetime

        num_comp = len(component_list)

        delta = []

        for comp_num in component_list:

            print('Plotting ROI %i of %i ...' % (comp_num,num_comp))

            roi_fig = self.visualize_triggered(comp_num, window, triggered, triggered_mean, triggered_median,
                                               comp_spcesers, comp_timesers, plot_trials=plot_trials)


            from scipy.stats import wilcoxon
            t_stat,t_prob = wilcoxon(triggered[comp_num][:,np.logical_and(window/self.im_rate <= 0,
                                                                          window/self.im_rate >= -1.)].mean(1),
                                     triggered[comp_num][:,np.logical_and(window/self.im_rate > 0,
                                                                          window/self.im_rate <= 1.)].mean(1))
            print(t_stat,t_prob)

            # save components with large change
            if t_prob < 1e-10:
                mark = 'o'
                delta.append(comp_num)
            else:
                mark = 'x'

            if save_name:
                roi_fig.savefig(self.savepath+save_name+'-'+str(plot_trials)+'-'+str(comp_num)+
                                '-'+datetime.datetime.now().strftime("%Y-%m-%d_%H-%M")+'-'+mark+'.png')

            if close_fig:
                plt.close(roi_fig)

        if output: output.put(delta)
        else: return delta


    def visualize_triggereds(self, component_list, window, triggered, triggered_mean, triggered_median,
                             comp_spcesers, comp_timesers,
                             plot_trials=False, save_name='visualize_triggered_comp', close_fig=True,
                             parallelize=False):

        
        if parallelize:

            import multiprocessing as mp

            num_processes = min(mp.cpu_count(),len(component_list))
            
            # divide clusters into all processes
            components_list = np.array_split(component_list,num_processes)

            output=mp.Queue()

            processes = [mp.Process(target=self.visualize_multiple_triggered,
                                    args=(components_list[proc], window, triggered, triggered_mean, triggered_median,
                                          comp_spcesers, comp_timesers),
                                    kwargs={"plot_trials": plot_trials,
                                            "save_name": save_name,
                                            "close_fig": True, "output": output}) \
                         for proc in range(num_processes)]

            

            print("Starting %i processes..." % num_processes)
            for p in processes: p.start()
            for p in processes: p.join()
            result = [output.get() for p in processes]
            
            print("Done!")

            return result

        else:

            result = self.visualize_multiple_triggered(component_list, window, triggered, triggered_mean, triggered_median,
                                                       comp_spcesers, comp_timesers, plot_trials=plot_trials,
                                                       save_name=save_name, close_fig=close_fig, output=None)
            return result


    def calculate_DFF(self, bg_multiplier=0.8, debug=False):
        
        self.dff = (self.Cell_timesers1 - self.Cell_baseline1) / \
            (self.Cell_baseline1 - self.background * bg_multiplier)
        

    def check_NMF(self, comp_spcsers, comp_timesers, weight_percentile=99.5, save_name='component_ts'):

        from colors import tableau20
        import datetime
        import random

        dff = (self.Cell_timesers1 - self.Cell_baseline1) / (self.Cell_baseline1 - self.background * 0.8)

        nclust = comp_spcsers.shape[0]

        from analysis_toolbox.utils import get_transparent_cm
        trans_inferno = get_transparent_cm('hot',tvmax=1,gradient=False)

        clust_fig, clust_ax = plt.subplots(nclust, 6, figsize=(20,nclust*2),
                                           gridspec_kw = {'width_ratios':[1,1,3,3,3,3]})

        for clust in range(nclust):

            clust_volume = self.find_component(comp_spcsers, clust)
            vmax = np.max(np.array([np.max(clust_volume.max(0)),np.max(clust_volume.max(1))]))
            vmin = np.min(np.array([np.min(clust_volume.max(0)),np.min(clust_volume.max(1))]))

            # Plot brain and ROI (xy projection)
            clust_ax[clust,0].imshow(self.image_mean.max(0).T,cmap='gray')
            clust_ax[clust,0].imshow(clust_volume.max(0).T,cmap=trans_inferno,vmin=vmin,vmax=vmax)
            clust_ax[clust,0].axis('off')

            # Plot brain and ROI (zy projection)
            clust_ax[clust,1].imshow(self.image_mean.max(1).T,cmap='gray',aspect='auto')
            clust_imshow = clust_ax[clust,1].imshow(clust_volume.max(1).T,cmap=trans_inferno,vmin=vmin,vmax=vmax,aspect='auto')
            clust_ax[clust,1].axis('off')

            clust_fig.colorbar(clust_imshow, ax=clust_ax[clust,1])

            # plot all weights    
            clust_ax[clust,2].plot(comp_spcsers[clust])

            perc = weight_percentile
            perct = np.percentile(comp_spcsers[clust],perc)
            clust_ax[clust,2].axhline(y=perct, color='r', label=str(perct))
            clust_ax[clust,2].text(25000,perct+0.1,"%.1f percentile: %.1f" % (perc, perct))
            clust_ax[clust,2].set_ylabel('Weight')
            clust_ax[clust,2].set_xlabel('Cell # (unsorted)')


            # plot distribution of weights
            clust_ax[clust,3].hist(np.ravel(comp_spcsers[clust]),bins=200)
            clust_ax[clust,3].axvline(x=perct, color='r', label=str(perct))
            clust_ax[clust,3].text(perct-0.6,10**3, "%.1f percentile: %.1f" % (perc, perct))
            clust_ax[clust,3].set_yscale('log')
            clust_ax[clust,3].set_xlabel('Weight')
            clust_ax[clust,3].set_ylabel(r'$\log(Counts)$')

            # plot comparison of time series
            clust_ax[clust,4].plot(np.linspace(0,len(comp_timesers[clust])/self.im_rate,num=len(comp_timesers[clust])),
                                   comp_timesers[clust])

            # find highly weighted cells
            clust_cells = np.where(comp_spcsers[clust] > perct)[0]
            for cell in clust_cells:
                clust_ax[clust,4].plot(np.linspace(0,len(dff[cell])/self.im_rate,num=len(dff[cell])),
                                       dff[cell], alpha=0.4)

            win_size = 10  # in seconds
            randslice = random.randint(0, int(len(comp_timesers[clust])/self.im_rate - win_size))
            clust_ax[clust,4].set_xlim([randslice, randslice+win_size])

            clust_ax[clust,4].set_ylim([np.min([-0.1,np.percentile(comp_timesers[clust],0.1)]),np.percentile(comp_timesers[clust],99.5)])
            clust_ax[clust,4].set_ylabel('$\Delta F / F$')
            clust_ax[clust,4].set_xlabel('Time [s]')

            # find the standard deviation
            dff_std = np.std(dff[clust_cells],0)

            clust_ax[clust,5].plot(np.linspace(0,len(comp_timesers[clust])/self.im_rate,num=len(comp_timesers[clust])),
                                   comp_timesers[clust],color=tableau20[0])
            clust_ax[clust,5].fill_between(np.linspace(0,len(comp_timesers[clust])/self.im_rate,num=len(comp_timesers[clust])),
                                          comp_timesers[clust]-dff_std, comp_timesers[clust]+dff_std, alpha=0.8, color=tableau20[1])
            clust_ax[clust,5].set_xlim([randslice, randslice+win_size])
            clust_ax[clust,5].set_ylim([np.min([-0.1,np.percentile(comp_timesers[clust],0.1)]),np.percentile(comp_timesers[clust],99.5)])
            clust_ax[clust,5].set_ylabel('$\Delta F / F$')
            clust_ax[clust,5].set_xlabel('Time [s]')

        clust_fig.tight_layout()
        clust_fig.savefig(self.savepath+save_name+'-'+datetime.datetime.now().strftime("%Y-%m-%d_%H-%M")+'.png')

                            
                          

######################################################################
### mika_helper.py ends here
